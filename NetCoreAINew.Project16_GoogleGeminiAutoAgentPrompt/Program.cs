using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

class Program
{
    static async Task Main(string[] args)
    {
        string apiKey = "";
        string model = "gemini-1.5-pro";
        string endpoint = $"https://generativelanguage.googleapis.com/v1/models/{model}:generateContent?Key={apiKey}";

        string context = "Sen bir yapay zeka içerik planlayıcısısın kullanıcının fikrine göre içerik üretmesine yardım ediceksin fikri aldıkdan sonra kullanıcıya doğru soruları sorarak onu yönlendirecek sonra da içerik planını çıkaracaksın";

        Console.Write("Bir fikrin mi var ? (örnek: bir kafe açmak istiyorum): ");

        string userInput = Console.ReadLine();

        string userPrompt = $"{context}\n\n Kullanıcının fikri: {userInput}\n Şimdi ona adım adım sorular sormaya başla. ";

        for (int i = 0; i < 5; i++)
        {
            string question = await SendToGemini(apiKey, endpoint, userPrompt);
            Console.WriteLine($"Agent: {question}");
            Console.Write("Sen: ");
            string answer = Console.ReadLine();
            userPrompt += $"\n\n Kullanıcının Cevabı: {answer}\n Yeni sorunu sor.";
        }
        string finalPrompt = $"{userPrompt}\n\n Artık yeterli bilgiye sahipsin. Kullanıcı için detaylı bir içerik planı oluştur";
        string finalOutput = await SendToGemini(apiKey, endpoint, finalPrompt);
        Console.WriteLine("\n Nihai İçerik Planı: \n" + finalOutput);
    }

    static async Task<string> SendToGemini(string apiKey, string endpoint, string prompt)
    {
        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new
                        {
                            text=prompt
                        }
                    }
                }
            }
        };
        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var response = await client.PostAsync(endpoint, content);
        var responseText = await response.Content.ReadAsStringAsync();

        try
        {
            var doc = JsonDocument.Parse(responseText);
            return doc.RootElement.GetProperty("candidates")[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString();
        }
        catch (Exception)
        {
            return "Bir Hata Oluştu!";
        }

    }
}