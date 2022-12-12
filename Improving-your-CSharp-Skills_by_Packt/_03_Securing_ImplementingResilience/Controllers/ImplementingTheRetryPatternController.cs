using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Polly;

using _03_Securing_ImplementingResilience.Models;

namespace _03_Securing_ImplementingResilience.Controllers;

[ApiController]
[Route("[controller]")]
public class ImplementingTheRetryPatternController : Controller
{
  HttpClient _client;
  public ImplementingTheRetryPatternController()
  {
    _client = new HttpClient();
  }

  [HttpGet]
  public bool PostImplementingTheRetryPatternTest()
  {
    return true;
  }
  

  /// <summary>
  /// 再試行パターンのテスト
  /// </summary>
  [HttpPost]
  public async Task PostImplementingTheRetryPatternTest([FromBody]User user)
  {
    // Email service URL（実際には機能しない。また別で稼働しているマイクロサービス想定）
    string emailServiceUrl = "http://localhost:5270/api/Email";
    
    // ユーザ情報をjsonに変換
    HttpContent content = new StringContent(JsonConvert.SerializeObject(user));

    // Content-Type を application/json に
    _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    
    // リクエスト再試行回数
    int maxRetries = 3;

    // ポリシーの作成（3秒間隔で再試行する）
    var retryPolicy = Policy.Handle<HttpRequestException>().WaitAndRetryAsync(
      maxRetries,
      sleepDuration => TimeSpan.FromSeconds(3)
    );
    
    // ポリシーを用いて実行
    await retryPolicy.ExecuteAsync(async () => {
      var response = await _client.PostAsync(emailServiceUrl, content);
      Console.WriteLine($"Retry test status: {response.StatusCode}");
      response.EnsureSuccessStatusCode();
    });
  }
}