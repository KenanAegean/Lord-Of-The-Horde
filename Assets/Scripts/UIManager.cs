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

    // References for pop-up text
    public TextMeshProUGUI xpPopupText;
    public TextMeshProUGUI damagePopupText;
    public TextMeshProUGUI healthPopupText;

    // Show XP Gain Pop-Up
    public void ShowXPGainPopup(float xpAmount)
    {
        StartCoroutine(ShowPopupText(xpPopupText, "+" + xpAmount + " XP", Color.green));
    }

    // Show Damage Pop-Up
    public void ShowDamagePopup(float damageAmount)
    {
        StartCoroutine(ShowPopupText(damagePopupText, "-" + damageAmount + " HP", Color.red));
    }

    // Show Health Restore Pop-Up
    public void ShowHealthRestorePopup(float restoreAmount)
    {
        StartCoroutine(ShowPopupText(healthPopupText, "+" + restoreAmount + " HP", Color.cyan));
    }

    // Coroutine to display and fade out the text
    private IEnumerator ShowPopupText(TextMeshProUGUI popupText, string message, Color textColor)
    {
        // Set text and color
        popupText.text = message;
        popupText.color = textColor;

        // Show the text by setting alpha to 1
        popupText.alpha = 1f;

        // Make the text visible (if it's hidden)
        popupText.gameObject.SetActive(true);

        // Optionally, move the text upwards slightly as it fades
        Vector3 initialPosition = popupText.transform.localPosition;
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
        popupText.transform.localPosition = initialPosition; // Reset position
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
