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
    
    //attack
    //pick new box, repeat

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _myStats = GetComponent<EnemyStats>();
        
        var list = GameObject.FindGameObjectsWithTag("Wall");

        _agent.SetDestination(list[4].transform.position);
    }

    private void Update()
    {
        if(!_agent.isStopped)
            if(_chasing == null && !_shouldSearch)
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
        _chasing = other.transform.parent.GetChild(other.transform.parent.childCount - 1).GetComponent<Collider>();

        if (_chasing == _oldChasing && _chasing.transform.parent.childCount > 0)
            _shouldSearch = true;
                
        _agent.isStopped = false;
        _agent.SetDestination(_chasing.transform.position);
    }

    private IEnumerator Attack()
    {
        while (_shouldAttack)
        {
            if(!ReferenceEquals(_chasing, null))
            {
                if (_chasing.GetComponent<WallController>().TakeHit(_myStats.hitPoints) > 0)
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
}