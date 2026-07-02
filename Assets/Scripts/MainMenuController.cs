using System.Collections;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject panelLoading1;
    public GameObject panelLoading2;
    public GameObject panelMainMenu;

    [Header("Settings")]
    public float initialBlackScreenTime = 2f; // Waktu layar hitam pertama (detik)

    private bool canTapToStart = false;

    void Start()
    {
        // Pastikan kondisi awal saat game dibuka
        panelLoading1.SetActive(true);
        panelLoading2.SetActive(true);
        panelMainMenu.SetActive(true);

        // Mulai proses loading
        StartCoroutine(LoadingSequence());
    }

    IEnumerator LoadingSequence()
    {
        // 1. Tunggu beberapa detik di layar hitam (Loading 1)
        yield return new WaitForSeconds(initialBlackScreenTime);

        // 2. Matikan layar hitam pertama, munculkan layar "Tap to Start" (Loading 2)
        panelLoading1.SetActive(false);
        canTapToStart = true;
    }

    void Update()
    {
        // 3. Jika sudah di layar "Tap to Start" dan player klik/tap layar
        if (canTapToStart && Input.GetMouseButtonDown(0))
        {
            // Matikan layar loading 2, otomatis Main Menu akan terlihat
            panelLoading2.SetActive(false);
            canTapToStart = false; // Matikan input ini agar tidak berulang
        }
    }
}