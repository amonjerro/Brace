using System.Collections.Generic;
namespace InputManagement
{
    public class InputBufferItem
    {
        List<InputMessage> inputMessages;
        InputMessage winningAction;
        public bool Acknowledged {  get; set; }
        public bool WinningActionSet { get { return winningAction != null; } }

        public InputBufferItem()
        {
            inputMessages = new List<InputMessage>();
            winningAction = null;
        }

        // Winning action can return null
        public InputMessage FindWinningAction()
        {
            if (WinningActionSet) { 
                return winningAction;
            }

            int minPriority = 999;
            int inputPriority = 999;
            for(int i = 0; i < inputMessages.Count; i++)
            {
                // Avoid null references
                if (inputMessages[i] == null)
                {
                    continue;
                }

                if (inputMessages[i].consumed)
                {
                    Acknowledged = true;
                    break;
                }

                inputPriority = InputBuffer.GetInputPriority(inputMessages[i].actionType);
                if (inputMessages[i].isRelease)
                {
                    inputPriority -= 1;
                }
                if (inputPriority < minPriority){
                    minPriority = inputPriority;
                    winningAction = inputMessages[i];
                }
            }
            return winningAction;
        }

        public override string ToString()
        {
            string s = "";
            foreach (InputMessage m in inputMessages) { 
                if (WinningActionSet)
                {
                    if (m == winningAction)
                    {
                        s += "<color=red>" + m.ToString() + "</color> /";
                    }
                    else
                    {
                        s += m.ToString() + " / ";
                    }
                }
            }
            return s;
        }

        public void AddInput(InputMessage m)
        {
            inputMessages.Add(m);
        }
    }
}
