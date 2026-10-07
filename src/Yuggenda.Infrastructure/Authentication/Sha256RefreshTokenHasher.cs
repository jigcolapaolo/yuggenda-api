using System.Security.Cryptography;
using System.Text;
using Yuggenda.Application.Abstractions.Authentication;

namespace Yuggenda.Infrastructure.Authentication;

public class Sha256RefreshTokenHasher : IRefreshTokenHasher
{
    public string Hash(string refreshToken)
    {
        var bytes = SHA256.HashData(
            Encoding.UTF8.GetBytes(refreshToken));

        return Convert.ToHexString(bytes);
    }

    public bool Verify(string refreshToken, string hash)
    {
        var computedHash = Hash(refreshToken);

        return CryptographicOperations.FixedTimeEquals(
            Convert.FromHexString(computedHash),
            Convert.FromHexString(hash));
    }
}