using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Example
{
    public static class Counter
    {
        private static int _count = 0;

        public static int GetCount
        {
            get
            {
                return (int)_count;
            }
        }

        public static int Increment()
        {
            Interlocked.Increment(ref _count);
            return (int)_count;
        }

        public static int Decrement()
        {
            Interlocked.Decrement(ref _count);
            return (int)_count;
        }
    }
}
