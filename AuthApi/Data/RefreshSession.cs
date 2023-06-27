namespace AuthApi.Data;

public sealed class RefreshSession
{
    public RefreshSession()
    {
        
    }

    public RefreshSession(string fingerprint, DateTime expireDate, int userId)
    {
        Fingerprint = fingerprint;
        ExpireDate = expireDate;
        UserId = userId;
    }

    public long Id { get; set; }
    public string Fingerprint { get; set; } = null!;
    public DateTime ExpireDate { get; set; }
    public int UserId { get; set; }
}