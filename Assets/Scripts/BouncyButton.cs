using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems; // Wajib untuk mendeteksi interaksi mouse/pointer

public class BouncyButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    private Vector3 originalScale;

    [Header("Animation Settings")]
    [Tooltip("Ukuran tombol saat disorot mouse (1.05 berarti membesar 5%)")]
    public float hoverScaleMultiplier = 1.05f;

    [Tooltip("Ukuran tombol saat diklik (0.95 berarti mengecil 5%)")]
    public float pressScaleMultiplier = 0.95f;

    [Tooltip("Kecepatan transisi animasi (dalam detik)")]
    public float animationDuration = 0.1f;

    private Coroutine scaleCoroutine;

    void Start()
    {
        // Menyimpan ukuran asli tombol saat game pertama kali dimulai
        originalScale = transform.localScale;
    }

    // 1. Saat Mouse Masuk / Hover
    public void OnPointerEnter(PointerEventData eventData)
    {
        StartScaleTransition(originalScale * hoverScaleMultiplier);
    }

    // 2. Saat Mouse Keluar
    public void OnPointerExit(PointerEventData eventData)
    {
        StartScaleTransition(originalScale);
    }

    // 3. Saat Tombol Ditekan (Klik Kiri)
    public void OnPointerDown(PointerEventData eventData)
    {
        StartScaleTransition(originalScale * pressScaleMultiplier);
    }

    // 4. Saat Klik Dilepas
    public void OnPointerUp(PointerEventData eventData)
    {
        // Kembali ke ukuran hover jika mouse masih di atas tombol, atau kembali ke normal jika sudah di luar
        StartScaleTransition(originalScale * hoverScaleMultiplier);
    }

    // Fungsi pembantu untuk menghentikan animasi lama dan memulai animasi baru
    private void StartScaleTransition(Vector3 targetScale)
    {
        if (scaleCoroutine != null)
        {
            StopCoroutine(scaleCoroutine);
        }

        // MODIFIKASI 1: Cek apakah GameObject aktif sebelum menjalankan Coroutine.
        // Ini menghindari error ketika tombol tiba-tiba disembunyikan.
        if (gameObject.activeInHierarchy)
        {
            scaleCoroutine = StartCoroutine(ScaleOverTime(targetScale));
        }
    }

    // Coroutine untuk mengubah ukuran secara halus (Lerp)
    IEnumerator ScaleOverTime(Vector3 targetScale)
    {
        Vector3 startScale = transform.localScale;
        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            // MODIFIKASI 2: Ubah Time.deltaTime menjadi Time.unscaledDeltaTime
            // Ini membuat animasi mengabaikan Time.timeScale = 0 (game saat pause)
            elapsedTime += Time.unscaledDeltaTime;

            transform.localScale = Vector3.Lerp(startScale, targetScale, elapsedTime / animationDuration);
            yield return null;
        }

        transform.localScale = targetScale;
    }

    // Reset ukuran jika tombol tiba-tiba dinonaktifkan agar tidak stuck dalam posisi membesar/mengecil
    void OnDisable()
    {
        transform.localScale = originalScale;
    }
}