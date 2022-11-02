namespace CapitalBroker.ViewModels
{
    public class NewWorkingOrder
    {
        public string direction { get; set; }
        public string epic { get; set; }
        public int size { get; set; }
        public int level { get; set; }
        public string? goodTillDate { get; set; }
        public bool? guaranteedStop { get; set; }
        public bool? trailingStop { get; set; }
        public int? stopLevel { get; set; }
        public int? stopDistance { get; set; }
        public int? stopAmount { get; set; }
        public int? profitLevel { get; set; }
        public int? profitDistance { get; set; }
        public int? profitAmount { get; set; }
    }
}
