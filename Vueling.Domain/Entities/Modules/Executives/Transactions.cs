namespace Vueling.Domain.Entities.Modules.Executives
{
    public class Transactions : IEntity
    {
        public int Id { get; set; }
        public string Sku { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
    }
}
