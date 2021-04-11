using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats stats;
    
    public float health, maxHealth;

    public float hitPoints, takeDPS;

    private void Awake()
    {
        if (!stats)
            stats = this;
        else
            Destroy(stats);
    }

    private void Start()
    {
        health = maxHealth;
    }
}
