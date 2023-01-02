using Microsoft.AspNetCore.Mvc;

using _03_Securing_ImplementingResilience.Models;
using _03_Securing_ImplementingResilience.Services;

namespace _03_Securing_ImplementingResilience.Controllers;

[ApiController]
[Route("[controller]")]
public class TimeoutController : Controller
{
  IResilientHttpClient _resilientHttpClient;

  public TimeoutController(IResilientHttpClient resilientHttpClient)
  {
    _resilientHttpClient = resilientHttpClient;
  }

  /// <summary>
  /// タイムアウトポリシーでラップした、フォールバック & 再試行 & サーキットブレーカー のテスト
  /// </summary>
  [HttpPost]
  public async Task<ActionResult<object>> Post(User user)
  {
    // Email service URL（実際には機能しない。別で稼働しているマイクロサービス想定）
    string emailServiceUrl = "http://localhost:5270/TestEmail/Unavailable";

    var response = _resilientHttpClient.PostTimeout(emailServiceUrl, user);
    var content = await response.Content.ReadAsStringAsync();
    return Ok(content);
  }

}