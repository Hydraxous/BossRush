using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace BossRush.UI
{
    public class HighScoreBox : MonoBehaviour
    {
        [SerializeField] private Text lapText, timeText, deathsText;
        [SerializeField] private GameObject hardcoreLabel, subcontainer;

        private void Refresh()
        {
            int difficulty = PrefsManager.Instance.GetInt("difficulty");

            BossRushRecord record = StatRecords.GetRecord(difficulty);

            if(!record.ValidRecord())
            {
                lapText.text = "";
                timeText.text = "";
                deathsText.text = "";
                hardcoreLabel.SetActive(false);
                subcontainer.SetActive(false);
                return;
            }

            lapText.text = record.laps.ToString("000");
            timeText.text = StringHelper.GetTimeString(record.time);
            deathsText.text = record.deaths.ToString("000");
            deathsText.gameObject.SetActive(!record.hardcore);
            hardcoreLabel.SetActive(record.hardcore);
            subcontainer.SetActive(true);
        }

        

        private void OnEnable()
        {
            Refresh();
        }
    }
}
