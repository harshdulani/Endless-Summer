using System.Collections.Generic;
using UnityEngine;

public class TargetColliderController : MonoBehaviour
{
    public List<GameObject> active;

    private void Start()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        active.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        active.Remove(other.gameObject);
    }
}
