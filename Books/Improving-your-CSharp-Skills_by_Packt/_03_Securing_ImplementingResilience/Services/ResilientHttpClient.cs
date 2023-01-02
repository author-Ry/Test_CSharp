using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Polly;
using Polly.CircuitBreaker;
using Polly.Fallback;
using Polly.Timeout;
using Polly.Wrap;

namespace _03_Securing_ImplementingResilience.Services;

public interface IResilientHttpClient
{
  HttpResponseMessage Get(string uri);
  HttpResponseMessage Post<T>(string uri, T item);
  HttpResponseMessage Put<T>(string uri, T item);
  HttpResponseMessage Delete(string uri);

  // フォールバックポリシー（有）
  HttpResponseMessage PostFallback<T>(string uri, T item);

  // タイムアウトポリシー（有）
  HttpResponseMessage PostTimeout<T>(string uri, T item);
}

public class ResilientHttpClient : IResilientHttpClient
{
  CircuitBreakerPolicy<HttpResponseMessage> _circuitBreakerPolicy;
  Policy<HttpResponseMessage> _retryPolicy;
  HttpClient _client;

  // Fallback Policy
  FallbackPolicy<HttpResponseMessage> _fallbackPolicy;
  FallbackPolicy<HttpResponseMessage> _fallbackCircuitBreakerPolicy;

  // TimeOut Policy
  TimeoutPolicy<HttpResponseMessage> _timeoutPolicy;

  public ResilientHttpClient(HttpClient client, CircuitBreakerPolicy<HttpResponseMessage> circuitBreakerPolicy)
  {
    // HttpClientの初期化
    _client = client;
    _client.DefaultRequestHeaders.Accept.Clear();
    _client.DefaultRequestHeaders.Accept.Add(
      new MediaTypeWithQualityHeaderValue("application/json")
    );
    
    // サーキットブレーカーの初期化
    _circuitBreakerPolicy = circuitBreakerPolicy;
    
    // 再試行ポリシーの初期化
    _retryPolicy = Policy.HandleResult<HttpResponseMessage>(x => {
      var result = !x.IsSuccessStatusCode;
      return result;
    })
    // 再試行回数:3 再試行間隔:3秒
    .WaitAndRetry(3, sleepDurations => TimeSpan.FromSeconds(3));

    // フォールバック用サーキットブレーカーポリシーの初期化
    _fallbackCircuitBreakerPolicy = Policy<HttpResponseMessage>
      .Handle<BrokenCircuitException>()
      .Fallback(new HttpResponseMessage(HttpStatusCode.OK) {
        Content = new StringContent("Please try again later[Circuit breaker is Open]")
      });

    // フォールバックポリシーの初期化
    _fallbackPolicy = Policy.HandleResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.InternalServerError)
      .Fallback(new HttpResponseMessage(HttpStatusCode.OK) {
        Content = new StringContent("Some error occured")
      });

    // タイムアウトポリシーの初期化
    _timeoutPolicy = Policy.Timeout<HttpResponseMessage>(1);
  }

  /// <summary>
  /// 再試行 & サーキットブレーカー ポリシーで実行
  /// </summary>
  public HttpResponseMessage Execute_With_Retry_CircuitBreaker(string uri, Func<HttpResponseMessage> func)
  {
    var res = _retryPolicy.Wrap(_circuitBreakerPolicy).Execute(() => func());
    return res;
  }

  /// <summary>
  /// フォールバック & 再試行 & サーキットブレーカー ポリシーで実行
  /// </summary>
  public HttpResponseMessage Execute_With_Fallback_Retry_CircuitBreaker(string uri, Func<HttpResponseMessage> func)
  {
    // サーキットブレーカーを、再試行ポリシーでラップ
    PolicyWrap<HttpResponseMessage> resiliencePolicyWrap = Policy.Wrap(_retryPolicy, _circuitBreakerPolicy);
    
    // 再試行ポリシーでラップされたサーキットブレーカーを、フォールバックポリシーでラップ
    PolicyWrap<HttpResponseMessage> fallbackPolicyWrap = _fallbackPolicy.Wrap(_fallbackCircuitBreakerPolicy.Wrap(resiliencePolicyWrap));
    
    var res = fallbackPolicyWrap.Execute(() => func());
    return res;
  }

  /// <summary>
  /// タイムアウト & フォールバック & 再試行 & サーキットブレーカー ポリシーで実行
  /// </summary>
  public HttpResponseMessage Execute_With_Timeout_Fallback_Retry_CircuitBreaker(string uri, Func<HttpResponseMessage> func)
  {
    // ポリシーをラップ: タイムアウト（再試行（サーキットブレーカー））
    PolicyWrap<HttpResponseMessage> resiliencePolicyWrap = Policy.Wrap(_timeoutPolicy, _retryPolicy, _circuitBreakerPolicy);
    
    // 再試行ポリシーでラップされたサーキットブレーカーを、フォールバックポリシーでラップ
    PolicyWrap<HttpResponseMessage> fallbackPolicyWrap = _fallbackPolicy.Wrap(_fallbackCircuitBreakerPolicy.Wrap(resiliencePolicyWrap));
    
    var res = fallbackPolicyWrap.Execute(() => func());
    return res;
  }

  /// <summary>
  /// [HttpGet] 再試行ポリシー & サーキットブレーカーポリシー
  /// </summary>
  public HttpResponseMessage Get(string uri)
  {
    return Execute_With_Retry_CircuitBreaker(uri, () => {
      try
      {
        var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

        var response = _client.SendAsync(requestMessage).Result;

        return response;
      }
      catch (Exception)
      {
        HttpResponseMessage res = new HttpResponseMessage();
        res.StatusCode = HttpStatusCode.InternalServerError;
        return res;
      }
    });
  }

  /// <summary>
  /// [HttpPost] 再試行ポリシー & サーキットブレーカーポリシー
  /// </summary>
  public HttpResponseMessage Post<T>(string uri, T item)
  {
    return Execute_With_Retry_CircuitBreaker(uri, () => {
      try
      {
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

        requestMessage.Content = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json");

        var response = _client.SendAsync(requestMessage).Result;

        return response;
      }
      catch (Exception)
      {
        HttpResponseMessage res = new HttpResponseMessage();
        res.StatusCode = HttpStatusCode.InternalServerError;
        return res;
      }
    });
  }

  /// <summary>
  /// [HttpPut] 再試行ポリシー & サーキットブレーカーポリシー
  /// </summary>
  public HttpResponseMessage Put<T>(string uri, T item)
  {
    return Execute_With_Retry_CircuitBreaker(uri, () => {
      try
      {
        var requestMessage = new HttpRequestMessage(HttpMethod.Put, uri);

        requestMessage.Content = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json");

        var response = _client.SendAsync(requestMessage).Result;

        return response;
      }
      catch (Exception)
      {
        HttpResponseMessage res = new HttpResponseMessage();
        res.StatusCode = HttpStatusCode.InternalServerError;
        return res;
      }
    });
  }

  /// <summary>
  /// [HttpDelete] 再試行ポリシー & サーキットブレーカーポリシー
  /// </summary>
  public HttpResponseMessage Delete(string uri)
  {
    return Execute_With_Retry_CircuitBreaker(uri, () => {
      try
      {
        var requestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

        var response = _client.SendAsync(requestMessage).Result;

        return response;
      }
      catch (Exception)
      {
        HttpResponseMessage res = new HttpResponseMessage();
        res.StatusCode = HttpStatusCode.InternalServerError;
        return res;
      }
    });
  }

  /// <summary>
  /// [HttpPost] フォールバックポリシー & 再試行ポリシー & サーキットブレーカーポリシー
  /// </summary>
  public HttpResponseMessage PostFallback<T>(string uri, T item)
  {
    return Execute_With_Fallback_Retry_CircuitBreaker(uri, () => {
      try
      {
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

        requestMessage.Content = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json");

        var response = _client.SendAsync(requestMessage).Result;

        return response;
      }
      catch (Exception)
      {
        HttpResponseMessage res = new HttpResponseMessage();
        res.StatusCode = HttpStatusCode.InternalServerError;
        return res;
      }
    });
  }

  /// <summary>
  /// [HttpPost] タイムアウトポリシー & フォールバックポリシー & 再試行ポリシー & サーキットブレーカーポリシー
  /// </summary>
  public HttpResponseMessage PostTimeout<T>(string uri, T item)
  {
    return Execute_With_Timeout_Fallback_Retry_CircuitBreaker(uri, () => {
      try
      {
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

        requestMessage.Content = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json");

        var response = _client.SendAsync(requestMessage).Result;

        return response;
      }
      catch (Exception)
      {
        HttpResponseMessage res = new HttpResponseMessage();
        res.StatusCode = HttpStatusCode.InternalServerError;
        return res;
      }
    });
  }

}