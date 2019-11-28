using com.b_velop.stack.DataContext.Entities;
using com.b_velop.stack.DataContext.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace com.b_velop.Stack.Air.Server.Bl
{
    public class UserService : IUserService
    {
        private IRepositoryWrapper _rep;
        private ILogger<UserService> _logger;

        public UserService(
            IRepositoryWrapper rep,
            ILogger<UserService> logger)
        {
            _rep = rep;
            _logger = logger;
        }

        public async Task<AirUser> Authenticate(
            string username,
            string password)
        {
            Expression<Func<AirUser, bool>> expression = user => user.Username == username && user.Password == password;

            var user = (await _rep.AirUser.SelectByConditionAsync(expression))?.FirstOrDefault();

            // return null if user not found
            if (user == null)
                return null;

            // authentication successful so return user details without password
            return user.WithoutPassword();
        }

        public async Task<IEnumerable<AirUser>> GetAll()
            => (await _rep.AirUser.SelectAllAsync()).WithoutPasswords();
    }
    public class User
    {

    }

    public interface IUserService
    {
        Task<AirUser> Authenticate(string unsername, string password);
        Task<IEnumerable<AirUser>> GetAll();
    }

    public class AuthenticateModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }

    public static class ExtensionMethods
    {
        public static IEnumerable<AirUser> WithoutPasswords(this IEnumerable<AirUser> users)
        {
            return users.Select(_ => _.WithoutPassword());
        }

        public static AirUser WithoutPassword(this AirUser user)
        {
            user.Password = null;
            return user;
        }
    }
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IUserService _userService;
        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IUserService userService) : base(options, logger, encoder, clock)
        {
            _userService = userService;
        }

        protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
                return AuthenticateResult.Fail("Missing Authorization Header");

            AirUser user = null;
            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(new[] { ':' }, 2);
                var username = credentials[0];
                var password = credentials[1];
                user = await _userService.Authenticate(username, password);
            }
            catch
            {
                return AuthenticateResult.Fail("Invalid Authorization Header");
            }

            if (user == null)
                return AuthenticateResult.Fail("Invalid Username or Password");

            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
            };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }
}
