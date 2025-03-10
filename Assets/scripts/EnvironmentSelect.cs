using Assets.scripts.Models;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnvironmentSelect : MonoBehaviour
{
    public CanvasGroup LoadingScreenPanel;
    public CanvasGroup MainContentPanel;
    private ApiConnecter apiConnecter;
    public GameObject activeRoomPrefab;
    public GameObject formPrefab;
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
        StartCoroutine(apiConnecter.SendAuthGetRequest("api/Environment", (string response, string error) =>
        {
            if (error == null)
            {
                RectTransform parentRectTransform = MainContentPanel.GetComponent<RectTransform>();
                Debug.Log(response);
                List<Environment2D> environments = JsonConvert.DeserializeObject<List<Environment2D>>(response);
                Instantiate(formPrefab, coordsList[0], Quaternion.identity, parentRectTransform);
                for (int i = 1; i < environments.Count + 1; i++)
                {
                    GameObject environment = Instantiate(activeRoomPrefab, coordsList[i], Quaternion.identity, parentRectTransform);
                    if (environment != null && environment.transform.childCount >= 2)
                    {
                        // Access the second child and get the TextMeshProUGUI component
                        TextMeshProUGUI environmentNameLabel = environment.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                        environmentNameLabel.text = environments[i - 1].Name;
                        TextMeshProUGUI environmentSizeLabel = environment.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                        environmentSizeLabel.text = $"Size: {environments[i - 1].MaxHeight}x{environments[i - 1].MaxLength}";
                    }
                }
            }
            else
            {
                Debug.LogError(error);
            }
            LoadingScreenPanel.alpha = 0f;
            LoadingScreenPanel.interactable = false;
            MainContentPanel.alpha = 1f;
            MainContentPanel.interactable = true;
        }));

    }

    void Update()
    {
        
    }
}
