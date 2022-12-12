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

        
    }
}
