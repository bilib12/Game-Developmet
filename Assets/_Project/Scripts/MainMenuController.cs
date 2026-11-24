using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace KnightsQuest
{
    public class MainMenuController : MonoBehaviour
    {
        [Header("Main Panels")]
        [SerializeField] GameObject mainOptionsPanel;
        [SerializeField] GameObject levelSelectPanel;
        [SerializeField] GameObject controlsPanel;

        [Header("Main Menu Buttons")]
        [SerializeField] Button playButton;
        [SerializeField] Button levelSelectButton;
        [SerializeField] Button controlsButton;
        [SerializeField] Button exitButton;

        [Header("Level Select Buttons")]
        [SerializeField] Button level1Button;
        [SerializeField] Button level2Button;
        [SerializeField] Button backFromLevelSelectButton;
        [SerializeField] TextMeshProUGUI level2LockedText;

        [Header("Controls Panel")]
        [SerializeField] Button backFromControlsButton;
        [SerializeField] TextMeshProUGUI controlsBodyText;

        [Header("Progression Display")]
        [SerializeField] TextMeshProUGUI totalGoldText;

        void Start()
        {
            Time.timeScale = 1f;
            ShowMainOptions();
            SetupListeners();
            RefreshProgression();
        }

        void SetupListeners()
        {
            if (playButton != null) playButton.onClick.AddListener(OnPlayClicked);
            if (levelSelectButton != null) levelSelectButton.onClick.AddListener(ShowLevelSelect);
            if (controlsButton != null) controlsButton.onClick.AddListener(ShowControls);
            if (exitButton != null) exitButton.onClick.AddListener(OnExitClicked);

            if (level1Button != null) level1Button.onClick.AddListener(() => LoadLevel("Level1_EnchantedForest"));
            if (level2Button != null) level2Button.onClick.AddListener(() => LoadLevel("Level2_DarkCastle"));
            if (backFromLevelSelectButton != null) backFromLevelSelectButton.onClick.AddListener(ShowMainOptions);

            if (backFromControlsButton != null) backFromControlsButton.onClick.AddListener(ShowMainOptions);
        }

        void RefreshProgression()
        {
            int unlocked = GameProgress.UnlockedLevel;

            if (level2Button != null)
            {
                bool isUnlocked = unlocked >= 2;
                level2Button.interactable = isUnlocked;

                if (level2LockedText != null)
                {
                    level2LockedText.gameObject.SetActive(!isUnlocked);
                }
            }

            if (totalGoldText != null)
            {
                totalGoldText.text = $"Gold Collected: {GameProgress.TotalCoins}";
            }

            if (controlsBodyText != null)
            {
                controlsBodyText.text = 
                    "KNIGHT'S CONTROLS:\n\n" +
                    "• Movement: [A] / [D] or Left / Right Arrows\n" +
                    "• Jump: [Space]\n" +
                    "• Melee Slash: [E] or [Enter]\n" +
                    "• Ranged Attack: [R]\n" +
                    "• Sprint: [Left Shift]\n" +
                    "• Pause / In-Game Menu: [Escape]";
            }
        }

        void ShowMainOptions()
        {
            if (mainOptionsPanel != null) mainOptionsPanel.SetActive(true);
            if (levelSelectPanel != null) levelSelectPanel.SetActive(false);
            if (controlsPanel != null) controlsPanel.SetActive(false);
        }

        void ShowLevelSelect()
        {
            RefreshProgression();
            if (mainOptionsPanel != null) mainOptionsPanel.SetActive(false);
            if (levelSelectPanel != null) levelSelectPanel.SetActive(true);
            if (controlsPanel != null) controlsPanel.SetActive(false);
        }

        void ShowControls()
        {
            if (mainOptionsPanel != null) mainOptionsPanel.SetActive(false);
            if (levelSelectPanel != null) levelSelectPanel.SetActive(false);
            if (controlsPanel != null) controlsPanel.SetActive(true);
        }

        void OnPlayClicked()
        {
            // Default Play starts Chapter 1 for a new journey
            LoadLevel("Level1_EnchantedForest");
        }

        void LoadLevel(string sceneName)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(sceneName);
        }

        void OnExitClicked()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}

