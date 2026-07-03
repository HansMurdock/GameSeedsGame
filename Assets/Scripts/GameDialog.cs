using UnityEngine;
using TMPro;
using System.Collections;

public class GameDialog : MonoBehaviour
{
    public static GameDialog Instance { get; private set; }

    public GameObject dialogUI;
    public TMP_Text dialogText;

    private Coroutine current;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start() { if (dialogUI != null) dialogUI.SetActive(false); }

    public static void Show(string line, float duration = 4f)
    {
        if (Instance == null) return;
        if (Instance.current != null) Instance.StopCoroutine(Instance.current);
        Instance.current = Instance.StartCoroutine(Instance.Run(line, duration));
    }

    IEnumerator Run(string line, float duration)
    {
        dialogText.text = line;
        dialogUI.SetActive(true);
        yield return new WaitForSeconds(duration);
        dialogUI.SetActive(false);
        current = null;
    }
}