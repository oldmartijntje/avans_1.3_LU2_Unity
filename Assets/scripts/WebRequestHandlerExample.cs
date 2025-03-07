using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class WebRequestHandlerExample : MonoBehaviour
{
    private string baseUrl = "https://avansict2225486.azurewebsites.net/"; // Example API URL

    // GET Request
    public void GetRequest(int id, Action<string, string> callback)
    {
        StartCoroutine(SendGetRequest(id, callback));
    }

    private IEnumerator SendGetRequest(int id, Action<string, string> callback)
    {
        string url = $"{baseUrl}";
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
                callback?.Invoke(request.downloadHandler.text, null);
            else
                callback?.Invoke(null, request.error);
        }
    }

    // POST Request
    public void PostRequest(string jsonData, Action<string, string> callback)
    {
        StartCoroutine(SendPostRequest(jsonData, callback));
    }

    private IEnumerator SendPostRequest(string jsonData, Action<string, string> callback)
    {
        using (UnityWebRequest request = new UnityWebRequest(baseUrl, "POST"))
        {
            byte[] jsonToSend = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
                callback?.Invoke(request.downloadHandler.text, null);
            else
                callback?.Invoke(null, request.error);
        }
    }

    // PUT Request
    public void PutRequest(int id, string jsonData, Action<string, string> callback)
    {
        StartCoroutine(SendPutRequest(id, jsonData, callback));
    }

    private IEnumerator SendPutRequest(int id, string jsonData, Action<string, string> callback)
    {
        string url = $"{baseUrl}/{id}";
        using (UnityWebRequest request = new UnityWebRequest(url, "PUT"))
        {
            byte[] jsonToSend = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
                callback?.Invoke(request.downloadHandler.text, null);
            else
                callback?.Invoke(null, request.error);
        }
    }

    // DELETE Request
    public void DeleteRequest(int id, Action<string, string> callback)
    {
        StartCoroutine(SendDeleteRequest(id, callback));
    }

    private IEnumerator SendDeleteRequest(int id, Action<string, string> callback)
    {
        string url = $"{baseUrl}/{id}";
        using (UnityWebRequest request = UnityWebRequest.Delete(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
                callback?.Invoke(request.downloadHandler.text, null);
            else
                callback?.Invoke(null, request.error);
        }
    }
}
