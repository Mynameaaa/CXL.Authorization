using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using System.Globalization;
using System.Runtime.ExceptionServices;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text;

namespace _003_JWT.相关源码
{
    //     JWT-bearer based authentication.
    public class JwtBearerHandler : AuthenticationHandler<JwtBearerOptions>
    {
        private OpenIdConnectConfiguration _configuration;

        //
        // 摘要:
        //     The handler calls methods on the events which give the application control at
        //     certain points where processing is occurring. If it is not provided a default
        //     instance is supplied which does nothing when the methods are called.
        protected new JwtBearerEvents Events
        {
            get
            {
                return (JwtBearerEvents)base.Events;
            }
            set
            {
                base.Events = value;
            }
        }

        //
        // 摘要:
        //     Initializes a new instance of Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler.
        public JwtBearerHandler(IOptionsMonitor<JwtBearerOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task<object> CreateEventsAsync()
        {
            return Task.FromResult((object)new JwtBearerEvents());
        }

        //
        // 摘要:
        //     Searches the 'Authorization' header for a 'Bearer' token. If the 'Bearer' token
        //     is found, it is validated using Microsoft.IdentityModel.Tokens.TokenValidationParameters
        //     set in the options.
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            AuthenticateResult result = default(AuthenticateResult);
            try
            {
                MessageReceivedContext messageReceivedContext = new MessageReceivedContext(base.Context, base.Scheme, base.Options);
                await Events.MessageReceived(messageReceivedContext);
                if (messageReceivedContext.Result != null)
                {
                    result = messageReceivedContext.Result;
                    return result;
                }

                string token = messageReceivedContext.Token;
                if (string.IsNullOrEmpty(token))
                {
                    string text = base.Request.Headers.Authorization;
                    if (string.IsNullOrEmpty(text))
                    {
                        result = AuthenticateResult.NoResult();
                        return result;
                    }

                    if (text.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    {
                        token = text.Substring("Bearer ".Length).Trim();
                    }

                    if (string.IsNullOrEmpty(token))
                    {
                        result = AuthenticateResult.NoResult();
                        return result;
                    }
                }

                if (_configuration == null && base.Options.ConfigurationManager != null)
                {
                    _configuration = await base.Options.ConfigurationManager.GetConfigurationAsync(base.Context.RequestAborted);
                }

                TokenValidationParameters tokenValidationParameters = base.Options.TokenValidationParameters.Clone();
                if (_configuration != null)
                {
                    string[] array = new string[1] { _configuration.Issuer };
                    tokenValidationParameters.ValidIssuers = tokenValidationParameters.ValidIssuers?.Concat(array) ?? array;
                    tokenValidationParameters.IssuerSigningKeys = tokenValidationParameters.IssuerSigningKeys?.Concat(_configuration.SigningKeys) ?? _configuration.SigningKeys;
                }

                List<Exception> list = null;
                SecurityToken validatedToken = null;
                foreach (ISecurityTokenValidator securityTokenValidator in base.Options.SecurityTokenValidators)
                {
                    if (!securityTokenValidator.CanReadToken(token))
                    {
                        continue;
                    }

                    ClaimsPrincipal principal;
                    try
                    {
                        principal = securityTokenValidator.ValidateToken(token, tokenValidationParameters, out validatedToken);
                    }
                    catch (Exception ex)
                    {
                        base.Logger.TokenValidationFailed(ex);
                        if (base.Options.RefreshOnIssuerKeyNotFound && base.Options.ConfigurationManager != null && ex is SecurityTokenSignatureKeyNotFoundException)
                        {
                            base.Options.ConfigurationManager.RequestRefresh();
                        }

                        if (list == null)
                        {
                            list = new List<Exception>(1);
                        }

                        list.Add(ex);
                        continue;
                    }

                    base.Logger.TokenValidationSucceeded();
                    TokenValidatedContext tokenValidatedContext = new TokenValidatedContext(base.Context, base.Scheme, base.Options)
                    {
                        Principal = principal,
                        SecurityToken = validatedToken
                    };
                    tokenValidatedContext.Properties.ExpiresUtc = GetSafeDateTime(validatedToken.ValidTo);
                    tokenValidatedContext.Properties.IssuedUtc = GetSafeDateTime(validatedToken.ValidFrom);
                    await Events.TokenValidated(tokenValidatedContext);
                    if (tokenValidatedContext.Result != null)
                    {
                        result = tokenValidatedContext.Result;
                        return result;
                    }

                    if (base.Options.SaveToken)
                    {
                        tokenValidatedContext.Properties.StoreTokens(new AuthenticationToken[1]
                        {
                        new AuthenticationToken
                        {
                            Name = "access_token",
                            Value = token
                        }
                        });
                    }

                    tokenValidatedContext.Success();
                    result = tokenValidatedContext.Result;
                    return result;
                }

                if (list != null)
                {
                    AuthenticationFailedContext authenticationFailedContext2 = new AuthenticationFailedContext(base.Context, base.Scheme, base.Options)
                    {
                        Exception = ((list.Count == 1) ? list[0] : new AggregateException(list))
                    };
                    await Events.AuthenticationFailed(authenticationFailedContext2);
                    if (authenticationFailedContext2.Result != null)
                    {
                        result = authenticationFailedContext2.Result;
                        return result;
                    }

                    result = AuthenticateResult.Fail(authenticationFailedContext2.Exception);
                    return result;
                }

                result = AuthenticateResult.Fail("No SecurityTokenValidator available for token.");
                return result;
            }
            catch (Exception ex2)
            {
                Exception ex3 = ex2;
                base.Logger.ErrorProcessingMessage(ex3);
                AuthenticationFailedContext authenticationFailedContext2 = new AuthenticationFailedContext(base.Context, base.Scheme, base.Options)
                {
                    Exception = ex3
                };
                await Events.AuthenticationFailed(authenticationFailedContext2);
                if (authenticationFailedContext2.Result != null)
                {
                    return authenticationFailedContext2.Result;
                }

                ExceptionDispatchInfo.Capture((ex2 as Exception) ?? throw ex2).Throw();
            }

            return result;
        }

        private static DateTime? GetSafeDateTime(DateTime dateTime)
        {
            if (dateTime == DateTime.MinValue)
            {
                return null;
            }

            return dateTime;
        }

        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            AuthenticateResult authenticateResult = await HandleAuthenticateOnceSafeAsync();
            JwtBearerChallengeContext eventContext = new JwtBearerChallengeContext(base.Context, base.Scheme, base.Options, properties)
            {
                AuthenticateFailure = authenticateResult?.Failure
            };
            if (base.Options.IncludeErrorDetails && eventContext.AuthenticateFailure != null)
            {
                eventContext.Error = "invalid_token";
                eventContext.ErrorDescription = CreateErrorDescription(eventContext.AuthenticateFailure);
            }

            await Events.Challenge(eventContext);
            if (eventContext.Handled)
            {
                return;
            }

            base.Response.StatusCode = 401;
            if (string.IsNullOrEmpty(eventContext.Error) && string.IsNullOrEmpty(eventContext.ErrorDescription) && string.IsNullOrEmpty(eventContext.ErrorUri))
            {
                base.Response.Headers.Append(HeaderNames.WWWAuthenticate, base.Options.Challenge);
                return;
            }

            StringBuilder stringBuilder = new StringBuilder(base.Options.Challenge);
            if (base.Options.Challenge.IndexOf(' ') > 0)
            {
                stringBuilder.Append(',');
            }

            if (!string.IsNullOrEmpty(eventContext.Error))
            {
                stringBuilder.Append(" error=\"");
                stringBuilder.Append(eventContext.Error);
                stringBuilder.Append('"');
            }

            if (!string.IsNullOrEmpty(eventContext.ErrorDescription))
            {
                if (!string.IsNullOrEmpty(eventContext.Error))
                {
                    stringBuilder.Append(',');
                }

                stringBuilder.Append(" error_description=\"");
                stringBuilder.Append(eventContext.ErrorDescription);
                stringBuilder.Append('"');
            }

            if (!string.IsNullOrEmpty(eventContext.ErrorUri))
            {
                if (!string.IsNullOrEmpty(eventContext.Error) || !string.IsNullOrEmpty(eventContext.ErrorDescription))
                {
                    stringBuilder.Append(','); 
                }

                stringBuilder.Append(" error_uri=\"");
                stringBuilder.Append(eventContext.ErrorUri);
                stringBuilder.Append('"');
            }

            base.Response.Headers.Append(HeaderNames.WWWAuthenticate, stringBuilder.ToString());
        }

        protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
        {
            ForbiddenContext context = new ForbiddenContext(base.Context, base.Scheme, base.Options);
            base.Response.StatusCode = 403;
            return Events.Forbidden(context);
        }

        private static string CreateErrorDescription(Exception authFailure)
        {
            IReadOnlyCollection<Exception> readOnlyCollection = ((!(authFailure is AggregateException ex)) ? ((IReadOnlyCollection<Exception>)(object)new Exception[1] { authFailure }) : ((IReadOnlyCollection<Exception>)ex.InnerExceptions));
            List<string> list = new List<string>(readOnlyCollection.Count);
            foreach (Exception item in readOnlyCollection)
            {
                if (!(item is SecurityTokenInvalidAudienceException ex2))
                {
                    if (!(item is SecurityTokenInvalidIssuerException ex3))
                    {
                        if (!(item is SecurityTokenNoExpirationException))
                        {
                            if (!(item is SecurityTokenInvalidLifetimeException ex4))
                            {
                                if (!(item is SecurityTokenNotYetValidException ex5))
                                {
                                    if (!(item is SecurityTokenExpiredException ex6))
                                    {
                                        if (!(item is SecurityTokenSignatureKeyNotFoundException))
                                        {
                                            if (item is SecurityTokenInvalidSignatureException)
                                            {
                                                list.Add("The signature is invalid");
                                            }
                                        }
                                        else
                                        {
                                            list.Add("The signature key was not found");
                                        }
                                    }
                                    else
                                    {
                                        list.Add("The token expired at '" + ex6.Expires.ToString(CultureInfo.InvariantCulture) + "'");
                                    }
                                }
                                else
                                {
                                    list.Add("The token is not valid before '" + ex5.NotBefore.ToString(CultureInfo.InvariantCulture) + "'");
                                }
                            }
                            else
                            {
                                list.Add("The token lifetime is invalid; NotBefore: '" + (ex4.NotBefore?.ToString(CultureInfo.InvariantCulture) ?? "(null)") + "', Expires: '" + (ex4.Expires?.ToString(CultureInfo.InvariantCulture) ?? "(null)") + "'");
                            }
                        }
                        else
                        {
                            list.Add("The token has no expiration");
                        }
                    }
                    else
                    {
                        list.Add("The issuer '" + (ex3.InvalidIssuer ?? "(null)") + "' is invalid");
                    }
                }
                else
                {
                    list.Add("The audience '" + (ex2.InvalidAudience ?? "(null)") + "' is invalid");
                }
            }

            return string.Join("; ", list);
        }
    }
}
