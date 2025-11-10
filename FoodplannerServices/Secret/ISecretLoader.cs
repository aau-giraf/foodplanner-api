namespace FoodplannerServices.Secret;

public interface ISecretLoader
{
   public string GetSecret(string secretName, string path = "/"); 
}
