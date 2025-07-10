using System;
using Newtonsoft.Json.Linq;

namespace Synapse.SignalBoosterExample
{
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                Console.WriteLine("Starting SignalBooster utility...");

                // Load physician note
                Console.WriteLine("Loading physician note...");
                string physicianNote = SignalBoosterHelper.LoadPhysicianNote();

                // Extract device and order details
                Console.WriteLine("Extracting device and order details...");
                string deviceType = SignalBoosterHelper.ExtractDeviceType(physicianNote);
                string maskType = deviceType == "CPAP"
                    ? SignalBoosterHelper.ExtractMaskType(physicianNote)
                    : null;
                string addOn = SignalBoosterHelper.ExtractAddOn(physicianNote);
                string qualifier = SignalBoosterHelper.ExtractQualifier(physicianNote);
                string providerName = SignalBoosterHelper.ExtractProviderName(physicianNote);

                string liters = null;
                string usage = null;
                if (deviceType == "Oxygen Tank")
                {
                    (liters, usage) = SignalBoosterHelper.ExtractOxygenDetails(physicianNote);
                }

                JObject payload = SignalBoosterHelper.BuildPayload(
                    deviceType, maskType, addOn, qualifier, providerName, liters, usage);

                Console.WriteLine("Sending payload to API...");
                SignalBoosterHelper.SendPayloadToApi(payload);

                Console.WriteLine("Process completed successfully.");
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Fatal error: {ex.Message}");
                return 1;
            }
        }
    }
}