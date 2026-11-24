namespace FoodplannerServices.Image;

public interface IWebClientWrapper
{
    byte[] DownloadData(string address);
}