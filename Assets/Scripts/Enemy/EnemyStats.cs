using UnityEngine;
using UnityEngine.Serialization;

public class EnemyStats : MonoBehaviour
{
    public float health, maxHealth;

    public float hitPoints;
    
    private void Start()
    {
        health = maxHealth;
    }
}
