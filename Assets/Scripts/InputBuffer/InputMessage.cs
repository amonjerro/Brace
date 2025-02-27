namespace InputManagement{
    public enum EInput
    {
        Fireball,
        Jump,
        Block
    }

    public class InputMessage
    {
        public EInput actionType;
        public bool consumed;

        public InputMessage(EInput input)
        {
            actionType = input;
        }


        public override string ToString() {  
            return $"{actionType.ToString()} C:{consumed.ToString()} ";
        }
    }
}

