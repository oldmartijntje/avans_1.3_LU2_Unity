using Assets.scripts.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SandboxLogic : MonoBehaviour
{
    private ApiConnecter apiConnecter;
    public CanvasGroup LoadingScreenPanel;
    public CanvasGroup MainContentPanel;
    private FullEnvironment2DObject fullEnvironment2DObject;

    void Start()
    {
        LoadingScreenPanel.alpha = 1f;
        MainContentPanel.alpha = 0f;
        MainContentPanel.interactable = false;
        LoadingScreenPanel.interactable = true;
        if (LoadingScreenPanel != null && LoadingScreenPanel.transform.childCount >= 2)
        {
            // Access the second child and get the TextMeshProUGUI component
            TextMeshProUGUI LoadingScreenTooltipLabel = LoadingScreenPanel.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            LoadingScreenTooltipLabel.text = "Manually placing your objects...";
        }
        apiConnecter = FindFirstObjectByType<ApiConnecter>();
        if (apiConnecter == null)
        {
            Debug.LogError("No API Connector found!");
        }
        Debug.Log($"api/Environment/{MainManager.Instance.environmentSelected}");
        StartCoroutine(apiConnecter.SendAuthGetRequest($"/api/Environment/{MainManager.Instance.environmentSelected}", HandleGetResponse));
    }

    private void HandleGetResponse(string response, string error)
    {
        if (apiConnecter.HandleLoginError(response, error, true, LoadingScreenPanel, MainContentPanel))
        {
            return;
        }
        if (error == null)
        {
            try
            {
                Debug.Log(response);
                FullEnvironment2DObject json = JsonConvert.DeserializeObject<FullEnvironment2DObject>(response);
                Refresh();
            }
            catch
            {
                Debug.LogError($"Unparsable: {response}");
                SceneManager.LoadScene("LoginScene");
            }
            
        } else
        {
            SceneManager.LoadScene("EnvironmentSelect");
            Debug.Log(error);
        }
    }

    public void Refresh()
    {
        Debug.Log("Refreshing UI");
    }

    void Update()
    {
        
    }
}
