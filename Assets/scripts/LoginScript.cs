using TMPro;
using UnityEngine;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using Assets.scripts.Models;
using ColorUtility = UnityEngine.ColorUtility;
using UnityEngine.UI;

public class LoginScript : MonoBehaviour
{
    private string passwordValue = "";
    private string usernameValue = "";
    public TextMeshProUGUI errorMessageLabel;
    public TMP_InputField passwordField;
    private ApiConnecter apiConnecter;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        apiConnecter = FindFirstObjectByType<ApiConnecter>();
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
                errorMessageLabel.text = "Username already taken.";
                Debug.LogError(error);
            }
        }));
    }

    private void LoginUser()
    {
        string json = JsonConvert.SerializeObject(new { email= usernameValue, password= passwordValue }, Formatting.Indented);
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
            }
            else
            {
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
        } else
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
