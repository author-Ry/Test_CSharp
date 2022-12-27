using Microsoft.AspNetCore.Mvc;

using _03_Securing_ImplementingResilience.Models;
using _03_Securing_ImplementingResilience.Services;

namespace _03_Securing_ImplementingResilience.Controllers;

[ApiController]
[Route("[controller]")]
public class WrappingTheCircuitBreakerWithRetryController : Controller
{
  IResilientHttpClient _resilientHttpClient;

  public WrappingTheCircuitBreakerWithRetryController(IResilientHttpClient resilientHttpClient)
  {
    _resilientHttpClient = resilientHttpClient;
  }

  /// <summary>
  /// 再試行ポリシーでラップしたサーキットブレーカーのテスト
  /// </summary>
  [HttpPost]
  public async Task<ActionResult> Post(User user)
  {
    // Email service URL（実際には機能しない。別で稼働しているマイクロサービス想定）
    string emailServiceUrl = "http://localhost:5270/TestEmail";

    var response = _resilientHttpClient.Post(emailServiceUrl, user);
    
    if (response.IsSuccessStatusCode)
    {
      var result = await response.Content.ReadAsStringAsync();
      return Ok();
    }

    return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
  }

}