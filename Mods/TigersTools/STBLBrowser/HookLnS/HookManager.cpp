#define _CRT_SECURE_NO_WARNINGS
#include <windows.h>

#pragma data_seg(".shared")

wchar_t gTargetProcess[1024] = L"";
unsigned int gFunctionPointerAddr = 0;

wchar_t gFoundStrings[32768] = L"";

volatile int gReadPosition = 0;
volatile int gWritePosition = 0;

volatile LONG gOverflows = 0;
volatile LONG gIgnored = 0;

#pragma data_seg()
#pragma comment(linker, "/section:.shared,rws")

#define BUFFER_SIZE (sizeof(gFoundStrings) / sizeof(*gTargetProcess))

HMODULE ghThisModule = NULL;
HHOOK ghHookCbt;

void AttachProcess (HMODULE hModule);
void InstallHook ();
void RemoveHook ();

void CheckString (const wchar_t *key, const wchar_t *localized);

void * gOrigHookAddress = NULL;

BOOL APIENTRY DllMain (HMODULE hModule,
					   DWORD ul_reason_for_call,
					   LPVOID lpReserved
					   )
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
		AttachProcess(hModule);
		break;
	case DLL_PROCESS_DETACH:
		if (gOrigHookAddress != 0)
			RemoveHook();
		break;
	}
	return TRUE;
}

void AttachProcess (HMODULE hModule)
{
	ghThisModule = hModule;

	wchar_t filename[1024];
	int length = GetModuleFileName(NULL, filename, sizeof (filename) / sizeof (wchar_t));
	if (length <= 0)
		return;

	int targetprocesslen = ::wcslen (gTargetProcess);
	if ((length > targetprocesslen && filename[length - targetprocesslen - 1] == '\\' ||
		length == targetprocesslen) &&
		!_wcsicmp (&filename[length - targetprocesslen], gTargetProcess))
	{
		InstallHook ();
	}
}

void * SetHookValue(void * newval)
{

	if (gFunctionPointerAddr == 0 || newval == NULL ||
		*(unsigned char *)gFunctionPointerAddr != 0xe8)
		return NULL;

	unsigned long adjustedval = (unsigned long)newval - (gFunctionPointerAddr + 5);

	DWORD dwNewProtection = PAGE_EXECUTE_READWRITE;
	DWORD dwOldProtection;
	VirtualProtect((void *)gFunctionPointerAddr, 8, dwNewProtection, &dwOldProtection);
	adjustedval = InterlockedExchange((LONG *)(gFunctionPointerAddr + 1), adjustedval);
	VirtualProtect((void *)gFunctionPointerAddr, 8, dwOldProtection, &dwOldProtection);
	
	return (void *)(adjustedval + gFunctionPointerAddr + 5);
}

wchar_t __declspec(naked) *StringHook(wchar_t *data)
{
	__asm {
		push		gOrigHookAddress
		push		[esp + 12]
		call		[esp + 4]
		call		[esp + 8]
		add			esp, 12
		; Pop an extra DWORD off the stack for the return address from the CALL here
		; Since the rest of the function has already been executed now
		
		push		eax
		push		eax
		push		[esp + 12]
		call		CheckString
		add			esp, 8
		pop			eax
		ret
	}
}

void InstallHook ()
{
	gOrigHookAddress = (void *)SetHookValue(&StringHook);
}

void RemoveHook ()
{
	if (gOrigHookAddress)
		SetHookValue(gOrigHookAddress);
	gOrigHookAddress = NULL;
}

unsigned long long FNV64(const wchar_t *string, unsigned length)
{
        unsigned long long hash = 0xcbf29ce484222325LL;
		for (int loop = 0; loop < length; loop++)
        {
                hash *= 0x00000100000001B3LL;
                hash ^= tolower(string[loop]);
        }
		return hash;
}

unsigned long long lasthash;

void CheckString (const wchar_t *key, const wchar_t *localized)
{
	if (!key || !localized)
	{
		InterlockedIncrement(&gIgnored);
		return;
	}

	unsigned length1 = *(unsigned long *)(key + 4);
	unsigned length2 = *(unsigned long *)(localized + 4);
	if (length1 == length2 || length2 == 0)
	{
		InterlockedIncrement(&gIgnored);
		return;
	}

	key += 6;
	localized += 6;

	if (_wcsnicmp(key, localized, length1))
	{
		unsigned long long hash = FNV64(key, length1);
		if (hash == lasthash)
		{
			InterlockedIncrement(&gIgnored);
			return;
		}
		lasthash = hash;

		if (length1 >= ((BUFFER_SIZE - (gWritePosition + 1 - gReadPosition)) & (BUFFER_SIZE - 1)))
		{
			InterlockedIncrement(&gOverflows);
		}
		else
		{
			length1++;
			unsigned wrappos = BUFFER_SIZE - gWritePosition;
			if (wrappos > length1)
				wrappos = length1;

			wcsncpy(&gFoundStrings[gWritePosition], key, wrappos);
			if (wrappos < length1)
			{
				wcsncpy(&gFoundStrings[0], &key[wrappos], length1 - wrappos);
			}

			gWritePosition = (gWritePosition + length1) & (BUFFER_SIZE - 1);
		}
	}
	else
		InterlockedIncrement(&gIgnored);
}

// Don't actually DO anything here, just use it as an excuse to get loaded early
LRESULT CALLBACK CBTProc(	int nCode,
							WPARAM wParam,
							LPARAM lParam
						)
{
	return CallNextHookEx(ghHookCbt, nCode, wParam, lParam);
}

extern "C" __declspec(dllexport) bool InstallCBTHook(void)
{
	if (ghHookCbt != NULL)
		return false;

	ghHookCbt = SetWindowsHookEx (WH_CBT, CBTProc, ghThisModule, 0);

	return ghHookCbt != NULL;
}

extern "C" __declspec(dllexport) bool UninstallCBTHook(void)
{
	if (ghHookCbt == NULL)
		return false;

	UnhookWindowsHookEx (ghHookCbt);
	ghHookCbt = NULL;

	return true;
}

extern "C" __declspec(dllexport) bool SetTargetFilename(const wchar_t *filename, unsigned long addr)
{
	if (ghHookCbt != NULL)
		return false;

	wcsncpy (gTargetProcess, filename, sizeof(gTargetProcess) / sizeof(*gTargetProcess));
	gFunctionPointerAddr = addr;

	return true;
}

extern "C" __declspec(dllexport) bool GetNextString(wchar_t **string)
{
	if (ghHookCbt == NULL)
		return false;

	if (gReadPosition == gWritePosition)
		return false;

	unsigned length = 0;
	for (unsigned loop = gReadPosition; loop < gWritePosition + BUFFER_SIZE; loop++)
	{
		if (gFoundStrings[loop & (BUFFER_SIZE - 1)])
			length++;
		else
			break;
	}

	length++;

	*string = (wchar_t *)CoTaskMemAlloc(length * sizeof(wchar_t));

	unsigned wrappos = BUFFER_SIZE - gReadPosition;
	if (wrappos > length)
		wrappos = length;

	wcsncpy(*string, &gFoundStrings[gReadPosition], wrappos);
	if (wrappos < length)
	{
		wcsncpy(&(*string)[wrappos], &gFoundStrings[0], length - wrappos);
	}

	gReadPosition = (gReadPosition + length) & (BUFFER_SIZE - 1);

	return true;
}

extern "C" __declspec(dllexport) void GetCounts(int *dropped, int *ignored)
{
	*dropped = InterlockedExchange(&gOverflows, 0);
	*ignored = InterlockedExchange(&gIgnored, 0);
}