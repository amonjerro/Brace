using System.Collections.Generic;
public class InputBuffer
{
    int bufferLength;
    InputMessage activeMessage;
    List<InputMessage> inputQueue;
}