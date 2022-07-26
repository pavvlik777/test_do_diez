namespace TwilightSparkle.PapersPlease.Api.Services
{
    public interface IPassportService
    {
        string ReadMachineReadingZoneData(byte[] passportImage);
    }
}