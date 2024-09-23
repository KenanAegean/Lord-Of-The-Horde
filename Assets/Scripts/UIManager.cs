using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider healthBar;
    public Slider xpBar;
    public TextMeshProUGUI levelText;

    // Pop-up texts
    public TextMeshProUGUI xpPopupText;
    public TextMeshProUGUI damagePopupText;
    public TextMeshProUGUI healthPopupText;

    // Store the initial positions of the pop-up texts
    private Vector3 xpPopupInitialPosition;
    private Vector3 damagePopupInitialPosition;
    private Vector3 healthPopupInitialPosition;

    private void Start()
    {
        // Store the initial positions of the pop-up texts
        xpPopupInitialPosition = xpPopupText.transform.localPosition;
        damagePopupInitialPosition = damagePopupText.transform.localPosition;
        healthPopupInitialPosition = healthPopupText.transform.localPosition;
    }

    // Show XP Gain Pop-Up
    public void ShowXPGainPopup(float xpAmount)
    {
        StartCoroutine(ShowPopupText(xpPopupText, "+" + xpAmount + " XP", Color.green, xpPopupInitialPosition));
    }

    // Show Damage Pop-Up
    public void ShowDamagePopup(float damageAmount)
    {
        StartCoroutine(ShowPopupText(damagePopupText, "-" + damageAmount + " HP", Color.red, damagePopupInitialPosition));
    }

    // Show Health Restore Pop-Up
    public void ShowHealthRestorePopup(float restoreAmount)
    {
        StartCoroutine(ShowPopupText(healthPopupText, "+" + restoreAmount + " HP", Color.cyan, healthPopupInitialPosition));
    }

    // Coroutine to display and fade out the text, resetting its position before showing
    private IEnumerator ShowPopupText(TextMeshProUGUI popupText, string message, Color textColor, Vector3 initialPosition)
    {
        // Reset the position to the initial position
        popupText.transform.localPosition = initialPosition;

        // Set text and color
        popupText.text = message;
        popupText.color = textColor;

        // Show the text by setting alpha to 1
        popupText.alpha = 1f;

        // Make the text visible (if it's hidden)
        popupText.gameObject.SetActive(true);

        // Animate the text upwards slightly as it fades
        Vector3 targetPosition = initialPosition + new Vector3(0, 50, 0); // Move up 50 units

        float duration = 1.5f; // Time for text to be fully visible
        float elapsedTime = 0f;

        // Animate position and fade out
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            // Move the text upwards
            popupText.transform.localPosition = Vector3.Lerp(initialPosition, targetPosition, elapsedTime / duration);

            // Gradually fade out
            popupText.alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);

            yield return null;
        }

        // Hide the text after fading
        popupText.gameObject.SetActive(false);
        popupText.transform.localPosition = initialPosition; // Reset position in case it needs to show again
    }

    // Health UI update
    public void UpdateHealthUI(float currentHealth, float maxHealth)
    {
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
    }

    // XP UI update
    public void UpdateXPUI(float currentXP, float xpToNextLevel)
    {
        if (xpBar != null)
        {
            xpBar.maxValue = xpToNextLevel;
            xpBar.value = currentXP;
        }
    }

    // Level UI update
    public void UpdateLevelUI(int playerLevel)
    {
        if (levelText != null)
        {
            levelText.text = "LEVEL: " + playerLevel;
        }
    }
}
