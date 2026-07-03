using UnityEngine;

public class FootstepSFX : MonoBehaviour
{
    public AudioClip footstepClip;
    public float walkInterval = 0.5f;   // jeda antar langkah pas jalan
    public float runInterval = 0.3f;    // jeda pas lari

    public AudioClip jumpClip;
    public float jumpVolume = 0.5f;
    public float volume = 0.5f;

    private AudioSource audioSource;
    private float timer;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            audioSource.pitch = 1f;
            audioSource.PlayOneShot(jumpClip, jumpVolume);
        }
        bool isMoving = Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0;
        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        if (isMoving)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                audioSource.pitch = Random.Range(0.9f, 1.1f); // biar gak monoton
                audioSource.PlayOneShot(footstepClip, volume);
                timer = isRunning ? runInterval : walkInterval;
            }
        }
        else
        {
            timer = 0f; // langsung bunyi begitu mulai gerak lagi
        }
    }
}