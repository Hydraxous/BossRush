using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BossRush.UI
{
    public static class StringHelper
    {
        public static string GetTimeString(float timeInSeconds)
        {
            int hours = (int)Modulate(ref timeInSeconds, 3600);
            int minutes = (int)Modulate(ref timeInSeconds, 60);
            float seconds = timeInSeconds;

            return $"{hours.ToString("00")}:{minutes.ToString("00")}:{(seconds.ToString("00.00"))}";
        }

        public static float Modulate(ref float number, float modulationAmount)
        {
            float modulation = number % modulationAmount;
            float deduction = (number - modulation) / modulationAmount;
            number -= (deduction * modulationAmount);

            return deduction;
        }

        public static void DebugNull(string name, object obj)
        {
            Debug.Log($"{name} {((obj == null) ? "NULL" : "NOT NULL")}");
        }
    }
}
