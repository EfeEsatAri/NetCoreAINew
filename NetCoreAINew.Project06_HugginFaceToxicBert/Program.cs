using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

class Program
{
    static async Task Main(string[] args)
    {
        var apiKey = "";
        Console.Write("Enter Your Comment Here: ");
        string inputText = Console.ReadLine();

        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            var requestBody = new
            {
                inputs = inputText
            };
            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://api-inference.hugginface.co/models/unitary/toxic-bert", content);

            var responseString = await response.Content.ReadAsStringAsync();
            if (!responseString.TrimStart().StartsWith("["))
            {
                Console.WriteLine("❌ Model henüz hazır değil veya bir hata oluştu.");
                Console.WriteLine(responseString);
                return;
            }

            var doc = JsonDocument.Parse(responseString);
            Console.WriteLine("\n 🖨️ Yorum Analiz Sonucu: \n ");
            foreach (var item in doc.RootElement[0].EnumerateArray())
            {
                string label = item.GetProperty("label").GetString();
                double score = Math.Round(item.GetProperty("score").GetDouble() * 100, 2);

                if (score >= 50)
                {
                    Console.WriteLine($"{label.ToUpper()} ---> %{score}");
                }
            }

        }
    }
}