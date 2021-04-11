using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public float giveHitInterval;

    private Collider _chasing;
    
    private Vector3 _chasePos = Vector3.zero;
    private bool _shouldSearch, _isAttacking;
    
    private NavMeshAgent _agent;
    private EnemyStats _myStats;
    private HealthCanvasController _healthCanvas;

    private WaitForSeconds _waitLoop = new WaitForSeconds(0.01f);
    
    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _myStats = GetComponent<EnemyStats>();
        
        _healthCanvas = GetComponentInChildren<HealthCanvasController>();

        StartCoroutine(FindWall());
    }

    private void Update()
    {
        if (_shouldSearch)
        {
            StartCoroutine(FindWall());
            Debug.Log(gameObject.name + "'s Status for wall search", _chasing);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider != _chasing)
            return;

        _agent.isStopped = true;
        _isAttacking = true;
        StartCoroutine(Attack());
    }

    private IEnumerator Attack()
    {
        while (_isAttacking)
        {
            if(_chasing != null)
            {
                if (_chasing.GetComponent<WallController>().TakeDamage(_myStats.hitPoints) > 0)
                    yield return new WaitForSeconds(giveHitInterval);
                else
                    _isAttacking = false;
            }
            else
            {
                _isAttacking = false;
            }
        }
        _shouldSearch = true;
    }

    public float ChangeHealth(float amt)
    {
        print("taking damage");
        _myStats.health -= amt;
        
        _healthCanvas.UpdateHealthBar(_myStats.health, _myStats.maxHealth);

        if (_myStats.health > 0f) return _myStats.health;
        
        _agent.isStopped = true;
        Destroy(gameObject, 0.25f);
        return _myStats.health;
    }
    
    private IEnumerator FindWall()
    {
        _shouldSearch = false;
        foreach (var parent in GameObject.FindGameObjectsWithTag("WallParent"))
        {
            if (!parent.name.Equals("Wall (" + _myStats.spawnPointIndex + ")")) continue;

            var boxIndex = parent.transform.childCount - 1;
            Transform currBox = null;

            while (boxIndex >= 0)
            {
                //take a break
                yield return _waitLoop;
                
                currBox = parent.transform.GetChild(boxIndex);
                if (currBox.GetComponent<WallStats>().isBroken)
                    boxIndex--;
                else
                    break;
            }

            if (currBox != null)
            {
                _chasePos = currBox.position;
                _agent.SetDestination(_chasePos);
                _agent.isStopped = false;
                _chasing = currBox.GetComponent<Collider>();
            }

            //take a break
            yield return _waitLoop;
        }

        if (_chasing == null)
            _shouldSearch = true;
    }
}