
using Dapper;
using FoodplannerModels.Images;
using Microsoft.AspNetCore.Connections;

namespace FoodplannerDataAccessSql.Images;

public class ImageRepository(PostgreSQLConnectionFactory connectionFactor) : IImageRepository
{
    private readonly PostgreSQLConnectionFactory _connectionFactory;

    public ImagesRepository(PostgreSQLConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }


    public async Task<IEnumerable<FoodImageDTO>> GetAllAsync()
    {
        var sql = "Select * from food_images";
        using (var connection = _connectionFactory.Create())
        {
            connection.Open();
            var result = await connection.QueryAsync(<FoodImageDTO>(sql));
            return result.ToList();
        }
    }
    
    public async Task<int> SaveImageAync(int )
    
    
    
    

}