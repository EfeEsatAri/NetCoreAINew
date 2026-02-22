using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

class Program
{
    static async Task Main(string[] args)
    {
        string apiKey = "";
        Console.Write("---------- Context Area ----------");
        Console.WriteLine();
        Console.WriteLine();

        Console.Write("Enter Your Context Here: ");
        var context=Console.ReadLine();

        Console.WriteLine();
        Console.WriteLine();
        Console.Write("---------- Question Area ----------");
        Console.WriteLine();
        Console.WriteLine();
        Console.Write("Enter Your Question Here: ");

        var question=Console.ReadLine();

        using (var client=new HttpClient())
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var requestBody = new
            {
                inputs = new
                {
                    question = question,
                    context = context,
                }
            };

            var json=JsonSerializer.Serialize(requestBody);
            var content=new StringContent(json,Encoding.UTF8,"application/json");
            var response=await client.PostAsync("https://api-inference.hugginface.co/models/deepset/roberta-base-squad2", content);

            var responseString=await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(responseString);

            if(doc.RootElement.TryGetProperty("answer",out var answer))
            {
                Console.WriteLine("\n ❓Soru: "+question);
                Console.WriteLine("🖨️ Metin: " + context);
                Console.WriteLine("✅ Cevap: " + answer.GetString());
            }
            else
            {
                Console.WriteLine("❌ Cevap bulunamadı veya model henüz hazır değil.");
                Console.WriteLine(responseString);
            }
        }

        Console.WriteLine();
    }
}
