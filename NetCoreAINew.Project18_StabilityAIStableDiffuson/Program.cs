using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

class Program
{
    static async Task Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.WriteLine("🤖 Promptan Görsel Üretici V1 - Stability AI");
        Console.Write("Lütfen prompt girin");
        string prompt=Console.ReadLine();

        string apiKey = "";
        string engineId = "stable-diffusion-v1-6";
        string apiUrl = $"https://api.stability.ai/v1/generation/{engineId}/text-to-image";

        using var httpClient= new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

        var requestBody = new
        {
            text_prompts = new[]
            {
                new
                {
                    text=prompt
                }
            },
            cfg_scale = 12,
            height = 512,
            width = 512,
            steps = 30,
            samples = 1
        };

        var jsonContent=new StringContent(JsonSerializer.Serialize(requestBody),Encoding.UTF8,"application/json");
        var response=await httpClient.PostAsync(apiUrl,jsonContent);

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine("Hata: "+response.StatusCode);
            var error=await response.Content.ReadAsStringAsync();
            Console.WriteLine(error);
            return;
        }

        var responseString= await response.Content.ReadAsStringAsync();
        var responseJson=JsonDocument.Parse(responseString);

        string base64İmage = responseJson.RootElement.GetProperty("artifacts")[0].GetProperty("base64").GetString();

        byte[] imageBytes = Convert.FromBase64String(base64İmage);
        string fileName = $"generated_{DateTime.Now:yyyyMMdd_HHmmss}.jpg";
        await File.WriteAllBytesAsync(fileName, imageBytes);

        Console.WriteLine($"☑️ Görsel başarıyla oluşturuldu ve kaydedildi: {fileName}");

    }
}