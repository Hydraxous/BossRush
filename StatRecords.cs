using Newtonsoft.Json;
using System;
using System.IO;

namespace BossRush
{
    public static class StatRecords
    {
        private static RecordLedger ledger;

        public delegate void OnNewScoreHandler(BossRushRecord newScore);
        public static OnNewScoreHandler OnNewHighScore, OnNewScoreSubmitted;
        public static bool HighScoreRun;
        private static bool legderDirty = false;

        public static void MarkDirty()
        {
            legderDirty = true;
        }

        public static void LoadRecords()
        {
            string filePath = BossRushPaths.BossRushSaveFile;
            if (!File.Exists(filePath))
            {
                BossRush.BepInExLogger.LogInfo("No BossRush Stats found. Creating new file.");
                ledger = new RecordLedger(new BossRushRecord[6]);
                SaveInternal();
                return;
            }
            else
            {
                BossRush.BepInExLogger.LogInfo("Loading records.");
                try
                {
                    string json = File.ReadAllText(filePath);
                    ledger = JsonConvert.DeserializeObject<RecordLedger>(json);

                    if(ledger == null || ledger.Records == null || ledger.Records.Length < 6)
                    {
                        throw new Exception("Invalid records file. Corrupt?");
                    }

                    legderDirty = false;
                    BossRush.BepInExLogger.LogInfo("Loaded BossRush Stats.");
                }
                catch (Exception e)
                {
                    BossRush.BepInExLogger.LogFatal("Failed to load BossRush Stats.");
                    BossRush.BepInExLogger.LogFatal(e.Message + '\n' + e.StackTrace);


                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
                    string extension = ".json";

                    int i = 0;
                    while (File.Exists(Path.Combine(Path.GetDirectoryName(filePath), fileNameWithoutExtension + "_corrupted" + i + extension)))
                    {
                        i++;
                    }

                    string corruptedFilePath = Path.Combine(Path.GetDirectoryName(filePath), fileNameWithoutExtension + "_corrupted" + i + extension);
                    
                    File.Move(filePath, corruptedFilePath);
                    BossRush.BepInExLogger.LogError($"File renamed to allow for saving records.");
                    ledger = new RecordLedger(new BossRushRecord[6]);
                    SaveInternal();
                }

            }
        }

        public static BossRushRecord GetRecord(int difficulty = -1)
        {
            if(difficulty < 0)
                difficulty = PrefsManager.Instance.GetInt("difficulty");
            return ledger.Records[difficulty];
        }

        public static void SubmitRecord(BossRushRecord newRecord, int difficulty = -1)
        {
            if(difficulty < 0)
                difficulty = PrefsManager.Instance.GetInt("difficulty");

            BossRush.BepInExLogger.LogInfo("Submitting new record.");

            BossRushRecord currentRecord = GetRecord(difficulty);

            BossRush.BepInExLogger.LogInfo("Old Record " + currentRecord);
            BossRush.BepInExLogger.LogInfo("New Record " + newRecord);

            if (!newRecord.IsBetterThan(currentRecord))
            {
                BossRush.BepInExLogger.LogInfo("Record not sufficient for replacement.");
                return;
            }

            if(!newRecord.ValidRecord())
            {
                BossRush.BepInExLogger.LogInfo("Record invalid, discarding.");
                return;
            }

            ledger.Records[difficulty] = newRecord;
            HighScoreRun = true;
            MarkDirty();
            Save();
            OnNewHighScore?.Invoke(newRecord);
        }

        private static void SaveInternal()
        {
            string filePath = BossRushPaths.BossRushSaveFile;

            if (!Directory.Exists(Path.GetDirectoryName(filePath)))
            {
                BossRush.BepInExLogger.LogInfo("Creating BossRush Save Directory.");
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            }

            try
            {
                string json = JsonConvert.SerializeObject(ledger);
                File.WriteAllText(filePath, json);
                BossRush.BepInExLogger.LogInfo("Saved BossRush Stats.");
                legderDirty = false;
            }
            catch (Exception e)
            {
                BossRush.BepInExLogger.LogError("Failed to save BossRush Stats.");
                BossRush.BepInExLogger.LogFatal(e.Message + '\n' + e.StackTrace);
            }
        }

        public static void Save()
        {
            if(!legderDirty)
            {
                return;
            }

            SaveInternal();
        }
    }

    [Serializable]
    public class RecordLedger
    {
        public BossRushRecord[] Records;
        
        public RecordLedger()
        {
            Records = new BossRushRecord[6];
        }

        public RecordLedger(BossRushRecord[] records) 
        {
            Records = records;
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
