using UnityEngine;

public class UiEye : MonoBehaviour
{
    private Animator eAnim;

    private void Awake()
    {
        eAnim = GetComponent<Animator>();
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
        eAnim.SetBool("End", win);
    }
}
