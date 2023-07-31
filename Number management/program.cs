class Program
{
    static async Task Main()
    {
        int numberOfUrls = GetNumberOfUrlsFromUserInput();
        var urls = GetUrlsFromUserInput(numberOfUrls);

        var validUrls = urls.Where(IsValidUrl).ToList();

        var numbers = await FetchAndMergeNumbersAsync(validUrls);

        Console.WriteLine("Merged and sorted numbers:");
        foreach (var number in numbers)
        {
            Console.Write(number + " ");
        }
        Console.WriteLine();
    }

    static int GetNumberOfUrlsFromUserInput()
    {
        int numberOfUrls;
        while (true)
        {
            Console.WriteLine("Enter the number of URLs you wish to fetch numbers from:");
            if (int.TryParse(Console.ReadLine(), out numberOfUrls) && numberOfUrls > 0)
                break;
            Console.WriteLine("Invalid input. Please enter a valid positive integer.");
        }
        return numberOfUrls;
    }

    static List<string> GetUrlsFromUserInput(int numberOfUrls)
    {
        var urls = new List<string>();
        for (int i = 1; i <= numberOfUrls; i++)
        {
            Console.WriteLine($"Enter URL {i}:");
            var url = Console.ReadLine();
            urls.Add(url);
        }
        return urls;
    }

    static bool IsValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out _);
    }

    static async Task<List<int>> FetchAndMergeNumbersAsync(List<string> urls)
    {
        var httpClient = new HttpClient();
        var mergedNumbers = new List<int>();

        foreach (var url in urls)
        {
            try
            {
                var response = await httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var numbers = ParseNumbersFromJson(jsonString);
                    mergedNumbers.AddRange(numbers);
                }
                else
                {
                    Console.WriteLine($"Failed to fetch data from URL: {url}. Status code: {response.StatusCode}");
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Failed to fetch data from URL: {url}. Error: {ex.Message}");
            }
        }

        return mergedNumbers.Distinct().OrderBy(number => number).ToList();
    }

    static List<int> ParseNumbersFromJson(string jsonString)
    {
        var jsonObject = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, List<int>>>(jsonString);
        return jsonObject["numbers"];
    }
}
