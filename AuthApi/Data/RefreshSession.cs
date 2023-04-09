namespace AuthApi.Data;

public sealed class RefreshSession
{
    public long Id { get; set; }
    public string Fingerprint { get; set; } = null!;
    public DateTime ExpireDate { get; set; }
    public int UserId { get; set; }

    public RefreshSession()
    {
        
    }

    public RefreshSession(string fingeprint, DateTime expireDate, int userId)
    {
        Fingerprint = fingeprint;
        ExpireDate = expireDate;
        UserId = userId;
    }
}