using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace BossRush.UI
{
    public class BossRushDeathScreen : MonoBehaviour
    {
        public static BossRushDeathScreen Instance { get; private set; }
        [SerializeField] private GameObject screen, buttons, highScoreFlair;
        [SerializeField] private Text lapText, timeText, deathText;
        private bool open, finished, skip, canSkip;

        private void Awake()
        {
            Instance = this;
            Reset();
        }


        public void Update()
        {
            if (!open)
                return;

            if(Input.anyKeyDown && canSkip)
            {
                if(!finished)
                {
                    skip = true;
                }
            }

            if(finished)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                    Quit();
            }
        }

        private IEnumerator DisplayStats()
        {
            BossRushRecord current = StatRecords.GetRecord();

            lapText.text = "";
            timeText.text = "";
            deathText.text = "";

            int currentLaps = 0;
            float lapTime = 1.5f / current.laps;
            while(currentLaps < current.laps+1 && !skip)
            {
                lapText.text = currentLaps.ToString("000");
                yield return new WaitForSecondsRealtime(lapTime);
                currentLaps++;
            }

            canSkip = true;

            lapText.text = current.laps.ToString("000");

            float timer = 1.5f;
            float time = 0;
            while(timer > 0.0f && !skip)
            {
                time = Mathf.Lerp(0,current.time, 1-(timer/1.5f));
                timer -= 0.016f;
                yield return new WaitForSecondsRealtime(0.016f);
                timeText.text = StringHelper.GetTimeString(time);
            }

            timeText.text = StringHelper.GetTimeString(current.time);


            int currentDeaths = 0;
            float deathTime = 1.5f/((current.deaths == 0)? Mathf.Epsilon : current.deaths);
            while (currentDeaths < current.deaths && !skip)
            {
                deathText.text = currentDeaths.ToString("000");
                yield return new WaitForSecondsRealtime(deathTime);
                currentDeaths++;
            }

            deathText.text = current.deaths.ToString("000");

            if (!skip)
                yield return new WaitForSecondsRealtime(0.5f);

            highScoreFlair.SetActive(StatRecords.HighScoreRun);

            if (!skip)
                yield return new WaitForSecondsRealtime(0.5f);

            finished = true;
            buttons.SetActive(true);
        }

        public void Retry()
        {
            Time.timeScale = 1;
            Reset();
            BossRushController.StartBossRushMode(true);
        }

        public void Quit()
        {
            Reset();
            OptionsManager.Instance.QuitMission();
        }

        public void Open()
        {
            if (open)
                return;

            GameState endScreen = new GameState("br_deathscreen", screen);
            endScreen.cursorLock = LockMode.Unlock;
            endScreen.playerInputLock = LockMode.Lock;
            endScreen.cameraInputLock = LockMode.Lock;
            endScreen.priority = 10;
            GameStateManager.Instance.RegisterState(endScreen);

            Time.timeScale = 0;
            open = true;
            screen?.SetActive(true);
            StartCoroutine(DisplayStats());
        }

        private void Reset()
        {
            if(open)
                GameStateManager.Instance.PopState("br_deathscreen");

            open = false;
            finished = false;
            skip = false;
            buttons.SetActive(false);
            screen.SetActive(false);
            highScoreFlair.SetActive(false);
        }
    }
}
