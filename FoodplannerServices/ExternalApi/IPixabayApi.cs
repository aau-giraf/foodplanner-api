using FoodplannerModels.Image;

namespace FoodplannerServices.ExternalApi;

    public interface IPixabayApi
    {
        Task<Pixabay> SearchImages(
            string language,
            string searchTerm,
            int perPage,
            string imageType,
            string order,
            string category);
    }
