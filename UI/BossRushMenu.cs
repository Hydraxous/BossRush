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
            menuEsc = menuScreen?.AddComponent<MenuEsc>();
            hardcoreMode = BossRushController.HardcoreMode;
            menuScreen?.SetActive(false);
        }

        //Linked to Boss Rush UI in menu
        public void StartBossRush()
        {
            BossRushController.StartBossRushMode(hardcoreMode);
        }

        //Linked to the hardcore mode button in the menu
        public void ToggleHardcoreMode()
        {
            hardcoreMode = !hardcoreMode;
            UpdateHardcoreText();
        }

        //Used for closing the UI when pressing ESCAPE
        public static void SetLastPage(GameObject chapter)
        {
            lastPage = chapter;
        }

        public void OpenMenu()
        {
            lastPage?.SetActive(false);

            Refresh();
                
            menuScreen?.SetActive(true);
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


        //Updates fields and displayed information also detects current difficulty
        public void Refresh()
        {
            if (lastPage != null)
                menuEsc.previousPage = lastPage;

            UpdateHardcoreText();
        }

        //Visual indicator of hardcore mode
        private void UpdateHardcoreText()
        {
            if (hardcoreModeText == null)
                return;

            hardcoreModeText.color = (hardcoreMode) ? Color.red : new Color(0.3f, 0f, 0f, 1.0f);
        }
    }
}
