namespace CapitalBroker.ViewModels
{
    public class UpdateTradingWorkingOrder
    {
        public int? level { get; set; }
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
