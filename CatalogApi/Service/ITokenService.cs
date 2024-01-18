using CatalogApi.Model;

namespace CatalogApi.Service
{
    public interface ITokenService
    {
        string GenerateToken(string key, string issuer, string audience, User user);
    }
}
