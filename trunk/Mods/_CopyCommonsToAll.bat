set source=AnySim
set targets=DancersSpot DrunkardsBottle PaintedLady
set files=Debugger.cs I18n.cs Message.cs ToggleDebugger.cs

FOR %%t IN (%targets%) DO (
	FOR %%f IN (%files%) DO (
		copy %source%\%source%\%%f %%t\%%t\%%f
	)
)    

