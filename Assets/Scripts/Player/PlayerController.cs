using System;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Me;
    public float takeDamageInterval, giveDamageInterval;

    [Header("Lasers")] public LineRenderer[] attackLasers;
    public LineRenderer[] repairLasers;

    private LineRenderer _lineLeft, _lineRight;
    private Transform _hitTrans = null;

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
        
        _lineLeft = attackLasers[0];
        _lineRight = attackLasers[1];
        
        _enemyMask = LayerMask.GetMask("Default", "Enemy");
        _wallMask = LayerMask.GetMask( "Default", "BrokenWall");
    }

    private void Update()
    {
        if(!GameController.game.hasGameStarted) return;
        
        if (LitmusTest.Paper.litDay && DayNightCycle.isDayActiveLight)
        {
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

            _lineLeft.enabled = false;
            _lineRight.enabled = false;
            if (_isRepairing)
            {
                _lineLeft = repairLasers[0];
                _lineRight = repairLasers[1];
            }
            else
            {
                _lineLeft = attackLasers[0];
                _lineRight = attackLasers[1];
            }
        }

        try
        {
            if (!ReferenceEquals(_hitTrans, null))
            {
                if (_elapsedTimefromHitting <= giveDamageInterval)
                {
                    _lineLeft.enabled = true;
                    _lineLeft.positionCount = 2;
                    _lineLeft.SetPosition(0, _lineLeft.transform.position);
                    _lineLeft.SetPosition(1, _hitTrans.position);
                    
                    _lineRight.enabled = true;
                    _lineRight.positionCount = 2;
                    _lineRight.SetPosition(0, _lineRight.transform.position);
                    _lineRight.SetPosition(1, _hitTrans.position);
                    _position = _hitTrans.position;
                    
                    _rotatingToPosition = true;
                }
            }
        }
        catch (Exception e)
        {
            _lineLeft.enabled = false;
            _lineRight.enabled = false;
            _hitTrans = null;
        }

        if (_elapsedTimefromHitting > giveDamageInterval)
        {
            _lineLeft.enabled = false;
            _lineRight.enabled = false;
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
                Attack();
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
        
        MainMenuController.menu.ShowGameOver();
        Destroy(gameObject, 0.75f);
    }

    private void Attack()
    {
        if (!(_elapsedTimefromHitting >= giveDamageInterval)) return;
        
        Physics.Raycast(new Ray(transform.position, transform.forward), out _hit, 10f, _enemyMask);

        if(ReferenceEquals(_hit.collider, null)) return;
        
        if(_hit.collider.gameObject.CompareTag("Enemy"))
        {
            _hit.collider.GetComponent<EnemyController>().ChangeHealth(PlayerStats.stats.hitPoints);
            _hitTrans = _hit.transform;
            _elapsedTimefromHitting = 0f;
        }
    }

    private void Repair()
    {
        if (!(_elapsedTimefromHitting >= giveDamageInterval)) return;
        
        Physics.Raycast(new Ray(transform.position, transform.forward), out _hit, 10f, _wallMask);

        try
        {
            var x = _hit.collider.gameObject;
            if (ReferenceEquals(x, null)) return;

            if (x.CompareTag("Wall"))
            {
                x.GetComponent<WallController>().Repair(PlayerStats.stats.repairHitPoints);
                _hitTrans = x.transform;
            }

            _elapsedTimefromHitting = 0f;
        }
        catch
        {
            print("didnt find box to repair");
        }
    }
    
    private void Rotate(Vector3 position)
    {
        _desiredMovementDirection = position - transform.position;

        _desiredMovementDirection.y = 0f;

        if(_desiredMovementDirection != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_desiredMovementDirection), 0.2f);
    }
}
