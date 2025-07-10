using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Synapse.SignalBoosterExample
{
    public static class SignalBoosterHelperWithLLM
    {
        /// <summary>
        /// Loads a physician note from a file or JSON input.
        /// Accepts plain text or JSON-wrapped notes (with a "note" property).
        /// </summary>
        public static string LoadPhysicianNote(string input, bool isJson = false)
        {
            try
            {
                if (isJson)
                {
                    var jObj = JObject.Parse(input);
                    return jObj["note"]?.ToString() ?? "";
                }
                if (File.Exists(input))
                {
                    return File.ReadAllText(input);
                }
                Console.WriteLine($"File '{input}' not found. Using default note.");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error loading physician note: {ex.Message}");
            }
            return "Patient needs a CPAP with full face mask and humidifier. AHI > 20. Ordered by Dr. Cameron.";
        }

        /// <summary>
        /// Uses an LLM (e.g., OpenAI) to extract DME info from the note.
        /// The LLM should return a JSON object with device, mask_type, add_ons, qualifier, ordering_provider, etc.
        /// </summary>
        public static async Task<JObject> ExtractDmeInfoWithLlmAsync(string note, string openAiApiKey, string openAiEndpoint = "https://api.openai.com/v1/chat/completions")
        {
            try
            {
                var prompt = $"Extract the following fields from this physician note as JSON: device, mask_type, add_ons, qualifier, ordering_provider, liters, usage. Note: {note}";
                var requestBody = new
                {
                    model = "gpt-3.5-turbo",
                    messages = new[]
                    {
                        new { role = "system", content = "You are a medical data extraction assistant." },
                        new { role = "user", content = prompt }
                    }
                };

                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {openAiApiKey}");
                    var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync(openAiEndpoint, content);
                    response.EnsureSuccessStatusCode();
                    var responseString = await response.Content.ReadAsStringAsync();
                    var responseJson = JObject.Parse(responseString);
                    var completion = responseJson["choices"]?[0]?["message"]?["content"]?.ToString();
                    if (!string.IsNullOrEmpty(completion))
                    {
                        // Try to parse the LLM's JSON output
                        return JObject.Parse(completion);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error extracting DME info with LLM: {ex.Message}");
            }
            return null;
        }

        /// <summary>
        /// Sends the structured payload to the external API endpoint via HTTP POST.
        /// The API endpoint is configurable.
        /// </summary>
        public static async Task SendPayloadToApiAsync(JObject payload, string apiUrl)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    Console.WriteLine($"Sending payload to API at {apiUrl}...");
                    var content = new StringContent(payload.ToString(), Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync(apiUrl, content);
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