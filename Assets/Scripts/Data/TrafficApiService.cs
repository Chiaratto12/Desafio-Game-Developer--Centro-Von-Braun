using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using Newtonsoft.Json;

public class TrafficApiService : MonoBehaviour
{
    private const string API_URL = "http://localhost:3000/v1/traffic/status";

    /// <summary>
    /// Requisita para api os status de tráfico
    /// </summary>
    /// <param name="onSuccess">Chama ação caso tenha sucesso</param>
    /// <param name="onError"></param>
    /// <returns></returns>
    public IEnumerator FetchTrafficStatus(Action<TrafficResponse> onSuccess, Action<string> onError)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(API_URL))
        {
            request.SetRequestHeader("Accept", "application/json");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                TrafficResponse response = JsonConvert.DeserializeObject<TrafficResponse>(json);
                onSuccess?.Invoke(response);
            }
            else
            {
                Debug.LogWarning($"[API] Falha: {request.error}. Carregando mock local.");
                onError?.Invoke(request.error);
            }
        }
    }

    /// <summary>
    /// Pega o mock local caso não use uma api exterior
    /// </summary>
    /// <returns></returns>
    public TrafficResponse LoadLocalMock()
    {
        TextAsset mockFile = Resources.Load<TextAsset>("mock_response");
        return JsonConvert.DeserializeObject<TrafficResponse>(mockFile.text);
    }
}