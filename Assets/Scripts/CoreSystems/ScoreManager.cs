using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("Score Settings")]
    public float scoreMultiplier = 1.0f; // Multiplier for score calculation
    private float currentScore = 0;

    [Header("Die Canvas")]
    public TextMeshProUGUI dieCanvasScoreText;

    [Header("Menu Canvas")]
    public TextMeshProUGUI menuCanvasScoreText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    // Method to add score
    public void AddScore(float score)
    {
        currentScore += score * scoreMultiplier;
        currentScore += score;
        UpdateDieCanvasScore(); // Ensure die screen score is always up to date
    }

    // Method to reset the score
    public void ResetScore()
    {
        currentScore = 0;
        UpdateDieCanvasScore(); // Reset die screen score as well
    }

    // Method to get the current score
    public float GetScore()
    {
        return currentScore;
    }

    // Method to update the die canvas score directly
    public void UpdateDieCanvasScore()
    {
        if (dieCanvasScoreText != null)
        {
            dieCanvasScoreText.text = "SCORE: " + currentScore;
        }
    }
}
