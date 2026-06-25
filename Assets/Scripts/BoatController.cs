using UnityEngine;

public class BoatController : BaseVehicle
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float turnSpeed = 60f;
    public float waterLevel = 0f;

    protected override void Start()
    {
        base.Start();
        rb.useGravity = false;
    }

    void FixedUpdate()
    {
        Vector3 pos = rb.position;
        pos.y = waterLevel;
        rb.position = pos;

        if (!isOccupied) return;

        float move = Input.GetAxis("Vertical");
        float turn = Input.GetAxis("Horizontal");

        Vector3 moveDir = transform.forward * move * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + moveDir);

        if (Mathf.Abs(move) > 0.1f)
        {
            float rotation = turn * turnSpeed * Time.fixedDeltaTime;
            Quaternion turnRotation = Quaternion.Euler(0f, rotation, 0f);
            rb.MoveRotation(rb.rotation * turnRotation);
        }
    }
}