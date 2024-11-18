namespace CarDealershipBOT._1._Models.Result
{
    public class SaleResult
    {
        public int Price { get; set; }
        public CarSaleResult Car { get; set; }
        public DateTime DateSold { get; set; }
    }
    public class CarSaleResult
    {
        public string Name { get; set; }
        public string Model { get; set; }
        public string Type { get; set; }
        public int Price { get; set; }
        public string SerialNumber { get; set; }
        public string Description { get; set; }
        public string ImageURL { get; set; }

    }
}
