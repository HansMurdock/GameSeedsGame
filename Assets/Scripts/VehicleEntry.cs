using UnityEngine;

public class VehicleEntry : MonoBehaviour
{
    [Header("Settings")]
    public float enterRadius = 3f;
    public KeyCode enterKey = KeyCode.E;

    [Header("References")]
    public Camera playerCamera;

    private BaseVehicle nearbyVehicle;
    private BaseVehicle currentVehicle;
    private bool isInVehicle = false;
    private CharacterController charController;
    private PlayerMovement playerMovement;

    void Start()
    {
        charController = GetComponent<CharacterController>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        // Cari kendaraan terdekat
        Collider[] hits = Physics.OverlapSphere(transform.position, enterRadius);
        nearbyVehicle = null;
        foreach (var hit in hits)
        {
            BaseVehicle v = hit.GetComponent<BaseVehicle>();
            if (v != null) { nearbyVehicle = v; break; }
        }

        // Tampilkan/sembunyikan semua prompt
        foreach (var v in FindObjectsOfType<BaseVehicle>())
            v.ShowPrompt(v == nearbyVehicle && !isInVehicle);

        // Naik/turun kendaraan
        if (Input.GetKeyDown(enterKey))
        {
            if (!isInVehicle && nearbyVehicle != null)
                Enter(nearbyVehicle);
            else if (isInVehicle)
                Exit();
        }

        // Ikutin posisi seat
        if (isInVehicle && currentVehicle != null)
        {
            transform.position = currentVehicle.seatPoint.position;
            transform.rotation = currentVehicle.transform.rotation;
        }
    }

    void Enter(BaseVehicle vehicle)
    {
        isInVehicle = true;
        currentVehicle = vehicle;
        vehicle.SetOccupied(true);

        playerMovement.enabled = false;
        charController.enabled = false;
        GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
        playerCamera.gameObject.SetActive(false);
        if (vehicle.vehicleCamera != null)
            vehicle.vehicleCamera.gameObject.SetActive(true);
    }

    void Exit()
    {
        isInVehicle = false;
        currentVehicle.SetOccupied(false);
        if (currentVehicle.vehicleCamera != null)
            currentVehicle.vehicleCamera.gameObject.SetActive(false);
        currentVehicle = null;

        playerMovement.enabled = true;
        charController.enabled = true;
        GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
        playerCamera.gameObject.SetActive(true);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, enterRadius);
    }
}