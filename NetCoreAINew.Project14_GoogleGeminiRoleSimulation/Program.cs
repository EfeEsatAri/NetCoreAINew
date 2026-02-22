using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

class Program
{
    static async Task Main(string[] args)
    {
        string apiKey = "";
        string model = "gemini-1.5-pro";
        string endpoint = $"https://generativelanguage.googleapis.com/v1/models/{model}:generateContent?Key={apiKey}";

        Console.WriteLine("Rolünüzü Seçin: ");
        Console.WriteLine("1- Psikolog");
        Console.WriteLine("2- Maç Yorumcusu");
        Console.WriteLine("3- Finansal Yatırım Uzmanı");
        Console.WriteLine("4- Tarihçi");
        Console.WriteLine("5- Turist Rehberi");
        Console.WriteLine();
        Console.Write("Seçiminiz: ");
        string roleChoice=Console.ReadLine();
        string rolePrompt = roleChoice switch
        {
            "1" => "Sen Bir Psikologsun. Danışanın sorularına empatik, açıklayıcı ve sakin bir dille yanıt ver",
            "2" => "Sen bir maç yorumcususun. Sorulan soruya sanki maç başlamadan birkaç saat önce stada gitmiş gibi o atmosferi hisseden ve heyecanlandıran cevap ver.",
            "3" => "Sen bir yatırım danışmanısın. Kullanıcının bütçesine ve hedeflerine göre yatırım önerileri yap.",
            "4" => "Sen bir tarihçisin. Olayları bilimsel kaynaklara dayanarak detaylı olarak anlat.",
            "5" => "Sen bir turist rehbersin. Sorulan soruyu o şehri çok iyi bilen, o şehirde mutlaka görülmesi gereken ve yenilmesi gereken lezzetleri listeleyek yanıt ver"

        };
        Console.WriteLine();

        Console.Write("Sormak İstediğiniz cümleyi giriniz: ");
        string userInput=Console.ReadLine();
        string finalPrompt = $"{rolePrompt}\n\n Kullanıcıdan Gelen Soru: {userInput}";

        var requextBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new
                        {
                            text=finalPrompt
                        }
                    }
                }
            }
        };

        var json=JsonSerializer.Serialize(requextBody);
        var content = new StringContent(json, Encoding.UTF8,"application/json");

        using var client=new HttpClient();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var response=await client.PostAsync(endpoint, content);
        var responseText= await response.Content.ReadAsStringAsync();

        try
        {
            var doc = JsonDocument.Parse(responseText);
            string answer = doc.RootElement.GetProperty("candidates")[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString();

            Console.WriteLine(answer);
        }
        catch (Exception)
        {
            Console.WriteLine("Yanıt Hatası: " + responseText);
        }

    }
}
