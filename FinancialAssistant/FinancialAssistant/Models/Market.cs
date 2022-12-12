using Newtonsoft.Json;

namespace FinancialAssistant.Models
{
    public class Market
    {
        public Guid Id { get; set; }
        public string? MarketName { get; set; }
        [JsonIgnore] public virtual ICollection<Board>? Boards { get; set; }
        [JsonIgnore] public virtual ICollection<Tool>? Tools { get; set; }
    }
}
