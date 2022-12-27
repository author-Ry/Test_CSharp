using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Polly;
using Polly.CircuitBreaker;

namespace _03_Securing_ImplementingResilience.Services;

public interface IResilientHttpClient
{
  HttpResponseMessage Get(string uri);
  HttpResponseMessage Post<T>(string uri, T item);
  HttpResponseMessage Put<T>(string uri, T item);
  HttpResponseMessage Delete(string uri);
}

public class ResilientHttpClient : IResilientHttpClient
{
  CircuitBreakerPolicy<HttpResponseMessage> _circuitBreakerPolicy;
  Policy<HttpResponseMessage> _retryPolicy;
  HttpClient _client;

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
  }

  /// <summary>
  /// 再試行ポリシーでラップしたサーキットブレーカーポリシーで実行
  /// </summary>
  public HttpResponseMessage ExecuteWithRetryAndCircuitBreaker(string uri, Func<HttpResponseMessage> func)
  {
    var res = _retryPolicy.Wrap(_circuitBreakerPolicy).Execute(() => func());
    return res;
  }

  /// <summary>
  /// [HttpGet] 再試行ポリシー & サーキットブレーカーポリシー
  /// </summary>
  public HttpResponseMessage Get(string uri)
  {
    return ExecuteWithRetryAndCircuitBreaker(uri, () => {
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
    return ExecuteWithRetryAndCircuitBreaker(uri, () => {
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
    return ExecuteWithRetryAndCircuitBreaker(uri, () => {
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
    return ExecuteWithRetryAndCircuitBreaker(uri, () => {
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

}