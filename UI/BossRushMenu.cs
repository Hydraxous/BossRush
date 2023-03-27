using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace BossRush.UI
{
    public class BossRushMenu : MonoBehaviour
    {
        public static BossRushMenu Instance;


        [SerializeField] GameObject menuScreen;
        [SerializeField] Text hardcoreModeText;

        private bool hardcoreMode;

        private static GameObject lastPage;
        private MenuEsc menuEsc;

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            menuEsc = menuScreen?.AddComponent<MenuEsc>();
        }

        public void StartBossRush()
        {
            BossRushController.StartBossRushMode(hardcoreMode);
        }

        public void ToggleHardcoreMode()
        {
            hardcoreMode = !hardcoreMode;
            UpdateHardcoreText();
        }

        public static void SetLastPage(GameObject chapter)
        {
            lastPage = chapter;
        }

        public void OpenMenu()
        {
            lastPage?.SetActive(false);

            Refresh();
            if(!onViolent)
            {
                StartBossRush();
            }
            else
            {
                menuScreen?.SetActive(true);
            }
        }

        public static void Open()
        {
            if (Instance == null)
            {
                Debug.LogError("BR: Menu is null.");
                return;
            }

            Instance.OpenMenu();
        }

        private bool onViolent;

        public void Refresh()
        {
            if(lastPage != null)
            {
                menuEsc.previousPage = lastPage;
            }

            onViolent = (PrefsManager.Instance.GetInt("difficulty") >= 3);

            if(!onViolent)
            {
                hardcoreMode = false;
            }

            UpdateHardcoreText();
        }

        private void UpdateHardcoreText()
        {
            if (hardcoreModeText == null)
                return;

            hardcoreModeText.color = (hardcoreMode) ? Color.red : new Color(0.3f,0f,0f,1.0f);
        }
    }
}
