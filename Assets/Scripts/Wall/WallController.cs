using System;
using UnityEngine;

public class WallController : MonoBehaviour
{
    public GameObject brokenBox, normalBox;
    private HealthCanvasController _healthCanvas;
    private WallStats _myStats;

    private void Start()
    {
        _myStats = GetComponent<WallStats>();
        
        _healthCanvas = GetComponentInChildren<HealthCanvasController>();
    }

    public float TakeDamage(float amt)
    {
        _healthCanvas.SetVisibility(true);
        _myStats.health -= amt;
        
        if (_myStats.health <= 0f)
        {
            _myStats.isBroken = true;
            Phoenix();
        }
        
        _healthCanvas.UpdateHealthBar(_myStats.health, _myStats.maxHealth);
        
        return _myStats.health;
    }

    public void Repair(float amt)
    {
        if(_myStats.health > 1f) return;
        
        _myStats.health += amt;
        
        _healthCanvas.SetVisibility(true);
        _healthCanvas.UpdateHealthBar(_myStats.health, _myStats.maxHealth);
        
        if (_myStats.health >= 100f)
        {
            _myStats.isBroken = false;
            Phoenix();
        }
    }

    private void Phoenix()
    {
        var y = Instantiate(_myStats.isBroken ? brokenBox : normalBox, transform.position, Quaternion.identity);
        Debug.Log("phoenix " + y.name, y);
        Destroy(gameObject);
    }
}
