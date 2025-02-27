using System.Collections.Generic;

namespace InputManagement {
    public class InputBuffer
    {
        int bufferLength;
        float inputDuration;
        InputMessage activeMessage;
        Queue<InputBufferItem> inputQueue;

        public InputBuffer(float duration, int length)
        {
            bufferLength = length;
            inputDuration = duration;
            inputQueue = new Queue<InputBufferItem>();
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

        public Queue<InputBufferItem> GetInputQueue() { 
            return inputQueue;
        }
    }
}
