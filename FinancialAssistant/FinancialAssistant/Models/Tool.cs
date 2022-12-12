using System.Text.Json.Serialization;

namespace FinancialAssistant.Models
{
    public class Tool
    {
        public Guid Id { get; set; }
        public string? ToolName { get; set; }
        public string? SecId { get; set; }
        public virtual Market? Market { get; set; }
        public virtual Board? Board { get; set; }
        public virtual ICollection<TradeDate>? TradeDates { get; set; }
    }
}
