using UnityEngine;
using UnityEngine.UI;

public class HealthCanvasController : MonoBehaviour
{
    public bool visibleAtStart;
    private Image _health;

    private void Start()
    {
        _health = GetComponent<Image>();

        _health.enabled = visibleAtStart;
    }

    public void UpdateHealthBar(float currAmt, float maxAmt)
    {
        _health.fillAmount = currAmt * maxAmt / 10000f;
    }

    public void SetVisibility(bool visibility)
    {
        _health.enabled = visibility;
    }
}