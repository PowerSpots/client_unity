using System.Collections.Generic;
using System.Diagnostics;
using XLua;

namespace Gankx
{
    public class PerformanceCounterService : Singleton<PerformanceCounterService>
    {
        public struct CounterTask
        {
            public readonly string name;
            public readonly Stopwatch watch;
            public readonly List<CounterTask> childCounters;

            public CounterTask(string name)
            {
                this.name = name;
                watch = Stopwatch.StartNew();
                childCounters = new List<CounterTask>();
                Stop();
            }

            public void Start()
            {
                watch.Start();
            }

            public void Stop()
            {
                watch.Stop();
            }

            public long GetElapsedMilliseconds()
            {
                long childTime = 0;
                foreach (var childCase in childCounters)
                {
                    childTime += childCase.GetElapsedMilliseconds();
                }

                if (watch.ElapsedMilliseconds < childTime)
                {
                    return childTime;
                }

                return watch.ElapsedMilliseconds;
            }

            public override string ToString()
            {
                return name;
            }

            public static CounterTask AddChild(CounterTask parent, string name)
            {
                for (var i = 0; i < parent.childCounters.Count; i++)
                {
                    if (parent.childCounters[i].watch.IsRunning)
                    {
                        return AddChild(parent.childCounters[i], name);
                    }
                }

                var newCase = new CounterTask(name);
                parent.childCounters.Add(newCase);
                return newCase;
            }

            public static void Sort(CounterTask parent)
            {
                parent.childCounters.Sort((a, b) => b.GetElapsedMilliseconds().CompareTo(a.GetElapsedMilliseconds()));
                foreach (var child in parent.childCounters)
                {
                    Sort(child);
                }
            }
        }

        private CounterTask myCounterRoot;
        private Dictionary<string, CounterTask> myCounterMap = new Dictionary<string, CounterTask>();

        public void StartRecord()
        {
            myCounterRoot = new CounterTask("Root");
            myCounterMap.Clear();
            myCounterRoot.Start();
        }

        public void StopRecord()
        {
            if (myCounterRoot.watch == null)
            {
                return;
            }

            myCounterRoot.Stop();
        }

        public void Begin(string counterPath)
        {
            CounterTask task;

            var splitIndex = counterPath.IndexOf("/");
            if (splitIndex != -1)
            {
                var root = myCounterRoot;
                while ((splitIndex = counterPath.IndexOf("/")) != -1)
                {
                    var prefix = counterPath.Substring(0, splitIndex);

                    if (!myCounterMap.TryGetValue(prefix, out task))
                    {
                        task = CounterTask.AddChild(root, prefix);
                        myCounterMap[prefix] = task;
                    }

                    root = task;

                    counterPath = counterPath.Substring(splitIndex + 1);
                }

                if (!myCounterMap.TryGetValue(counterPath, out task))
                {
                    task = CounterTask.AddChild(root, counterPath);
                    myCounterMap[counterPath] = task;
                }

                task.Start();
                return;
            }

            if (!myCounterMap.TryGetValue(counterPath, out task))
            {
                task = CounterTask.AddChild(myCounterRoot, counterPath);
                myCounterMap[counterPath] = task;
            }

            task.Start();
        }

        public void End(string counterPath)
        {
            CounterTask task;
            var splitIndex = counterPath.LastIndexOf("/");
            if (splitIndex != -1)
            {
                counterPath = counterPath.Substring(splitIndex + 1);
            }

            if (!myCounterMap.TryGetValue(counterPath, out task))
            {
                return;
            }

            task.Stop();
        }

        public float GetRecordDuration()
        {
            return myCounterRoot.watch.ElapsedMilliseconds;
        }

        public CounterTask GetSortedRoot()
        {
            CounterTask.Sort(myCounterRoot);

            return myCounterRoot;
        }
    }
}