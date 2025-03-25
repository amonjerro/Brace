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

        // Push flag is used to determine if a new InputBufferItem can be pushed into the buffer
        public bool PushFlag { get; private set; }

        // Tests whether the buffer is paused
        public bool BufferPaused { get; set; }

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
            if (BufferPaused)
            {
                return;
            }

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
                InputBufferItem ibi = inputQueue.Dequeue();
                if (ibi.FindWinningAction() == activeMessage)
                {
                    activeMessage = null;
                }
            }

            if (!item.WinningActionSet) {
                item.FindWinningAction();
            }
            inputQueue.Enqueue(item);
        }

        // Updates the currently queued active message with a new one
        public void UpdateActiveMessage()
        {
            InputMessage contenderAction;

            foreach(InputBufferItem ibi in inputQueue)
            {
                // Ignore buffer items that have already been processed
                if (ibi.Acknowledged == true) {
                    continue;
                }

                // Ignore buffer items that do not have a winning action
                if (!ibi.WinningActionSet)
                {
                    continue;
                }


                contenderAction = ibi.FindWinningAction();
                
                // Update the buffer item if this action has been consumed
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

        // Clear the active message
        private void ClearActive()
        {
            activeMessage = null;
        }

        // Get the currently active message
        public InputMessage GetActiveMessage()
        {
            return activeMessage;
        }

        // Get this buffer's full input queue
        public Queue<InputBufferItem> GetInputQueue() { 
            return inputQueue;
        }

        // Toggle the buffer's push flag
        private void ToggleFlag()
        {
            PushFlag = !PushFlag;
        }
    }
}
