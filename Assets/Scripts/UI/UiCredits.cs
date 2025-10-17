using UnityEngine;

namespace _project.Scripts.UiManagers
{
    
    public class UiCredits : MonoBehaviour
    {
        private Animator cAnim;
        public GameObject gWin;
        public GameObject gLose;

        private void Start()
        {
            if (GameData.win)
            {
                Win();
            }
            else
            {
                Lose();
            }
        }
        private void Awake()
        {
            cAnim = GetComponent<Animator>();
        }
        private void OnEnable()
        {
            cAnim.SetBool("Credits", true);
        }
        public void Win()
        {
            cAnim.SetTrigger("Win");
            gWin.SetActive(true);
            gLose.SetActive(false);
            new WaitForSeconds(cAnim.GetCurrentAnimatorStateInfo(0).length);
        }
        public void Lose()
        {
            cAnim.SetTrigger("Lose");
            gWin.SetActive(true );
            gLose.SetActive(false);
        }
    }
    
}