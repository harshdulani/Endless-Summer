using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public float health, maxHealth;

    public float hitPoints;

    public int spawnPointIndex;
    
    private void Start()
    {
        health = maxHealth;
    }
}
