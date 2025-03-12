using Assets.scripts;
using Assets.scripts.Models;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SandboxLogic : MonoBehaviour
{
    private ApiConnecter apiConnecter;
    public CanvasGroup LoadingScreenPanel;
    public CanvasGroup MainContentPanel;
    public GameObject ObjectPrefab;
    public GameObject ObjectSpawnerPrefab;
    public int PixelsPerCoordinate = 8;
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
        RefreshInventoryUI();
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
                MainManager.Instance.fullEnvironment2DObject = JsonConvert.DeserializeObject<FullEnvironment2DObject>(response);
            }
            catch
            {
                Debug.LogError($"Unparsable: {response}");
                MainManager.Instance.NavigationScene = "";
                SceneManager.LoadScene("LoginScene");
            }
            var env = MainManager.Instance.fullEnvironment2DObject;
            if (MainContentPanel == null || MainContentPanel.transform.childCount < 2)
            {
                Debug.LogError($"Level panel not found");
                MainManager.Instance.NavigationScene = "";
                SceneManager.LoadScene("LoginScene");
            }
            RectTransform parentRectTransform = MainContentPanel.transform.GetChild(0).GetComponent<RectTransform>();
            Debug.Log($"Parent RectTransform: {parentRectTransform.name}, Child count before instantiation: {parentRectTransform.childCount}");
            parentRectTransform.sizeDelta = new Vector2((env.environmentData.MaxLength * PixelsPerCoordinate) + (PixelsOffset * 2), (env.environmentData.MaxHeight * PixelsPerCoordinate) + (PixelsOffset * 2));
            parentRectTransform.position = new Vector3(((env.environmentData.MaxLength * PixelsPerCoordinate) + (PixelsOffset * 2)) / 2, 
                ((env.environmentData.MaxHeight * PixelsPerCoordinate) + (PixelsOffset * 2)) / 2, 0);
            foreach (Transform child in parentRectTransform)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < env.environmentObjects.Count; i++)
            {
                var singleObject = env.environmentObjects[i];
                var vector = new Vector3((singleObject.PositionX * PixelsPerCoordinate) + PixelsOffset, (singleObject.PositionY * PixelsPerCoordinate) + PixelsOffset, singleObject.SortingLayer);
                Debug.Log($"{vector} - {singleObject.PositionX} : {singleObject.PositionY}");
                GameObject object2D = Instantiate(ObjectPrefab, vector, Quaternion.identity, parentRectTransform);
                GameSprite dragDropScript = object2D.GetComponent<GameSprite>();
                dragDropScript.ObjectData = singleObject;
                if (dragDropScript != null)
                {
                    dragDropScript.SetSprite(singleObject.PrefabId);
                }
                if (object2D != null && object2D.transform.childCount >= 4)
                {
                    
                }
            }
            LoadingScreenPanel.alpha = 0f;
            MainContentPanel.alpha = 1f;
            MainContentPanel.interactable = true;
            LoadingScreenPanel.interactable = false;
            LoadingScreenPanel.blocksRaycasts = false;

        } else
        {
            SceneManager.LoadScene("EnvironmentSelect");
            Debug.Log(error);
        }
    }

    public void RefreshInventoryUI()
    {
        RectTransform parentRectTransform = MainContentPanel.transform.GetChild(1).GetComponent<RectTransform>().GetChild(0).GetComponent<RectTransform>();
        Debug.Log($"Parent RectTransform: {parentRectTransform.name}, Child count before instantiation: {parentRectTransform.childCount}");
        foreach (Transform child in parentRectTransform)
        {
            Destroy(child.gameObject);
        }
        for (int i = 0; i < 4; i++)
        {
            var offsetX = 1700;
            var offsetY = 950;
            var vector = new Vector3(offsetX, offsetY + (i * -50), 0);
            GameObject object2D = Instantiate(ObjectSpawnerPrefab, vector, Quaternion.identity, parentRectTransform);
            GameSpriteSpawner dragDropScript = object2D.GetComponent<GameSpriteSpawner>();
            if (dragDropScript != null)
            {
                dragDropScript.SetSprite(i);
                dragDropScript.SetBasePos(vector);
            }
        }
    }

    public void HomeMenu()
    {
        SceneManager.LoadScene("EnvironmentSelect");
    }

    public void Refresh(bool showLoadingScreen)
    {
        if (showLoadingScreen)
        {
            LoadingScreenPanel.alpha = 1f;
            MainContentPanel.alpha = 0f;
            MainContentPanel.interactable = false;
            LoadingScreenPanel.blocksRaycasts = true;
            LoadingScreenPanel.interactable = true;
        }
        StartCoroutine(apiConnecter.SendAuthGetRequest($"api/Environment/{MainManager.Instance.environmentSelected}", RenderUI));
    }

    void Update()
    {
        
    }
}
