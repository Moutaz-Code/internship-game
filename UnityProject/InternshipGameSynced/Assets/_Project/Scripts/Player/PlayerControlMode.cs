namespace InternshipGame.Prototype
{
    public enum PlayerControlMode
    {
        Desktop,
        VR,
        Auto,
    }

    public static class PlayerControlModeResolver
    {
        public static PlayerControlMode Resolve(PlayerControlMode configuredMode, bool xrRuntimeAvailable)
        {
            return configuredMode == PlayerControlMode.Auto
                ? xrRuntimeAvailable ? PlayerControlMode.VR : PlayerControlMode.Desktop
                : configuredMode;
        }
    }
}
