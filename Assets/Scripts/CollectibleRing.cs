using UnityEngine;

public class CollectibleRing : MonoBehaviour
{
    [Header("Scoring")]
    [SerializeField] private int scoreValue = 1;

    [Header("Rotation")]
    [SerializeField] private Vector3 rotationSpeed = new Vector3(0f, 90f, 0f);

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

        gameObject.SetActive(false);
    }
}