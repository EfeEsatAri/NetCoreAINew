using System.Net.Http.Headers;
using System.Text;



class Program
{

    static async Task Main(string[] args)
    {
        string subscriptionKey = "";
        string region = "";
        string tokenEndPoint = $"https://{region}.api.cognitive.microsoft.com/sts/v1.0/issuetoken";

        var token = await GetTokenAsync(subscriptionKey, tokenEndPoint);
        string userText = "Merhaba Arkadaşlar Bu Bir Deneme Mesajıdır. Amacımız Microsoft Azure Kullanarak Metni Sese Dönüştürmektir. Umarım Başarılı Oluruz.";

        static async Task<string> GetTokenAsync(string Key, string endPoint)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", Key);
            var response = await client.PostAsync(endPoint, null);
            return await response.Content.ReadAsStringAsync();
        }

        static async Task SynthesizeSpeechAsync(string token, string region, string text)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            client.DefaultRequestHeaders.Add("User-Agent", "AzureTTSClient");
            client.DefaultRequestHeaders.Add("X-Microsoft-OutputFormat", "riff-16khz-16bit-mono-pcm");

            string ssml = $@"
<speak version='1.0' xml:lang='en-US'>
  <voice xml:lang='tr-TR' name='tr-TR-AhmetNeural'>{text}</voice>
    </speak>";

            var content = new StringContent(ssml, Encoding.UTF8, "application/ssml+xml");
            var result = await client.PostAsync($"https://{region}.tts.speech.microsoft.com/cognitiveservices/v1", content);

            if (result.IsSuccessStatusCode)
            {
                var audioBytes=await result.Content.ReadAsByteArrayAsync();
                File.WriteAllBytes("output2.wav", audioBytes);
                Console.WriteLine("Ses Dosyası Oluşturuldu: output.wav ");
            }
            else
            {
                Console.WriteLine("Hata: " + result.StatusCode);
                Console.WriteLine(await result.Content.ReadAsStringAsync());
            }

        }
    }
}
