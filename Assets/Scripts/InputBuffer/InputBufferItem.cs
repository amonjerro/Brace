using System.Collections.Generic;
namespace InputManagement
{
    public class InputBufferItem
    {
        List<InputMessage> inputMessages;

        public InputBufferItem()
        {
            inputMessages = new List<InputMessage>();
        }

        public override string ToString()
        {
            string s = "";
            foreach (InputMessage m in inputMessages) { 
                s += m.ToString() + "/ ";
            }
            return s;
        }

        public void AddInput(InputMessage m)
        {
            inputMessages.Add(m);
        }
    }
}
