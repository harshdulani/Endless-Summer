using UnityEngine;

public class WallStats : MonoBehaviour
{
    public float health, maxHealth;
    
    private void Start()
    {
        health = maxHealth;
    }
}
