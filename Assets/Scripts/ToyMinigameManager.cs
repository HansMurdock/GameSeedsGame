using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Otak minigame ring. Taruh SATU object kosong di scene (misal "ToyMinigameManager")
/// lalu tempel script ini. Tidak perlu isi referensi apa-apa di Inspector —
/// semua datang dari ToySpot & ToyItem saat runtime.
///
/// Alur:
/// 1. ToySpot memanggil StartMinigame() saat player menaruh mainan (E).
/// 2. Manager spawn kendaraan dari ToyItem.vehiclePrefab di vehicleSpawnPoint,
///    menyuntik referensi rintangan milik ToySpot, dan mengaktifkan ringsContainer.
/// 3. Player naik kendaraan (VehicleEntry yang sudah ada) dan mengumpulkan ring.
/// 4. Saat ring mencapai ToyItem.ringTargetCount:
///    - player dipaksa keluar dari kendaraan (balik ke exitPoint),
///    - kendaraan teleport balik ke vehicleSpawnPoint (tetap tampil sebagai display),
///    - RintanganGroup disembunyikan dengan jeda,
///    - ringsContainer dimatikan,
///    - event OnMinigameCompleted ditembakkan (buat UI Quest dsb).
/// </summary>
public class ToyMinigameManager : MonoBehaviour
{
    public static ToyMinigameManager Instance { get; private set; }

    [Header("Settings")]
    [Tooltip("Jeda (detik) sebelum RintanganGroup disembunyikan setelah minigame selesai")]
    public float obstacleHideDelay = 1f;

    /// <summary>Ditembakkan sekali tiap minigame selesai. UI Quest tinggal subscribe ke sini.</summary>
    public static event Action<ToyItem, ToySpot> OnMinigameCompleted;

    public bool IsMinigameActive { get; private set; }
    public int RingsCollected => _ringsCollected;
    public int RingTarget => _activeItem != null ? _activeItem.ringTargetCount : 0;

    private ToySpot _activeSpot;
    private ToyItem _activeItem;
    private BaseVehicle _activeVehicle;
    private int _ringsCollected;

    // kendaraan display yang pernah di-spawn per spot, biar bisa dihapus saat restart
    private readonly Dictionary<ToySpot, GameObject> _spawnedVehicles = new Dictionary<ToySpot, GameObject>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>Dipanggil oleh ToySpot saat player menaruh mainan.</summary>
    public void StartMinigame(PlayerToyCarrier carrier, ToySpot spot)
    {
        if (IsMinigameActive) return;                       // satu minigame dalam satu waktu
        if (carrier == null || spot == null) return;
        if (!carrier.IsCarrying || carrier.CarriedItem.vehiclePrefab == null) return;

        ToyItem item = carrier.PlaceCurrentToy();

        // Kalau minigame di spot ini pernah jalan, hapus dulu kendaraan display lamanya
        if (_spawnedVehicles.TryGetValue(spot, out GameObject old) && old != null)
            Destroy(old);

        // Spawn kendaraan di titik spawn milik spot
        GameObject vehicleGO = Instantiate(
            item.vehiclePrefab,
            spot.vehicleSpawnPoint.position,
            spot.vehicleSpawnPoint.rotation);
        _spawnedVehicles[spot] = vehicleGO;
        _activeVehicle = vehicleGO.GetComponent<BaseVehicle>();

        // PENTING: prefab tidak bisa menyimpan referensi object scene,
        // jadi rintangan & reveal camera disuntik dari ToySpot di sini.
        VehicleObstacleLink link = vehicleGO.GetComponent<VehicleObstacleLink>();
        if (link != null && spot.rintanganGroup != null)
            link.Inject(spot.rintanganGroup, spot.revealCamera);

        // Aktifkan area ring + nyalakan ulang semua ring (sisa run sebelumnya ikut hidup lagi)
        if (spot.ringsContainer != null)
        {
            spot.ringsContainer.SetActive(true);
            foreach (Transform child in spot.ringsContainer.transform)
                child.gameObject.SetActive(true);
        }

        _activeSpot = spot;
        _activeItem = item;
        _ringsCollected = 0;
        IsMinigameActive = true;

        CollectibleRing.RingCollected += HandleRingCollected;
    }

    private void HandleRingCollected()
    {
        if (!IsMinigameActive) return;

        _ringsCollected++;
        if (_ringsCollected >= _activeItem.ringTargetCount)
            StartCoroutine(EndMinigameRoutine());
    }

    private IEnumerator EndMinigameRoutine()
    {
        IsMinigameActive = false;
        CollectibleRing.RingCollected -= HandleRingCollected;

        // 1. Keluarkan player dari kendaraan (balik ke exitPoint di posisi sekarang)
        VehicleEntry entry = FindObjectOfType<VehicleEntry>();
        if (entry != null) entry.ForceExit();

        yield return null; // kasih 1 frame biar posisi player settle dulu

        // 2. Teleport kendaraan balik ke spawn point — tetap tampil sebagai display
        if (_activeVehicle != null && _activeSpot != null && _activeSpot.vehicleSpawnPoint != null)
        {
            Rigidbody vrb = _activeVehicle.GetComponent<Rigidbody>();
            if (vrb != null)
            {
                vrb.linearVelocity = Vector3.zero;
                vrb.angularVelocity = Vector3.zero;
            }
            _activeVehicle.transform.SetPositionAndRotation(
                _activeSpot.vehicleSpawnPoint.position,
                _activeSpot.vehicleSpawnPoint.rotation);
        }

        // 3. Sembunyikan rintangan dengan jeda
        if (_activeVehicle != null)
            _activeVehicle.HideObstacles(obstacleHideDelay);

        // 4. Matikan area ring
        if (_activeSpot != null && _activeSpot.ringsContainer != null)
            _activeSpot.ringsContainer.SetActive(false);

        // 5. Kabari sistem lain (UI Quest, foto fragment, dst.)
        OnMinigameCompleted?.Invoke(_activeItem, _activeSpot);

        _activeSpot = null;
        _activeItem = null;
        _activeVehicle = null;
    }

    private void OnDestroy()
    {
        CollectibleRing.RingCollected -= HandleRingCollected;
        if (Instance == this) Instance = null;
    }
}
