using UnityEngine;
using UnityEngine.SceneManagement;  // Ensure Unity's SceneManagement namespace is imported
using System.Collections;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance;  // Singleton instance
    public GameState currentState = GameState.Playing;  // Tracks the current game state

    public GameObject pauseMenuUI;  // Reference to the pause menu UI
    private bool isPaused = false;  // Tracks whether the game is paused

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

    // Pauses the game by disabling all active objects and showing the pause menu
    public void PauseGame()
    {
        if (currentState == GameState.Playing)
        {
            currentState = GameState.Paused;
            isPaused = true;
            pauseMenuUI.SetActive(true);
            SetGameObjectsState(false);  // Disable all active game objects
        }
    }

    // Resumes the game by enabling all active objects and hiding the pause menu
    public void ResumeGame()
    {
        if (currentState == GameState.Paused)
        {
            currentState = GameState.Playing;
            isPaused = false;
            pauseMenuUI.SetActive(false);
            SetGameObjectsState(true);  // Enable all active game objects
        }
    }

    // Restarts the current level
    public void RestartLevel()
    {
        if (currentState == GameState.Paused || currentState == GameState.Playing)
        {
            currentState = GameState.Transitioning;
            StartCoroutine(LoadScene(SceneManager.GetActiveScene().buildIndex));  // Reload current scene
        }
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
