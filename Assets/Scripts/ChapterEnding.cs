using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class ChapterEnding : MonoBehaviour
{
    public static ChapterEnding Instance { get; private set; }

    [Header("Trigger")]
    public int minigamesNeeded = 3;

    [Header("Fragment Reveal")]
    public GameObject fragmentMesh;        // mesh foto (child kamera, di depan muka)
    public GameObject darkBackdrop;        // Panel UI gelap semi-transparan
    public GameObject fragmentTitleText;   // tulisan "PHOTO FRAGMENT FOUND"
    public float fragmentSpinSpeed = 30f;  // muter pelan biar hidup
    public float fragmentShowDuration = 3.5f;

    [Header("Fade & Closing")]
    public Image fadeImage;
    public TMP_Text closingText;
    public float fadeDuration = 2f;

    [Header("Dialog")]
    [TextArea] public string fragmentLine = "Paper...? Looks like a torn photo... why does my chest feel tight? It's just paper.";
    [TextArea] public string closingLine = "It doesn't end here. There are still other memories waiting to be discovered...";

    private readonly HashSet<ToySpot> done = new HashSet<ToySpot>();
    private bool endingStarted = false;
    private bool spinning = false;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
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

    void OnEnable()  { ToyMinigameManager.OnMinigameCompleted += OnMinigameDone; }
    void OnDisable() { ToyMinigameManager.OnMinigameCompleted -= OnMinigameDone; }

    void OnMinigameDone(ToyItem item, ToySpot spot)
    {
        done.Add(spot);
        if (done.Count >= minigamesNeeded && !endingStarted)
        {
            endingStarted = true;
            StartCoroutine(EndingRoutine());
        }
    }

    void Update()
    {
        if (spinning && fragmentMesh != null)
            fragmentMesh.transform.Rotate(Vector3.up, fragmentSpinSpeed * Time.deltaTime, Space.Self);
    }

    IEnumerator EndingRoutine()
    {
        var pm = FindObjectOfType<PlayerMovementCustom>();
        if (pm != null) pm.enabled = false;

        // 1. Backdrop gelap + mesh fragment + tulisan
        if (darkBackdrop != null) darkBackdrop.SetActive(true);
        if (fragmentMesh != null) fragmentMesh.SetActive(true);
        if (fragmentTitleText != null) fragmentTitleText.SetActive(true);
        spinning = true;

        GameDialog.Show(fragmentLine, fragmentShowDuration);
        yield return new WaitForSeconds(fragmentShowDuration + 1f);

        // 2. Fade to black
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

        // 3. Closing
        if (closingText != null)
        {
            closingText.text = closingLine;
            closingText.gameObject.SetActive(true);
        }
    }
}