using Newtonsoft.Json;

namespace RetakeTest1.DTO_s;

public class GetClientsWithRentals
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Address { get; set; }
    public List<CarsRentals> CarsRentalsList { get; set; }

    public class CarsRentals
    {
        public string Vin { get; set; }
        public string Color { get; set; }
        public string Model { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public int TotalPrice { get; set; }
    }
}