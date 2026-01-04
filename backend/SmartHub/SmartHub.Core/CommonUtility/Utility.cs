namespace SmartHub.Core.CommonUtility
{
    public static class Utility
    {
        public static bool IsPasswordValid(string password)
        {
            if (password.Length < 8) return false;
            if (!password.Any(char.IsLetter)) return false;
            if (!password.Any(char.IsDigit)) return false;
            return true;
        }
    }
}
