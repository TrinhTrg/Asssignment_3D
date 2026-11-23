using UnityEngine;
using Unity.FPS.Game;

namespace Unity.FPS.UI
{
    public class RunResultUI : MonoBehaviour
    {
        [SerializeField] TMPro.TextMeshProUGUI resultScore;
        void Start()
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            if (resultScore != null)
                resultScore.SetText("SCORE {0}  |  ROBOTS {1} / 3", RunScoreManager.LastRunScore, RunScoreManager.LastRunKills);
        }
    }
}
