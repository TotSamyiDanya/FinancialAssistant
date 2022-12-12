using Newtonsoft.Json;

namespace FinancialAssistant.Models
{
    public class TradeDate
    {
        public Guid Id { get; set; }
        public string? Date { get; set; }
        public double? Price { get; set; }
        [JsonIgnore] public virtual Tool? Tool { get; set; }
    }
}
