using System.Runtime.InteropServices;

namespace HoytTech.CSharp
{
    public class SystemCalls
    {
        private const long TICKS_IN_MILLS = 10000;
        
        private static readonly double tickFrequency;
        
        static SystemCalls()
        {
            var frequency = QueryPerformanceFrequency();
            tickFrequency = 10000000.0;
            tickFrequency /= (double) frequency;
        }
        
        public static long MillsToFrequency(long milliseconds)
        {
            var value = milliseconds * TICKS_IN_MILLS;
            return (long) (value / tickFrequency);
        }
        
        [DllImport("System.Native", EntryPoint = "SystemNative_GetTimestamp")]
        internal static extern long QueryPerformanceCounter();
        
        [DllImport("System.Native", EntryPoint = "SystemNative_GetTimestampResolution")]
        internal static extern long QueryPerformanceFrequency();
        
        public static long GetTimestamp()
        {
          return QueryPerformanceCounter();
        }
    }
}