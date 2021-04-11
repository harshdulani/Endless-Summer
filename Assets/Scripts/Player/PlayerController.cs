using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    public float takeDamageEvery;
    
    private HealthCanvasController _healthCanvas;
    private NavMeshAgent _agent;
    private Ray _ray;
    private Camera _cam;
    
    private Vector3 _position = Vector3.zero;
    private float _elapsedTimeFromDamage;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _healthCanvas = GetComponentInChildren<HealthCanvasController>();
        _cam = Camera.main;
    }

    private void Update()
    {
        if (LitmusTest.Paper.litDay && DayNightCycle.isDayActiveLight)
        {
            //calculate damage per sec based on distance from sun
            print("hit by sun");
            ChangeHealth(PlayerStats.stats.takeDPS);
        }
        else if (LitmusTest.Paper.litNight && DayNightCycle.isNightActiveLight)
        {
            print("hit by moon");
            ChangeHealth(PlayerStats.stats.takeDPS);
        }
        
        if (!Input.GetButtonUp("Fire2")) return;
        // Reset ray with new mouse position
        _ray = _cam.ScreenPointToRay(Input.mousePosition);

        foreach (var hit in Physics.RaycastAll(_ray))
            _position = hit.point;

        _agent.SetDestination(_position);
    }

    private void ChangeHealth(float amt)
    {
        _elapsedTimeFromDamage += Time.deltaTime;

        if (!(_elapsedTimeFromDamage >= takeDamageEvery)) return;
        
        PlayerStats.stats.health -= amt;
        _elapsedTimeFromDamage = 0f;
        
        _healthCanvas.UpdateHealthBar(PlayerStats.stats.health, PlayerStats.stats.maxHealth);
    }
}
