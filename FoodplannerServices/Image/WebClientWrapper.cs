using System.Net;

namespace FoodplannerServices.Image;

public class WebClientWrapper : IWebClientWrapper
{
    public byte[] DownloadData(string address)
    {
        using var client = new WebClient();
        return client.DownloadData(address);
    }
}