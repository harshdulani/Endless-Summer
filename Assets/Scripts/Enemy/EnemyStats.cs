using System;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public float health, maxHealth;

    private void Start()
    {
        health = maxHealth;
    }
}
