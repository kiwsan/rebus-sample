using System;

namespace Messages
{
    public class TimeSpanMessage
    {
        public TimeSpan TimeSpan { get; }

        public TimeSpanMessage(TimeSpan timeSpan)
        {
            TimeSpan = timeSpan;
        }
    }
}