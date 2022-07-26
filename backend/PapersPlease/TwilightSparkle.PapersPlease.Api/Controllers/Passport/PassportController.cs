using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TwilightSparkle.PapersPlease.Api.Services;

namespace TwilightSparkle.PapersPlease.Api.Controllers.Passport
{
    [Route("api/passport")]
    [ApiController]
    public sealed class PassportController : ControllerBase
    {
        private readonly IPassportService _passportService;
        private readonly ILogger<PassportController> _logger;


        public PassportController(IPassportService passportService, ILogger<PassportController> logger)
        {
            _passportService = passportService;
            _logger = logger;
        }


        [HttpPost]
        public async Task<ActionResult<MachineReadableZoneResponse>> ParseMachineReadableZone(IFormFile passportImage)
        {
            if (passportImage == null)
            {
                return BadRequest();
            }

            _logger.LogInformation("Reading MRZ from passport image...");

            var passportImageStream = passportImage.OpenReadStream();
            using (var memoryStream = new MemoryStream())
            {
                await passportImageStream.CopyToAsync(memoryStream);
                var passportData = _passportService.ReadMachineReadingZoneData(memoryStream.ToArray());

                try
                {
                    var response = CreateFrom(passportData);
                    if (response == null)
                    {
                        _logger.LogWarning("Failed to read MRZ from passport image.");

                        return BadRequest();
                    }

                    _logger.LogInformation("Successfully read MRZ from passport image.");

                    return response;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to read MRZ from passport image.");

                    return BadRequest();
                }
            }
        }


        private static MachineReadableZoneResponse CreateFrom(string passportData)
        {
            if (passportData == null)
            {
                return null;
            }

            var lines = passportData.TrimEnd('\n').Split('\n');

            if (lines.Length != 2)
            {
                return null;
            }

            if (lines[0].Length != 44 || lines[1].Length != 44)
            {
                return null;
            }

            var documentType = lines[0].Substring(0, 2).Trim('<');

            var countryCode = lines[0].Substring(2, 3).Trim('<');
            var initialsString = lines[0].Substring(5);
            while (initialsString.IndexOf("<<", StringComparison.Ordinal) != -1)
            {
                initialsString = initialsString.Replace("<<", "<");
            }
            var initials = initialsString.Trim('<').Split('<');

            var passportNumber = lines[1].Substring(0, 9);
            var placeOfBirth = lines[1].Substring(10, 3).Trim('<');

            var dateOfBirthData = lines[1].Substring(13, 6);
            if (!DateTime.TryParseExact(dateOfBirthData, "yyMMdd", CultureInfo.GetCultureInfo("en-GB"), DateTimeStyles.None, out var dateOfBirth))
            {
                throw new Exception();
            }

            var sex = lines[1].Substring(20, 1);

            var dateOfExpireData = lines[1].Substring(21, 6);
            if (!DateTime.TryParseExact(dateOfExpireData, "yyMMdd", CultureInfo.GetCultureInfo("en-GB"), DateTimeStyles.None, out var dateOfExpire))
            {
                throw new Exception();
            }

            var identificationNumber = lines[1].Substring(28, 14).Trim('<');

            return new MachineReadableZoneResponse
            {
                Type = documentType,
                CountryCode = countryCode,
                Name = initials[1],
                Surname = initials[0],
                Patronymic = initials.Length > 2 ? initials[2] : null,
                PassportNumber = passportNumber,
                PlaceOfBirth = placeOfBirth,
                DateOfBirth = dateOfBirth,
                Sex = sex,
                DateOfExpire = dateOfExpire,
                IdentificationNumber = identificationNumber
            };
        }



        public sealed class MachineReadableZoneResponse
        {
            public string Type { get; set; }

            public string CountryCode { get; set; }

            public string Name { get; set; }

            public string Surname { get; set; }

            public string Patronymic { get; set; }

            public string PassportNumber { get; set; }

            public string PlaceOfBirth { get; set; }

            public DateTime DateOfBirth { get; set; }

            public string Sex { get; set; }

            public string IdentificationNumber { get; set; }

            public DateTime DateOfExpire { get; set; }
        }
    }
}