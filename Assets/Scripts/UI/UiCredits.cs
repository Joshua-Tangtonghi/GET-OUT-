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
            StartCoroutine(EYE());
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
            winAnim.SetBool("End", false);
        }
        public void Lose()
        {
            cAnim.SetTrigger("Lose");
            loseAnim.SetBool("End", false);
        }

        IEnumerator CreditsFalse()
        {
            yield return new WaitForSeconds(5f);
            cAnim.SetBool("Credits", false);
        }
        IEnumerator EYE()
        {
            yield return new WaitForSeconds(13f);
            if (GameData.win)
            {
                Win();
                yield return new WaitForSeconds(0.5f);
                winAnim.SetBool("End", true);
                winAnim.SetInteger("Mood", 2);
            }
            else
            {
                Lose();
                yield return new WaitForSeconds(0.5f);
                loseAnim.SetBool("End", true);
                loseAnim.SetInteger("Mood", -2);
            }
        }
    }
}