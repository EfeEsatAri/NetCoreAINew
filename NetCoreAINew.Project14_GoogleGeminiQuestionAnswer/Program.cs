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
        Console.WriteLine("Sormak İstediğiniz Soruyu Yazın: ");
        string question = Console.ReadLine();

        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new{text=question}
                    }
                }
            }
        };
        var json=JsonSerializer.Serialize(requestBody);
        var content=new StringContent(json,Encoding.UTF8,"application/json");

        using var client=new HttpClient();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var response=await client.PostAsync(endpoint, content);
        var responseText= await response.Content.ReadAsStringAsync();

        try
        {
            var doc = JsonDocument.Parse(responseText);
            string answer = doc.RootElement.GetProperty("candidates")[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString();
            Console.WriteLine("Gemini Cevap: " + answer);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Yanıt Çözümlemesi Başarısız Oldu");
            Console.WriteLine("Gelen Yanıt: " + responseText);
        }
    }
}

