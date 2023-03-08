License Key API
License Key API is a RESTful web service that allows users to generate and validate license keys for a given software package. The API is secured using an authentication secret, and uses cryptographic hashing to generate unique license keys.


*************************Please Set the following properties in appsettings.json*****************************************
 { 
   "PrivateKey": "MY_PRIVATE_KEY",
   "SoftwareName": "123",
   "INTERNAL_PRIVATE_KEY": "INTERNAL_PRIVATE_KEY"
  }

Installation
To run the License Key API, you'll need to install the following:

.NET 6 SDK
Visual Studio Code or Visual Studio IDE
Usage
Generating a License Key
To generate a license key, send a POST request to the /api/LicenseKeys/GenerateKey endpoint with the following parameters:
full_name: The full name of the end-user.
software_name: The name of the software package.
auth_secret: The authentication secret to validate API access.
The API will return a JSON response with the following format:

json

{

  "license_key": "ABCD-EFGH-IJKL-MNOP"
}
Validating a License Key
To validate a license key, send a POST request to the /api/LicenseKeys/IsValidLicenseKey endpoint with the following parameters:

full_name: The full name of the end-user.
license_key: The license key to validate.
If the license key is valid, the API will return a 204 No Content response. If the license key is invalid, the API will return a 404 Not Found response.

Configuration
The License Key API can be configured using the appsettings.json file, located in the project's root directory. The following settings can be configured:

Logging: Specifies the logging level for the application.
AllowedHosts: Specifies the allowed hosts for the application.
LicenseKeyConfig:InternalPrivateKey: Specifies the internal private key for license key generation.
Development
To develop the License Key API, follow these steps:

Clone the repository.
Open the project in Visual Studio Code or Visual Studio IDE.
Install the required dependencies using NuGet.
Run the application using the dotnet run command.
Testing
To test the License Key API, follow these steps:

Open the project in Visual Studio Code or Visual Studio IDE.
Run the tests using the Test Explorer.
Verify that all tests pass.
Contributors/Owner
Jai Choudhary
License
This project is licensed under the MIT License - see the LICENSE.md file for details.