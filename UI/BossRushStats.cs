using UnityEngine;
using UnityEngine.UI;

namespace BossRush.UI
{
    public class BossRushStats : MonoBehaviour
    {
        [SerializeField] Text timeText, lapsText, modeText, deathText;
        [SerializeField] Transform container, deathCounter, modeDisplay;

        internal static GameObject UK_LevelStatsObject;

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            bool displayStatsUI = CheckOpenState();

            container?.gameObject?.SetActive(displayStatsUI);

            if (!displayStatsUI)
                return;
            
            UpdateLapText();
            UpdateTimeText();
            UpdateThirdStat();
        }

        //Should the stats UI be displayed?
        private bool CheckOpenState()
        {
            if (UK_LevelStatsObject == null) //Probably not in a level, this should become filled when we load into a level.
                return false;

            if (!BossRushController.BossRushMode)
                return false;

            if (BossRushConfig.AlwaysShowStats.Value)
                return true;

            return UK_LevelStatsObject.gameObject.activeInHierarchy;
        }

        private void UpdateTimeText()
        {
            if (timeText == null)
                return;

            timeText.text = GetTimeString(BossRushController.TimeElapsed);
        }

        private void UpdateLapText()
        {
            if (lapsText == null)
                return;

            lapsText.text = BossRushController.Laps.ToString("000");
        }

        //Shows either death counter or hardcore mode depending on which mode is being played.
        private void UpdateThirdStat()
        {
            bool hardcore = BossRushController.HardcoreMode;

            deathCounter?.gameObject?.SetActive(!hardcore);
            modeDisplay?.gameObject?.SetActive(hardcore);

            if (hardcore)
            {
                if (modeText == null)
                    return;

                modeText.text = (!hardcore) ? "<color=orange>STANDARD</color>" : "<color=red>HARDCORE</color>";
            }
            else
            {
                if (deathText == null)
                    return;

                deathText.text = BossRushController.Deaths.ToString("000");
            }
        }

        private string GetTimeString(float timeInSeconds)
        {
            int hours = (int)Modulate(ref timeInSeconds, 3600);
            int minutes = (int)Modulate(ref timeInSeconds, 60);
            float seconds = timeInSeconds;

            return $"{hours.ToString("00")}:{minutes.ToString("00")}:{(seconds.ToString("00.00"))}";
        }

        private float Modulate(ref float number, float modulationAmount)
        {
            float modulation = number % modulationAmount;
            float deduction = (number - modulation) / modulationAmount;
            number -= (deduction * modulationAmount);

            return deduction;
        }

        public static void Spawn()
        {
            if (Assets.BossRushStatsPrefab == null)
                return;

            Instantiate(Assets.BossRushStatsPrefab);
        }

    }
}
