using EC_V2.Services.Interfaces;

namespace EC_V2.Services.Implementations
{
    public class TokenBlacklistService: ITokenBlacklistService
    {
        private readonly Dictionary<string, DateTime> _blacklist = new();
        private readonly ILogger<TokenBlacklistService> _logger;

        public TokenBlacklistService(ILogger<TokenBlacklistService> logger)
        {
            _logger = logger;
        }

        public void BlacklistToken(string jti, DateTime expiry)
        {
            _logger.LogInformation("Blacklisting token with JTI: {Jti}", jti);
            _blacklist[jti] = expiry;
        }

        public bool IsBlacklisted(string jti)
        {
            if (_blacklist.TryGetValue(jti, out var expiry))
            {
                if (expiry > DateTime.UtcNow)
                {
                    _logger.LogInformation("Token {Jti} is blacklisted", jti);
                    return true;
                }
                _blacklist.Remove(jti);
            }
            return false;
        }
    }
}
