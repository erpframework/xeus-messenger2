using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xeus2.xeus.Utilities
{
    public class Pool<T> where T : class, new()
    {
        Dictionary<T, bool> pool = new Dictionary<T, bool>();

        object poolLock = new object();

        public T Get()
        {
            lock (poolLock)
            {
                T instance = null;

                foreach (KeyValuePair<T, bool> item in this.pool)
                {
                    if (!item.Value)
                    {
                        instance = item.Key;
                        this.pool[instance] = true;
                        break;
                    }
                }

                if (instance == null)
                {
                    instance = new T();
                    this.pool.Add(instance, true);
                }

                return instance;
            }
        }

        public void Release(T instance)
        {
            lock (poolLock)
            {
                this.pool[instance] = false;
            }
        }
    }
}
