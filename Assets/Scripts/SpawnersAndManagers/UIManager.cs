using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI Elements")]
    [SerializeField] public Slider healthBar;
    [SerializeField] public Slider xpBar;
    [SerializeField] public TextMeshProUGUI levelText;
    [SerializeField] public TextMeshProUGUI scoreText;

    // Pop-up texts
    public TextMeshProUGUI xpPopupText;
    public TextMeshProUGUI damagePopupText;
    public TextMeshProUGUI healthPopupText;

    // Initial positions of the pop-up texts
    private Vector3 xpPopupInitialPosition;
    private Vector3 damagePopupInitialPosition;
    private Vector3 healthPopupInitialPosition;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // Check if all pop-up texts are assigned
        if (xpPopupText != null && damagePopupText != null && healthPopupText != null)
        {
            xpPopupInitialPosition = xpPopupText.transform.localPosition;
            damagePopupInitialPosition = damagePopupText.transform.localPosition;
            healthPopupInitialPosition = healthPopupText.transform.localPosition;
        }
        else
        {
            Debug.LogWarning("One or more pop-up texts are not assigned.");
        }
    }

    public void ShowXPGainPopup(float xpAmount)
    {
        StartCoroutine(ShowPopupText(xpPopupText, "+" + xpAmount + " XP", Color.green, xpPopupInitialPosition));
    }

    public void ShowDamagePopup(float damageAmount)
    {
        StartCoroutine(ShowPopupText(damagePopupText, "-" + damageAmount + " HP", Color.red, damagePopupInitialPosition));
    }

    public void ShowHealthRestorePopup(float restoreAmount)
    {
        StartCoroutine(ShowPopupText(healthPopupText, "+" + restoreAmount + " HP", Color.cyan, healthPopupInitialPosition));
    }

    // Display and fade out the pop-up text
    private IEnumerator ShowPopupText(TextMeshProUGUI popupText, string message, Color textColor, Vector3 initialPosition)
    {
        popupText.transform.localPosition = initialPosition;
        popupText.text = message;
        popupText.color = textColor;
        popupText.alpha = 1f;
        popupText.gameObject.SetActive(true);

        Vector3 targetPosition = initialPosition + new Vector3(0, 50, 0);
        float duration = 1.5f;
        float elapsedTime = 0f;

        // Animate position and fade out
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            popupText.transform.localPosition = Vector3.Lerp(initialPosition, targetPosition, elapsedTime / duration);
            popupText.alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
            yield return null;
        }

        // Hide and reset text position
        popupText.gameObject.SetActive(false);
        popupText.transform.localPosition = initialPosition;
    }

    public void UpdateHealthUI(float currentHealth, float maxHealth)
    {
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
    }

    public void UpdateXPUI(float currentXP, float xpToNextLevel)
    {
        if (xpBar != null)
        {
            xpBar.maxValue = xpToNextLevel;
            xpBar.value = currentXP;
        }
    }

    public void UpdateLevelUI(int playerLevel)
    {
        if (levelText != null)
        {
            levelText.text = "LEVEL: " + playerLevel;
        }
    }

    public void UpdateScoreUI(float playerScore)
    {
        if (scoreText != null)
        {
            scoreText.text = "SCORE: " + playerScore;
            Debug.Log("Score Updated: " + playerScore);
        }
        

    }

    public void ShowLastScore(float playerScore)
    {
        scoreText.text = "SCORE: " + playerScore;
    }
}
