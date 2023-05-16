using HydraDynamics.DataPersistence;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BossRush
{
    public static class StatRecords
    {
        private static DataFile<RecordLedger> records = new DataFile<RecordLedger>(new RecordLedger(), "records.hyd");

        public delegate void OnNewScoreHandler(BossRushRecord newScore);
        public static OnNewScoreHandler OnNewHighScore, OnNewScoreSubmitted;
        public static bool HighScoreRun;

        public static BossRushRecord GetRecord(int difficulty = -1)
        {
            if(difficulty < 0)
                difficulty = PrefsManager.Instance.GetInt("difficulty");
            return records.Data.Records[difficulty];
        }

        public static void SubmitRecord(BossRushRecord newRecord, int difficulty = -1)
        {
            if(difficulty < 0)
                difficulty = PrefsManager.Instance.GetInt("difficulty");

            BossRush.Logr.LogInfo("Submitting new record.");

            BossRushRecord currentRecord = GetRecord(difficulty);

            BossRush.Logr.LogInfo("Old Record " + currentRecord);
            BossRush.Logr.LogInfo("New Record " + newRecord);

            if (!newRecord.IsBetterThan(currentRecord))
            {
                BossRush.Logr.LogInfo("BR: Record not sufficient for replacement.");
                return;
            }

            if(!newRecord.ValidRecord())
            {
                BossRush.Logr.LogInfo("BR: Record invalid");
                return;
            }

            BossRush.Logr.LogInfo("BR: New record saved.");
            records.Data.Records[difficulty] = newRecord;
            HighScoreRun = true;
            Save();
            OnNewHighScore?.Invoke(newRecord);
        }

        public static void Save()
        {
            records.Save();
        }
    }

    [Serializable]
    public class RecordLedger : Validatable
    {
        public override bool AllowExternalRead => false;

        public BossRushRecord[] Records;
        
        public RecordLedger()
        {
            Records = new BossRushRecord[6];
        }

        public RecordLedger(BossRushRecord[] records) 
        {
            Records = records;
        }

        public override bool Validate()
        {
            if (Records == null)
                return false;

            if (Records.Length < 6 || Records.Length > 6)
                return false;

            return true;
        }
    }

    [Serializable]
    public struct BossRushRecord
    {
        public int laps = 0;
        public float time = 0.0f;
        public int deaths = 0;
        public bool hardcore;

        public BossRushRecord(int laps, float time, int deaths, bool hardcore)
        {
            this.laps = laps;
            this.time = time;
            this.deaths = deaths;
            this.hardcore = hardcore;
        }

        public BossRushRecord(BossRushRecord record)
        {
            this.laps = record.laps;
            this.time = record.time;
            this.deaths = record.deaths;
            this.hardcore = record.hardcore;
        }       

        public bool IsBetterThan(BossRushRecord record)
        {
            if (laps > record.laps)
                return true;
            

            if (hardcore && !record.hardcore)
                return true;
            

            if (time < record.time && record.ValidRecord())
                return true;
            
            if (time == record.time && deaths < record.deaths)
                return true;
            
            return false;
        }

        public bool ValidRecord()
        {
            if (laps <= 0)
                return false;

            if (time <= 0.0f)
                return false;

            if (deaths < 0)
                return false;

            if (deaths > 0 && hardcore)
                return false;

            return true;
        }

        public override string ToString()
        {
            return $"Laps: {laps} Time: {time} Deaths: {deaths} HC: {hardcore}";
        }
    }
}
