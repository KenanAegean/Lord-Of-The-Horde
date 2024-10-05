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

    public Sprite defaultHealthIcon;
    public Sprite defaultSpeedIcon;
    public Sprite defaultWeaponIcon;
    public Sprite defaultOrbitIcon;
    public Sprite defaultOrbitDIcon;

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

            // mainMenuCanvas should not be destroyed, as it will be used across game state transitions
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
    }

    private void Start()
    {
        // Explicitly deactivate main menu canvas at the start
        if (mainMenuCanvas != null)
        {
            mainMenuCanvas.SetActive(false);
        }

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

        UpgradeOption.SetDefaultIcons(defaultHealthIcon, defaultSpeedIcon, defaultWeaponIcon, defaultOrbitIcon, defaultOrbitDIcon);

        // Display the main menu only when the game starts (not on restart)
        ShowMainMenu();
    }

    private void Update()
    {
        // Prevent pausing while in the main menu
        if (mainMenuCanvas != null && mainMenuCanvas.activeSelf) return;

        if (Input.GetKeyDown(KeyCode.Escape) && !dieMenuUI.activeSelf)
        {
            if (currentState == GameState.Playing) PauseGame();
            else if (currentState == GameState.Paused) ResumeGame();
        }
    }

    // ---------------------- Main Menu Functions ---------------------- //

    public void ShowMainMenu()
    {
        // Set the game state to paused since we're in the main menu
        currentState = GameState.Paused;
        isPaused = true;

        // Activate the main menu panel
        if (mainMenuCanvas != null)
        {
            mainMenuCanvas.SetActive(true);
        }

        // Pause all game objects
        SetPausableObjectsState(false);
    }

    public void StartGame()
    {
        // Deactivate the main menu panel to start the game
        if (mainMenuCanvas != null)
        {
            mainMenuCanvas.SetActive(false);
        }
        RestartLevel();

        if (dieMenuUI != null)
        {
            dieMenuUI.SetActive(false);
        }


        // Reset player score or any other state necessary to start the game
        if (player != null)
        {
            player.ResetPlayerScore();
        }
        else
        {
            Debug.LogWarning("Player is null in StartGame.");
        }

        // Set game state
        currentState = GameState.Playing;
        isPaused = false;

        // Resume all pausable objects
        SetPausableObjectsState(true);
    }

    public void ReturnToMainMenu()
    {
        // Pause the game and show the main menu panel
        currentState = GameState.Paused;
        isPaused = true;

        if (mainMenuCanvas != null)
        {
            mainMenuCanvas.SetActive(true);
        }

        // Pause all game objects
        SetPausableObjectsState(false);
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
            player.ResetPlayerState(); // Assuming you create this method to reset health, XP, position, etc.
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

    // ---------------------- Upgrade Functions ---------------------- //

    public void ShowUpgradeChoices(List<UpgradeOption> upgrades, System.Action<UpgradeOption> onUpgradeSelected)
    {
        upgradePanel.SetActive(true);
        onUpgradeSelectedCallback = onUpgradeSelected;

        currentState = GameState.Paused;
        isPaused = true;

        for (int i = 0; i < upgradeButtons.Count; i++)
        {
            int index = i;
            UpgradeOption upgrade = upgrades[i];

            // Access the card's elements
            var card = upgradeButtons[i].transform.parent; // Assuming button is a child of card
            var icon = card.Find("Image").GetComponent<Image>(); // Find the image component
            var description = card.Find("Upgrade Desc").GetComponent<TextMeshProUGUI>(); // Find the text component

            // Check for nulls
            if (icon == null)
            {
                Debug.LogError($"Icon Image component missing for upgrade {i}");
                continue;
            }
            if (description == null)
            {
                Debug.LogError($"Description Text component missing for upgrade {i}");
                continue;
            }

            // Set the icon, name, and description for each upgrade card
            icon.sprite = upgrade.GetIcon(); // Use GetIcon() to assign the correct icon
            description.text = $"{upgrade.upgradeName}\n{upgrade.GetDescription()}";

            // Remove any previous listeners
            upgradeButtons[i].onClick.RemoveAllListeners();

            // Add a click listener to apply the upgrade and unpause the game
            upgradeButtons[i].onClick.AddListener(() =>
            {
                onUpgradeSelectedCallback(upgrade);
                upgradePanel.SetActive(false);

                // Resume the game after an upgrade is selected
                ResumeGame();
                Effects.LeveltUpFX(player.transform);
            });
        }
        SetPausableObjectsState(false);
        
    }

    // ---------------------- Restart and Utility Functions ---------------------- //

    public void RestartLevel()
    {
        // Ensure game state is properly reset to "playing"
        currentState = GameState.Playing;
        isPaused = false;

        // Hide any UI panels that shouldn't be visible during the game
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        if (dieMenuUI != null) dieMenuUI.SetActive(false);
        if (mainMenuCanvas != null) mainMenuCanvas.SetActive(false);
        if (upgradePanel != null) upgradePanel.SetActive(false);

        // Reset upgrades and player/game state through LevelManager's InitalValues()
        ResetUpgrades();

        // Reset enemies, bullets, or other gameplay objects
        ResetEnemies(); // Reset all enemies
        ResetCollectibles(); // Reset any collectibles
        ResetBullets(); // Reset all bullets

        // Resume all game objects that should start active
        SetPausableObjectsState(true);

        Debug.Log("Game restarted without reloading scene.");
    }

    private void ResetUpgrades()
    {
        // Reset the upgrade panel if it's active
        if (upgradePanel != null)
        {
            upgradePanel.SetActive(false);
        }

        // Reset upgrade buttons by removing listeners and clearing text
        foreach (Button button in upgradeButtons)
        {
            button.onClick.RemoveAllListeners();
            var card = button.transform.parent;
            var description = card.Find("Upgrade Desc").GetComponent<TextMeshProUGUI>();
            if (description != null) description.text = string.Empty;
        }

        // Call InitalValues() to reset the player and game state
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        if (levelManager != null)
        {
            levelManager.InitalValues();
        }
        else
        {
            Debug.LogError("LevelManager not found in the scene!");
        }
    }

    private void ResetEnemies()
    {
        // Find all enemies in the scene
        NewEnemy[] enemies = FindObjectsOfType<NewEnemy>();
        foreach (var enemy in enemies)
        {
            Destroy(enemy.gameObject); // Remove or reset each enemy as required
        }

        // Optionally, respawn enemies or set up the initial state
        // enemySpawner.SpawnInitialEnemies(); // Assuming you have an enemy spawner
    }

    private void ResetCollectibles()
    {
        // Find and destroy all collectibles in the scene
        Collectible[] collectibles = FindObjectsOfType<Collectible>();
        foreach (var collectible in collectibles)
        {
            Destroy(collectible.gameObject);
        }
    }

    private void ResetBullets()
    {
        // Find all bullets in the scene
        Bullet[] bullets = FindObjectsOfType<Bullet>();
        foreach (var bullet in bullets)
        {
            Destroy(bullet.gameObject); // Destroy each bullet
        }
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
