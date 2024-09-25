using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Linq;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance;  // Singleton instance
    public GameState currentState = GameState.Playing;  // Tracks the current game state

    public GameObject pauseMenuUI;  // Reference to the pause menu UI
    private bool isPaused = false;

    private void Awake()
    {
        // Ensure there is only one instance of this class
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Make sure the SceneManager persists across scenes
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Try to find the PauseMenuCanvas and make it persistent across scenes
        if (pauseMenuUI == null)
        {
            FindPauseMenuUI();
        }
    }

    private void Start()
    {
        if (pauseMenuUI != null)
        {
            // Make the pause menu canvas persistent across scenes
            DontDestroyOnLoad(pauseMenuUI.transform.root.gameObject);  // Ensures the entire Canvas isn't destroyed
        }
    }

    private void OnDestroy()
    {
        // Unregister the sceneLoaded callback
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Update()
    {
        // Check for pause button (Escape key)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentState == GameState.Playing)
            {
                PauseGame();
            }
            else if (currentState == GameState.Paused)
            {
                ResumeGame();
            }
        }
    }

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
            // Hide the pause menu at the start of the game
            pauseMenuUI.SetActive(false);

            // Make sure the Pause Menu Canvas persists across scenes
            DontDestroyOnLoad(pauseMenuUI.transform.root.gameObject);  // Ensure the canvas doesn't get destroyed
        }
    }

    // Pauses the game by pausing all IPausable objects and showing the pause menu
    public void PauseGame()
    {
        if (currentState == GameState.Playing)
        {
            currentState = GameState.Paused;
            isPaused = true;

            if (pauseMenuUI != null)
            {
                pauseMenuUI.SetActive(true);  // Show the pause menu UI
            }
            else
            {
                Debug.LogError("Pause Menu UI is missing!");
            }

            SetPausableObjectsState(false);  // Pause all IPausable objects
        }
    }

    // Resumes the game by resuming all IPausable objects and hiding the pause menu
    public void ResumeGame()
    {
        if (currentState == GameState.Paused)
        {
            currentState = GameState.Playing;
            isPaused = false;

            if (pauseMenuUI != null)
            {
                pauseMenuUI.SetActive(false);  // Hide the pause menu UI
            }

            SetPausableObjectsState(true);  // Resume all IPausable objects
        }
    }

    // Handle scene reloads to reinitialize necessary references
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (pauseMenuUI == null)
        {
            FindPauseMenuUI();  // Refind the UI if needed after a new scene load
        }
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

    // Returns to the main menu
    public void ReturnToMainMenu()
    {
        // Remove the PauseMenuCanvas from DontDestroyOnLoad
        if (pauseMenuUI != null)
        {
            Destroy(pauseMenuUI.transform.root.gameObject);  // Destroy the Pause Menu Canvas when returning to the main menu
        }

        TransitionToScene(0);  // Assuming the main menu is scene 0
    }

    // Exits the game
    public void ExitGame()
    {
        Debug.Log("Exiting game...");
        Application.Quit();
    }
}
