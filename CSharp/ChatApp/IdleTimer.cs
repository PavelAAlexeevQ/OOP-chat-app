using System;

namespace ChatApp
{
    public class IdleTimer
    {
        private readonly Action timeoutCallback;
        private readonly int timeoutTime;
        private System.Threading.Timer timeout;

        public IdleTimer(Action fn, int timeoutValue)
        {
            timeoutCallback = fn;
            timeoutTime = timeoutValue;
            timeout = new System.Threading.Timer(_ => fn(), null, timeoutValue, System.Threading.Timeout.Infinite);
        }

        public void ResetTimeout()
        {
            timeout.Change(timeoutTime, System.Threading.Timeout.Infinite);
        }
    }
}