using TMPro;
using UnityEngine;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using Assets.scripts.Models;
using ColorUtility = UnityEngine.ColorUtility;
using UnityEngine.UI;
using System.Collections;

public class LoginScript : MonoBehaviour
{
    private string passwordValue = "";
    private string usernameValue = "";
    public TextMeshProUGUI errorMessageLabel;
    public TMP_InputField passwordField;
    public CanvasGroup LoadingPanel;
    public CanvasGroup LoginPanel;
    private ApiConnecter apiConnecter;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        apiConnecter = FindFirstObjectByType<ApiConnecter>();
        StartCoroutine(DelayedRequest());
    }

    IEnumerator DelayedRequest()
    {
        yield return new WaitForSeconds(1f);
        StartCoroutine(apiConnecter.SendAuthGetRequest("account/checkAccessToken", (string response, string error) =>
        {
            if (error == null)
            {
                SceneManager.LoadScene("EnvironmentSelect");
            }
            else
            {
                string filePath = "UserSettings/playerLogin.json";
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
            LoadingPanel.alpha = 0f;
            LoginPanel.alpha = 1f;
            LoginPanel.interactable = true;
        }));
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void RegisterUser()
    {
        if (!Validator.IsValidEmail(usernameValue))
        {
            errorMessageLabel.text = "Invalid MailAddress";
            return;
        }
        else if (!Validator.IsValidPassword(passwordValue))
        {
            if (passwordValue.Length < 10)
            {
                errorMessageLabel.text = "Password must be 10+ characters";
                return;
            }
            else
            {
                errorMessageLabel.text = "Password must have 1 lowercase, 1 uppercase, 1 number and 1 special character.";
                return;
            }
        }
        string json = JsonConvert.SerializeObject(new { email = usernameValue, password = passwordValue }, Formatting.Indented);
        Debug.Log(json);
        StartCoroutine(apiConnecter.SendPostRequest(json, "account/register", (string response, string error) =>
        {
            SetTextColor("#FFFFFF", errorMessageLabel);
            errorMessageLabel.text = "Connecting...";
            if (error == null)
            {
                Debug.Log("Response: " + response);
                SetTextColor("#FFFFFF", errorMessageLabel);
                errorMessageLabel.text = "Account Created! Re-enter password to Login.";
                passwordField.Select();
                passwordField.text = "";
                passwordValue = "";
            }
            else
            {
                SetTextColor("#FF0000", errorMessageLabel);
                errorMessageLabel.text = "Username already taken.";
                Debug.LogError(error);
            }
        }));
    }

    private void LoginUser()
    {
        SetTextColor("#FFFFFF", errorMessageLabel);
        errorMessageLabel.text = "Connecting...";
        string json = JsonConvert.SerializeObject(new { email = usernameValue, password = passwordValue }, Formatting.Indented);
        Debug.Log(json);
        StartCoroutine(apiConnecter.SendPostRequest(json, "account/login", (string response, string error) =>
        {
            if (error == null)
            {
                errorMessageLabel.text = "";
                Debug.Log("Response: " + response);
                SceneManager.LoadScene("EnvironmentSelect");
                LoginResponse decodedResponse = JsonConvert.DeserializeObject<LoginResponse>(response);
                MainManager.Instance.SetLoginCredentials(decodedResponse);
                System.IO.File.WriteAllText("UserSettings/playerLogin.json", response);
            }
            else
            {
                SetTextColor("#FF0000", errorMessageLabel);
                errorMessageLabel.text = "Invalid username & password combination.";
            }
        }));
    }

    public void ClickButton(string registerOrLogin)
    {
        errorMessageLabel.text = "";
        SetTextColor("#FF0000", errorMessageLabel);
        if (registerOrLogin == "Register")
        {
            RegisterUser();
        }
        else
        {
            LoginUser();
        }
    }

    public void SetPasswordValue(string value)
    {
        passwordValue = value;
    }

    public void SetTextColor(string colorText, TextMeshProUGUI element)
    {
        Color color;
        if (ColorUtility.TryParseHtmlString(colorText, out color))
        {
            element.color = color;
        }
        else
        {
            Debug.LogError("Invalid color string");
        }
    }

    public void SetUsernameValue(string value)
    {
        usernameValue = value;
    }
}
