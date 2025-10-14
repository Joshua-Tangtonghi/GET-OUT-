using UnityEngine;

public class UiCredits : MonoBehaviour
{
    private Animator cAnim;

    private void Awake()
    {
        cAnim = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        cAnim.SetBool("Credits", true);
    }
}
