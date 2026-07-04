using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("UI Reference")]
    public TextMeshProUGUI dialogueText;

    [Header("Dialogue Settings")]
    [TextArea(3, 5)]
    public string isiDialog;
    public float durasiTeks = 4f;

    [Header("Trigger Options")]
    [Tooltip("Centang jika dialog ini hanya boleh muncul 1 kali")]
    public bool hancurSetelahMemicu = true;

    private bool sudahPernahMemicu = false;

    private void OnTriggerEnter(Collider other)
    {
        // Cuma karakter player (punya PlayerMovementCustom) yang boleh micu.
        // Kendaraan yang tag-nya "Player" gak akan micu dialog lagi.
        if (other.GetComponent<PlayerMovementCustom>() == null) return;

        if (!sudahPernahMemicu)
        {
            sudahPernahMemicu = true;
            StartCoroutine(TampilkanDialog());
        }
    }

    IEnumerator TampilkanDialog()
    {
        dialogueText.text = isiDialog;
        dialogueText.gameObject.SetActive(true);

        yield return new WaitForSeconds(durasiTeks);

        dialogueText.gameObject.SetActive(false);

        // Hancurkan objek trigger ini dari map agar tidak bisa diinjak lagi
        if (hancurSetelahMemicu)
        {
            Destroy(gameObject);
        }
    }
}