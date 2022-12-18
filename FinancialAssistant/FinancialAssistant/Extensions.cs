namespace FinancialAssistant
{
    public static class Extensions
    {
        public static bool IsNull(object? obj)
        {
            if (obj == null)
                return true;
            return false;
        }
    }
}
