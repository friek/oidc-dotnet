using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Distributed;

namespace OpenIDConnectPlayground;

/// <summary>
/// Our own naive implementation of a session ticket store  
/// </summary>
public class TicketStore : ITicketStore
{
    // Any key is ok, as long as it doesn't conflict with other values in the distributed cache
    private const string SessionKey = "localsession";
    
    private readonly IDistributedCache _cache;
    private readonly TicketSerializer _ticketSerializer = TicketSerializer.Default;

    public TicketStore(IDistributedCache cache)
    {
        _cache = cache;
    }

    public Task<string> StoreAsync(AuthenticationTicket ticket)
    {
        _cache.SetAsync(SessionKey, _ticketSerializer.Serialize(ticket));

        return Task.FromResult(SessionKey);
    }

    public Task RenewAsync(string key, AuthenticationTicket ticket)
    {
        return StoreAsync(ticket);
    }

    public Task<AuthenticationTicket?> RetrieveAsync(string key)
    {
        var authTicket = _cache.Get(SessionKey);
        if (authTicket is null)
        {
            return null!;
        }
        
        return Task.FromResult(_ticketSerializer.Deserialize(authTicket));
    }

    public Task RemoveAsync(string key)
    {
        return _cache.RemoveAsync(key);
    }
}