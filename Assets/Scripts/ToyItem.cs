using UnityEngine;

/// <summary>
/// Data satu jenis mainan. Bikin asset-nya lewat:
/// Klik kanan di Project window -> Create -> ToyStore -> Toy Item
///
/// Satu ToyItem = satu jenis mainan yang bisa diambil dari rak (misal "Truk Merah").
/// - heldPrefab   : versi kecil yang muncul di tangan player pas lagi dibawa
/// - vehiclePrefab: prefab kendaraan asli lo (yang udah punya CarController /
///                  BoatController / PlaneController + BaseVehicle), dipakai
///                  saat minigame di-spawn di ToySpot.
/// </summary>
[CreateAssetMenu(fileName = "NewToyItem", menuName = "ToyStore/Toy Item")]
public class ToyItem : ScriptableObject
{
    [Header("Info Umum")]
    public string toyName = "Truk Mainan";

    [Header("Visual saat dipegang Player")]
    [Tooltip("Prefab kecil yang muncul di tangan/holdPoint player pas lagi bawa mainan ini")]
    public GameObject heldPrefab;

    [Header("Untuk Minigame")]
    [Tooltip("Prefab kendaraan yang sudah ada BaseVehicle + Controller-nya (CarController/BoatController/PlaneController)")]
    public GameObject vehiclePrefab;

    [Tooltip("Jumlah ring yang harus dikumpulkan supaya minigame ini selesai")]
    public int ringTargetCount = 10;
}
