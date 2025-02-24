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
    }
}

