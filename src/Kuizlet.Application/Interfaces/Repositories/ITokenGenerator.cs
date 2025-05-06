

namespace Kuizlet.Application.Interfaces.Repositories
{
    public interface ITokenGenerator
    {
        string GenerateToken(string username);
    }
}
