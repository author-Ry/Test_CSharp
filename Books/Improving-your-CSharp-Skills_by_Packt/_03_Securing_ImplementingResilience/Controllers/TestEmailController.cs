using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

using _03_Securing_ImplementingResilience.Models;

namespace _03_Securing_ImplementingResilience.Controllers;

[ApiController]
[Route("[controller]")]
public class TestEmailController : Controller
{
  /// <summary> 
  /// テスト用メールサービス（外部のマイクロサービス想定）
  /// </summary>
  public TestEmailController() { }

  [HttpPost]
  public async Task<IActionResult> Post([FromBody]User user)
  {
    // ユーザ情報をjsonに変換
    HttpContent content = new StringContent(JsonConvert.SerializeObject(user));

    // 1/2でエラー
    if (new Random().Next(1, 10) % 2 == 0)
    {
      throw new Exception();
    }

    return Ok(await content.ReadAsStringAsync());
  }

  [HttpPost("Unavailable")]
  public async Task<IActionResult> PostUnavailable([FromBody]User user)
  {
    // ユーザ情報をjsonに変換
    HttpContent content = new StringContent(JsonConvert.SerializeObject(user));

    // 10秒後にエラー
    await Task.Run(() => Thread.Sleep(TimeSpan.FromSeconds(10)));
    throw new Exception("Timeout");
  }

}