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

    private bool isPaused = false;
    public GameState currentState = GameState.Playing;

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
        if (pauseMenuUI != null)
        {
            DontDestroyOnLoad(pauseMenuUI.transform.root.gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (mainMenuCanvas != null)
        {
            mainMenuCanvas.SetActive(scene.name == "MainMenu");
        }

        if (pauseMenuUI == null) FindPauseMenuUI();
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (currentState == GameState.Playing) PauseGame();
                else if (currentState == GameState.Paused) ResumeGame();
            }
        }
    }

    // ---------------------- Main Menu Functions ---------------------- //

    public void StartGame()
    {
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

    // ---------------------- Pause Menu Functions ---------------------- //

    private void FindPauseMenuUI()
    {
        pauseMenuUI = GameObject.Find("PauseMenuCanvas")?.transform.Find("PauseMenuPanel")?.gameObject;

        if (pauseMenuUI == null)
        {
            Debug.LogError("Pause Menu UI not found in the scene!");
        }
        else
        {
            pauseMenuUI.SetActive(false);
            DontDestroyOnLoad(pauseMenuUI.transform.root.gameObject);
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

    // ---------------------- Game Scene Transition Functions ---------------------- //

    public void RestartLevel()
    {
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
            //Destroy(pauseMenuUI.transform.root.gameObject);
        }

        TransitionToScene(0);
    }

    private void SetPausableObjectsState(bool isResuming)
    {
        IPausable[] pausableObjects = FindObjectsOfType<MonoBehaviour>().OfType<IPausable>().ToArray();

        foreach (IPausable pausable in pausableObjects)
        {
            if (isResuming) pausable.OnResume();
            else pausable.OnPause();
        }
    }
}
