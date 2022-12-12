using Newtonsoft.Json;

namespace FinancialAssistant.Models
{
    public class Board
    {
        public Guid Id { get; set; }
        public string? BoardName { get; set; }
        [JsonIgnore]public virtual Market? Market { get; set; }
        [JsonIgnore] public virtual ICollection<Tool>? Tools { get; set; }
    }
}
