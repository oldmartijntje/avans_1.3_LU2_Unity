using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;

public class EnvSelectPrefebFunctions : MonoBehaviour
{
    private string nameValue = "";
    private int lengthValue = 0;
    private int heightValue = 0;
    public TextMeshProUGUI errorMessageLabel;
    private EnvironmentSelect environmentSelect;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        FindEnvSelect();
    }

    private void FindEnvSelect()
    {
        EnvironmentSelect environmentSelectFindResult = FindFirstObjectByType<EnvironmentSelect>();
        if (environmentSelectFindResult != null)
        {
            environmentSelect = environmentSelectFindResult;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateEnvironment()
    {
        errorMessageLabel.text = "";
        FindEnvSelect();
        if (environmentSelect != null)
        {
            environmentSelect.CreateEnvironment(nameValue, lengthValue, heightValue, (string response, string error) => {
                if (error == "HTTP/1.1 401 Unauthorized" || error == "Not logged in")
                {
                    Debug.LogWarning("Login Session Illegal/Expired");
                    SceneManager.LoadScene("LoginScene");
                }
                if (error == null) 
                {
                    Debug.Log(response);
                    environmentSelect.Refresh();
                } else
                {
                    Debug.Log(error);
                    errorMessageLabel.text = error;
                }
            });
        }
    }
    public void SetNameValue(string value)
    {
        nameValue = value;
    }

    public void SetLengthValue(string value)
    {
        if (int.TryParse(value, out int result))
        {
            lengthValue = result;
        }
    }

    public void SetHeightValue(string value)
    {
        if (int.TryParse(value, out int result))
        {
            heightValue = result;
        }
    }
}
