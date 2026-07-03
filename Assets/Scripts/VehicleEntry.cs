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
    private PlayerMovementCustom playerMovement; // <- diganti dari PlayerMovement ke PlayerMovementCustom

    void Start()
    {
        charController = GetComponent<CharacterController>();
        playerMovement = GetComponent<PlayerMovementCustom>();
    }

    void Update()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, enterRadius);
        nearbyVehicle = null;
        foreach (var hit in hits)
        {
            BaseVehicle v = hit.GetComponent<BaseVehicle>();
            if (v != null) { nearbyVehicle = v; break; }
        }

        foreach (var v in FindObjectsOfType<BaseVehicle>())
            v.ShowPrompt(v == nearbyVehicle && !isInVehicle);

        if (Input.GetKeyDown(enterKey))
        {
            if (!isInVehicle && nearbyVehicle != null)
                Enter(nearbyVehicle);
            else if (isInVehicle)
                Exit();
        }

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

        playerMovement.enabled = false; // sekarang beneran matiin script yang aktif
        charController.enabled = false;
        GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
        playerCamera.gameObject.SetActive(false);

        vehicle.SetOccupied(true);
    }

    void Exit()
    {
        isInVehicle = false;
        currentVehicle.SetOccupied(false);
        if (currentVehicle.vehicleCamera != null)
            currentVehicle.vehicleCamera.gameObject.SetActive(false);

        Transform exit = currentVehicle.exitPoint != null ? currentVehicle.exitPoint : currentVehicle.seatPoint;
        transform.position = exit.position;
        transform.rotation = Quaternion.Euler(0f, exit.eulerAngles.y, 0f);

        currentVehicle = null;

        playerMovement.ResetVerticalVelocity(); // buang tumpukan velocity negatif sebelum re-enable
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