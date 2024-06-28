using CarRentalAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using RetakeTest1.DTO_s;

namespace RetakeTest1.Controllers;
[ApiController]
public class CarsRentalsController : ControllerBase
{
    private CarsAndRentalsRepo _carsAndRentalsRepo;

    public CarsRentalsController(CarsAndRentalsRepo carsAndRentalsRepo)
    {
        _carsAndRentalsRepo = carsAndRentalsRepo;
    }

    [HttpGet("api/clients/{clientID}")]
    public async Task<IActionResult> GetById(int clientID)
    {
       var CarWithRentals= _carsAndRentalsRepo.GetClientWithRentalsAsync(clientID);

       return Ok(CarWithRentals);
    }
    
    [HttpPost("api/clients")]
    public async Task<IActionResult> AddClientWithRental(ClientAndRentalDTO clientAndRentalDTO)
    {
       
            var clientId = await _carsAndRentalsRepo.AddClientWithRentalAsync(clientAndRentalDTO);
            return Ok(new { ClientId = clientId });
        
        
    }
}