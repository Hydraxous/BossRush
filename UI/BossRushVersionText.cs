using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace BossRush.UI
{
    public class BossRushVersionText : MonoBehaviour
    {
        private Text text;

        private void Awake()
        {
            text = GetComponent<Text>();
        }

        private void OnEnable()
        {
            text.text = $"{ConstInfo.NAME} - {ConstInfo.VERSION} - {((BossRush.LatestVersion) ? "(<color=green>Latest</color>)" : "(<color=red>Update Available</color>)")}";
        }
    }
}
