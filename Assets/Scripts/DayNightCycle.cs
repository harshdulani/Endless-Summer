using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public static bool isDayActiveLight = false, isNightActiveLight = true;

    private Animator _anim;
    private static readonly int HasGameStarted = Animator.StringToHash("hasGameStarted");

    private void Start()
    {
        _anim = GetComponent<Animator>();
        isDayActiveLight = false;
        isNightActiveLight = true;
    }

    public void StartAnimations()
    {
        _anim.SetBool(HasGameStarted, true);
    }

    public void NightLightFarAway()
    {
        isNightActiveLight = false;
        isDayActiveLight = true;
        GameController.score++;
        MainMenuController.menu.UpdateScore();
    }

    public void NightLightActive()
    {
        isNightActiveLight = true;
        isDayActiveLight = false;
    }
}
