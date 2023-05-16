using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace BossRush.UI
{
    public class BossRushStats : MonoBehaviour
    {
        [SerializeField] Text timeText, lapsText, modeText, deathText;
        [SerializeField] Transform container, deathCounter, modeDisplay;
        [SerializeField] private GameObject newScoreFlash;

        internal static GameObject UK_LevelStatsObject;

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            StatRecords.OnNewHighScore += (_) => FlashNewScore();
            newScoreFlash.SetActive(false);
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
        
        private bool flashing = false;
        private void FlashNewScore()
        {
            if(!flashing)
            {
                flashing = true;
                //play a sound
                StartCoroutine(FlashNewHighScoreText());
            }
        }
        
        private IEnumerator FlashNewHighScoreText()
        {
            newScoreFlash.SetActive(true);
            int flashTimes = 15;
            while(flashTimes > 0)
            {
                newScoreFlash.SetActive(true);
                yield return new WaitForSecondsRealtime(0.25f);
                newScoreFlash.SetActive(false);
                yield return new WaitForSecondsRealtime(0.25f);
                --flashTimes;
            }

            newScoreFlash.SetActive(false);

            flashing = false;
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

            timeText.text = StringHelper.GetTimeString(BossRushController.TimeElapsed);
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

        public static void Spawn()
        {
            if (Assets.BossRushStatsPrefab == null)
                return;

            Instantiate(Assets.BossRushStatsPrefab);
        }

    }
}
