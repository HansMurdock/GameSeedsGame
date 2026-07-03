using UnityEngine;

/// <summary>
/// Tempel di object kendaraan (yang sama dengan BaseVehicle).
/// Mainkan suara mesin loop saat dinaiki; pitch naik mengikuti kecepatan.
/// </summary>
public class VehicleSFX : MonoBehaviour
{
    [Header("Clips")]
    public AudioClip engineLoop;

    [Header("Settings")]
    public float volume = 0.4f;
    public float idlePitch = 0.8f;    // pitch pas diem
    public float maxPitch = 1.6f;     // pitch pas ngebut
    public float maxSpeed = 15f;      // kecepatan yang dianggap "ngebut penuh"

    private AudioSource src;
    private BaseVehicle vehicle;
    private Rigidbody rb;

    void Start()
    {
        vehicle = GetComponent<BaseVehicle>();
        rb = GetComponent<Rigidbody>();

        src = gameObject.AddComponent<AudioSource>();
        src.clip = engineLoop;
        src.loop = true;
        src.playOnAwake = false;
        src.spatialBlend = 1f;   // 3D: kendaraan lewat = suaranya lewat
        src.volume = volume;
    }

    void Update()
    {
        bool on = vehicle != null && vehicle.IsOccupied;

        if (on && !src.isPlaying) src.Play();
        if (!on && src.isPlaying) src.Stop();

        if (on && rb != null)
        {
            float speed01 = Mathf.Clamp01(rb.linearVelocity.magnitude / maxSpeed);
            src.pitch = Mathf.Lerp(idlePitch, maxPitch, speed01);
        }
    }
}