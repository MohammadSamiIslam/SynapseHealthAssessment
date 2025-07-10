using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace Synapse.SignalBoosterExample
{
    public static class SignalBoosterHelper
    {
        /// <summary>
        /// Loads the physician note from a file, or returns a default note if not found.
        /// Logs all steps and errors.
        /// </summary>
        public static string LoadPhysicianNote(string filePath = "physician_note.txt")
        {
            const string defaultNote = "Patient needs a CPAP with full face mask and humidifier. AHI > 20. Ordered by Dr. Cameron.";
            try
            {
                Console.WriteLine($"Attempting to read physician note from '{filePath}'...");
                if (File.Exists(filePath))
                {
                    string note = File.ReadAllText(filePath);
                    Console.WriteLine("Physician note loaded successfully.");
                    return note;
                }
                else
                {
                    Console.WriteLine($"File '{filePath}' not found. Using default note.");
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error reading physician note: {ex.Message}");
            }
            return defaultNote;
        }

        /// <summary>
        /// Extracts the device type from the note.
        /// </summary>
        public static string ExtractDeviceType(string note)
        {
            try
            {
                if (note.Contains("CPAP", StringComparison.OrdinalIgnoreCase)) return "CPAP";
                if (note.Contains("oxygen", StringComparison.OrdinalIgnoreCase)) return "Oxygen Tank";
                if (note.Contains("wheelchair", StringComparison.OrdinalIgnoreCase)) return "Wheelchair";
                return "Unknown";
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error extracting device type: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Extracts the mask type for CPAP devices.
        /// </summary>
        public static string ExtractMaskType(string note)
        {
            try
            {
                return note.Contains("full face", StringComparison.OrdinalIgnoreCase) ? "full face" : null;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error extracting mask type: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Extracts add-on features from the note.
        /// </summary>
        public static string ExtractAddOn(string note)
        {
            try
            {
                return note.Contains("humidifier", StringComparison.OrdinalIgnoreCase) ? "humidifier" : null;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error extracting add-on: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Extracts the qualifier from the note.
        /// </summary>
        public static string ExtractQualifier(string note)
        {
            try
            {
                return note.Contains("AHI > 20") ? "AHI > 20" : "";
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error extracting qualifier: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Extracts the provider name from the note.
        /// </summary>
        public static string ExtractProviderName(string note)
        {
            try
            {
                int providerIndex = note.IndexOf("Dr.");
                if (providerIndex >= 0)
                {
                    return note.Substring(providerIndex)
                        .Replace("Ordered by ", "")
                        .Trim('.')
                        .Trim();
                }
                return "Unknown";
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error extracting provider name: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Extracts oxygen tank details (liters and usage) from the note.
        /// </summary>
        public static (string Liters, string Usage) ExtractOxygenDetails(string note)
        {
            try
            {
                string liters = null;
                string usage = null;

                Match litersMatch = Regex.Match(note, @"(\d+(\.\d+)?) ?L", RegexOptions.IgnoreCase);
                if (litersMatch.Success)
                {
                    liters = litersMatch.Groups[1].Value + " L";
                }

                bool mentionsSleep = note.Contains("sleep", StringComparison.OrdinalIgnoreCase);
                bool mentionsExertion = note.Contains("exertion", StringComparison.OrdinalIgnoreCase);

                if (mentionsSleep && mentionsExertion)
                    usage = "sleep and exertion";
                else if (mentionsSleep)
                    usage = "sleep";
                else if (mentionsExertion)
                    usage = "exertion";

                return (liters, usage);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error extracting oxygen details: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Builds the payload to send to the API.
        /// </summary>
        public static JObject BuildPayload(
            string deviceType,
            string maskType,
            string addOn,
            string qualifier,
            string providerName,
            string liters,
            string usage)
        {
            try
            {
                var payload = new JObject
                {
                    ["device"] = deviceType,
                    ["mask_type"] = maskType,
                    ["add_ons"] = addOn != null ? new JArray(addOn) : null,
                    ["qualifier"] = qualifier,
                    ["ordering_provider"] = providerName
                };

                if (deviceType == "Oxygen Tank")
                {
                    payload["liters"] = liters;
                    payload["usage"] = usage;
                }

                Console.WriteLine("Payload built successfully.");
                return payload;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error building payload: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Sends the payload to the external API endpoint.
        /// </summary>
        public static void SendPayloadToApi(JObject payload, string apiUrl = "https://alert-api.com/DrExtract")
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    Console.WriteLine($"Sending payload to API at {apiUrl}...");
                    var content = new StringContent(payload.ToString(), Encoding.UTF8, "application/json");
                    var response = httpClient.PostAsync(apiUrl, content).GetAwaiter().GetResult();
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Payload sent successfully.");
                    }
                    else
                    {
                        Console.Error.WriteLine($"API call failed. Status: {response.StatusCode}, Reason: {response.ReasonPhrase}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error sending payload to API: {ex.Message}");
                throw;
            }
        }
    }
}