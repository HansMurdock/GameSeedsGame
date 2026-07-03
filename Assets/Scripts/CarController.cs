using UnityEngine;

public class CarController : BaseVehicle
{
    [Header("Movement")]
    public float moveSpeed = 10f;
    public float turnSpeed = 80f;

    [Header("Wheels")]
    public Transform[] wheels;
    public float wheelRadius = 0.3f; // sesuain sama ukuran roda mobil lu
    public Vector3 wheelSpinAxis = Vector3.right; // sumbu putar lokal roda, biasanya X

    protected override void Start()
    {
        base.Start();
    }

    void FixedUpdate()
    {
        if (!isOccupied) return;

        float move = Input.GetAxis("Vertical");
        float turn = Input.GetAxis("Horizontal");

        Vector3 moveDir = transform.right * move * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + moveDir);

        if (Mathf.Abs(move) > 0.1f)
        {
            float rotation = turn * turnSpeed * Time.fixedDeltaTime;
            Quaternion turnRotation = Quaternion.Euler(0f, rotation, 0f);
            rb.MoveRotation(rb.rotation * turnRotation);
        }

        SpinWheels(move);
    }

    private void SpinWheels(float move)
    {
        if (wheels == null || wheels.Length == 0) return;

        // Kecepatan sudut (derajat/detik) berdasarkan kecepatan gerak & keliling roda
        float distancePerSecond = move * moveSpeed;
        float spinSpeed = (distancePerSecond / (2f * Mathf.PI * wheelRadius)) * 360f;

        foreach (Transform wheel in wheels)
        {
            if (wheel != null)
                wheel.Rotate(wheelSpinAxis * spinSpeed * Time.fixedDeltaTime);
        }
    }
}