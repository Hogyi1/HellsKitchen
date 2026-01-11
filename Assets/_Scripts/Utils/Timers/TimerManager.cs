using System.Collections.Generic;

namespace BetterTimers
{
    public static class TimerManager
    {
        static readonly List<Timer> timers = new();

        public static void RegisterTimer(Timer timer) => timers.Add(timer);
        public static void DeregisterTimer(Timer timer) => timers.Remove(timer);

        public static void UpdateTimers()
        {
            for (int i = 0; i < timers.Count; ++i)
            {
                timers[i]?.Tick();
            }
        }

        public static void Clear() => timers.Clear();
    }
}