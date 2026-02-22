using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

var apiKey = "";

Console.Write("Enter your text here: ");
var text = Console.ReadLine();

var modelUrl = "https://api-inference.huggingface.co/models/cardiffnlp/twitter-roberta-base-sentiment";

using var client = new HttpClient();
client.DefaultRequestHeaders.Authorization =
    new AuthenticationHeaderValue("Bearer", apiKey);

var requestBody = new
{
    inputs = text
};

var json = JsonSerializer.Serialize(requestBody);
var content = new StringContent(json, Encoding.UTF8, "application/json");

var response = await client.PostAsync(modelUrl, content);
var result = await response.Content.ReadAsStringAsync();

using var doc = JsonDocument.Parse(result);

var items = doc.RootElement[0];

var topLabel = items
    .EnumerateArray()
    .OrderByDescending(e => e.GetProperty("score").GetDouble())
    .First();

var label = topLabel.GetProperty("label").GetString();
var score = topLabel.GetProperty("score").GetDouble();

string labelText = label switch
{
    "LABEL_0" => "NEGATİF 😡",
    "LABEL_1" => "NÖTR 😐",
    "LABEL_2" => "POZİTİF 😍",
    _ => "BİLİNMİYOR"
};

Console.OutputEncoding = Encoding.UTF8;

Console.WriteLine("\n🗒️ Girdi Metni: ");
Console.WriteLine(text);

Console.WriteLine("🎭 Duygu Analizi: ");
Console.WriteLine($"✅ Duygu Durumu: {labelText}");
Console.WriteLine($"🎯 Güven Skoru: %{(score * 100).ToString("F2", CultureInfo.InvariantCulture)}");