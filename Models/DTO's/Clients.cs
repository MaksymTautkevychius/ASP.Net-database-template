using Newtonsoft.Json;

namespace RetakeTest1.DTO_s;

public class Clients
{
  
    public int id;
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string address { get; set; }
    
    public List<CarsRentals> CarsRentalsList;

    public class CarsRentals
    {
        public string vin { get; set; }
        public string color { get; set; }
        public string model { get; set; }
        public DateTime dateFrom {get; set; }
        public DateTime dateTo {get; set; }
        public int TotalPrice { get; set; }
    }
}