using CarRentalAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using RetakeTest1.DTO_s;

namespace RetakeTest1.Controllers;
[ApiController]
public class CarsRentalsController : ControllerBase
{
    private ClientsAndRentalsRepo _clientsAndRentalsRepo;

    public CarsRentalsController(ClientsAndRentalsRepo clientsAndRentalsRepo)
    {
        _clientsAndRentalsRepo = clientsAndRentalsRepo;
    }

    [HttpGet("api/clients/{clientID}")]
    public async Task<IActionResult> GetById(int clientID)
    {
       var CarWithRentals= _clientsAndRentalsRepo.GetClientWithRentalsAsync(clientID);

       return Ok(CarWithRentals);
    }
    
    [HttpPost("api/clients")]
    public async Task<IActionResult> AddClientWithRental(ClientAndRentalDTO clientAndRentalDTO)
    {
       
            var clientId = await _clientsAndRentalsRepo.AddClientWithRentalAsync(clientAndRentalDTO);
            return Ok(new { ClientId = clientId });
        
        
    }
}