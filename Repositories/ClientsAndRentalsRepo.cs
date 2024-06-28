using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;

using RetakeTest1.DTO_s;

namespace CarRentalAPI.Repositories
{
    public class ClientsAndRentalsRepo
    {
        private readonly IConfiguration _configuration;

        public ClientsAndRentalsRepo(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<GetClientsWithRentals> GetClientWithRentalsAsync(int id)
        {
            var query = @"
    SELECT 
        clients.ID,
        clients.FirstName,
        clients.LastName,
        clients.Address,
        cars.VIN,
        colors.Name AS Color,
        models.Name AS Model,
        car_rentals.DateFrom,
        car_rentals.DateTo,
        car_rentals.TotalPrice
    FROM clients
    INNER JOIN car_rentals ON clients.ID = car_rentals.ClientID
    INNER JOIN cars ON car_rentals.CarID = cars.ID
    INNER JOIN colors ON cars.ColorID = colors.ID
    INNER JOIN models ON cars.ModelID = models.ID
    WHERE clients.ID = @ID";

            using var sqlConnection = new SqlConnection(_configuration.GetConnectionString("Default"));
            using var sqlCommand = new SqlCommand(query, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@ID", id);

            GetClientsWithRentals getClientWithRentals = null;

            await sqlConnection.OpenAsync();
            using var reader = await sqlCommand.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                if (getClientWithRentals == null)
                {
                    getClientWithRentals = new GetClientsWithRentals
                    {
                        id = reader.GetInt32(reader.GetOrdinal("ID")),
                        FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                        LastName = reader.GetString(reader.GetOrdinal("LastName")),
                        address = reader.GetString(reader.GetOrdinal("Address")),
                        CarsRentalsList = new List<GetClientsWithRentals.CarsRentals>()
                    };
                }
                getClientWithRentals.CarsRentalsList.Add(new GetClientsWithRentals.CarsRentals
                {
                    vin = reader.GetString(reader.GetOrdinal("VIN")),
                    color = reader.GetString(reader.GetOrdinal("Color")),
                    model = reader.GetString(reader.GetOrdinal("Model")),
                    dateFrom = reader.GetDateTime(reader.GetOrdinal("DateFrom")),
                    dateTo = reader.GetDateTime(reader.GetOrdinal("DateTo")),
                    TotalPrice = reader.GetInt32(reader.GetOrdinal("TotalPrice"))
                });
            }

            return getClientWithRentals;
        }

       public async Task<int> AddClientWithRentalAsync(ClientAndRentalDTO clientAndRentalDTO)
{
    var insertClientQuery = @"
        INSERT INTO clients (FirstName, LastName, Address)
        VALUES (@FirstName, @LastName, @Address);
        SELECT SCOPE_IDENTITY();";

    var checkCarQuery = @"
        SELECT COUNT(*) 
        FROM cars 
        WHERE ID = @CarID";

    var insertRentalQuery = @"
        INSERT INTO car_rentals (ClientID, CarID, DateFrom, DateTo, TotalPrice)
        VALUES (@ClientID, @CarID, @DateFrom, @DateTo, @TotalPrice);";

    using var sqlConnection = new SqlConnection(_configuration.GetConnectionString("Default"));
    await sqlConnection.OpenAsync();

    using var transaction = sqlConnection.BeginTransaction();
    try
    {
     
        using var checkCarCommand = new SqlCommand(checkCarQuery, sqlConnection, transaction);
        checkCarCommand.Parameters.AddWithValue("@CarID", clientAndRentalDTO.ID);
        var carExists = (int)await checkCarCommand.ExecuteScalarAsync() > 0;

        if (!carExists)
        {
            throw new Exception("Car is not exist");
        }

        
        var clientID = clientAndRentalDTO.ID; 
        if (clientID == 0) 
        {
            using var insertClientCommand = new SqlCommand(insertClientQuery, sqlConnection, transaction);
            insertClientCommand.Parameters.AddWithValue("@FirstName", clientAndRentalDTO.FirstName);
            insertClientCommand.Parameters.AddWithValue("@LastName", clientAndRentalDTO.LastName);
            insertClientCommand.Parameters.AddWithValue("@Address", clientAndRentalDTO.Address);
            clientID = Convert.ToInt32(await insertClientCommand.ExecuteScalarAsync());
        }

    
        var totalDays = (clientAndRentalDTO.DateTo - clientAndRentalDTO.DateFrom).Days + 1;

        var getPricePerDayQuery = @"
            SELECT PricePerDay
            FROM cars
            WHERE ID = @CarID";

        using var getPricePerDayCommand = new SqlCommand(getPricePerDayQuery, sqlConnection, transaction);
        getPricePerDayCommand.Parameters.AddWithValue("@CarID", clientAndRentalDTO.ID);
        var pricePerDay = (int)await getPricePerDayCommand.ExecuteScalarAsync();
        var totalPrice = totalDays * pricePerDay;

        using var insertRentalCommand = new SqlCommand(insertRentalQuery, sqlConnection, transaction);
        insertRentalCommand.Parameters.AddWithValue("@ClientID", clientID);
        insertRentalCommand.Parameters.AddWithValue("@CarID", clientAndRentalDTO.ID);
        insertRentalCommand.Parameters.AddWithValue("@DateFrom", clientAndRentalDTO.DateFrom);
        insertRentalCommand.Parameters.AddWithValue("@DateTo", clientAndRentalDTO.DateTo);
        insertRentalCommand.Parameters.AddWithValue("@TotalPrice", totalPrice);

        await insertRentalCommand.ExecuteNonQueryAsync();
        transaction.Commit();

        return clientID;
    }
    catch (Exception)
    {
        transaction.Rollback();
        throw;
    }
}

    }
}
