using UnityEngine;
using UnityEngine.UI;

public class HealthCanvasController : MonoBehaviour
{
    private Image _health;

    private void Start()
    {
        _health = GetComponent<Image>();
    }

    public void UpdateHealthBar(float currAmt, float maxAmt)
    {
        _health.fillAmount = currAmt * maxAmt / 10000f;
    }
}