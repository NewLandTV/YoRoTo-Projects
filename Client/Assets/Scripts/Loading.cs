using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    [SerializeField]
    private Image progressBar;
    [SerializeField]
    private Text percentText;

    private static Scenes loadScene;

    public enum Scenes
    {
        Title,
        Loading,
        Game
    }

    private void Start()
    {
        StartCoroutine(LoadinSceneProgress());
    }

    public static void LoadScene(Scenes loadScene)
    {
        Loading.loadScene = loadScene;

        SceneManager.LoadScene((int)Scenes.Loading);
    }

    #region GetScene

    public static Scenes GetScene()
    {
        return (Scenes)SceneManager.GetActiveScene().buildIndex;
    }

    #endregion

    private IEnumerator LoadinSceneProgress()
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync((int)loadScene);

        asyncOperation.allowSceneActivation = false;

        float timer = 0f;

        while (!asyncOperation.isDone)
        {
            if (asyncOperation.progress < 0.9f)
            {
                progressBar.fillAmount = asyncOperation.progress;
                percentText.text = string.Format("{0:0}%", asyncOperation.progress * 100f);
            }
            else
            {
                timer += Time.unscaledDeltaTime;

                progressBar.fillAmount = Mathf.Lerp(0.9f, 1f, timer);
                percentText.text = string.Format("{0:0}%", progressBar.fillAmount * 100f);

                if (progressBar.fillAmount >= 1f)
                {
                    asyncOperation.allowSceneActivation = true;

                    yield break;
                }
            }

            yield return null;
        }
    }
}
