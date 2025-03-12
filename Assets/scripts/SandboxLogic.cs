using Assets.scripts.Models;
using Newtonsoft.Json;
using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SandboxLogic : MonoBehaviour
{
    private ApiConnecter apiConnecter;
    public CanvasGroup LoadingScreenPanel;
    public CanvasGroup MainContentPanel;
    private FullEnvironment2DObject fullEnvironment2DObject;
    public GameObject ObjectPrefab;
    public int PixelsPerCoordinate = 32;
    public int PixelsOffset = 16;
    [SerializeField] private Canvas mainCanvas;

    void Start()
    {
        MainManager.Instance.NavigationScene = "Sandbox";
        LoadingScreenPanel.alpha = 1f;
        MainContentPanel.alpha = 0f;
        MainContentPanel.interactable = false;
        LoadingScreenPanel.interactable = true;
        LoadingScreenPanel.blocksRaycasts = true;
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
        if (apiConnecter.HandleLoginError(response, error, true, LoadingScreenPanel, MainContentPanel))
        {
            return;
        }
        if (error == null)
        {
            try
            {
                Debug.Log(response);
                fullEnvironment2DObject = JsonConvert.DeserializeObject<FullEnvironment2DObject>(response);
            }
            catch
            {
                Debug.LogError($"Unparsable: {response}");
                MainManager.Instance.NavigationScene = "";
                SceneManager.LoadScene("LoginScene");
            }
            if (MainContentPanel == null || MainContentPanel.transform.childCount < 2)
            {
                Debug.LogError($"Level panel not found");
                MainManager.Instance.NavigationScene = "";
                SceneManager.LoadScene("LoginScene");
            }
            RectTransform parentRectTransform = MainContentPanel.transform.GetChild(0).GetComponent<RectTransform>();
            Debug.Log($"Parent RectTransform: {parentRectTransform.name}, Child count before instantiation: {parentRectTransform.childCount}");

            foreach (Transform child in parentRectTransform)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < fullEnvironment2DObject.environmentObjects.Count; i++)
            {
                var singleObject = fullEnvironment2DObject.environmentObjects[i];
                var vector = new Vector3((singleObject.PositionX * PixelsPerCoordinate) + PixelsOffset, (singleObject.PositionY * PixelsPerCoordinate) + PixelsOffset, singleObject.SortingLayer);
                Debug.Log($"{vector} - {singleObject.PositionX} : {singleObject.PositionY}");
                GameObject object2D = Instantiate(ObjectPrefab, vector, Quaternion.identity, parentRectTransform);
                DragDrop dragDropScript = object2D.GetComponent<DragDrop>();
                if (dragDropScript != null)
                {
                    //dragDropScript.SetCanvas(mainCanvas);
                }
                if (object2D != null && object2D.transform.childCount >= 4)
                {
                    //// Access the second child and get the TextMeshProUGUI component
                    //TextMeshProUGUI environmentNameLabel = environment.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                    //environmentNameLabel.text = $"{environments[currentIndex].Name}";
                    //TextMeshProUGUI environmentSizeLabel = environment.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                    //environmentSizeLabel.text = $"Size: {environments[currentIndex].MaxHeight}x{environments[currentIndex].MaxLength}";
                    //Button environmentDeleteButton = environment.transform.GetChild(2).GetComponent<Button>();
                    //environmentDeleteButton.onClick.RemoveAllListeners();
                    //environmentDeleteButton.onClick.AddListener(() => DeleteEnvironment(environments[currentIndex].Id));
                    //Button environmentLoadButton = environment.transform.GetChild(3).GetComponent<Button>();
                    //environmentLoadButton.onClick.RemoveAllListeners();
                    //environmentLoadButton.onClick.AddListener(() => LoadEnvironment(environments[currentIndex].Id));
                }
                LoadingScreenPanel.alpha = 0f;
                MainContentPanel.alpha = 1f;
                MainContentPanel.interactable = true;
                LoadingScreenPanel.interactable = false;
                LoadingScreenPanel.blocksRaycasts = false;
            }

        } else
        {
            SceneManager.LoadScene("EnvironmentSelect");
            Debug.Log(error);
        }
    }

    public void Refresh()
    {
        LoadingScreenPanel.alpha = 1f;
        MainContentPanel.alpha = 0f;
        MainContentPanel.interactable = false;
        LoadingScreenPanel.blocksRaycasts = true;
        LoadingScreenPanel.interactable = true;
        StartCoroutine(apiConnecter.SendAuthGetRequest($"api/Environment/{MainManager.Instance.environmentSelected}", RenderUI));
    }

    void Update()
    {
        
    }
}
