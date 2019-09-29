using System;
using com.b_velop.App.IdentityProvider;
using Microsoft.Extensions.Options;
using com.b_velop.stack.Air.BL;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;
using com.b_velop.App.IdentityProvider.Model;
using com.b_velop.Home.Classes;

namespace com.b_velop.stack.Air.Services
{
    public class HttpUploadService
    {
        private readonly HttpClient _client;
        private readonly ILogger<HttpUploadService> _logger;
        private readonly IIdentityProviderService _identity;
        private readonly ApiSecret _secret;
        private Token _token;
        private DateTimeOffset _expires;

        public HttpUploadService(
            HttpClient client,
            ILogger<HttpUploadService> logger,
            IIdentityProviderService identity,
            IOptions<ApiSecret> secret)
        {
            _client = client;
            _client.Timeout = TimeSpan.FromSeconds(5);
            _logger = logger;
            _identity = identity;
            _secret = secret.Value;
        }

        public async Task<bool> TryRequestTokenAsync()
        {
            try
            {
                if (_token != null && _expires > DateTimeOffset.Now)
                    return true;

                var infoItem = new InfoItem(
                    _secret.ClientId,
                    _secret.ClientSecret,
                    _secret.Scope,
                    _secret.AuthorityUrl);

                _token = await _identity.GetTokenAsync(infoItem);
                if (_token == null)
                {
                    _logger.LogWarning(2432, $"Error occurred while request token Client: '{infoItem.ClientId}', Scope '{infoItem.Scope}'.");
                    return false;
                }
                _expires = DateTimeOffset.Now.AddSeconds(_token.ExpiresIn - 60);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(2432, ex, $"Error occurred while request token.");
                return false;
            }
        }

        public async Task<bool> UploadValuesAsync(
            Airdata values)
        {
            try
            {
                var success = await TryRequestTokenAsync();
                if (!success)
                    return false;
                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(2432, ex, $"Error occurred while uploading Air values '{values}'.", values);
                return false;
            }
        }
    }
}
