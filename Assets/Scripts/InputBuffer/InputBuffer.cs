using System.Collections.Generic;

namespace InputManagement {
    public class InputBuffer
    {
        float bufferCounter;
        int bufferLength;
        float inputDuration;
        InputMessage activeMessage;
        Queue<InputBufferItem> inputQueue;
        static Dictionary<EInput, int> priorities;

        public bool PushFlag { get; private set; }

        //** Static helper messages **//
        public static int GetInputPriority(EInput type)
        {
            return priorities[type];
        }

        public static void SetPriorities(Dictionary<EInput, int> pairings)
        {
            priorities = pairings;
        }

        // Constructor
        public InputBuffer(float duration, int length)
        {
            bufferLength = length;
            inputDuration = duration;
            bufferCounter = 0f;
            inputQueue = new Queue<InputBufferItem>();
        }

        // Update the state of the buffer
        public void Update(float deltaTime)
        {
            bufferCounter += deltaTime;

            // Move the buffer
            if (bufferCounter > inputDuration) { 
                bufferCounter = 0f;
                ToggleFlag();
            }
            if (activeMessage == null)
            {
                return;
            }

            if (activeMessage.consumed == true)
            {
                ClearActive();
            }
        }

        // Push a new frame into the buffer
        public void Push(InputBufferItem item)
        {
            ToggleFlag();

            if (inputQueue.Count >= bufferLength)
            {
                inputQueue.Dequeue();
            }

            if (!item.WinningActionSet) {
                item.FindWinningAction();
            }
            inputQueue.Enqueue(item);
        }

        public void UpdateActiveMessage()
        {
            InputMessage contenderAction;

            foreach(InputBufferItem ibi in inputQueue)
            {
                if (ibi.Acknowledged == true) {
                    continue;
                }
                if (!ibi.WinningActionSet)
                {
                    continue;
                }


                contenderAction = ibi.FindWinningAction();
                if (contenderAction.consumed == true)
                {
                    ibi.Acknowledged = true;
                    continue;
                }
                // Test the buffer itself to see if this action automatically gets set
                if (activeMessage == null) { 
                    activeMessage = contenderAction;
                    return;
                }

                // Continue testing after this set - the active message can still be replaced
                if (GetInputPriority(contenderAction.actionType) < GetInputPriority(activeMessage.actionType))
                {
                    activeMessage = contenderAction;
                }
            }
        }

        private void ClearActive()
        {
            activeMessage = null;
        }

        private void SetActive(InputMessage newActive)
        {
            activeMessage = newActive;
        }

        public InputMessage GetActiveMessage()
        {
            return activeMessage;
        }

        public void SetActiveConsumed()
        {
            activeMessage.consumed = true;
        }

        public Queue<InputBufferItem> GetInputQueue() { 
            return inputQueue;
        }

        private void ToggleFlag()
        {
            PushFlag = !PushFlag;
        }
    }
}
