using UnityEngine;
using TMPro;

/// <summary>
/// Tempel di setiap slot mainan di rak (satu GameObject per mainan).
/// Polanya sengaja dibikin mirip DoorInteract.cs yang udah ada di project,
/// biar konsisten: trigger collider + prompt UI + tombol E.
///
/// Setup di Inspector:
/// 1. Pasang ToyItem yang sesuai di field "toy".
/// 2. Tambahkan Collider (Is Trigger dicentang) di GameObject ini,
///    ukurannya kira-kira jangkauan interact (mirip radius di VehicleEntry).
/// 3. Pasang model visual mainan di rak sebagai child, masukin ke "displayModel".
/// 4. Buat world-space UI (Canvas + TMP_Text), bisa pake BillboardUI.cs yang
///    udah ada biar ngadep kamera terus, masukin ke "promptUI" & "promptText".
/// </summary>
public class ToyPickupSlot : MonoBehaviour
{
    [Header("Data Mainan")]
    public ToyItem toy;

    [Header("Visual di Rak")]
    public GameObject displayModel;

    [Header("UI")]
    public GameObject promptUI;
    public TMP_Text promptText;

    [Header("Settings")]
    public KeyCode pickupKey = KeyCode.E;

    [Tooltip("Kalau true, mainan muncul lagi di rak setelah beberapa waktu (enak buat testing)")]
    public bool respawnAfterTaken = false;
    public float respawnDelay = 5f;

    private bool _playerNearby = false;
    private bool _taken = false;
    private PlayerToyCarrier _nearbyCarrier;

    private void Start()
    {
        if (promptUI != null) promptUI.SetActive(false);
    }

    private void Update()
    {
        if (_taken || !_playerNearby || _nearbyCarrier == null) return;

        // Update teks prompt sesuai kondisi (mirip DoorInteract)
        bool canTake = !_nearbyCarrier.IsCarrying;
        if (promptText != null)
        {
            promptText.text = canTake
                ? $"[E] Ambil {toy.toyName}"
                : "Tangan kamu penuh, taruh mainan dulu";
        }

        if (canTake && Input.GetKeyDown(pickupKey))
        {
            _nearbyCarrier.PickUp(toy);
            SetTaken(true);

            if (respawnAfterTaken)
                Invoke(nameof(Respawn), respawnDelay);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        _nearbyCarrier = other.GetComponent<PlayerToyCarrier>();
        if (_nearbyCarrier == null) return;

        _playerNearby = true;
        if (!_taken && promptUI != null) promptUI.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        _playerNearby = false;
        _nearbyCarrier = null;
        if (promptUI != null) promptUI.SetActive(false);
    }

    private void SetTaken(bool value)
    {
        _taken = value;
        if (displayModel != null) displayModel.SetActive(!value);
        if (promptUI != null) promptUI.SetActive(!value && _playerNearby);
    }

    private void Respawn()
    {
        SetTaken(false);
    }
}
