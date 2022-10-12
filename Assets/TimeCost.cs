// 时间开销记录工具
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace YX
{
    public class TimeCost
    {
        public const int RECORD_DEEP = 20;

        public class TimeSpan
        {
            public string Name { get; private set; }
            public long BeginTicks { get; private set; }
            public long EndTicks { get; private set; }
            public float SpanMS { get { return (EndTicks - BeginTicks) / 10000f; } }

            public void End()
            {
                EndTicks = DateTime.Now.Ticks;
            }

            public override string ToString()
            {
                return string.Format("{0}用时{1}ms", Name, ((EndTicks - BeginTicks) / 10000f).ToString());
            }

            public static TimeSpan Now(string name)
            {
                var inst = new TimeSpan();
                inst.Name = name;
                inst.EndTicks = 0;
                inst.BeginTicks = DateTime.Now.Ticks;
                return inst;
            }
        }

        private static Stack<TimeSpan> _recordStack = new Stack<TimeSpan>(RECORD_DEEP);

        public static void Begin(string name)
        {
            if (_recordStack.Count >= RECORD_DEEP)
                throw new OutOfMemoryException();

            var rec = TimeSpan.Now(name);
            _recordStack.Push(rec);
        }

        public static TimeSpan End()
        {
            if (_recordStack.Count <= 0)
                throw new NullReferenceException();

            var rec = _recordStack.Pop();
            rec.End();
            return rec;
        }
    }
}
