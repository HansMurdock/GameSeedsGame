using UnityEngine;
using System;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public int CurrentScore { get; private set; } = 0;

    // Event ini nanti bisa "didengerin" UI atau sistem quest buat update tampilan
    public static event Action<int> OnScoreChanged;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void AddScore(int amount)
    {
        CurrentScore += amount;
        OnScoreChanged?.Invoke(CurrentScore);
        Debug.Log("Score sekarang: " + CurrentScore);
    }
}