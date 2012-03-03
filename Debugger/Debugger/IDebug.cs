

namespace Misukisu.Debug
{
    public interface IDebuggable
    {
        bool IsDebugging();
        void setDebugger(IDebugger debugger);
        void Debug(object sender, string msg);
    }

    public interface IDebugger
    {
        void Debug(object sender, string msg);
    }
}