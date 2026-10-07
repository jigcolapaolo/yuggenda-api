namespace Yuggenda.Application.Abstractions.Authentication;

public interface IAccessTokenGenerator
{
    string Generate(Guid userId, string email);
}