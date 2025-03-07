using TMPro;
using UnityEngine;

public class LoginScript : MonoBehaviour
{
    private string passwordValue = "";
    private string usernameValue = "";
    public TextMeshProUGUI errorMessageLabel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

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
        errorMessageLabel.text = "";
        errorMessageLabel.text = "Can't reach the server. Try again later.";
    }

    private void LoginUser()
    {
        errorMessageLabel.text = "Can't reach the server. Try again later.";
    }

    public void ClickButton(string registerOrLogin)
    {
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

    public void SetUsernameValue(string value)
    {
        usernameValue = value;
    }
}
