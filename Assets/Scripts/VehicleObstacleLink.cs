using UnityEngine;

public class VehicleObstacleLink : MonoBehaviour
{
    [SerializeField] private GameObject rintanganGroup;
    [SerializeField] private Camera revealCamera;
    [SerializeField] private float spawnDelay = 1f;
    [SerializeField] private float revealDuration = 3.5f;
    [SerializeField] private GameObject spawnEffectPrefab;

    public GameObject RintanganGroup => rintanganGroup;
    public Camera RevealCamera => revealCamera;
    public float SpawnDelay => spawnDelay;
    public float RevealDuration => revealDuration;
    public GameObject SpawnEffectPrefab => spawnEffectPrefab;
}