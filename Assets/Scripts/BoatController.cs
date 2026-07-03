using UnityEngine;

public class BoatController : BaseVehicle
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float turnSpeed = 60f;

    [Header("Water Float")]
    public float waterLevel = 0f;
    public float floatSmoothing = 5f; // seberapa cepat perahu nyesuain ke ketinggian air

    protected override void Start()
    {
        base.Start();
        rb.useGravity = false;
    }

    void FixedUpdate()
    {
        // Selalu jaga ketinggian perahu di level air, tapi lewat MovePosition
        // (bukan set rb.position langsung), biar physics engine tetep proses
        // collision dengan benar dan gak "teleport" ngelewatin collider lain.
        float targetY = Mathf.Lerp(rb.position.y, waterLevel, Time.fixedDeltaTime * floatSmoothing);
        Vector3 floatCorrection = new Vector3(0f, targetY - rb.position.y, 0f);

        if (!isOccupied)
        {
            // Cegah drift/gerak sendiri pas lagi diparkir/gak dinaikin
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            if (floatCorrection.sqrMagnitude > 0.0001f)
                rb.MovePosition(rb.position + floatCorrection);

            return;
        }

        if (controlsFrozen)
        {
            if (floatCorrection.sqrMagnitude > 0.0001f)
                rb.MovePosition(rb.position + floatCorrection);
            return;
        }

        float move = Input.GetAxis("Vertical");
        float turn = Input.GetAxis("Horizontal");

        Vector3 moveDir = transform.forward * move * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + moveDir + floatCorrection);

        if (Mathf.Abs(move) > 0.1f)
        {
            float rotation = turn * turnSpeed * Time.fixedDeltaTime;
            Quaternion turnRotation = Quaternion.Euler(0f, rotation, 0f);
            rb.MoveRotation(rb.rotation * turnRotation);
        }
    }
}