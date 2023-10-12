using UnityEngine;

namespace MobileConsole
{
    internal static class Utils
    {
        public static int CalculateHashCode(LogType type, string logString, string stackTrace)
        {
            var hashCode = type.GetHashCode();
            hashCode = (hashCode * 397) ^ (logString?.GetHashCode() ?? 0);
            hashCode = (hashCode * 397) ^ (stackTrace?.GetHashCode() ?? 0);
            return hashCode;
        }
    }
}