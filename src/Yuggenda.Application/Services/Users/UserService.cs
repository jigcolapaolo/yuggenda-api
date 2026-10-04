using Yuggenda.Application.Abstractions.Authentication;
using Yuggenda.Application.Abstractions.Persistence;
using Yuggenda.Application.DTOs.Users;

namespace Yuggenda.Application.Services.Users;

public class UserService
{
    private readonly ICurrentUser _currentUser;
    private readonly IUserRepository _userRepository;

    public UserService(
        ICurrentUser currentUser,
        IUserRepository userRepository
    )
    {
        _currentUser = currentUser;
        _userRepository = userRepository;
    }

    public async Task<UserResponse> GetCurrentUserAsync(
        CancellationToken cancellationToken
    )
    {
        var user = await _userRepository.GetByIdAsync(_currentUser.UserId, cancellationToken);

        if (user is null)
        {
            throw new UnauthorizedAccessException("Invalid user identity.");
        }

        return new UserResponse
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            CreatedAt = user.CreatedAt
        };
    }
}