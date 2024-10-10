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
    private System.Action<UpgradePrefab> onUpgradeSelectedCallback;

    public Sprite defaultHealthIcon;
    public Sprite defaultSpeedIcon;
    public Sprite defaultWeaponIcon;
    public Sprite defaultOrbitIcon;
    public Sprite defaultOrbitDIcon;

    private bool isPaused = false;
    public GameState currentState = GameState.Playing;
    private NewPlayer player;
    private float lastRunScore = 0f;

    // Player Selection Variables
    [Header("Player Selection")]
    [SerializeField] private List<GameObject> playerPrefabs; 
    [SerializeField] private GameObject characterSelectionCanvas; 
    [SerializeField] private Image playerImage; 
    [SerializeField] private Image playerMainWeaponImage; 
    [SerializeField] private TextMeshProUGUI playerNameText; 
    [SerializeField] private TextMeshProUGUI playerDescriptionText; 
    private int currentIndex = 0;

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
    }

    private void Start()
    {

        if (mainMenuCanvas != null)
        {
            mainMenuCanvas.SetActive(false);
        }

        if (characterSelectionCanvas != null)
        {
            characterSelectionCanvas.SetActive(false);
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

        UpgradePrefab.SetDefaultIcons(defaultHealthIcon, defaultSpeedIcon, defaultWeaponIcon, defaultOrbitIcon, defaultOrbitDIcon);

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

        if (characterSelectionCanvas != null && characterSelectionCanvas.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                Navigate(-1); 
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                Navigate(1); 
            }
        }
    }

    // ---------------------- Player Selection Functions ---------------------- //

    public void StartCharacterSelection()
    {
        characterSelectionCanvas.SetActive(true);
        UpdatePlayerSelectionUI();
        
        
    }

    public void Navigate(int direction)
    {
        currentIndex += direction;
        if (currentIndex < 0) currentIndex = playerPrefabs.Count - 1;
        if (currentIndex >= playerPrefabs.Count) currentIndex = 0;

        UpdatePlayerSelectionUI();
    }

    private void UpdatePlayerSelectionUI()
    {
        GameObject selectedPlayerPrefab = playerPrefabs[currentIndex];
        PlayerInitializer initializer = selectedPlayerPrefab.GetComponent<PlayerInitializer>();
        if (initializer == null) return;

        playerNameText.text = initializer.playerName;
        playerDescriptionText.text = initializer.playerDescription;

        SpriteRenderer spriteRenderer = selectedPlayerPrefab.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            playerImage.sprite = spriteRenderer.sprite;
        }

        Transform mainWeaponTransform = selectedPlayerPrefab.transform.Find("WeaponHand/MainWeapon");
        if (mainWeaponTransform != null)
        {
            Weapon mainWeapon = mainWeaponTransform.GetComponent<Weapon>();
            if (mainWeapon != null)
            {
                SpriteRenderer weaponSpriteRenderer = mainWeapon.GetComponent<SpriteRenderer>();
                if (weaponSpriteRenderer != null)
                {
                    playerMainWeaponImage.sprite = weaponSpriteRenderer.sprite;
                }
                // Build the description dynamically using string interpolation
                playerDescriptionText.text = $"Health: {initializer.maxHealth}\n" +
                                             $"Speed: {initializer.speed}\n" +
                                             $"Orbital Speed: {initializer.orbitalSpeed}";
            }
        }
    }

    public void ConfirmCharacterSelection()
    {
        GameObject selectedPlayerPrefab = playerPrefabs[currentIndex];
        PlayerInitializer initializer = selectedPlayerPrefab.GetComponent<PlayerInitializer>();
        if (initializer == null) return;

        ApplyDataToPlayer(initializer, selectedPlayerPrefab);

        characterSelectionCanvas.SetActive(false);

        currentState = GameState.Playing;
        isPaused = false;

        SetPausableObjectsState(true);
    }


    private void ApplyDataToPlayer(PlayerInitializer initializer, GameObject selectedPlayerPrefab)
    {
        Weapon weapon = player.GetComponentInChildren<Weapon>();


        player.maxHealth = initializer.maxHealth;
        player.health = initializer.startHealth;
        player.currentXP = initializer.startXP;
        player.xpToNextLevel = initializer.xpToNextLevel;
        player.ObjectSpeed = initializer.speed;
        weapon.rotationSpeed = initializer.orbitalSpeed;

        SpriteRenderer playerSpriteRenderer = player.GetComponent<SpriteRenderer>();
        SpriteRenderer selectedSpriteRenderer = selectedPlayerPrefab.GetComponent<SpriteRenderer>();

        if (playerSpriteRenderer != null && selectedSpriteRenderer != null)
        {
            playerSpriteRenderer.sprite = selectedSpriteRenderer.sprite;
        }

        UpdateMainWeapon(initializer, selectedPlayerPrefab);

        player.UpdateUI();
    }

    private void UpdateMainWeapon(PlayerInitializer initializer, GameObject selectedPlayerPrefab)
    {
        Transform mainWeaponTransform = selectedPlayerPrefab.transform.Find("WeaponHand/MainWeapon");

        if (mainWeaponTransform != null)
        {
            Transform existingMainWeaponTransform = player.transform.Find("WeaponHand/MainWeapon");

            if (existingMainWeaponTransform != null)
            {
                Destroy(existingMainWeaponTransform.gameObject);
            }

            GameObject newWeapon = Instantiate(mainWeaponTransform.gameObject, player.transform.Find("WeaponHand"));
            newWeapon.name = "MainWeapon";
        }
    }



    // ---------------------- Main Menu Functions ---------------------- //

    public void ShowMainMenu()
    {
        currentState = GameState.Paused;
        isPaused = true;

        if (mainMenuCanvas != null)
        {
            mainMenuCanvas.SetActive(true);
        }

        SetPausableObjectsState(false);
    }

    public void StartGame()
    {
        if (mainMenuCanvas != null)
        {
            mainMenuCanvas.SetActive(false);
        }

        StartCharacterSelection();

        if (dieMenuUI != null)
        {
            dieMenuUI.SetActive(false);
        }

        if (player != null)
        {
            player.ResetPlayerScore();
        }
        else
        {
            Debug.LogWarning("Player is null in StartGame.");
        }

        
    }


    public void ReturnToMainMenu()
    {
        ResetAll();

        currentState = GameState.Paused;
        isPaused = true;

        if (mainMenuCanvas != null)
        {
            mainMenuCanvas.SetActive(true);
        }

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

    public void RestartFromDieMenu()
    {
        if (player != null)
        {
            player.ResetPlayerScore();
            player.ResetPlayerState(); 
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

    public void ShowUpgradeChoices(List<UpgradePrefab> upgrades, System.Action<UpgradePrefab> onUpgradeSelected)
    {
        upgradePanel.SetActive(true);
        onUpgradeSelectedCallback = onUpgradeSelected;

        currentState = GameState.Paused;
        isPaused = true;

        // Loop through the list of upgrade buttons and assign values
        for (int i = 0; i < upgradeButtons.Count; i++)
        {
            int index = i;
            UpgradePrefab upgrade = upgrades[i]; // Get the upgrade prefab

            var card = upgradeButtons[i].transform.parent;
            var icon = card.Find("Image").GetComponent<Image>();
            var description = card.Find("Upgrade Desc").GetComponent<TextMeshProUGUI>();

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

            // Use the GetIcon() method to assign either a custom or default icon to the button
            icon.sprite = upgrade.GetIcon();

            // Set the description
            description.text = $"{upgrade.upgradeName}\n{upgrade.GetDescription()}";

            // Set up the button click listener for the upgrade
            upgradeButtons[i].onClick.RemoveAllListeners();
            upgradeButtons[i].onClick.AddListener(() =>
            {
                onUpgradeSelectedCallback(upgrade);
                upgradePanel.SetActive(false);

                ResumeGame();
                Effects.LeveltUpFX(player.transform);
            });
        }

        SetPausableObjectsState(false);
    }



    // ---------------------- Restart and Utility Functions ---------------------- //

    public void RestartLevel()
    {
        currentState = GameState.Playing;
        isPaused = false;

        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        if (dieMenuUI != null) dieMenuUI.SetActive(false);
        if (mainMenuCanvas != null) mainMenuCanvas.SetActive(false);
        if (upgradePanel != null) upgradePanel.SetActive(false);

        ResetUpgrades();

        ResetEnemies(); 
        ResetCollectibles(); 
        ResetBullets();
        player.ResetPlayerState();

        SetPausableObjectsState(true);

        Debug.Log("Game restarted without reloading scene.");
    }

    public void ResetAll()
    {
        currentState = GameState.Paused;
        isPaused = true;

        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        if (dieMenuUI != null) dieMenuUI.SetActive(false);
        if (mainMenuCanvas != null) mainMenuCanvas.SetActive(true);
        if (upgradePanel != null) upgradePanel.SetActive(false);

        ResetUpgrades();

        ResetEnemies(); 
        ResetCollectibles(); 
        ResetBullets();
        player.ResetPlayerState();

        SetPausableObjectsState(false);

        Debug.Log("Game restarted without reloading scene.");
    }

    private void ResetUpgrades()
    {
        if (upgradePanel != null)
        {
            upgradePanel.SetActive(false);
        }

        foreach (Button button in upgradeButtons)
        {
            button.onClick.RemoveAllListeners();
            var card = button.transform.parent;
            var description = card.Find("Upgrade Desc").GetComponent<TextMeshProUGUI>();
            if (description != null) description.text = string.Empty;
        }

        LevelManager levelManager = FindObjectOfType<LevelManager>();
        if (levelManager != null)
        {
            levelManager.InitializeValues(player);
        }
        else
        {
            Debug.LogError("LevelManager not found in the scene!");
        }
    }

    private void ResetEnemies()
    {
        NewEnemy[] enemies = FindObjectsOfType<NewEnemy>();
        foreach (var enemy in enemies)
        {
            Destroy(enemy.gameObject); 
        }
    }

    private void ResetCollectibles()
    {
        Collectible[] collectibles = FindObjectsOfType<Collectible>();
        foreach (var collectible in collectibles)
        {
            Destroy(collectible.gameObject);
        }
    }

    private void ResetBullets()
    {
        Bullet[] bullets = FindObjectsOfType<Bullet>();
        foreach (var bullet in bullets)
        {
            Destroy(bullet.gameObject);
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
