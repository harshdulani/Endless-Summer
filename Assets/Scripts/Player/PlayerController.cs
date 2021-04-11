using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Me;
    public float takeDamageInterval, giveDamageInterval;

    private HealthCanvasController _healthCanvas;
    private NavMeshAgent _agent;
    private Ray _ray;
    private Camera _cam;
    
    private Vector3 _position = Vector3.zero;
    private float _elapsedTimeFromDamage, _elapsedTimefromHitting;

    private RaycastHit _hit;
    private Vector3 _desiredMovementDirection;
    private bool _rotatingToPosition, _isRepairing;
    private int _enemyMask, _wallMask;

    private void Awake()
    {
        if (!Me)
            Me = this;
        else 
            Destroy(this);
    }

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _healthCanvas = GetComponentInChildren<HealthCanvasController>();
        _cam = Camera.main;

        _enemyMask = LayerMask.GetMask("Enemy", "Default");
        _wallMask = LayerMask.GetMask("BrokenWall", "Default");
    }

    private void Update()
    {
        if (LitmusTest.Paper.litDay && DayNightCycle.isDayActiveLight)
        {
            //calculate damage per sec based on distance from sun
            //print("hit by sun");
            ChangeHealth(PlayerStats.stats.takeDPS);
        }
        else if (LitmusTest.Paper.litNight && DayNightCycle.isNightActiveLight)
        {
            //print("hit by moon");
            ChangeHealth(PlayerStats.stats.takeDPS);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            _isRepairing = !_isRepairing;
        }
        
        _elapsedTimefromHitting += Time.deltaTime;
        if (Input.GetButton("Fire1"))
        {
            // Reset ray with new mouse position
            _ray = _cam.ScreenPointToRay(Input.mousePosition);

            foreach (var hit in Physics.RaycastAll(_ray))
                _position = hit.point;

            _rotatingToPosition = true;

            if (_isRepairing)
                Repair();
            else
                GiveAttack();
        }
        if (Input.GetButtonUp("Fire2"))
        {
            // Reset ray with new mouse position
            _ray = _cam.ScreenPointToRay(Input.mousePosition);

            foreach (var hit in Physics.RaycastAll(_ray))
                _position = hit.point;

            _agent.SetDestination(_position);
        }
        
        if(_rotatingToPosition)
        {
            Rotate(_position);
        }
    }

    private void ChangeHealth(float amt)
    {
        _elapsedTimeFromDamage += Time.deltaTime;

        if (!(_elapsedTimeFromDamage >= takeDamageInterval)) return;
        
        PlayerStats.stats.health -= amt;
        _elapsedTimeFromDamage = 0f;
        
        _healthCanvas.UpdateHealthBar(PlayerStats.stats.health, PlayerStats.stats.maxHealth);
        
        if (PlayerStats.stats.health > 0f) return;
        
        _agent.isStopped = true;
        Destroy(gameObject, 0.75f);
    }

    private void GiveAttack()
    {
        if (!(_elapsedTimefromHitting >= giveDamageInterval)) return;
        
        Physics.Raycast(new Ray(transform.position, transform.forward), out _hit, 10f, _enemyMask);

        if(ReferenceEquals(_hit.collider, null)) return;
        
        if(_hit.collider.gameObject.CompareTag("Enemy"))
        {
            _hit.collider.GetComponent<EnemyController>().ChangeHealth(PlayerStats.stats.hitPoints);
        }
        _elapsedTimefromHitting = 0f;
    }

    private void Repair()
    {
        if (!(_elapsedTimefromHitting >= giveDamageInterval)) return;
        
        Physics.Raycast(new Ray(transform.position, transform.forward), out _hit, 10f, _wallMask);

        if(ReferenceEquals(_hit.collider.gameObject, null)) return;
        
        var x = _hit.collider.gameObject;
        Debug.Log(x.name, x);
        if(x.CompareTag("Wall"))
        {
            x.GetComponent<WallController>().Repair(PlayerStats.stats.repairHitPoints);
        }
        _elapsedTimefromHitting = 0f;
    }
    
    private void Rotate(Vector3 position)
    {
        _desiredMovementDirection = position - transform.position;

        _desiredMovementDirection.y = 0f;

        if(_desiredMovementDirection != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_desiredMovementDirection), 0.2f);
    }
}
