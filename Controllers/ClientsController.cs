using CarRentalAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using RetakeTest1.DTO_s;

namespace RetakeTest1.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ClientsController : ControllerBase
{
    private ClientsAndRentalsRepo _carsAndRentalsRepo;

    public ClientsController(ClientsAndRentalsRepo carsAndRentalsRepo)
    {
        _carsAndRentalsRepo = carsAndRentalsRepo;
    }

    [HttpGet("{clientID}")]
    public async Task<IActionResult> GetById(int clientID)
    {
       var CarWithRentals= _carsAndRentalsRepo.GetClientWithRentalsAsync(clientID);

       return Ok(CarWithRentals);
    }
    
    [HttpPost]
    public async Task<IActionResult> AddClientWithRental(ClientAndRentalDTO clientAndRentalDTO)
    {
       
            var clientId = await _carsAndRentalsRepo.AddClientWithRentalAsync(clientAndRentalDTO);
            return Ok(new { ClientId = clientId });
        
        
    }
}