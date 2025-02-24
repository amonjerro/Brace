using System.Collections.Generic;

namespace InputBuffer {
    public class InputBuffer
    {
        int bufferLength;
        int duration;
        InputMessage activeMessage;
        List<InputBufferItem> inputQueue;

        public InputBuffer(int duration, int length)
        {
            bufferLength = length;
            this.duration = duration;
        }

        public void Add(InputBufferItem item)
        {
            if (inputQueue.Count >= bufferLength)
            {
                return;
            }

            inputQueue.Add(item);
        }
    }
}
