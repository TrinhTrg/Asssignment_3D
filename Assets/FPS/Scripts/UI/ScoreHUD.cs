using UnityEngine;
using Unity.FPS.Game;

namespace Unity.FPS.UI
{
    public class ScoreHUD : MonoBehaviour
    {
        [SerializeField] RunScoreManager score;
        [SerializeField] Health playerHealth;
        [SerializeField] TMPro.TextMeshProUGUI scoreText;
        [SerializeField] TMPro.TextMeshProUGUI progressText;
        [SerializeField] TMPro.TextMeshProUGUI healthText;

        void OnEnable()
        {
            if (score != null) score.Changed += RefreshScore;
        }
        void Start() => RefreshScore();
        void OnDisable()
        {
            if (score != null) score.Changed -= RefreshScore;
        }
        void RefreshScore()
        {
            if (score == null) return;
            if (scoreText != null) scoreText.SetText("SCORE {0}", score.Score);
            if (progressText != null) progressText.SetText("ROBOTS {0} / {1}", score.Kills, score.TargetEnemies);
        }
        void Update()
        {
            if (healthText != null && playerHealth != null)
                healthText.SetText("HP {0} / {1}", Mathf.CeilToInt(playerHealth.CurrentHealth), playerHealth.MaxHealth);
        }
    }
}
