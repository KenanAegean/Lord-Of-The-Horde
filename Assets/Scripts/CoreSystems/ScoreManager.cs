using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("Die Canvas")]
    public TextMeshProUGUI dieCanvasScoreText;

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

    public void UpdateDieCanvasScore(float score)
    {
        if (dieCanvasScoreText != null)
        {
            dieCanvasScoreText.text = "SCORE: " + score;
        }
    }
}
