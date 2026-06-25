using UnityEngine;

public class CarController : BaseVehicle
{
    [Header("Movement")]
    public float moveSpeed = 10f;
    public float turnSpeed = 80f;

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
    }
}