using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    private NavMeshAgent _agent;
    private Collider _chasing;
    
    private bool _shouldSearch = true;
    
    //attack
    //pick new box, repeat

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        
        var list = GameObject.FindGameObjectsWithTag("Wall");

        _agent.SetDestination(list[4].transform.position);
    }

    private void OnCollisionEnter(Collision other)
    {
        if(other.collider != _chasing) return;

        _agent.isStopped = true;
        StartCoroutine(Attack());
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!_shouldSearch) return;
        if(!other.CompareTag("Wall")) return;
        
        _shouldSearch = false;
        _agent.SetDestination(other.transform.parent.GetChild(other.transform.parent.childCount - 1).position);
        _chasing = other.transform.parent.GetChild(other.transform.parent.childCount - 1).GetComponent<Collider>();
        Debug.Log(other.name, other.gameObject);
    }

    private IEnumerator Attack()
    {
        print("Start attacking");
        yield break;
    }
}