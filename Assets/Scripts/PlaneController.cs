using UnityEngine;

public class PlaneController : BaseVehicle
{
    [Header("Movement")]
    public float moveSpeed = 20f;
    public float turnSpeed = 60f;
    public float ascendSpeed = 8f;
    public float minHeight = 2f;

    [Header("Idle Glide (pas gak digas)")]
    public float idleForwardSpeed = 6f;
    public float idleDescendSpeed = 3f;

    [Header("Rotation")]
    public float pitchAngle = 30f;      // sudut hidung naik/turun
    public float rollAngle = 25f;       // sudut miring kiri/kanan
    public float rotationSmooth = 5f;   // seberapa smooth rotasinya

    private float currentPitch = 0f;
    private float currentRoll = 0f;
    private bool hasStartedMoving = false; // jadi true setelah pertama kali pencet W

    [Header("Propeller")]
    public Transform propeller;
    public Vector3 propellerSpinAxis = Vector3.forward; // sumbu putar lokal, biasanya Z atau X tergantung model
    public float propellerSpeed = 1080f; // derajat per detik, atur sesuai selera

    protected override void Start()
    {
        base.Start();
        rb.useGravity = false;
        rb.freezeRotation = true; // kita handle rotasi manual
    }

    public override void SetOccupied(bool occupied)
    {
        base.SetOccupied(occupied);
        if (occupied)
        {
            hasStartedMoving = false; // reset tiap kali naik ulang
        }
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
            // Cegah drift/ngambang sendiri pas lagi diparkir/gak dinaikin
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // Kembaliin rotasi ke normal kalau ditinggal
            Quaternion neutralRot = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
            rb.MoveRotation(Quaternion.Lerp(rb.rotation, neutralRot, Time.fixedDeltaTime * rotationSmooth));
            return;
        }

        if (controlsFrozen) return;

        float move = Input.GetAxis("Vertical");
        float turn = Input.GetAxis("Horizontal");
        float ascend = 0f;

        if (Input.GetKey(KeyCode.Space)) ascend = 1f;
        else if (Input.GetKey(KeyCode.LeftShift)) ascend = -1f;

        bool isGassing = Mathf.Abs(move) > 0.1f;

        if (isGassing) hasStartedMoving = true; // <- TAMBAHIN LAGI baris ini

        if (isGassing && propeller != null)
        {
            propeller.Rotate(propellerSpinAxis * propellerSpeed * Time.fixedDeltaTime);
        }

        float forwardAmount;
        float verticalAmount;

        if (isGassing)
        {
            forwardAmount = move * moveSpeed;
            verticalAmount = ascend * ascendSpeed;
        }
        else if (hasStartedMoving)
        {
            // Udah pernah gas sebelumnya, sekarang lepas W -> glide
            forwardAmount = idleForwardSpeed;
            verticalAmount = (ascend != 0f) ? ascend * ascendSpeed : -idleDescendSpeed;
        }
        else
        {
            // Belum pernah gas sama sekali sejak naik -> diem total (kecuali naik/turun manual)
            forwardAmount = 0f;
            verticalAmount = ascend * ascendSpeed;
        }

        // Maju + naik/turun
        Vector3 moveDir = transform.forward * forwardAmount * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + moveDir + Vector3.up * verticalAmount * Time.fixedDeltaTime);

        // Pitch — hidung naik saat Space, turun saat Shift
        float targetPitch = -ascend * pitchAngle;
        currentPitch = Mathf.Lerp(currentPitch, targetPitch, Time.fixedDeltaTime * rotationSmooth);

        // Roll — miring saat belok
        float targetRoll = -turn * rollAngle;
        currentRoll = Mathf.Lerp(currentRoll, targetRoll, Time.fixedDeltaTime * rotationSmooth);

        // Yaw — belok kiri/kanan
        float yaw = transform.eulerAngles.y;
        if (Mathf.Abs(forwardAmount) > 0.1f)
            yaw += turn * turnSpeed * Time.fixedDeltaTime;

        // Gabungin semua rotasi
        Quaternion targetRotation = Quaternion.Euler(currentPitch, yaw, currentRoll);
        rb.MoveRotation(Quaternion.Lerp(rb.rotation, targetRotation, Time.fixedDeltaTime * rotationSmooth));
    }
}