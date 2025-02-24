namespace InputBuffer{
    public enum EInput
    {
        Fireball,
        Jump,
        Block
    }

    public class InputMessage
    {
        EInput actionType;
        bool consumed;
    }
}

