using UnityEngine;

public class PlaneController : BaseVehicle
{
    [Header("Movement")]
    public float moveSpeed = 20f;
    public float turnSpeed = 60f;
    public float ascendSpeed = 8f;
    public float minHeight = 2f;

    [Header("Rotation")]
    public float pitchAngle = 30f;      // sudut hidung naik/turun
    public float rollAngle = 25f;       // sudut miring kiri/kanan
    public float rotationSmooth = 5f;   // seberapa smooth rotasinya

    private float currentPitch = 0f;
    private float currentRoll = 0f;

    protected override void Start()
    {
        base.Start();
        rb.useGravity = false;
        rb.freezeRotation = true; // kita handle rotasi manual
    }

    void FixedUpdate()
    {
        if (rb.position.y < minHeight)
        {
            Vector3 pos = rb.position;
            pos.y = minHeight;
            rb.position = pos;
        }

        if (!isOccupied)
        {
            // Kembaliin rotasi ke normal kalau ditinggal
            Quaternion neutralRot = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
            rb.MoveRotation(Quaternion.Lerp(rb.rotation, neutralRot, Time.fixedDeltaTime * rotationSmooth));
            return;
        }

        float move = Input.GetAxis("Vertical");
        float turn = Input.GetAxis("Horizontal");
        float ascend = 0f;

        if (Input.GetKey(KeyCode.Space)) ascend = 1f;
        else if (Input.GetKey(KeyCode.LeftShift)) ascend = -1f;

        // Maju + naik/turun
        Vector3 moveDir = transform.forward * move * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + moveDir + Vector3.up * ascend * ascendSpeed * Time.fixedDeltaTime);

        // Pitch — hidung naik saat Space, turun saat Shift
        float targetPitch = -ascend * pitchAngle;
        currentPitch = Mathf.Lerp(currentPitch, targetPitch, Time.fixedDeltaTime * rotationSmooth);

        // Roll — miring saat belok
        float targetRoll = -turn * rollAngle;
        currentRoll = Mathf.Lerp(currentRoll, targetRoll, Time.fixedDeltaTime * rotationSmooth);

        // Yaw — belok kiri/kanan
        float yaw = transform.eulerAngles.y;
        if (Mathf.Abs(move) > 0.1f)
            yaw += turn * turnSpeed * Time.fixedDeltaTime;

        // Gabungin semua rotasi
        Quaternion targetRotation = Quaternion.Euler(currentPitch, yaw, currentRoll);
        rb.MoveRotation(Quaternion.Lerp(rb.rotation, targetRotation, Time.fixedDeltaTime * rotationSmooth));
    }
}