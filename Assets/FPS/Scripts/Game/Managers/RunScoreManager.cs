using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.FPS.Game
{
    public class RunScoreManager : MonoBehaviour
    {
        [SerializeField] int pointsPerEnemy = 100;
        [SerializeField] int targetEnemies = 3;
        readonly HashSet<int> m_KilledEnemies = new HashSet<int>();
        public int Score { get; private set; }
        public int Kills => m_KilledEnemies.Count;
        public int TargetEnemies => targetEnemies;
        public event Action Changed;
        public static int LastRunScore { get; private set; }
        public static int LastRunKills { get; private set; }
        public static bool LastRunWon { get; private set; }
        bool m_Ended;

        void Awake()
        {
            LastRunScore = 0;
            LastRunKills = 0;
            LastRunWon = false;
            Score = 0;
        }

        void OnEnable() => EventManager.AddListener<EnemyKillEvent>(OnEnemyKilled);
        void OnDisable() => EventManager.RemoveListener<EnemyKillEvent>(OnEnemyKilled);

        void OnEnemyKilled(EnemyKillEvent evt)
        {
            if (m_Ended || evt.Enemy == null || !m_KilledEnemies.Add(evt.Enemy.GetInstanceID())) return;
            Score += pointsPerEnemy;
            Changed?.Invoke();
        }

        public void CompleteRun(bool won)
        {
            if (m_Ended) return;
            m_Ended = true;
            LastRunScore = Score;
            LastRunKills = Kills;
            LastRunWon = won;
        }
    }
}
