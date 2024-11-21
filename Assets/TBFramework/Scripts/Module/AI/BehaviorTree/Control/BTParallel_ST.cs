
namespace TBFramework.AI.BT
{
    public class BTParallel_ST : BTControl
    {
        private bool isAllSuccess = true;

        private bool haveRunningReturnDirectly = true;

        private bool haveResultReturnDirectly = true;

        private bool isRunningPriority = false;

        public void SetParallel(bool isAllSuccess, bool haveRunningReturnDirectly, bool haveResultReturnDirectly, bool isRunningPriority)
        {
            this.isAllSuccess = isAllSuccess;
            this.haveRunningReturnDirectly = haveRunningReturnDirectly;
            this.haveResultReturnDirectly = haveResultReturnDirectly;
            this.isRunningPriority = isRunningPriority;
        }

        public override E_BTNodeState Evaluate(BaseContext context)
        {
            if (isRandom && index == 0)
            {
                Random();
            }
            return SingleThreadEvaluate(context);
        }

        private E_BTNodeState SingleThreadEvaluate(BaseContext context)
        {
            int success = 0;
            int running = 0;
            int failure = 0;
            int memoryIndex = -1;
            for (int i = index; i < nodes.Count; i++)
            {
                BTNode node = nodes[i];
                switch (node.Evaluate(context))
                {
                    case E_BTNodeState.Failure:
                        failure++;
                        if (memoryType == E_BTControlMemoryType.AllMemory && memoryIndex < 0)
                        {
                            memoryIndex = i;
                        }
                        if (isAllSuccess)
                        {
                            if (haveResultReturnDirectly)
                            {
                                if (memoryIndex >= 0)
                                {
                                    this.index = memoryIndex;
                                }
                                else if (memoryType == E_BTControlMemoryType.AllReStart)
                                {
                                    this.index = 0;
                                }
                                return E_BTNodeState.Failure;
                            }
                        }
                        break;
                    case E_BTNodeState.Running:
                        running++;
                        if ((memoryType == E_BTControlMemoryType.AllMemory || memoryType == E_BTControlMemoryType.MemoryInRunning) && memoryIndex < 0)
                        {
                            memoryIndex = i;
                        }
                        if (haveRunningReturnDirectly)
                        {
                            if (memoryIndex >= 0)
                            {
                                this.index = memoryIndex;
                            }
                            else if (memoryType == E_BTControlMemoryType.AllReStart)
                            {
                                this.index = 0;
                            }
                            return E_BTNodeState.Running;
                        }
                        break;
                    case E_BTNodeState.Success:
                        success++;
                        if (!isAllSuccess)
                        {
                            if (haveResultReturnDirectly)
                            {
                                this.index = 0;
                                return E_BTNodeState.Success;
                            }
                        }
                        break;
                }
            }
            if (memoryIndex >= 0)
            {
                this.index = memoryIndex;
            }
            else if (memoryType == E_BTControlMemoryType.AllReStart)
            {
                this.index = 0;
            }
            if (isAllSuccess)
            {
                if (failure == 0 && running == 0)
                {
                    this.index = 0;
                    return E_BTNodeState.Success;
                }
            }
            else
            {
                if (success > 0)
                {
                    this.index = 0;
                    return E_BTNodeState.Success;
                }
            }
            if (failure > 0 && success > 0)
            {
                if (isRunningPriority)
                {
                    return E_BTNodeState.Running;
                }
                else
                {
                    return E_BTNodeState.Failure;
                }
            }
            else if (failure > 0)
            {
                return E_BTNodeState.Failure;
            }
            else
            {
                return E_BTNodeState.Running;
            }
        }
    }
}