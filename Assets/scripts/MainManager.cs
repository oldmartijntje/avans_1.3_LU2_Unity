using Assets.scripts.Models;
using Newtonsoft.Json;
using System.IO;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    // Start() and Update() methods deleted - we don't need them right now

    public static MainManager Instance;
    public LoginResponse? LoginResponse;

    private void Awake()
    {
        // start of new code
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        // end of new code

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetLoginCredentials(LoginResponse loginResponse)
    {
        MainManager.Instance.LoginResponse = loginResponse;
    }

    private void Start()
    {
        string filePath = "UserSettings/playerLogin.json";

        if (System.IO.File.Exists(filePath))
        {
            string jsonString = System.IO.File.ReadAllText(filePath);
            LoginResponse = JsonConvert.DeserializeObject<LoginResponse>(jsonString);
        }
        else
        {
            Debug.Log("No data found.");
        }
    }


}