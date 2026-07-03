using UnityEngine;
using TMPro;

public class DoorInteract : MonoBehaviour
{
    [Header("Pintu & Engsel")]
    public Transform leftDoor;
    public Transform rightDoor;
    public Transform leftHingePoint;
    public Transform rightHingePoint;
    public float openAngle = 90f;
    public float swingSpeed = 120f;   // derajat per detik

    [Header("UI")]
    public GameObject promptUI;

    public TMP_Text promptText;

    [Header("SFX")]
    public AudioClip openClip;
    public AudioClip closeClip;
    public float sfxVolume = 0.5f;
    private AudioSource audioSource;

    private float currentAngle = 0f;
    private bool isOpen = false;
    private bool playerNearby = false;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f;   // 3D: makin deket pintu makin kenceng
        promptUI.SetActive(false);
    }

    void Update()
    {
       if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            isOpen = !isOpen;
            audioSource.PlayOneShot(isOpen ? openClip : closeClip, sfxVolume);
            promptText.text = isOpen ? "[E] Close Door" : "[E] Open Door";
        }

        float target = isOpen ? openAngle : 0f;
        float step = Mathf.MoveTowards(currentAngle, target, swingSpeed * Time.deltaTime) - currentAngle;

        if (Mathf.Abs(step) > 0.0001f)
        {
            leftDoor.RotateAround(leftHingePoint.position,  Vector3.up,  step);
            rightDoor.RotateAround(rightHingePoint.position, Vector3.up, -step);
            currentAngle += step;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("MASUK TRIGGER: " + other.name + " | tag: " + other.tag);
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            promptText.text = isOpen ? "[E] Close Door" : "[E] Open Door";
            promptUI.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            promptUI.SetActive(false);
        }
    }

    
}