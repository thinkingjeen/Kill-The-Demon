using UnityEngine;

namespace MobileConsole
{
    internal class LogCall
    {
        public LogType Type;
        public string LogString;
        public string StackTrace;
        
        public override int GetHashCode()
        {
            return Utils.CalculateHashCode(Type, LogString, StackTrace);
        }
    }
}