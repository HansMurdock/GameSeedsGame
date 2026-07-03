using UnityEngine;
using System.Collections;

public class BaseVehicle : MonoBehaviour
{
    [Header("References")]
    public Transform seatPoint;
    public Transform exitPoint; // taruh di luar collider kendaraan, misal di samping pintu

    [Header("UI")]
    public GameObject enterPrompt;

    [Header("Camera")]
    public Camera vehicleCamera;

    protected Rigidbody rb;
    protected bool isOccupied = false;
    protected bool controlsFrozen = false;

    private VehicleObstacleLink obstacleLink;
    private bool obstacleRevealed = false;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        obstacleLink = GetComponent<VehicleObstacleLink>();
    }

    public virtual void SetOccupied(bool occupied)
    {
        isOccupied = occupied;

        if (!occupied)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            if (vehicleCamera != null) vehicleCamera.gameObject.SetActive(false);
            return;
        }

        if (!obstacleRevealed && obstacleLink != null && obstacleLink.RintanganGroup != null)
        {
            obstacleRevealed = true;
            StartCoroutine(RevealSequence()); // rintangan diaktifin DI DALAM coroutine, bukan di sini lagi
            return;
        }

        if (vehicleCamera != null) vehicleCamera.gameObject.SetActive(true);
    }

    private IEnumerator RevealSequence()
    {
        controlsFrozen = true;

        Camera revealCam = obstacleLink.RevealCamera;
        if (revealCam != null) revealCam.gameObject.SetActive(true);

        yield return new WaitForSeconds(obstacleLink.SpawnDelay);

        obstacleLink.RintanganGroup.SetActive(true);
        SpawnObstacleEffects(obstacleLink.RintanganGroup, obstacleLink.SpawnEffectPrefab);

        yield return new WaitForSeconds(obstacleLink.RevealDuration);

        if (revealCam != null) revealCam.gameObject.SetActive(false);
        if (vehicleCamera != null) vehicleCamera.gameObject.SetActive(true);

        controlsFrozen = false;
    }

    private void SpawnObstacleEffects(GameObject group, GameObject effectPrefab)
    {
        if (effectPrefab == null) return;

        foreach (Transform child in group.transform)
        {
            GameObject fx = Instantiate(effectPrefab, child.position, Quaternion.identity);
            Destroy(fx, 2f); // bersihin object efek setelah 2 detik biar gak numpuk di scene
        }
    }

    public void ShowPrompt(bool show)
    {
        if (enterPrompt != null)
            enterPrompt.SetActive(show);
    }
}