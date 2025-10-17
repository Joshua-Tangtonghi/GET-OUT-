using System.Collections;
using UnityEngine;

namespace _project.Scripts.UiManagers
{
    
    public class UiCredits : MonoBehaviour
    {
        private Animator cAnim;
        public GameObject gWin;
        public GameObject gLose;
        private Animator winAnim;
        private Animator loseAnim;

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
            StartCoroutine(CreditsFalse());
        }
        private void Awake()
        {
            cAnim = GetComponent<Animator>();
            winAnim = gWin.GetComponentInChildren<Animator>();
            loseAnim = gLose.GetComponentInChildren<Animator>();
        }
        private void OnEnable()
        {
            cAnim.SetBool("Credits", true);
        }
        public void Win()
        {
            cAnim.SetTrigger("Win");
            gWin.SetActive(true);
            winAnim.SetInteger("Mood",1);
            winAnim.SetBool("End",false);
        }
        public void Lose()
        {
            cAnim.SetTrigger("Lose");
            gLose.SetActive(true);
            winAnim.SetInteger("Mood", -1);
            loseAnim.SetBool("End", false);
        }

        IEnumerator CreditsFalse()
        {
           yield return new WaitForSeconds(5f);
            cAnim.SetBool("Credits", false);
        }
    }
    
}