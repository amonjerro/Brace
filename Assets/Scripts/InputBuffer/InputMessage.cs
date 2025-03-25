namespace InputManagement{
    public enum EInput
    {
        None,
        Fireball,
        Jump,
        Block,
        Up,
        Down,
        Left,
        Right
    }

    /// <summary>
    /// Class that defines the information package for a player input
    /// </summary>
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
            string s = isRelease ? "R" : "P";
            return $"{s}-{actionType.ToString()} C:{consumed.ToString()} ";
        }
    }
}

