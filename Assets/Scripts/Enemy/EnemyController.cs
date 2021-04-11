using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public float giveHitInterval;
    
    private NavMeshAgent _agent;
    private Collider _chasing, _oldChasing = null;
    
    private bool _shouldSearch = true, _shouldAttack;
    
    private EnemyStats _myStats;
    private HealthCanvasController _healthCanvas;
    
    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _myStats = GetComponent<EnemyStats>();
        
        _healthCanvas = GetComponentInChildren<HealthCanvasController>();
        
        var list = GameObject.FindGameObjectsWithTag("Wall");

        _agent.SetDestination(list[4].transform.position);
    }

    private void Update()
    {
        if (_agent.isStopped) return;
        if(ReferenceEquals(_chasing, null) && !_shouldSearch)
            ForceTriggerCheck();
    }

    private void OnCollisionEnter(Collision other)
    {
        if(other.collider != _chasing) return;

        _agent.isStopped = true;
        _shouldAttack = true;
        StartCoroutine(Attack());
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!_shouldSearch) return;
        if(!other.CompareTag("Wall")) return;
        
        _shouldSearch = false;
        _oldChasing = _chasing;

        try
        {
            var parent = other.transform.parent;
            _chasing = parent.GetChild(parent.childCount - 1).GetComponent<Collider>();

            if (_chasing == _oldChasing && _chasing.transform.parent.childCount > 0)
                _shouldSearch = true;

            _agent.isStopped = false;
            _agent.SetDestination(_chasing.transform.position);
        }
        catch (Exception e)
        {
            _shouldSearch = true;
        }
    }

    private IEnumerator Attack()
    {
        while (_shouldAttack)
        {
            if(!ReferenceEquals(_chasing, null))
            {
                if (_chasing.GetComponent<WallController>().TakeDamage(_myStats.hitPoints) > 0)
                    yield return new WaitForSeconds(giveHitInterval);
                else
                    _shouldAttack = false;
            }
            else
            {
                _shouldAttack = false;
            }
        }
        _shouldSearch = true;
        ForceTriggerCheck();
    }

    private void ForceTriggerCheck()
    {
        _shouldSearch = true;
        foreach(var c in  Physics.OverlapSphere(transform.position, 7f))
            gameObject.SendMessage("OnTriggerEnter", c);
    }

    public void ChangeHealth(float amt)
    {
        print("taking damage");
        _myStats.health -= amt;
        
        _healthCanvas.UpdateHealthBar(_myStats.health, _myStats.maxHealth);

        if (_myStats.health > 0f) return;
        
        _agent.isStopped = true;
        Destroy(gameObject, 0.75f);
    }
}