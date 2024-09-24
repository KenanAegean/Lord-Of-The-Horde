using UnityEngine;
using UnityEngine.SceneManagement;  // For scene loading
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
        }

        // Register the sceneLoaded callback
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        // Try to find the PauseMenuPanel if it has not been assigned
        if (pauseMenuUI == null)
        {
            //pauseMenuUI = GameObject.Find("PauseMenuPanel");
            pauseMenuUI = GameObject.Find("PauseMenuCanvas").transform.Find("PauseMenuPanel").gameObject;

            if (pauseMenuUI != null)
            {
                // Disable it immediately after finding it
                pauseMenuUI.SetActive(false);
            }
            else
            {
                Debug.LogError("Pause Menu UI not found in the scene!");
            }
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
        // Reassign the pause menu UI for the newly loaded scene if it's not already assigned
        if (pauseMenuUI == null)
        {
            pauseMenuUI = GameObject.Find("PauseMenuCanvas").transform.Find("PauseMenuPanel").gameObject;

            if (pauseMenuUI == null)
            {
                Debug.LogError("Pause Menu UI not found in the new scene!");
            }
            else
            {
                pauseMenuUI.SetActive(false);  // Hide it immediately after finding
            }
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
        // Reset the game state to playing and make sure the pause menu is hidden
        currentState = GameState.Playing;
        isPaused = false;

        // Hide the pause menu UI
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }

        // Reload the current scene
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
        yield return SceneManager.LoadSceneAsync(sceneIndex);  // Use Unity's SceneManager for scene loading
        currentState = GameState.Playing;  // Set the game state back to playing after the scene loads
    }

    // Returns to the main menu
    public void ReturnToMainMenu()
    {
        TransitionToScene(0);  // Assuming the main menu is scene 0
    }

    // Exits the game
    public void ExitGame()
    {
        Debug.Log("Exiting game...");
        Application.Quit();
    }

    // Sets the active state of all relevant game objects
    private void SetGameObjectsState(bool state)
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.CompareTag("Player") || obj.CompareTag("Enemy"))  // Assuming player/enemy tags for controllable objects
            {
                obj.SetActive(state);  // Enable or disable based on the game state
            }
        }
    }
}
