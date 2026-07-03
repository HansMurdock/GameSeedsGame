using UnityEngine;
using TMPro;

/// <summary>
/// Tempel di lokasi tempat player nanti naruh mainan (misal meja/jendela kamar).
/// Polanya sama seperti ToyPickupSlot & DoorInteract: trigger collider + prompt + tombol E.
///
/// Setup di Inspector:
/// 1. Pasang "vehicleSpawnPoint" - Transform kosong tempat kendaraan bakal muncul
///    (letakkan di area terbuka yang cukup luas buat mobil/kapal/pesawat gerak).
/// 2. Pasang "ringsContainer" - GameObject kosong yang berisi semua CollectibleRing
///    (children) yang udah ditata manual di area minigame. Container ini akan
///    di non-aktifkan by default, dan diaktifkan otomatis pas minigame mulai.
/// 3. Pasang UI prompt seperti biasa (promptUI + promptText).
/// </summary>
public class ToySpot : MonoBehaviour
{
    [Header("Spawn Kendaraan")]
    public Transform vehicleSpawnPoint;

    [Header("Rintangan (referensi object SCENE, bukan prefab)")]
    [Tooltip("Group rintangan untuk minigame di spot ini — disuntik ke kendaraan saat spawn")]
    public GameObject rintanganGroup;
    [Tooltip("Kamera reveal rintangan untuk spot ini (opsional)")]
    public Camera revealCamera;

    [Header("Area Ring Minigame")]
    [Tooltip("Parent object yang isinya semua CollectibleRing di area ini, non-aktif secara default")]
    public GameObject ringsContainer;

    [Header("UI")]
    public GameObject promptUI;
    public TMP_Text promptText;

    [Header("Settings")]
    public KeyCode placeKey = KeyCode.E;

    private bool _playerNearby = false;
    private PlayerToyCarrier _nearbyCarrier;

    private void Start()
    {
        if (promptUI != null) promptUI.SetActive(false);
        if (ringsContainer != null) ringsContainer.SetActive(false);
    }

    private void Update()
    {
        if (!_playerNearby || _nearbyCarrier == null) return;

        // Kalau minigame lagi jalan, jangan tampilkan prompt apa pun
        // (teks "Ambil mainan dulu dari rak" jadi menyesatkan saat main)
        if (ToyMinigameManager.Instance != null && ToyMinigameManager.Instance.IsMinigameActive)
        {
            if (promptUI != null) promptUI.SetActive(false);
            return;
        }
        if (promptUI != null) promptUI.SetActive(true);

        bool canPlace = _nearbyCarrier.IsCarrying;
        if (promptText != null)
        {
            promptText.text = canPlace
                ? $"[E] Taruh {_nearbyCarrier.CarriedItem.toyName}"
                : "Ambil mainan dulu dari rak";
        }

        if (canPlace && Input.GetKeyDown(placeKey))
        {
            ToyMinigameManager.Instance.StartMinigame(_nearbyCarrier, this);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        _nearbyCarrier = other.GetComponent<PlayerToyCarrier>();
        if (_nearbyCarrier == null) return;

        _playerNearby = true;
        if (promptUI != null) promptUI.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        _playerNearby = false;
        _nearbyCarrier = null;
        if (promptUI != null) promptUI.SetActive(false);
    }
}
