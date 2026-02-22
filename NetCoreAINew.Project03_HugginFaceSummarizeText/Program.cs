
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

Console.Write("Enter your text here: ");

var apiKey = "";
var inputText=Console.ReadLine();

var requestData = new
{
    inputx = inputText
};

var json=JsonSerializer.Serialize(requestData);
var content=new StringContent(json,Encoding.UTF8,"application/json");

using var client = new HttpClient();

    client.DefaultRequestHeaders.Authorization=new AuthenticationHeaderValue("Bearer",apiKey);

var response=await client.PostAsync("https://api-inference.huggingface.co/models/sshleifer/distilbart-cnn-12-6", content);
var reponseContent=await response.Content.ReadAsStringAsync();

Console.WriteLine("📖 Text Summarize: ");
Console.WriteLine(reponseContent);
