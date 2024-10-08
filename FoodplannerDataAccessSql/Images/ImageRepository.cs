
using Dapper;
using FoodplannerModels.Images;
using Microsoft.AspNetCore.Connections;

namespace FoodplannerDataAccessSql.Images;

public class ImageRepository(PostgreSQLConnectionFactory connectionFactory) : IImageRepository
{

 


    public async Task<IEnumerable<FoodImageDTO>> GetAllImagesAsync()
    {
        var sql = "SELECT * FROM food_images";
        using (var connection = connectionFactory.Create())
        {
            connection.Open();
            var result = await connection.QueryAsync<FoodImageDTO>(sql);
            return result.ToList();
        }
    }

    public async Task<FoodImageDTO?> GetImageByIdAsync(int id)
    {
        throw new NotImplementedException();
        
    }

    public async Task<FoodImageDTO> SaveImageAsync(FoodImageDTO foodImage)
    {
        var sql = ''

        throw new NotImplementedException();
    }

    public async Task<int> UpdateImageAsync(FoodImageDTO image)
    {
        throw new NotImplementedException();
    }

    public async Task<int> DeleteImageAsync(int id)
    {
        throw new NotImplementedException();
    }
    
    
    
    

}