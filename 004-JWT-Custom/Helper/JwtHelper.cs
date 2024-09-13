//using System;
//using System.IdentityModel.Tokens.Jwt;
//using System.IO;
//using System.Security.Claims;
//using System.Security.Cryptography;
//using Microsoft.AspNetCore.Authentication;
//using Microsoft.IdentityModel.Tokens;

//namespace _004_JWT_Custom;
//public class JWTHelper
//{
//    private readonly string _privateKeyPath;
//    private readonly string _publicKeyPath;

//    public JWTHelper(string privateKeyPath, string publicKeyPath)
//    {
//        _privateKeyPath = privateKeyPath;
//        _publicKeyPath = publicKeyPath;
//    }

//    // Generate access and refresh token
//    public (string accessToken, string refreshToken) GenerateTokens(TokenDataModel tokenData)
//    {
//        var accessToken = GenerateToken(tokenData, expiresInMinutes: 15);  // Shorter expiration time for accessToken
//        var refreshToken = GenerateToken(tokenData, expiresInMinutes: 60 * 24 * 7);  // Longer expiration for refreshToken (e.g., 1 week)
//        return (accessToken, refreshToken);
//    }

//    // Generate a token (used for both access and refresh tokens)
//    private string GenerateToken(TokenDataModel tokenData, int expiresInMinutes)
//    {
//        var key = File.ReadAllBytes(_privateKeyPath);
//        var signingKey = new RsaSecurityKey(RSA.Create());
//        signingKey.ImportRSAPrivateKey(key, out _);

//        var tokenDescriptor = new SecurityTokenDescriptor
//        {
//            Subject = new ClaimsIdentity(new Claim[]
//            {
//                new Claim(JwtRegisteredClaimNames.Sub, tokenData.UserId.ToString()),
//                new Claim(JwtRegisteredClaimNames.UniqueName, tokenData.Username),
//                new Claim(ClaimTypes.Role, tokenData.Role)
//            }),
//            Expires = DateTime.UtcNow.AddMinutes(expiresInMinutes),
//            SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.RsaSha256)
//        };

//        var tokenHandler = new JwtSecurityTokenHandler();
//        var token = tokenHandler.CreateToken(tokenDescriptor);
//        return tokenHandler.WriteToken(token);
//    }

//    // Parse the token to get the payload (without validation)
//    public TokenDataModel ParseToken(string token)
//    {
//        var tokenHandler = new JwtSecurityTokenHandler();
//        var rsa = RSA.Create();
//        rsa.ImportSubjectPublicKeyInfo(File.ReadAllBytes(_publicKeyPath), out _);

//        var validationParameters = new TokenValidationParameters
//        {
//            ValidateIssuerSigningKey = true,
//            IssuerSigningKey = new RsaSecurityKey(rsa),
//            ValidateIssuer = false,
//            ValidateAudience = false,
//            ValidateLifetime = false  // Not validating lifetime here, just extracting payload
//        };

//        ClaimsPrincipal principal = tokenHandler.ValidateToken(token, validationParameters, out _);
//        var userId = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
//        var username = principal.FindFirst(JwtRegisteredClaimNames.UniqueName)?.Value;
//        var role = principal.FindFirst(ClaimTypes.Role)?.Value;

//        return new TokenDataModel
//        {
//            UserId = Convert.ToInt32(userId),
//            Username = username,
//            Role = role
//        };
//    }

//    // Validate the token (including signature and expiration)
//    public bool ValidateToken(string token, out TokenDataModel payload)
//    {
//        var tokenHandler = new JwtSecurityTokenHandler();
//        var rsa = RSA.Create();
//        rsa.ImportSubjectPublicKeyInfo(File.ReadAllBytes(_publicKeyPath), out _);

//        var validationParameters = new TokenValidationParameters
//        {
//            ValidateIssuerSigningKey = true,
//            IssuerSigningKey = new RsaSecurityKey(rsa),
//            ValidateIssuer = false,
//            ValidateAudience = false,
//            ValidateLifetime = true,  // Ensure token is not expired
//            ClockSkew = TimeSpan.Zero  // Remove the default clock skew
//        };

//        try
//        {
//            ClaimsPrincipal principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
//            var userId = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
//            var username = principal.FindFirst(JwtRegisteredClaimNames.UniqueName)?.Value;
//            var role = principal.FindFirst(ClaimTypes.Role)?.Value;

//            payload = new TokenDataModel
//            {
//                UserId = Convert.ToInt32(userId),
//                Username = username,
//                Role = role
//            };
//            return true;
//        }
//        catch (Exception)
//        {
//            payload = null;
//            return false;
//        }
//    }

//    // Generate new access token using the refresh token
//    public string RefreshAccessToken(string refreshToken)
//    {
//        if (ValidateToken(refreshToken, out TokenDataModel payload))
//        {
//            return GenerateToken(payload, expiresInMinutes: 15);
//        }
//        throw new SecurityTokenException("Invalid refresh token.");
//    }

//    // Generate a new RSA key pair and save to files
//    public static void GenerateKeyPair(string privateKeyPath, string publicKeyPath)
//    {
//        using (var rsa = RSA.Create(2048))
//        {
//            // Export private and public keys
//            File.WriteAllBytes(privateKeyPath, rsa.ExportRSAPrivateKey());
//            File.WriteAllBytes(publicKeyPath, rsa.ExportSubjectPublicKeyInfo());
//        }
//    }
//}
