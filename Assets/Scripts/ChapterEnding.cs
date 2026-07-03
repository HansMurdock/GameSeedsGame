using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ChapterEnding : MonoBehaviour
{
    public static ChapterEnding Instance { get; private set; }

    [Header("Trigger")]
    [Tooltip("Total ring yang harus dikumpulkan sebelum ending. Isi -1 untuk auto-hitung semua ring di scene.")]
    public int ringsNeeded = -1;

    [Header("Fragment Reveal")]
    public GameObject fragmentMesh;
    public GameObject darkBackdrop;
    public GameObject fragmentTitleText;
    public float fragmentSpinSpeed = 30f;
    public float fragmentShowDuration = 3.5f;

    [Header("Fade & Closing")]
    public Image fadeImage;
    public TMP_Text closingText;
    public float fadeDuration = 2f;

    [Header("Dialog")]
    [TextArea] public string fragmentLine = "Paper...? Looks like a torn photo... why does my chest feel tight? It's just paper.";
    [TextArea] public string closingLine = "...I have to find the rest.";

    private bool endingStarted = false;
    private bool spinning = false;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        // auto-hitung semua ring di scene (termasuk yang nonaktif) kalau ringsNeeded < 0
        if (ringsNeeded < 0)
        {
            var rings = FindObjectsByType<CollectibleRing>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            ringsNeeded = rings.Length;
            Debug.Log("ChapterEnding: total ring di scene = " + ringsNeeded);
        }

        if (fragmentMesh != null) fragmentMesh.SetActive(false);
        if (darkBackdrop != null) darkBackdrop.SetActive(false);
        if (fragmentTitleText != null) fragmentTitleText.SetActive(false);
        if (fadeImage != null)
        {
            var c = Color.black; c.a = 0f;
            fadeImage.color = c;
            fadeImage.raycastTarget = false;
        }
        if (closingText != null) closingText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (spinning && fragmentMesh != null)
            fragmentMesh.transform.Rotate(Vector3.up, fragmentSpinSpeed * Time.deltaTime, Space.Self);

        if (endingStarted) return;

        if (ScoreManager.Instance != null && ScoreManager.Instance.CurrentScore >= ringsNeeded && ringsNeeded > 0)
        {
            endingStarted = true;
            StartCoroutine(EndingRoutine());
        }
    }

    IEnumerator EndingRoutine()
    {
        var pm = FindObjectOfType<PlayerMovementCustom>();
        if (pm != null) pm.enabled = false;

        if (darkBackdrop != null) darkBackdrop.SetActive(true);
        if (fragmentMesh != null) fragmentMesh.SetActive(true);
        if (fragmentTitleText != null) fragmentTitleText.SetActive(true);
        spinning = true;

        GameDialog.Show(fragmentLine, fragmentShowDuration);
        yield return new WaitForSeconds(fragmentShowDuration + 1f);

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            var c = fadeImage.color;
            c.a = Mathf.Clamp01(t / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }

        spinning = false;
        if (fragmentMesh != null) fragmentMesh.SetActive(false);
        if (darkBackdrop != null) darkBackdrop.SetActive(false);
        if (fragmentTitleText != null) fragmentTitleText.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        if (closingText != null)
        {
            closingText.text = closingLine;
            closingText.gameObject.SetActive(true);
        }
    }
}