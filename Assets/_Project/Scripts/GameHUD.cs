using Blocks.Character;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace KnightsQuest
{
    public class GameHUD : MonoBehaviour
    {
        public static GameHUD Instance { get; private set; }

        [Header("Instruction Screen")]
        [SerializeField] GameObject instructionPanel;
        [SerializeField] TextMeshProUGUI instructionLevelTitle;
        [SerializeField] TextMeshProUGUI instructionStoryText;
        [SerializeField] TextMeshProUGUI instructionObjectiveText;
        [SerializeField] TextMeshProUGUI instructionControlsText;
        [SerializeField] Button instructionContinueButton;
        [SerializeField] Button instructionBackButton;

        [Header("In-Game HUD Elements")]
        [SerializeField] GameObject inGameHudRoot;
        [SerializeField] TextMeshProUGUI levelNameText;
        [SerializeField] TextMeshProUGUI coinCountText;
        [SerializeField] Slider healthSlider;
        [SerializeField] TextMeshProUGUI healthText;

        [Header("Pause Menu")]
        [SerializeField] GameObject pauseMenuRoot;
        [SerializeField] Button pauseResumeButton;
        [SerializeField] Button pauseRestartButton;
        [SerializeField] Button pauseMainMenuButton;

        [Header("Game Over Screen")]
        [SerializeField] GameObject gameOverRoot;
        [SerializeField] Button gameOverRetryButton;
        [SerializeField] Button gameOverMainMenuButton;

        [Header("Level Complete / Victory Screen")]
        [SerializeField] GameObject levelCompleteRoot;
        [SerializeField] TextMeshProUGUI levelCompleteTitle;
        [SerializeField] TextMeshProUGUI levelCompleteStats;
        [SerializeField] Button nextLevelButton;
        [SerializeField] Button victoryMainMenuButton;

        [Header("Player Reference")]
        [SerializeField] BuildingBlocksCharacter playerCharacter;

        bool m_IsPaused;
        bool m_InstructionActive;

        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            SetupButtons();

            if (LevelController.Instance != null && inGameHudRoot != null)
            {
                if (levelNameText != null)
                {
                    levelNameText.text = LevelController.Instance.LevelName;
                }
            }

            if (playerCharacter != null)
            {
                playerCharacter.OnHealthChanged += UpdateHealthUI;
                // Initial health display
                UpdateHealthUI(playerCharacter.HealthRatio * 100f, 100f);
            }

            // Always show instructions on level entry
            ShowInstructions(true);
        }

        void OnDestroy()
        {
            if (playerCharacter != null)
            {
                playerCharacter.OnHealthChanged -= UpdateHealthUI;
            }
        }

        void Update()
        {
            var keyboard = Keyboard.current;
            var gamepad = Gamepad.current;

            // Handle Instruction screen keyboard / gamepad shortcuts
            if (m_InstructionActive)
            {
                if ((keyboard != null && (keyboard.spaceKey.wasPressedThisFrame || keyboard.enterKey.wasPressedThisFrame || keyboard.eKey.wasPressedThisFrame)) ||
                    (gamepad != null && (gamepad.buttonSouth.wasPressedThisFrame || gamepad.startButton.wasPressedThisFrame)))
                {
                    OnInstructionContinue();
                    return;
                }

                if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame)
                {
                    OnInstructionBack();
                    return;
                }
            }

            bool pauseTriggered = false;
            if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame)
            {
                pauseTriggered = true;
            }

            if (gamepad != null && gamepad.startButton.wasPressedThisFrame)
            {
                pauseTriggered = true;
            }

            if (pauseTriggered)
            {
                if (!m_InstructionActive && !gameOverRoot.activeSelf && !levelCompleteRoot.activeSelf)
                {
                    TogglePause();
                }
            }

            // Update coin counter dynamically
            if (LevelController.Instance != null && coinCountText != null)
            {
                coinCountText.text = $"{LevelController.Instance.CurrentCoins}";
            }
        }

        void SetupButtons()
        {
            if (instructionContinueButton != null)
            {
                instructionContinueButton.onClick.AddListener(OnInstructionContinue);
            }
            if (instructionBackButton != null)
            {
                instructionBackButton.onClick.AddListener(OnInstructionBack);
            }

            if (pauseResumeButton != null)
            {
                pauseResumeButton.onClick.AddListener(ResumeGame);
            }
            if (pauseRestartButton != null)
            {
                pauseRestartButton.onClick.AddListener(() => LevelController.Instance?.RestartLevel());
            }
            if (pauseMainMenuButton != null)
            {
                pauseMainMenuButton.onClick.AddListener(() => LevelController.Instance?.LoadMainMenu());
            }

            if (gameOverRetryButton != null)
            {
                gameOverRetryButton.onClick.AddListener(() => LevelController.Instance?.RestartLevel());
            }
            if (gameOverMainMenuButton != null)
            {
                gameOverMainMenuButton.onClick.AddListener(() => LevelController.Instance?.LoadMainMenu());
            }

            if (nextLevelButton != null)
            {
                nextLevelButton.onClick.AddListener(() => LevelController.Instance?.LoadNextLevel());
            }
            if (victoryMainMenuButton != null)
            {
                victoryMainMenuButton.onClick.AddListener(() => LevelController.Instance?.LoadMainMenu());
            }
        }

        void UpdateHealthUI(float current, float max)
        {
            if (healthSlider != null)
            {
                healthSlider.value = Mathf.Clamp01(max > 0f ? current / max : 0f);
            }
            if (healthText != null)
            {
                healthText.text = $"{Mathf.CeilToInt(current)}/{Mathf.CeilToInt(max)}";
            }
        }

        public void ShowInstructions(bool show)
        {
            m_InstructionActive = show;
            if (instructionPanel != null)
            {
                instructionPanel.SetActive(show);
            }

            if (show)
            {
                Time.timeScale = 0f;
                PopulateInstructionContent();
            }
            else
            {
                Time.timeScale = 1f;
            }
        }

        void PopulateInstructionContent()
        {
            int lvl = LevelController.Instance != null ? LevelController.Instance.LevelNumber : 1;
            if (lvl == 1)
            {
                if (instructionLevelTitle != null) instructionLevelTitle.text = "LEVEL 1: ENCHANTED FOREST";
                if (instructionStoryText != null) instructionStoryText.text = "You are Sir Gareth, champion of the realm. Shadows have fallen upon the sacred forest. Traverse ancient ruins, defeat hostile prowlers, and breach the forest gate!";
                if (instructionObjectiveText != null) instructionObjectiveText.text = "OBJECTIVES:\n• Defeat goblin and skeleton guards\n• Collect shiny ancient coins\n• Reach the Golden Forest Portal";
            }
            else
            {
                if (instructionLevelTitle != null) instructionLevelTitle.text = "LEVEL 2: DARK CASTLE";
                if (instructionStoryText != null) instructionStoryText.text = "The trail has led to the Dark Castle citadel. Inside lurks the feared Alpha Wolf Boss guarding the stolen crown. Steel your blade, hero!";
                if (instructionObjectiveText != null) instructionObjectiveText.text = "OBJECTIVES:\n• Brave treacherous platforms and spiked hazards\n• Vanquish the Citadel Boss\n• Claim the Royal Victory Door";
            }

            if (instructionControlsText != null)
            {
                instructionControlsText.text = 
                    "CONTROLS (Input Actions):\n" +
                    "• Move: [A] / [D] or Arrow Keys\n" +
                    "• Jump: [Space]\n" +
                    "• Melee Slash: [E] or [Enter]\n" +
                    "• Ranged Attack: [R]\n" +
                    "• Sprint: [Left Shift]\n" +
                    "• Pause / Menu: [Escape]";
            }
        }

        void OnInstructionContinue()
        {
            ShowInstructions(false);
        }

        void OnInstructionBack()
        {
            Time.timeScale = 1f;
            LevelController.Instance?.LoadMainMenu();
        }

        public void TogglePause()
        {
            m_IsPaused = !m_IsPaused;
            if (pauseMenuRoot != null)
            {
                pauseMenuRoot.SetActive(m_IsPaused);
            }
            Time.timeScale = m_IsPaused ? 0f : 1f;
        }

        public void ResumeGame()
        {
            m_IsPaused = false;
            if (pauseMenuRoot != null)
            {
                pauseMenuRoot.SetActive(false);
            }
            Time.timeScale = 1f;
        }

        public void ShowGameOver(bool show)
        {
            if (gameOverRoot != null)
            {
                gameOverRoot.SetActive(show);
            }
            if (show)
            {
                Time.timeScale = 0f;
            }
        }

        public void ShowLevelComplete(bool show)
        {
            if (levelCompleteRoot != null)
            {
                levelCompleteRoot.SetActive(show);
            }

            if (show)
            {
                Time.timeScale = 0f;
                if (levelCompleteTitle != null) levelCompleteTitle.text = "LEVEL CLEAR!";
                if (levelCompleteStats != null && LevelController.Instance != null)
                {
                    levelCompleteStats.text = $"Coins Gathered: {LevelController.Instance.CurrentCoins}\nTotal Gold: {GameProgress.TotalCoins}";
                }
            }
        }

        public void ShowVictory(bool show)
        {
            if (levelCompleteRoot != null)
            {
                levelCompleteRoot.SetActive(show);
            }

            if (show)
            {
                Time.timeScale = 0f;
                if (levelCompleteTitle != null) levelCompleteTitle.text = "VICTORY! REALM SAVED!";
                if (levelCompleteStats != null && LevelController.Instance != null)
                {
                    levelCompleteStats.text = $"Congratulations, Brave Knight!\nTotal Coins Collected: {GameProgress.TotalCoins}\nThe kingdom celebrates your courage!";
                }
                if (nextLevelButton != null)
                {
                    nextLevelButton.gameObject.SetActive(false);
                }
            }
        }
    }
}

