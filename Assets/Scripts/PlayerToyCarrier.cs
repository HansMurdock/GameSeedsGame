using UnityEngine;

/// <summary>
/// Tempel script ini di GameObject Player yang sama, satu tempat dengan
/// PlayerMovementCustom dan VehicleEntry.
///
/// Setup di Inspector:
/// 1. Buat empty child di model player, taruh di sekitar tangan/dada
///    (misal nama "ToyHoldPoint"), lalu pasang ke field "holdPoint".
/// 2. (Opsional) Field "animator" akan otomatis kepake dari PlayerMovementCustom
///    kalau ada, buat trigger animasi "PickUp" yang emang udah ada di script itu.
/// </summary>
public class PlayerToyCarrier : MonoBehaviour
{
    [Header("Referensi")]
    [Tooltip("Titik tempat model mainan kecil muncul saat dibawa (child transform di tangan/dada)")]
    public Transform holdPoint;

    private ToyItem _carriedItem;
    private GameObject _heldVisualInstance;
    private PlayerMovementCustom _playerMovement;

    public bool IsCarrying => _carriedItem != null;
    public ToyItem CarriedItem => _carriedItem;

    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovementCustom>();
    }

    /// <summary>
    /// Dipanggil oleh ToyPickupSlot saat player ambil mainan dari rak.
    /// </summary>
    public void PickUp(ToyItem item)
    {
        if (IsCarrying || item == null) return;

        _carriedItem = item;

        if (item.heldPrefab != null && holdPoint != null)
        {
            _heldVisualInstance = Instantiate(item.heldPrefab, holdPoint.position, holdPoint.rotation, holdPoint);
        }

        // Manfaatin trigger animasi "PickUp" yang udah ada di PlayerMovementCustom
        if (_playerMovement != null && _playerMovement.animator != null)
        {
            _playerMovement.animator.SetTrigger("PickUp");
        }
    }

    /// <summary>
    /// Dipanggil oleh ToyMinigameManager saat mainan ditaruh di ToySpot.
    /// Mengembalikan ToyItem yang lagi dibawa, lalu bersihin state carrier.
    /// </summary>
    public ToyItem PlaceCurrentToy()
    {
        ToyItem item = _carriedItem;

        if (_heldVisualInstance != null)
        {
            Destroy(_heldVisualInstance);
            _heldVisualInstance = null;
        }

        _carriedItem = null;
        return item;
    }
}
