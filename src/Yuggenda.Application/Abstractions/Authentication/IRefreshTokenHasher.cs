namespace Yuggenda.Application.Abstractions.Authentication;

public interface IRefreshTokenHasher
{
    string Hash(string refreshToken);

    bool Verify(string refreshToken, string hash);
}