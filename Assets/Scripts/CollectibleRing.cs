using UnityEngine;

public class CollectibleRing : MonoBehaviour
{
    // Ditembakkan tiap kali ring mana pun keambil. ToyMinigameManager subscribe ke sini.
    public static event System.Action RingCollected;

    [Header("Scoring")]
    [SerializeField] private int scoreValue = 1;

    [Header("Rotation")]
    [SerializeField] private Vector3 rotationSpeed = new Vector3(0f, 90f, 0f);

    [Header("SFX")]
    [SerializeField] private AudioClip collectClip;
    [SerializeField] private float sfxVolume = 0.6f;

    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(scoreValue);
        }

        RingCollected?.Invoke();

        // SFX koin — pakai PlayClipAtPoint biar suaranya tetap bunyi
        // walau ring-nya langsung di-SetActive(false)
        if (collectClip != null)
            AudioSource.PlayClipAtPoint(collectClip, transform.position, sfxVolume);

        gameObject.SetActive(false);
    }
}