namespace InputManagement{
    public enum EInput
    {
        None,
        Fireball,
        Jump,
        Block
    }

    public class InputMessage
    {
        public EInput actionType;
        public bool consumed;
        public bool isRelease;

        public InputMessage(EInput input, bool isRelease = false)
        {
            actionType = input;
            this.isRelease = isRelease;

        }


        public override string ToString() {  
            return $"{actionType.ToString()} C:{consumed.ToString()} ";
        }
    }
}

