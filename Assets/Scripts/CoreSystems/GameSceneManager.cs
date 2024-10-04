using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using TMPro;
using System.Collections.Generic;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance;

    [Header("Main Menu Panels")]
    public GameObject mainMenuCanvas;
    public GameObject settingsPanel;
    public GameObject creditsPanel;
    public TextMeshProUGUI lastRunScoreText;

    [Header("Pause Menu")]
    public GameObject pauseMenuUI;

    [Header("Die Menu")]
    public GameObject dieMenuUI;

    [Header("Upgrade System")]
    public GameObject upgradePanel; // The panel containing the upgrade buttons
    public List<Button> upgradeButtons; // A list of buttons for the upgrades
    public TextMeshProUGUI upgradeDescriptionText; // The text field for showing descriptions
    private System.Action<UpgradeOption> onUpgradeSelectedCallback;

    private bool isPaused = false;
    public GameState currentState = GameState.Playing;
    private NewPlayer player;
    private float lastRunScore = 0f;

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

        // If the scene is the main menu, update the last run score text
        if (scene.name == "MainMenu" && lastRunScoreText != null)
        {
            lastRunScoreText.text = "LAST RUN SCORE: " + lastRunScore.ToString("F0");
        }

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

    // ---------------------- Upgrade Functions ---------------------- //

    public void ShowUpgradeChoices(List<UpgradeOption> upgrades, System.Action<UpgradeOption> onUpgradeSelected)
    {
        upgradePanel.SetActive(true);
        onUpgradeSelectedCallback = onUpgradeSelected;

        for (int i = 0; i < upgradeButtons.Count; i++)
        {
            currentState = GameState.Paused;
            isPaused = true;
            int index = i;
            UpgradeOption upgrade = upgrades[i];

            // Set the button text to the upgrade name
            upgradeButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = upgrade.upgradeName;

            // Remove any previous listeners
            upgradeButtons[i].onClick.RemoveAllListeners();

            // Add a click listener to apply the upgrade
            upgradeButtons[i].onClick.AddListener(() =>
            {
                onUpgradeSelectedCallback(upgrade);
                upgradePanel.SetActive(false); // Hide panel after selection
                ResumeGame();
            });

            // Add listeners for showing the description on hover/selection
            upgradeButtons[i].onClick.AddListener(() =>
            {
                upgradeDescriptionText.text = upgrade.description; // Show description on hover
            });

            // Alternatively, if using hover effects (requires Unity's Event Trigger system):
            var trigger = upgradeButtons[i].gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
            var entry = new UnityEngine.EventSystems.EventTrigger.Entry
            {
                eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter
            };
            entry.callback.AddListener((data) => { upgradeDescriptionText.text = upgrade.description; });
            trigger.triggers.Add(entry);
        }

        // Reset the description text when the panel is first shown
        upgradeDescriptionText.text = "Hover over an upgrade to see its description.";
        SetPausableObjectsState(false);
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

    public void SetLastRunScore(float score)
    {
        lastRunScore = score;

        // Update the main menu's last score display if it's already active
        if (lastRunScoreText != null)
        {
            lastRunScoreText.text = "Last Run Score: " + lastRunScore.ToString("F0");
        }
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
