using Assets.scripts.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnvironmentSelect : MonoBehaviour
{
    public CanvasGroup LoadingScreenPanel;
    public CanvasGroup MainContentPanel;
    private ApiConnecter apiConnecter;
    public GameObject activeRoomPrefab;
    public GameObject formPrefab;
    private GameObject environmentCreationForm;
    private double paddingX = 56;
    private double paddingY = 61;
    private double width = 563.4;
    private double height = 274.8;
    public double offsetX = 340;
    public double offsetY = -125;
    private double amountX = 3;
    private double amountY = 3;
    private List<Vector3> coordsList = new List<Vector3>();
    void Start()
    {
        MainManager.Instance.NavigationScene = "EnvironmentSelect";
         for (var yi = amountY; yi > 0; yi--)
        {
            for (var xi = 0; xi < amountX; xi++)
            {
                coordsList.Add(new Vector3(Mathf.Round((float)(offsetX + (xi * (paddingX + width)))), Mathf.Round((float)(offsetY + (yi * (paddingY + height)))), 0));
            }
        }

        LoadingScreenPanel.alpha = 1f;
        MainContentPanel.alpha = 0f;
        MainContentPanel.interactable = false;
        LoadingScreenPanel.blocksRaycasts = true;
        LoadingScreenPanel.interactable = true;
        if (LoadingScreenPanel != null && LoadingScreenPanel.transform.childCount >= 2)
        {
            // Access the second child and get the TextMeshProUGUI component
            TextMeshProUGUI LoadingScreenTooltipLabel = LoadingScreenPanel.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            LoadingScreenTooltipLabel.text = "Loading your environments...";
        }
        apiConnecter = FindFirstObjectByType<ApiConnecter>();
        if (apiConnecter == null)
        {
            Debug.LogError("No API Connector found!");
        }
        StartCoroutine(apiConnecter.SendAuthGetRequest("api/Environment", RenderUI));

    }

    private void RenderUI(string response, string error)
    {
        if (apiConnecter.HandleLoginError(response, error, true))
        {
            return;
        }
        if (error == null)
        {
            RectTransform parentRectTransform = MainContentPanel.GetComponent<RectTransform>();

            foreach (Transform child in parentRectTransform)
            {
                Destroy(child.gameObject);
            }

            List<Environment2D> environments = JsonConvert.DeserializeObject<List<Environment2D>>(response);
            environmentCreationForm = Instantiate(formPrefab, coordsList[0], Quaternion.identity, parentRectTransform);
            for (int i = 1; i < environments.Count + 1; i++)
            {
                if (i - 1 >= environments.Count) continue;
                int currentIndex = i - 1;

                GameObject environment = Instantiate(activeRoomPrefab, coordsList[i], Quaternion.identity, parentRectTransform);
                if (environment != null && environment.transform.childCount >= 4)
                {
                    // Access the second child and get the TextMeshProUGUI component
                    TextMeshProUGUI environmentNameLabel = environment.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                    environmentNameLabel.text = $"{environments[currentIndex].Name}";
                    TextMeshProUGUI environmentSizeLabel = environment.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                    environmentSizeLabel.text = $"Size: {environments[currentIndex].MaxHeight}x{environments[currentIndex].MaxLength}";
                    Button environmentDeleteButton = environment.transform.GetChild(2).GetComponent<Button>();
                    environmentDeleteButton.onClick.RemoveAllListeners();
                    environmentDeleteButton.onClick.AddListener(() => DeleteEnvironment(environments[currentIndex].Id));
                    Button environmentLoadButton = environment.transform.GetChild(3).GetComponent<Button>();
                    environmentLoadButton.onClick.RemoveAllListeners();
                    environmentLoadButton.onClick.AddListener(() => LoadEnvironment(environments[currentIndex].Id));
                }
            }
        }
        else
        {
            Debug.LogError(error);
        }
        LoadingScreenPanel.alpha = 0f;
        LoadingScreenPanel.interactable = false;
        LoadingScreenPanel.blocksRaycasts = false;
        MainContentPanel.alpha = 1f;
        MainContentPanel.interactable = true;
    }

    void Update()
    {
        
    }

    public void CreateEnvironment(string name, int width, int height, Action<string, string> callback)
    {
        string jsonString = JsonConvert.SerializeObject(new
        {
            name = name,
            maxHeight = height,
            maxLength = width
        });
        StartCoroutine(apiConnecter.SendAuthPostRequest(jsonString, "api/Environment", callback));
    }

    public void LoadEnvironment(int identifier)
    {
        MainManager.Instance.environmentSelected = identifier;
        SceneManager.LoadScene("Sandbox");
    }

    public void DeleteEnvironment(int identifier)
    {
        StartCoroutine(apiConnecter.SendAuthDeleteRequest($"api/Environment/{identifier}", (string response, string error) =>
        {
            if (apiConnecter.HandleLoginError(response, error,true))
            {
                return;
            }
            Refresh();
        }));
    }

    public void Refresh()
    {
        LoadingScreenPanel.alpha = 1f;
        MainContentPanel.alpha = 0f;
        MainContentPanel.interactable = false;
        LoadingScreenPanel.interactable = true;
        LoadingScreenPanel.blocksRaycasts = true;
        StartCoroutine(apiConnecter.SendAuthGetRequest("api/Environment", RenderUI));
    }

}
