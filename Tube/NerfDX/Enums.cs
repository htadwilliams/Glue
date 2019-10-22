namespace NerfDX
{
    /// <summary>
    /// Friendly translation for JoystickUpdate values when JoystickOffset is a button
    /// </summary>
    public enum ButtonValues
    {
        Unknown = -1,
        Release = 0,
        Press = 128,
    }

    /// <summary>
    /// Friendly translation for POV values when JoystickOffset is a POV hat or d-pad
    /// </summary>
    public enum POVStates
    {
        Release = -1,
        Up = 0,
        Upright = 4500,
        Right = 9000,
        Downright = 13500,
        Down = 18000,
        Downleft = 22500,
        Left = 27000,
        Upleft = 31500,
    }
}
