using System.ComponentModel.DataAnnotations;

namespace LicenseKeyAPI.Data
{
    public class LicenseKeyModel
    {
        [Required(ErrorMessage = "Full Name is required")]
        public string FullName { get; set; }
        public string SoftwareName { get; set; }

        public string Key { get; set; }
    }
}
