using System;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public bool _isSunLight;
    
    public float cycleSpeed, movementSpeed;
    public float zBounds;

    private Vector3 myPos;
    private bool _shouldGoLeft;
    private int _idMultiplierForLeft;

    private void Start()
    {
        //if it is moonlight, left and right are - and + resp
        _idMultiplierForLeft = _isSunLight ? -1 : 1;
    }

    private void Update()
    {
        transform.Rotate(Vector3.right, cycleSpeed);

        myPos = transform.position;
        
        transform.position = new Vector3(myPos.x, myPos.y, myPos.z + movementSpeed * LeftOrRight());
    }

    private int LeftOrRight()
    {
        if (_shouldGoLeft)
        {
            if (myPos.z <= _idMultiplierForLeft * zBounds)
                _shouldGoLeft = false;
            else
                return -1;
        }
        else
        {
            if (myPos.z >= -_idMultiplierForLeft * zBounds)
                _shouldGoLeft = true;
            else
                return 1;
        }

        return 0;
    }
}
