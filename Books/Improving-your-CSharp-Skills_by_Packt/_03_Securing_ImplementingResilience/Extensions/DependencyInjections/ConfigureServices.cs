using Polly;
using Polly.CircuitBreaker;

using _03_Securing_ImplementingResilience.Services;

namespace _03_Securing_ImplementingResilience.DependencyInjections;

public static class ConfigureServices
{
  public static void AddConfigureServices(IServiceCollection services)
  {
    var circuitBreakerPolicy = Policy.HandleResult<HttpResponseMessage>(result => !result.IsSuccessStatusCode)
      .AdvancedCircuitBreaker(
        0.1, TimeSpan.FromSeconds(60),
        5, TimeSpan.FromSeconds(10),
        OnBreak, OnReset, OnHalfOpen
      );
    services.AddSingleton<HttpClient>();
    services.AddSingleton<CircuitBreakerPolicy<HttpResponseMessage>>(circuitBreakerPolicy);
    
    // 再試行ポリシー & サーキットブレーカーポリシーを適用したHttpClient
    services.AddSingleton<IResilientHttpClient, ResilientHttpClient>();
  }

  private static void OnBreak(DelegateResult<HttpResponseMessage> responseMessage, TimeSpan timeSpan) 
  {
    // ログなどの処理
  }

  private static void OnReset() 
  {
    // ログなどの処理
  }

  private static void OnHalfOpen() 
  {
    // ログなどの処理
  }
}
