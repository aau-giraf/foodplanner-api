namespace FoodplannerServices.Secret;

// Interface for secret loader
public interface ISecretLoader
{
   public string GetSecret(string secretName, string path = "/"); 
}
