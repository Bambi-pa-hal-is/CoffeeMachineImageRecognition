using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeMachineImageRecognition
{
    public class BeverageQuota
    {
        public int CurrentCount { get; private set; }
        public int MaxCount { get; }
        public TimeSpan ResetInterval { get; }
        private DateTime LastResetTime { get; set; }

        public BeverageQuota(int maxCount, TimeSpan? resetInterval = null)
        {
            MaxCount = maxCount;
            ResetInterval = resetInterval ?? TimeSpan.FromMinutes(5); // Default reset interval
            CurrentCount = 0;
            LastResetTime = DateTime.UtcNow;
        }

        public bool ConsumeQuota()
        {
            if (DateTime.UtcNow - LastResetTime >= ResetInterval)
            {
                ResetQuota();
            }

            if (CurrentCount < MaxCount)
            {
                CurrentCount++;
                return true;
            }
            return false;
        }

        public void ResetQuota()
        {
            CurrentCount = 0;
            LastResetTime = DateTime.UtcNow; // Update last reset time
        }
    }
}
