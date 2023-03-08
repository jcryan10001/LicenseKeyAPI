namespace LicenseKeyAPI.Data
{
    public class LicenseKeyRequest
    {
        public string full_name { get; set; }
        public string software_name { get; set; }
        public string auth_secret { get; set; }
    }
}
