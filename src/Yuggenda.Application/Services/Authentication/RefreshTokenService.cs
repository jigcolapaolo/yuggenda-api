using Yuggenda.Application.Abstractions.Authentication;
using Yuggenda.Application.Abstractions.Persistence;
using Yuggenda.Application.DTOs.Authentication;
using Yuggenda.Domain.Entities;

namespace Yuggenda.Application.Services.Authentication;

public class RefreshTokenService
{
    private readonly IUserRepository _userRepository;
    private readonly ISessionRepository _sessionRepository;
    private readonly IRefreshTokenHasher _refreshTokenHasher;
    private readonly IRefreshTokenGenerator _refreshTokenGenerator;
    private readonly IAccessTokenGenerator _accessTokenGenerator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly AuthenticationOptions _authenticationOptions;

    public RefreshTokenService(
        IUserRepository userRepository,
        ISessionRepository sessionRepository,
        IRefreshTokenHasher refreshTokenHasher,
        IRefreshTokenGenerator refreshTokenGenerator,
        IAccessTokenGenerator accessTokenGenerator,
        IUnitOfWork unitOfWork,
        AuthenticationOptions authenticationOptions)
    {
        _userRepository = userRepository;
        _sessionRepository = sessionRepository;
        _refreshTokenHasher = refreshTokenHasher;
        _refreshTokenGenerator = refreshTokenGenerator;
        _accessTokenGenerator = accessTokenGenerator;
        _unitOfWork = unitOfWork;
        _authenticationOptions = authenticationOptions;
    }

    public async Task<LoginResponse> RefreshAsync(
        RefreshTokenRequest request,
        CancellationToken cancellationToken
    )
    {
        var refreshTokenHash = _refreshTokenHasher.Hash(request.RefreshToken);

        var session = await _sessionRepository
            .GetByRefreshTokenHashAsync(refreshTokenHash, cancellationToken);

        if (session is null ||
            session.RevokedAt is not null ||
            session.ExpiresAt <= DateTime.UtcNow
        )
        {
            throw new UnauthorizedAccessException("Invalid refresh token.");
        }

        var user = await _userRepository.GetByIdAsync(session.UserId, cancellationToken);

        if (user is null)
        {
            throw new UnauthorizedAccessException("Invalid refresh token.");
        }

        session.Revoke();

        var accessToken = _accessTokenGenerator.Generate(user.Id, user.Email);

        var newRefreshToken = _refreshTokenGenerator.Generate();

        var newRefreshTokenHash = _refreshTokenHasher.Hash(newRefreshToken);

        var newSession = new Session(
            user.Id,
            newRefreshTokenHash,
            DateTime.UtcNow.AddDays(_authenticationOptions.RefreshTokenLifetimeDays)
        );

        await _sessionRepository.AddAsync(newSession, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken,
            UserId = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName
        };
    }
}