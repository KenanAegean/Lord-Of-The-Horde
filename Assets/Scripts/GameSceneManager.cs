using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Linq;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance;  // Singleton instance

    [Header("Main Menu Panels")]
    public GameObject settingsPanel;  // Reference to the SettingsPanel
    public GameObject creditsPanel;   // Reference to the CreditsPanel

    [Header("Pause Menu")]
    public GameObject pauseMenuUI;  // Reference to the pause menu UI

    private bool isPaused = false;
    public GameState currentState = GameState.Playing;  // Tracks the current game state

    private void Awake()
    {
        // Ensure there is only one instance of this class
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Persist across scenes
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // If Pause Menu is not set, attempt to find it
        if (pauseMenuUI == null)
        {
            FindPauseMenuUI();
        }
    }

    private void Start()
    {
        // Make the pause menu persistent across scenes if it's set
        if (pauseMenuUI != null)
        {
            DontDestroyOnLoad(pauseMenuUI.transform.root.gameObject);
        }
    }

    private void Update()
    {
        // Check for pause button (Escape key) to pause or resume the game
        if (Input.GetKeyDown(KeyCode.Escape) && currentState == GameState.Playing)
        {
            PauseGame();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && currentState == GameState.Paused)
        {
            ResumeGame();
        }
    }

    // ---------------------- Main Menu Functions ---------------------- //

    // This method is called when the "Start Game" button is clicked
    public void StartGame()
    {
        SceneManager.LoadScene(1);  // Load game scene (assumed to be scene 1)
    }

    // This method is called when the "Settings" button is clicked
    public void OpenSettings()
    {
        settingsPanel.SetActive(true);  // Show settings panel
    }

    // This method is called when the "Credits" button is clicked
    public void OpenCredits()
    {
        creditsPanel.SetActive(true);  // Show credits panel
    }

    // Close the Settings or Credits panel
    public void ClosePanel(GameObject panel)
    {
        panel.SetActive(false);  // Hide the panel (either settings or credits)
    }
    // Exits the game
    public void ExitGame()
    {
        Debug.Log("Exiting game...");
        Application.Quit();  // This works only in a built application, not in the editor
    }

    // ---------------------- Pause Menu Functions ---------------------- //

    // Finds the Pause Menu UI in the scene
    private void FindPauseMenuUI()
    {
        pauseMenuUI = GameObject.Find("PauseMenuCanvas")?.transform.Find("PauseMenuPanel")?.gameObject;

        if (pauseMenuUI == null)
        {
            Debug.LogError("Pause Menu UI not found in the scene!");
        }
        else
        {
            pauseMenuUI.SetActive(false);  // Hide the pause menu at the start
            DontDestroyOnLoad(pauseMenuUI.transform.root.gameObject);  // Make it persistent across scenes
        }
    }

    // Pauses the game by pausing all IPausable objects and showing the pause menu
    public void PauseGame()
    {
        currentState = GameState.Paused;
        isPaused = true;

        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(true);  // Show the pause menu UI
        }

        SetPausableObjectsState(false);  // Pause all IPausable objects
    }

    // Resumes the game by resuming all IPausable objects and hiding the pause menu
    public void ResumeGame()
    {
        currentState = GameState.Playing;
        isPaused = false;

        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);  // Hide the pause menu UI
        }

        SetPausableObjectsState(true);  // Resume all IPausable objects
    }

    // ---------------------- Game Scene Transition Functions ---------------------- //

    // Restarts the current level
    public void RestartLevel()
    {
        currentState = GameState.Playing;
        isPaused = false;

        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);  // Hide the pause menu UI before restarting
        }

        StartCoroutine(LoadScene(SceneManager.GetActiveScene().buildIndex));
    }

    // General method to transition to another scene
    public void TransitionToScene(int sceneIndex)
    {
        currentState = GameState.Transitioning;
        StartCoroutine(LoadScene(sceneIndex));
    }

    // Coroutine to load scenes asynchronously
    private IEnumerator LoadScene(int sceneIndex)
    {
        yield return SceneManager.LoadSceneAsync(sceneIndex);  // Load the scene asynchronously
        currentState = GameState.Playing;  // Set the game state back to playing after the scene loads
    }

    // Handle scene reloads to reinitialize necessary references
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (pauseMenuUI == null)
        {
            FindPauseMenuUI();  // Refind the UI if needed after a new scene load
        }
    }

    // Returns to the main menu
    public void ReturnToMainMenu()
    {
        
        // Completely destroy the PauseMenuCanvas when returning to the main menu
        if (pauseMenuUI != null)
        {
            Destroy(pauseMenuUI.transform.root.gameObject);  // Destroy Pause Menu when returning to the main menu
        }

        //TransitionToScene(0);  // Assuming the main menu is scene 0
        
        SceneManager.LoadScene("MainMenu");
    }

    // Sets the state of all IPausable objects in the scene
    private void SetPausableObjectsState(bool isResuming)
    {
        IPausable[] pausableObjects = FindObjectsOfType<MonoBehaviour>().OfType<IPausable>().ToArray();

        foreach (IPausable pausable in pausableObjects)
        {
            if (isResuming)
            {
                pausable.OnResume();
            }
            else
            {
                pausable.OnPause();
            }
        }
    }
}
