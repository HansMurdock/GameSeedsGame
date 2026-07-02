using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject panelLoading1;
    public GameObject panelLoading2;
    public GameObject panelMainMenu;

    [Header("Transition Settings")]
    public float initialBlackScreenTime = 2f;
    public float fadeDuration = 1f;

    [Header("Scene Settings")]
    public string gameplaySceneName = "NamaSceneGameUtamaKamu";

    private bool canTapToStart = false;
    private CanvasGroup loading2CanvasGroup;

    void Start()
    {
        panelLoading1.SetActive(true);
        panelLoading2.SetActive(true);
        panelMainMenu.SetActive(true);

        loading2CanvasGroup = panelLoading2.GetComponent<CanvasGroup>();
        if (loading2CanvasGroup == null)
        {
            loading2CanvasGroup = panelLoading2.AddComponent<CanvasGroup>();
        }

        StartCoroutine(LoadingSequence());
    }

    IEnumerator LoadingSequence()
    {
        yield return new WaitForSeconds(initialBlackScreenTime);
        panelLoading1.SetActive(false);
        canTapToStart = true;
    }

    void Update()
    {
        if (canTapToStart && Input.GetMouseButtonDown(0))
        {
            canTapToStart = false;
            StartCoroutine(FadeOutLoading2());
        }
    }

    IEnumerator FadeOutLoading2()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            loading2CanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            yield return null;
        }

        panelLoading2.SetActive(false);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(gameplaySceneName);
    }

    public void QuitGame()
    {
        // Memakai "UnityEngine." di depannya agar tidak bentrok dengan library lain
        UnityEngine.Debug.Log("Game ditutup!");
        UnityEngine.Application.Quit();
    }
}