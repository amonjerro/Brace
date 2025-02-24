using System.Collections.Generic;

namespace InputManagement {
    public class InputBuffer
    {
        int bufferLength;
        int duration;
        InputMessage activeMessage;
        Queue<InputBufferItem> inputQueue;

        public InputBuffer(int duration, int length)
        {
            bufferLength = length;
            this.duration = duration;
        }

        public InputBufferItem Push(InputBufferItem item)
        {
            InputBufferItem returnable = null;
            if (inputQueue.Count >= bufferLength)
            {
                returnable = inputQueue.Dequeue();
            }

            inputQueue.Enqueue(item);
            return returnable;
        }

        public void ClearActive()
        {
            activeMessage = null;
        }

        public void SetActive(InputMessage newActive)
        {
            if (!activeMessage.consumed)
            {
                return;
            }
            activeMessage = newActive;
        }

        public void SetActiveConsumed()
        {
            activeMessage.consumed = true;
        }
    }
}
