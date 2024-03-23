using UnityEngine;

public class Title : MonoBehaviour
{
    public void OnLoginButtonClick()
    {
        // TODO login

        Loading.LoadScene(Loading.Scenes.Game);
    }

    public void OnPlayButtonClick()
    {
        Loading.LoadScene(Loading.Scenes.Game);
    }
}
