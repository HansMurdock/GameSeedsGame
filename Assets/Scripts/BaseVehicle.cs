using UnityEngine;

public class BaseVehicle : MonoBehaviour
{
    [Header("References")]
    public Transform seatPoint;

    [Header("UI")]
    public GameObject enterPrompt;

    [Header("Camera")]
    public Camera vehicleCamera;

    protected Rigidbody rb;
    protected bool isOccupied = false;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public virtual void SetOccupied(bool occupied)
    {
        isOccupied = occupied;
        if (!occupied)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    public void ShowPrompt(bool show)
    {
        if (enterPrompt != null)
            enterPrompt.SetActive(show);
    }
}