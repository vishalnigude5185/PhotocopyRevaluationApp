using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PhotocopyRevaluationApp.Services {
    public class GenerateUidService {
        public GenerateUidService() { }

        public static string GenerateSecureKey(int size = 32) // size in bytes
        {
            var key = new byte[size];
            using (var rng = RandomNumberGenerator.Create()) {
                rng.GetBytes(key);
            }
            return Convert.ToBase64String(key); // Return as Base64 string
        }

        public async Task<string> GenerateJwtToken(string username) {
            var secretKey = GenerateSecureKey(); // This will give you a secure key

            await KeyVaultManager.StoreSecretToKeyVaultAsync("JWTTokenSecretKey", secretKey);
            var tokenHandler = new JwtSecurityTokenHandler();

            // Define your secret key and algorithm
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey /*_mySettings.JWTSecretKey*/));
            //OR
            //    var securityKey = Encoding.ASCII.GetBytes(_mySettings.JWTSecretKey); // Ensure this is stored securely
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256 /*OR HmacSha256Signature*/);

            // Set the token expiration time
            var expirationTime = DateTime.Now.AddHours(1);

            // Define your claims
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("aud", "yourAudience") // Include audience claim
            };

            // Create the JWT token
            var token = new JwtSecurityToken(
                issuer: "https://auth.yourapp.com",
                audience: "https://api.yourapp.com", // Define audience
                claims: claims,
                expires: expirationTime,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
