using System.Collections;
using System.Collections.Generic;
using Blocks.Character;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KnightsQuest
{
    public class LevelController : MonoBehaviour
    {
        public static LevelController Instance { get; private set; }

        [Header("Level Settings")]
        [SerializeField] int levelNumber = 1;
        [SerializeField] string levelName = "Enchanted Forest";
        [SerializeField] bool isFinalLevel = false;

        [Header("Objectives")]
        [SerializeField] int targetCoins = 5;
        [SerializeField] List<BuildingBlocksCharacter> requiredBosses = new();

        [Header("References")]
        [SerializeField] BuildingBlocksCharacter playerCharacter;
        [SerializeField] Transform initialSpawnPoint;

        public int CurrentCoins { get; private set; }
        public int LevelNumber => levelNumber;
        public string LevelName => levelName;
        public int TargetCoins => targetCoins;
        public bool IsFinalLevel => isFinalLevel;

        Vector3 m_ActiveCheckpointPos;
        bool m_HasCheckpoint;
        bool m_IsGameOver;
        bool m_IsLevelComplete;

        void Awake()
        {
            Instance = this;
            if (initialSpawnPoint != null)
            {
                m_ActiveCheckpointPos = initialSpawnPoint.position;
                m_HasCheckpoint = true;
            }
            else if (playerCharacter != null)
            {
                m_ActiveCheckpointPos = playerCharacter.transform.position;
                m_HasCheckpoint = true;
            }
        }

        void OnEnable()
        {
            CollectibleCoin.OnCoinCollected += HandleCoinCollected;
            Checkpoint.OnCheckpointActivated += HandleCheckpointActivated;

            if (playerCharacter != null)
            {
                playerCharacter.OnEliminated += HandlePlayerEliminated;
                playerCharacter.OnVictory += HandlePlayerVictory;
            }
        }

        void OnDisable()
        {
            CollectibleCoin.OnCoinCollected -= HandleCoinCollected;
            Checkpoint.OnCheckpointActivated -= HandleCheckpointActivated;

            if (playerCharacter != null)
            {
                playerCharacter.OnEliminated -= HandlePlayerEliminated;
                playerCharacter.OnVictory -= HandlePlayerVictory;
            }
        }

        void HandleCoinCollected(int amount)
        {
            CurrentCoins += amount;
            GameProgress.TotalCoins += amount;
        }

        void HandleCheckpointActivated(Checkpoint cp)
        {
            m_ActiveCheckpointPos = cp.transform.position;
            m_HasCheckpoint = true;
        }

        void HandlePlayerEliminated()
        {
            if (m_IsGameOver || m_IsLevelComplete) return;

            // If player does not automatically respawn, trigger game over
            if (!playerCharacter.RespawnsAfterElimination)
            {
                m_IsGameOver = true;
                StartCoroutine(TriggerGameOverDelayed(1.2f));
            }
            else
            {
                // When character respawns, move to checkpoint
                StartCoroutine(MovePlayerToCheckpointOnRespawn());
            }
        }

        IEnumerator MovePlayerToCheckpointOnRespawn()
        {
            yield return null;
            if (m_HasCheckpoint && playerCharacter != null)
            {
                playerCharacter.transform.position = m_ActiveCheckpointPos;
            }
        }

        IEnumerator TriggerGameOverDelayed(float delay)
        {
            yield return new WaitForSeconds(delay);
            if (GameHUD.Instance != null)
            {
                GameHUD.Instance.ShowGameOver(true);
            }
        }

        void HandlePlayerVictory()
        {
            if (m_IsLevelComplete) return;
            m_IsLevelComplete = true;

            // Unlock next level in progression
            GameProgress.UnlockedLevel = levelNumber + 1;

            StartCoroutine(ShowVictoryFlow(1.5f));
        }

        IEnumerator ShowVictoryFlow(float delay)
        {
            yield return new WaitForSeconds(delay);

            if (GameHUD.Instance != null)
            {
                if (isFinalLevel)
                {
                    GameHUD.Instance.ShowVictory(true);
                }
                else
                {
                    GameHUD.Instance.ShowLevelComplete(true);
                }
            }
        }

        public void RestartLevel()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void LoadNextLevel()
        {
            Time.timeScale = 1f;
            if (levelNumber == 1)
            {
                SceneManager.LoadScene("Level2_DarkCastle");
            }
            else
            {
                SceneManager.LoadScene("MainMenu");
            }
        }

        public void LoadMainMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu");
        }
    }
}

