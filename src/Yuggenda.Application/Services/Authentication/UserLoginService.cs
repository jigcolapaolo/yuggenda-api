using Yuggenda.Application.Abstractions.Authentication;
using Yuggenda.Application.Abstractions.Persistence;
using Yuggenda.Application.DTOs.Authentication;
using Yuggenda.Domain.Entities;

namespace Yuggenda.Application.Services.Authentication;

public class UserLoginService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IAccessTokenGenerator _accessTokenGenerator;
    private readonly IRefreshTokenGenerator _refreshTokenGenerator;
    private readonly IRefreshTokenHasher _refreshTokenHasher;
    private readonly ISessionRepository _sessionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly AuthenticationOptions _authenticationOptions;

    public UserLoginService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IAccessTokenGenerator accessTokenGenerator,
        IRefreshTokenGenerator refreshTokenGenerator,
        IRefreshTokenHasher refreshTokenHasher,
        ISessionRepository sessionRepository,
        IUnitOfWork unitOfWork,
        AuthenticationOptions authenticationOptions)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _accessTokenGenerator = accessTokenGenerator;
        _refreshTokenGenerator = refreshTokenGenerator;
        _refreshTokenHasher = refreshTokenHasher;
        _sessionRepository = sessionRepository;
        _unitOfWork = unitOfWork;
        _authenticationOptions = authenticationOptions;
    }

    public async Task<LoginResponse> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken
    )
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);

        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        var accessToken = _accessTokenGenerator.Generate(user.Id, user.Email);

        var refreshToken = _refreshTokenGenerator.Generate();
        var refreshTokenHash = _refreshTokenHasher.Hash(refreshToken);

        var session = new Session(
            user.Id,
            refreshTokenHash,
            DateTime.UtcNow.AddDays(_authenticationOptions.RefreshTokenLifetimeDays)
        );

        await _sessionRepository.AddAsync(session, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            UserId = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName
        };
    }
}