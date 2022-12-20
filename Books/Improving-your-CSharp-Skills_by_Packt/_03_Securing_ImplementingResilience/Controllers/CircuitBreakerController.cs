using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Polly.CircuitBreaker;

using _03_Securing_ImplementingResilience.Models;

namespace _03_Securing_ImplementingResilience.Controllers;

[ApiController]
[Route("[controller]")]
public class CircuitBreakerController : Controller
{
  HttpClient _client;
  CircuitBreakerPolicy<HttpResponseMessage> _circuitBreakerPolicy;

  public CircuitBreakerController(HttpClient client, CircuitBreakerPolicy<HttpResponseMessage> circuitBreakerPolicy)
  {
    _client = client;
    _circuitBreakerPolicy = circuitBreakerPolicy;
  }

  /// <summary>
  /// サーキットブレーカーのテスト
  /// </summary>
  [HttpPost]
  public async Task<IActionResult> PostCircuitBreakerTest([FromBody]User user)
  {
    // Email service URL（実際には機能しない。別で稼働しているマイクロサービス想定）
    string emailServiceUrl = "http://localhost:5270/TestEmail";
    
    // ユーザ情報をjsonに変換
    HttpContent content = new StringContent(JsonConvert.SerializeObject(user));

    // Content-Type を application/json に
    content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");

    // CircuitBreakerポリシーを用いて実行
    HttpResponseMessage response = await Task.Run(() => _circuitBreakerPolicy.Execute(() => _client.PostAsync(emailServiceUrl, content).GetAwaiter().GetResult()));
    
    if (response.IsSuccessStatusCode)
    {
      var result = response.Content.ReadAsStringAsync();
      return Ok(result);
    }

    return StatusCode((int)response.StatusCode, response.Content.ReadAsStringAsync());
  }
}