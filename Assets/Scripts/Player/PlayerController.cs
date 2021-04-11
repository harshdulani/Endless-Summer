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
        _wallMask = LayerMask.GetMask( "Default", "Wall");
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
            ToggleAttackRepair();
        }

        try
        {
            if (_hitTrans != null)
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
        if (Input.GetButton("Fire2"))
        {
            // target
            // Reset ray with new mouse position
            _ray = _cam.ScreenPointToRay(Input.mousePosition);

            foreach (var hit in Physics.RaycastAll(_ray, 50f))
            {
                if(_isRepairing)
                {
                    if (hit.collider.gameObject.CompareTag("BrokenWall"))
                    {
                        _position = hit.point;
                        _hit = hit;
                    }
                }
                else
                {
                    if (hit.collider.gameObject.CompareTag("Enemy"))
                    {
                        _position = hit.point;
                        _hit = hit;
                    }
                }
            }
            
            _position.y = 0.5f;
            
            _rotatingToPosition = true;

            if(_hit.collider != null)
            {
                if (_isRepairing)
                    Repair();
                else
                    Attack();
            }
        }
        if (Input.GetButtonUp("Fire1"))
        {
            //movement
            // Reset ray with new mouse position
            _ray = _cam.ScreenPointToRay(Input.mousePosition);

            foreach (var hit in Physics.RaycastAll(_ray, 20f))
                _position = hit.point;

            _position.y = 0.5f;

            _agent.SetDestination(_position);
        }
        
        if(_rotatingToPosition)
        {
            Rotate(_position);
        }
    }

    private void Attack()
    {
        if (!(_elapsedTimefromHitting >= giveDamageInterval)) return;

        if (!_hit.collider.gameObject.CompareTag("Enemy")) return;

        int foundWallAt = 0;
        int foundEnemyAt = 0;
        int i = 0;

        var _rayStartPos = transform.position;
        _rayStartPos.y = 0.5f;

        var direction = _position - transform.position;
        
        Debug.DrawRay(_rayStartPos, direction, Color.red, 2f);

        foreach (var hit in Physics.RaycastAll(_rayStartPos, direction, 20f))
        {
            i++;
            if (hit.collider.gameObject.CompareTag("Wall"))
            {
                Debug.Log("i", hit.collider);
                foundWallAt = i;
            }
            if (hit.collider.gameObject.CompareTag("Enemy"))
            {
                foundEnemyAt = i;
            }
            if (foundEnemyAt != foundWallAt)
                break;
        }
        
        print(foundEnemyAt + " " + foundWallAt);

        if(foundEnemyAt == 0) return;
        if(foundEnemyAt > foundWallAt && foundWallAt > 0) return;
        
        if (_hit.collider.GetComponent<EnemyController>().ChangeHealth(PlayerStats.stats.hitPoints) <= 0)
        {
            _hitTrans = null;
        }
        _hitTrans = _hit.transform;
        _elapsedTimefromHitting = 0f;
    }

    private void Repair()
    {
        if (!(_elapsedTimefromHitting >= giveDamageInterval)) return;

        var x = _hit.collider.gameObject;
        
        int foundWallAt = 0;
        int foundEnemyAt = 0;
        int i = 0;

        var _rayStartPos = transform.position;
        _rayStartPos.y = 0.5f;

        var direction = _position - transform.position;
        
        Debug.DrawRay(_rayStartPos, direction, Color.red, 2f);

        foreach (var hit in Physics.RaycastAll(_rayStartPos, direction, 20f))
        {
            i++;
            if (hit.collider.gameObject.CompareTag("Wall"))
            {
                Debug.Log("i", hit.collider);
                foundWallAt = i;
            }
            if (hit.collider.gameObject.CompareTag("Enemy"))
            {
                foundEnemyAt = i;
            }
            if (foundEnemyAt != foundWallAt)
                break;
        }
        
        print(foundEnemyAt + " " + foundWallAt);

        if(foundEnemyAt == 0) return;
        if(foundEnemyAt < foundWallAt && foundEnemyAt > 0) return;

        
        if (x.CompareTag("BrokenWall"))
        {
            x.GetComponent<WallController>().Repair(PlayerStats.stats.repairHitPoints);
            _hitTrans = x.transform;
        }

        _elapsedTimefromHitting = 0f;
    }

    private void ToggleAttackRepair()
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
    
    private void Rotate(Vector3 position)
    {
        _desiredMovementDirection = position - transform.position;

        _desiredMovementDirection.y = 0f;

        if(_desiredMovementDirection != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_desiredMovementDirection), 0.2f);
    }
}
