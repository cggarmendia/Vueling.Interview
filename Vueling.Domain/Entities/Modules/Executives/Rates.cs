namespace Vueling.Domain.Entities.Modules.Executives
{
    public class Rates : IEntity
    {
        public string From { get; set; }
        public string To { get; set; }
        public decimal Rate { get; set; }
    }
}
