using DeviceId;
using DeviceId.Encoders;
using DeviceId.Formatters;
using System;

namespace ComputerInfo
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var builder = new DeviceIdBuilder();
            builder.Formatter = new StringDeviceIdFormatter(new PlainTextDeviceIdComponentEncoder());
            var machineName = builder.AddProcessorId().ToString();
            var cpu = builder.AddMachineName().ToString();
            var motherCard = builder.AddMotherboardSerialNumber().ToString();
            var osInstallation = builder.AddOSInstallationID().ToString();
            var systemDrive = builder.AddSystemDriveSerialNumber().ToString();

            Console.WriteLine($"----------------------------------------\n" +
                $"machine name: {machineName}\n" +
                $"cpu name: {cpu}\n" +
                $"motherCard name: {motherCard}\n" +
                $"osInstallation name: {osInstallation}\n" +
                $"systemDrive name: {systemDrive}\n" +
                $"----------------------------------------");

            Console.ReadLine();
        }
    }
}