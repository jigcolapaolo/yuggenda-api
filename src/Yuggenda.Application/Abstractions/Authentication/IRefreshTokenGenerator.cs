namespace Yuggenda.Application.Abstractions.Authentication;

public interface IRefreshTokenGenerator
{
    string Generate();
}