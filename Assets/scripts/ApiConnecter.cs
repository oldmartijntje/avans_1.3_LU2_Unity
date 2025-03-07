using Newtonsoft.Json;
using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class ApiConnecter : MonoBehaviour
{
    private string baseUrl = "https://avansict2225486.azurewebsites.net";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HandleResponse(string response, string error)
    {
        if (error == null)
        {
            Debug.Log("Response: " + response);
        }
        else
        {
            Debug.LogError("Error: " + error);
        }
    }

    public IEnumerator SendGetRequest(string path, Action<string, string> callback)
    {
        string url = $"{baseUrl}/{path}";
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
                callback?.Invoke(request.downloadHandler.text, null);
            else
                callback?.Invoke(null, request.error);
        }
    }

    public IEnumerator SendAuthGetRequest(string path, Action<string, string> callback)
    {
        string url = $"{baseUrl}/{path}";
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            if (MainManager.Instance.LoginResponse == null)
            {
                callback?.Invoke(null, "Not logged in");
            } else
            {
                request.SetRequestHeader("Authorization", $"Bearer {MainManager.Instance.LoginResponse.accessToken}");

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    callback?.Invoke(request.downloadHandler.text, null);
                }
                else
                {
                    Debug.LogError(JsonConvert.SerializeObject(request.error));
                    callback?.Invoke(null, request.error);
                }
            }
        }
    }

    public IEnumerator SendPostRequest(string jsonData, string path, Action<string, string> callback)
    {
        string url = $"{baseUrl}/{path}";
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
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

    public IEnumerator SendPostRequest(string jsonData, string path, string accessToken, Action<string, string> callback)
    {
        string url = $"{baseUrl}/{path}";
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] jsonToSend = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // Add Authorization header
            request.SetRequestHeader("Authorization", $"Bearer {accessToken}");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
                callback?.Invoke(request.downloadHandler.text, null);
            else
                callback?.Invoke(null, request.error);
        }
    }
}
