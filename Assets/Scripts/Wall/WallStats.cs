using UnityEngine;

public class WallStats : MonoBehaviour
{
    public bool isBroken;
    public float health, maxHealth;
    
    private void Start()
    {
        health = isBroken ? 0f : maxHealth;
    }
}
