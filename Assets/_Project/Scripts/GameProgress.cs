using UnityEngine;

namespace KnightsQuest
{
    public static class GameProgress
    {
        const string KEY_UNLOCKED_LEVEL = "KQ_UnlockedLevel";
        const string KEY_TOTAL_COINS = "KQ_TotalCoins";

        public static int UnlockedLevel
        {
            get => PlayerPrefs.GetInt(KEY_UNLOCKED_LEVEL, 1);
            set
            {
                if (value > UnlockedLevel)
                {
                    PlayerPrefs.SetInt(KEY_UNLOCKED_LEVEL, value);
                    PlayerPrefs.Save();
                }
            }
        }

        public static int TotalCoins
        {
            get => PlayerPrefs.GetInt(KEY_TOTAL_COINS, 0);
            set
            {
                PlayerPrefs.SetInt(KEY_TOTAL_COINS, Mathf.Max(0, value));
                PlayerPrefs.Save();
            }
        }

        public static void ResetProgress()
        {
            PlayerPrefs.DeleteKey(KEY_UNLOCKED_LEVEL);
            PlayerPrefs.DeleteKey(KEY_TOTAL_COINS);
            PlayerPrefs.Save();
        }
    }
}

