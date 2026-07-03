using System.Collections;
using UnityEngine;

public class GameIntroController : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject panelIntro;
    public GameObject panelHUD; // Tambahan slot untuk HUD
    
    [Header("Timers")]
    public float introDuration = 10f; // Berapa lama diam di layar
    public float fadeDuration = 2f;   // Waktu efek memudar

    private CanvasGroup introCanvasGroup;
    private CanvasGroup hudCanvasGroup;

    void Start()
    {
        // Memastikan kedua panel menyala saat awal game
        panelIntro.SetActive(true);
        panelHUD.SetActive(true);
        
        // Memasang CanvasGroup ke Intro secara otomatis jika belum ada
        introCanvasGroup = panelIntro.GetComponent<CanvasGroup>();
        if (introCanvasGroup == null) 
        {
            introCanvasGroup = panelIntro.AddComponent<CanvasGroup>();
        }

        // Memasang CanvasGroup ke HUD secara otomatis jika belum ada
        hudCanvasGroup = panelHUD.GetComponent<CanvasGroup>();
        if (hudCanvasGroup == null) 
        {
            hudCanvasGroup = panelHUD.AddComponent<CanvasGroup>();
        }
        
        // Mulai menghitung waktu mundur
        StartCoroutine(IntroSequence());
    }

    IEnumerator IntroSequence()
    {
        // 1. Diam selama durasi yang ditentukan (10 detik)
        yield return new WaitForSeconds(introDuration);

        // 2. Proses memudar (Fade Out) untuk kedua panel
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            
            // Menghitung nilai alpha (transparansi) dari 1 ke 0
            float alphaValue = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            
            // Terapkan ke kedua panel
            introCanvasGroup.alpha = alphaValue;
            hudCanvasGroup.alpha = alphaValue;
            
            yield return null; 
        }

        // 3. Matikan GameObject sepenuhnya agar tidak mengganggu performa dan klik
        panelIntro.SetActive(false);
        panelHUD.SetActive(false);
    }
}

