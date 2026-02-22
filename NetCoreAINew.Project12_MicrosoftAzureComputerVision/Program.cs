using System.Net.Http.Headers;
using System.Text.Json;

class Program
{
    static async Task Main(string[] args)
    {
        string imagePath = "C:\\Yeni klasor\\3.png";
        string subscriptionKey = "";
        string apiUrl = "";

        string requestParameters = "visualFeatures=Categories,Description,Tags,Color&Language=en";
        string uri = apiUrl + "?" + requestParameters;

        if(!File.Exists(imagePath))
        {
            Console.WriteLine("Görsel dosyası bulunamadı!" + imagePath);
            return;
        }
        byte[] immageBytes = await File.ReadAllBytesAsync(imagePath);

        using(HttpClient client=new HttpClient()) 
            using(ByteArrayContent content=new ByteArrayContent(immageBytes))
        {
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            HttpResponseMessage response=await client.PostAsync(uri, content);
            string result=await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Azure Yanıtı: ");
                JsonDocument json=JsonDocument.Parse(result);
                var description = json.RootElement.GetProperty("description").GetProperty("captions")[0];
                string text = description.GetProperty("text").GetString();
                double confidence=description.GetProperty("confidence").GetDouble();
                Console.WriteLine($"Açıklama: {text} (Gücen: %{confidence * 100:0.00}");

            }
            else
            {
                Console.WriteLine("Bir Hata Oluştu!");
                Console.WriteLine($"Status: {response.StatusCode}");
                Console.WriteLine("Yanıt: " + result);
            }
        }
    }
}