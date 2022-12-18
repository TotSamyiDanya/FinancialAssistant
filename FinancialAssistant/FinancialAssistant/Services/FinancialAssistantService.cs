namespace FinancialAssistant.Services
{
    public class FinancialAssistantService
    {
        private static FinancialAssistantService instance;
        private FinancialAssistantService()
        {

        }
        public static FinancialAssistantService getInstance()
        {
            if (instance == null)
            {
                instance = new FinancialAssistantService();
            }
            return instance;
        }
        public override bool Equals(object? obj)
        {
            return base.Equals(obj);
        }
    }
}
