using System.Collections.Generic;
namespace InputManagement
{
    /// <summary>
    /// Every frame of the input buffer is composed of a single buffer item
    /// A buffer item is a wrapper for a list of InputMessages that have arrived within one input frame
    /// </summary>
    public class InputBufferItem
    {
        // Internals
        List<InputMessage> inputMessages;
        InputMessage winningAction;

        // Properties
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

            // Do a minimum priority search among the input messages contained within this item
            int minPriority = 999;
            int inputPriority = 999;
            for(int i = 0; i < inputMessages.Count; i++)
            {
                // Avoid null references
                if (inputMessages[i] == null)
                {
                    continue;
                }

                // When an input message's action has been consumed and effected, it should no longer be taken into account
                if (inputMessages[i].consumed)
                {
                    Acknowledged = true;
                    break;
                }

                // We now need to evaluate priorities between the objects in this buffer and set the lowest one
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

        /// <summary>
        /// Overrides the ToString method to allow the DebugInputBuffer tool to see what is happening
        /// </summary>
        /// <returns></returns>
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

        // Add an input signal to this buffer item
        public void AddInput(InputMessage m)
        {
            inputMessages.Add(m);
        }
    }
}
