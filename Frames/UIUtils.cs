namespace NoxRaven.Frames
{
    public static class UIUtils
    {
        public static string BonusStringFromValue(int val)
        {
            if (val == 0)
                return "";
            return (val > 0 ? " |cff00ff00+ " : " |cffff0000- ") + val + "|r";
        }
        public static string BonusStringFromValue(float val)
        {
            return BonusStringFromValue(War3Api.Common.R2I(val));
        }

    }
}