using System;
using UnityEngine;

public class WallController : MonoBehaviour
{
    private WallStats _myStats;

    private void Start()
    {
        _myStats = GetComponent<WallStats>();
    }

    public float TakeHit(float amt)
    {
        _myStats.health -= amt;

        if (_myStats.health <= 0f)
            Destruct();
        
        return _myStats.health;
    }

    private void Destruct()
    {
        Debug.Log("destruct", gameObject);
        Destroy(gameObject, 0.1f);
    }
}
