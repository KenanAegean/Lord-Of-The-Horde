using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Linq;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance;

    [Header("Main Menu Panels")]
    public GameObject mainMenuCanvas;
    public GameObject settingsPanel;
    public GameObject creditsPanel;

    [Header("Pause Menu")]
    public GameObject pauseMenuUI;

    [Header("Die Menu")]
    public GameObject dieMenuUI;

    private bool isPaused = false;
    public GameState currentState = GameState.Playing;
    private NewPlayer player;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (mainMenuCanvas != null)
            {
                DontDestroyOnLoad(mainMenuCanvas);
            }
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (pauseMenuUI == null) FindPauseMenuUI();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        // Find the player instance safely
        player = NewPlayer.Instance;
        if (player == null)
        {
            Debug.LogWarning("NewPlayer instance not found in the scene.");
        }

        if (pauseMenuUI != null || dieMenuUI != null)
        {
            DontDestroyOnLoad(pauseMenuUI.transform.root.gameObject);
            DontDestroyOnLoad(dieMenuUI.transform.root.gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (mainMenuCanvas != null)
        {
            mainMenuCanvas.SetActive(scene.name == "MainMenu");
        }

        if (pauseMenuUI == null || dieMenuUI == null) FindPauseMenuUI();

        // Re-find the player when a new scene is loaded
        player = NewPlayer.Instance;
        if (player == null)
        {
            Debug.LogWarning("NewPlayer instance not found after scene load.");
        }
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            if (Input.GetKeyDown(KeyCode.Escape) && !dieMenuUI.activeSelf)
            {
                Debug.Log(dieMenuUI.activeSelf);
                if (currentState == GameState.Playing) PauseGame();
                else if (currentState == GameState.Paused) ResumeGame();
            }
        }
    }

    // ---------------------- Main Menu Functions ---------------------- //

    public void StartGame()
    {
        // Check if player is not null before resetting score
        if (player != null)
        {
            player.ResetPlayerScore();
        }
        else
        {
            Debug.LogWarning("Player is null in StartGame.");
        }

        SceneManager.LoadScene(1);
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    public void OpenCredits()
    {
        creditsPanel.SetActive(true);
    }

    public void ClosePanel(GameObject panel)
    {
        panel.SetActive(false);
    }

    public void ExitGame()
    {
        Debug.Log("Exiting game...");
        Application.Quit();
    }

    // ---------------------- Pause & Die Menu Functions ---------------------- //

    private void FindPauseMenuUI()
    {
        pauseMenuUI = GameObject.Find("PauseMenuCanvas")?.transform.Find("PauseMenuPanel")?.gameObject;
        dieMenuUI = GameObject.Find("DieMenuCanvas")?.transform.Find("DieMenuPanel")?.gameObject;

        if (pauseMenuUI == null || dieMenuUI == null)
        {
            Debug.LogError("Pause Menu UI or Die Menu UI not found in the scene!");
        }
        else
        {
            pauseMenuUI.SetActive(false);
            DontDestroyOnLoad(pauseMenuUI.transform.root.gameObject);
            dieMenuUI.SetActive(false);
            DontDestroyOnLoad(dieMenuUI.transform.root.gameObject);
        }
    }

    public void PauseGame()
    {
        currentState = GameState.Paused;
        isPaused = true;

        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(true);
        }

        SetPausableObjectsState(false);
    }

    public void ResumeGame()
    {
        currentState = GameState.Playing;
        isPaused = false;

        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }

        SetPausableObjectsState(true);
    }

    // Method to restart the level when "Restart" is clicked
    public void RestartFromDieMenu()
    {
        if (player != null)
        {
            player.ResetPlayerScore();
        }
        else
        {
            Debug.LogWarning("Player is null in RestartFromDieMenu.");
        }

        if (dieMenuUI != null)
        {
            dieMenuUI.SetActive(false);
        }
        RestartLevel();
    }

    // Method to return to the main menu when "Main Menu" is clicked
    public void ReturnToMainMenuFromDieMenu()
    {
        if (dieMenuUI != null)
        {
            dieMenuUI.SetActive(false);
        }
        ReturnToMainMenu();
    }

    // ---------------------- Game Scene Transition Functions ---------------------- //

    public void RestartLevel()
    {
        if (player != null)
        {
            player.ResetPlayerScore();
        }
        else
        {
            Debug.LogWarning("Player is null in RestartLevel.");
        }

        currentState = GameState.Playing;
        isPaused = false;

        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }

        StartCoroutine(LoadScene(SceneManager.GetActiveScene().buildIndex));
    }

    public void TransitionToScene(int sceneIndex)
    {
        currentState = GameState.Transitioning;
        StartCoroutine(LoadScene(sceneIndex));
    }

    private IEnumerator LoadScene(int sceneIndex)
    {
        yield return SceneManager.LoadSceneAsync(sceneIndex);
        currentState = GameState.Playing;
    }

    public void ReturnToMainMenu()
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }

        TransitionToScene(0);
    }

    public void SetPausableObjectsState(bool isResuming)
    {
        IPausable[] pausableObjects = FindObjectsOfType<MonoBehaviour>().OfType<IPausable>().ToArray();

        foreach (IPausable pausable in pausableObjects)
        {
            if (isResuming) pausable.OnResume();
            else pausable.OnPause();
        }
    }
}
