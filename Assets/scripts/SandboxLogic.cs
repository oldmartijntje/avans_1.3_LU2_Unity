using Newtonsoft.Json;
using TMPro;
using UnityEngine;

public class SandboxLogic : MonoBehaviour
{
    private ApiConnecter apiConnecter;
    public CanvasGroup LoadingScreenPanel;
    public CanvasGroup MainContentPanel;

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
        StartCoroutine(apiConnecter.SendAuthGetRequest($"/api/Environment/{MainManager.Instance.environmentSelected}", RenderUI));
    }

    private void RenderUI(string response, string error)
    {
        if (apiConnecter.HandleLoginError(response, error, true))
        {
            return;
        }
        if (error == null)
        {
            Debug.Log(response);
        } else
        {
            Debug.Log(error);
        }
    }



    void Update()
    {
        
    }
}
