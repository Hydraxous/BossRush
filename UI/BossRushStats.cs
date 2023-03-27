using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI;

namespace BossRush.UI
{
    public class BossRushStats : MonoBehaviour
    {
        [SerializeField] Text timeText, lapsText, modeText, deathText;
        [SerializeField] Transform container, deathCounter, modeDisplay;

        internal static GameObject LevelStatsUI;

        private bool uiOpen;

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            uiOpen = CheckOpenState();

            container?.gameObject?.SetActive(uiOpen);

            if(!uiOpen)
            {
                return;
            }

            UpdateLapText();
            UpdateTimeText();
            UpdateThirdStat();
        }

        private bool CheckOpenState()
        {
            if (LevelStatsUI == null)
                return false;

            if (!BossRushController.BossRushMode)
                return false;

            if (BossRushConfig.AlwaysShowStats.Value)
                return true;

            return LevelStatsUI.gameObject.activeInHierarchy;
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
            int hours = (int) Modulate(ref timeInSeconds, 3600);
            int minutes = (int) Modulate(ref timeInSeconds, 60);
            float seconds = timeInSeconds;

            return $"{hours.ToString("00")}:{minutes.ToString("00")}:{(seconds.ToString("00.00"))}";
        }

        private float Modulate(ref float number, float modulationAmount)
        {
            float modulation = number % modulationAmount;

            float deduction = (number-modulation)/modulationAmount;

            number -= (deduction*modulationAmount);

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
