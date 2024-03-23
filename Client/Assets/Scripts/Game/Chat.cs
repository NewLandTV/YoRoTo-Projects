using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class Chat : MonoBehaviour
{
    public static Chat instance;

    [SerializeField]
    private GameObject chatBackground;
    [SerializeField]
    private InputField messageInputField;
    [SerializeField]
    private Text chatMessageText;

    private StringBuilder chatMessageValue = new StringBuilder();

    private void Awake()
    {
        instance = this;
    }

    private IEnumerator Start()
    {
        while (true)
        {
            ToggleChatActive();

            yield return null;
        }
    }

    private void ToggleChatActive()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            chatBackground.SetActive(!chatBackground.activeSelf);
        }
    }

    public void OnReceivedLobbyChatMessage(string message)
    {
        chatMessageValue.AppendLine(message);

        chatMessageText.text = chatMessageValue.ToString();
    }

    public void OnSendButtonClick()
    {
        ClientSend.ChatSend(messageInputField.text);

        messageInputField.text = string.Empty;
    }
}
