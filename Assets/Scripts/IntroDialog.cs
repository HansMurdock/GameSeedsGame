using UnityEngine;

public class IntroDialog : MonoBehaviour
{
    [TextArea] public string line = "The lights are still on... but there's no one here.";
    public float delay = 1.5f;
    public float duration = 4f;

    void Start() { Invoke(nameof(ShowLine), delay); }
    void ShowLine() { GameDialog.Show(line, duration); }
}