using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using LicenseKeyAPI.Data;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace LicenseKeyAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class LicenseKeysController : Controller
    {
        private string SoftwareName => _licenseKeyConfig.SoftwareName;
        private string INTERNAL_PRIVATE_KEY => _licenseKeyConfig.INTERNAL_PRIVATE_KEY;
        private readonly IWebHostEnvironment environment;
        private readonly HttpClient _httpClient;
        private readonly LicenseKeyConfig _licenseKeyConfig;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<LicenseKeysController> _logger;

        public LicenseKeysController(IWebHostEnvironment environment, HttpClient httpClient, IOptions<LicenseKeyConfig> licenseKeyConfig)
        {
            this.environment = environment;
            _httpClient = httpClient;
            _licenseKeyConfig = licenseKeyConfig.Value;
        }
        [HttpPost("GenerateKey")]
        public IActionResult GenerateKey([FromBody] LicenseKeyRequest request)
        {

            // Validate authentication secret
            if (request.auth_secret != INTERNAL_PRIVATE_KEY)
            {
                return Unauthorized();
            }
            // Use cryptographic hashing function to generate key
            // You can use any secure hashing algorithm, for example SHA-256
            using var sha256 = SHA256.Create();
            var keyBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes($"{request.full_name}:{request.software_name}:{INTERNAL_PRIVATE_KEY}"));
            var key = BitConverter.ToString(keyBytes).Replace("-", "");
            return Ok(new { license_key = key });
        }

        [HttpPost("IsValidLicenseKey")]
        public async Task<IActionResult> IsValidLicenseKey([FromBody] ValidateKeyRequest request)
        {
            string expectedKey = "";
            // Create a LicenseKeyRequest object with the necessary properties
            var request2 = new LicenseKeyRequest
            {
                full_name = request.full_name,
                software_name = SoftwareName,
                auth_secret = INTERNAL_PRIVATE_KEY
            };
            // Call GenerateKey API to get expected license key
            var response = await _httpClient.PostAsJsonAsync("api/LicenseKeys/GenerateKey", request2);
            response.EnsureSuccessStatusCode();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var result = await response.Content.ReadFromJsonAsync<GenerateLicenseKeyResponse>();
                expectedKey = result.license_key;
            }
            

            // Compare generated key to input key
            if (string.Equals(expectedKey, request.license_key, StringComparison.Ordinal))
            {
                return NoContent();
            }
            else
            {
                return NotFound();
            }
            
        }
        private class GenerateLicenseKeyResponse
        {
            public string license_key { get; set; }
        }


    }
}
