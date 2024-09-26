using UnityEngine;
using UnityEngine.SceneManagement;  // To handle scene transitions

public class MainMenuManager : MonoBehaviour
{
    public GameObject settingsPanel;  // Reference to the SettingsPanel
    public GameObject creditsPanel;   // Reference to the CreditsPanel

    // This method is called when the "Start Game" button is clicked
    public void StartGame()
    {
        // Load the game scene (assuming it's scene 1 in the build settings)
        SceneManager.LoadScene(1);
    }

    // This method is called when the "Settings" button is clicked
    public void OpenSettings()
    {
        // Show the Settings Panel
        settingsPanel.SetActive(true);
    }

    // This method is called when the "Credits" button is clicked
    public void OpenCredits()
    {
        // Show the Credits Panel
        creditsPanel.SetActive(true);
    }

    // This method is called when the "Exit Game" button is clicked
    public void ExitGame()
    {
        Debug.Log("Exiting game...");
        Application.Quit();  // This will only work in a built application, not in the Unity editor
    }

    // Optional: To close Settings and Credits panels
    public void CloseSettings()
    {
        settingsPanel.SetActive(false);  // Hide the Settings Panel
    }

    public void CloseCredits()
    {
        creditsPanel.SetActive(false);  // Hide the Credits Panel
    }
}
