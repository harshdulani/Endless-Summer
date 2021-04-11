using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public float health, maxHealth;

    public float hitPoints;
    
    private void Start()
    {
        health = maxHealth;
    }
}
