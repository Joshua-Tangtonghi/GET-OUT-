using UnityEngine;
using UnityEngine.UI;


namespace DefaultNamespace
{
    public class StartMenu: MonoBehaviour 
    {
        public Button button;

        private void Start()
        {
            button.onClick.AddListener(OnClick);
        }
        private void OnClick()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("UI_MainScene");
        }
    }
}