using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TBFramework.AI.BT
{
    public class BTParallel_MT : BTControl
    {
        private bool isRunning = false;
        private long nowRunningTime = 0;
        private long previousTime = 0;
        private E_BTNodeState previousState = E_BTNodeState.Success;
        private int overCount = 0;
        private int successCount = 0;
        private int failureCount = 0;
        private int runningCount = 0;

        public object locker = new object();

        private Dictionary<int, Task> tasks = new Dictionary<int, Task>();

        private Dictionary<int, (E_BTNodeState state, long runTime)> states = new Dictionary<int, (E_BTNodeState state, long runTime)>();
        private bool isAllSuccess = true;

        private bool isRunningPriority = false;

        public void SetParallel(bool isAllSuccess, bool isRunningPriority)
        {
            this.CountReset();
            this.states.Clear();
            this.isAllSuccess = isAllSuccess;
            this.isRunningPriority = isRunningPriority;
        }
        public override E_BTNodeState Evaluate(BaseContext context)
        {
            lock (locker)
            {
                //在不运行的时候创建运行
                if (!isRunning)
                {
                    int addIndex = -1;
                    this.previousTime = this.nowRunningTime;
                    this.nowRunningTime = DateTime.Now.Ticks;
                    long myRunningTime = this.nowRunningTime;
                    foreach (BTNode node in nodes)
                    {
                        addIndex++;
                        int myIndex = addIndex;
                        bool shoudeRun = true;
                        if (this.memoryType != E_BTControlMemoryType.AllReStart && states.ContainsKey(myIndex) && states[myIndex].runTime == this.previousTime && states[myIndex].state == E_BTNodeState.Success)
                        {
                            switch (this.memoryType)
                            {
                                case E_BTControlMemoryType.MemoryInRunning:
                                    if (previousState == E_BTNodeState.Running)
                                    {
                                        shoudeRun = false;
                                    }
                                    break;
                                case E_BTControlMemoryType.AllMemory:
                                    shoudeRun = false;
                                    break;
                            }
                        }
                        if (shoudeRun)
                        {
                            BTNode myNode = node;
                            Task task = Task.Run(() =>
                            {
                                lock (locker)
                                {
                                    E_BTNodeState state = myNode.Evaluate(context);
                                    if (isRunning && myRunningTime == nowRunningTime)
                                    {
                                        if (states.ContainsKey(myIndex))
                                        {
                                            states[myIndex] = (state, myRunningTime);
                                        }
                                        else
                                        {
                                            states.Add(myIndex, (state, myRunningTime));
                                        }
                                        switch (state)
                                        {
                                            case E_BTNodeState.Success:
                                                successCount++;
                                                break;
                                            case E_BTNodeState.Failure:
                                                failureCount++;
                                                break;
                                            case E_BTNodeState.Running:
                                                runningCount++;
                                                break;
                                        }
                                    }
                                }
                            });
                            tasks.Add(myIndex, task);
                        }
                    }
                }
                //运行中，判断是否可以返回
                else
                {
                    if ((!isAllSuccess && successCount > 0) || (isAllSuccess && overCount == nodes.Count && successCount == nodes.Count))
                    {
                        this.CountReset();
                        previousState = E_BTNodeState.Success;
                        return E_BTNodeState.Success;
                    }
                    if ((overCount == nodes.Count && isAllSuccess) || !isAllSuccess)
                    {
                        if (failureCount > 0 || runningCount > 0)
                        {
                            if (failureCount > 0 && runningCount > 0)
                            {
                                this.CountReset();
                                if (isRunningPriority)
                                {
                                    previousState = E_BTNodeState.Running;
                                    return E_BTNodeState.Running;
                                }
                                else
                                {
                                    previousState = E_BTNodeState.Failure;
                                    return E_BTNodeState.Failure;
                                }
                            }
                            else if (failureCount > 0)
                            {
                                this.CountReset();
                                previousState = E_BTNodeState.Failure;
                                return E_BTNodeState.Failure;
                            }
                            else if (runningCount > 0)
                            {
                                this.CountReset();
                                previousState = E_BTNodeState.Running;
                                return E_BTNodeState.Running;
                            }
                        }
                    }
                }
                return E_BTNodeState.Running;
            }
        }

        public void CountReset()
        {
            lock (locker)
            {
                this.tasks.Clear();
                isRunning = false;
                overCount = 0;
                successCount = 0;
                failureCount = 0;
                runningCount = 0;
            }
        }

        public override void Reset()
        {
            base.Reset();
            this.states.Clear();
            this.CountReset();
        }
    }
}