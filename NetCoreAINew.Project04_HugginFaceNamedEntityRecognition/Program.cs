using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

class Program
{
    static async Task Main(string[] args)
    {
        var apiKey = "";
        Console.Write("Please input text here: ");
        var inputText=Console.ReadLine();

        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var requestBody = new
            {
                inputs = inputText
            };

            var json=JsonSerializer.Serialize(requestBody);
            var content=new StringContent(json,Encoding.UTF8,"application/json");

            var response = await client.PostAsync("https://api-inference.hugginface.co/models/dslim/bert-base-NER", content);
            var responseString= await response.Content.ReadAsStringAsync();

            Console.WriteLine("🎈 NER Çıktısı");
            Console.WriteLine();

            var doc = JsonDocument.Parse(responseString);
            foreach(var item in doc.RootElement.EnumerateArray())
            {
                string entity = item.GetProperty("entity_group").GetString();
                string word=item.GetProperty("word").GetString();
                double score = Math.Round(item.GetProperty("score").GetDouble() * 100, 2);
                Console.WriteLine($" --> {word}");
                Console.WriteLine($"     |-Türü:   {entity}");
                Console.WriteLine($"     |-Güven: %{score}");
            }
        }
    }
}