using UnityEngine;
using UnityEngine.UI;

public class Account : MonoBehaviour
{
    [SerializeField]
    private GameObject accountGroup;
    [SerializeField]
    private GameObject loadingGroup;

    [SerializeField]
    private Text titleText;
    [SerializeField]
    private InputField idInputField;
    [SerializeField]
    private InputField passwordInputField;

    private bool isLogin;

    public void OpenAccountGroup(bool isLogin, string titleText)
    {
        this.titleText.text = titleText;
        this.isLogin = isLogin;

        accountGroup.SetActive(true);
    }

    public void OnConfirmButtonClick()
    {
        if (isLogin)
        {
            // TODO ClientSend.TryLogin(string id, string password);
        }
        else
        {
            // TODO ClientSend.CreateYoRoToAccount(string id, string password);
        }

        idInputField.interactable = false;
        passwordInputField.interactable = false;

        loadingGroup.SetActive(true);
    }

    public void OnCancelButtonClick()
    {
        accountGroup.SetActive(false);

        idInputField.text = string.Empty;
        passwordInputField.text = string.Empty;
    }

    public void OnReceivedResult()
    {
        loadingGroup.SetActive(false);

        idInputField.interactable = true;
        passwordInputField.interactable = true;
    }
}
