using UnityEngine;

public class UiEye : MonoBehaviour
{
    private Animator eAnim;

    private void Awake()
    {
        eAnim = GetComponentInChildren<Animator>();
    }
    public void EyeMood(int i)
    {
        eAnim.SetInteger("Mood", i);
    }
    public void NeutralEye()
    {
        EyeMood(0);
    }
    public void HappyEye()
    {
        EyeMood(1);
    }
    public void SusEye()
    {
        EyeMood(-1);
    }
    public void EndingEye(bool win)
    {
        eAnim.SetBool("End", true);
        if (win)
            eAnim.SetInteger("Mood", 2);
        else 
            eAnim.SetInteger("Mood", -2);
    }
}
