namespace EC_V2.Services.Interfaces
{
    public interface ITokenBlacklistService
    {
        void BlacklistToken(string jti, DateTime expiry);
        bool IsBlacklisted(string jti);
    }
}
