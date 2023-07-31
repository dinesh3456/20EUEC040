using System.Text;

namespace TrainApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            const string baseUrl = "http://20.244.56.144";

            string authToken = await AuthenticateAsync(baseUrl);

            if (string.IsNullOrEmpty(authToken))
            {
                Console.WriteLine("Authentication failed.");
                return;
            }

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {authToken}");

                while (true)
                {
                    Console.WriteLine("Choose an option:");
                    Console.WriteLine("1. Get all trains in the next 12 hours");
                    Console.WriteLine("2. Get train by ID");
                    Console.WriteLine("3. Exit");
                    Console.Write("Enter your choice: ");

                    if (int.TryParse(Console.ReadLine(), out int choice))
                    {
                        switch (choice)
                        {
                            case 1:
                                await GetTrainsAsync(client, baseUrl);
                                break;
                            case 2:
                                Console.Write("Enter the train ID: ");
                                if (int.TryParse(Console.ReadLine(), out int trainId))
                                {
                                    await GetTrainByIdAsync(client, baseUrl, trainId);
                                }
                                else
                                {
                                    Console.WriteLine("Invalid train ID. Please enter a valid numeric value.");
                                }
                                break;
                            case 3:
                                Console.WriteLine("Exiting the application.");
                                return;
                            default:
                                Console.WriteLine("Invalid choice. Please choose a valid option.");
                                break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid choice. Please enter a numeric value.");
                    }

                    Console.WriteLine();
                }
            }
        }

        static async Task<string> AuthenticateAsync(string baseUrl)
        {
            try
            {
                // JSON payload for authentication
                var authPayload = new
                {
                    companyName = "Train Central",
                    clientID = "25244a8b-ef3a-4759-85cd-17b21940e800",
                    clientSecret = "CtypcFacBXhuwLuK",
                    ownerName = "Rahul",
                    ownerEmail = "20EUEC040@skcet.ac.in",
                    rollNo = "20EUEC040"
                };

                string jsonPayload = Newtonsoft.Json.JsonConvert.SerializeObject(authPayload);

                using (HttpClient client = new HttpClient())
                {
                    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync($"{baseUrl}/train/auth", content);
                    response.EnsureSuccessStatusCode();

                    string responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(responseBody);

                    // Parse the response to get the authentication token
                    dynamic responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject(responseBody);
                    string authToken = responseObject?.access_token?.ToString();
                    return authToken;
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }

        static async Task GetTrainsAsync(HttpClient client, string baseUrl)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"{baseUrl}/train/trains");
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseBody);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        static async Task GetTrainByIdAsync(HttpClient client, string baseUrl, int trainId)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"{baseUrl}/train/trains/{trainId}");
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseBody);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
