using Yuggenda.Application.Abstractions.Authentication;
using Yuggenda.Application.Abstractions.Persistence;
using Yuggenda.Application.DTOs.Users;
using Yuggenda.Application.Exceptions;

namespace Yuggenda.Application.Services.Users;

public class UserService
{
    private readonly ICurrentUser _currentUser;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ISessionRepository _sessionRepository;

    public UserService(
        ICurrentUser currentUser,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        ISessionRepository sessionRepository
    )
    {
        _currentUser = currentUser;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _sessionRepository = sessionRepository;
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

    public async Task<UserResponse> UpdateCurrentUserAsync(
        UpdateCurrentUserRequest request,
        CancellationToken cancellationToken
    )
    {
        var user = await _userRepository.GetByIdAsync(
            _currentUser.UserId,
            cancellationToken
        );

        if (user is null)
        {
            throw new UnauthorizedAccessException("Invalid user identity.");
        }

        string? normalizedEmail = null;

        if (request.Email is not null)
        {
            normalizedEmail = request.Email.Trim().ToLowerInvariant();

            var existingUser = await _userRepository.GetByEmailAsync(
                normalizedEmail,
                cancellationToken
            );

            if (existingUser is not null && existingUser.Id != user.Id)
            {
                throw new ConflictException(
                    "Unable to update the user with the provided information."
                );
            }
        }

        user.UpdateProfile(
            normalizedEmail,
            request.FirstName,
            request.LastName
        );

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new UserResponse
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task ChangePasswordAsync(
        ChangePasswordRequest request,
        CancellationToken cancellationToken
    )
    {
        var user = await _userRepository.GetByIdAsync(
            _currentUser.UserId,
            cancellationToken
        );

        if (user is null)
        {
            throw new UnauthorizedAccessException("Invalid user identity.");
        }

        if (!_passwordHasher.Verify(
            request.CurrentPassword,
            user.PasswordHash))
        {
            throw new UnauthorizedAccessException(
                "Invalid current password."
            );
        }

        var newPasswordHash = _passwordHasher.Hash(
            request.NewPassword);

        user.ChangePassword(newPasswordHash);

        await _sessionRepository.RevokeAllByUserIdAsync(
            user.Id,
            cancellationToken
        );

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}