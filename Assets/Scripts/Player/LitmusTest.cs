using System;
using UnityEngine;

public class LitmusTest : MonoBehaviour
{
    public static LitmusTest Paper;

    private void Awake()
    {
        if (Paper == null)
            Paper = this;
        else
            Destroy(this);
    }

    public Transform nightLight, dayLight;
    
    public bool litNight;
    public bool litDay;

    private RaycastHit _nightHits;
    private RaycastHit _dayHits;

    private void Update()
    {
        var x = nightLight.position - transform.position;
        var y = dayLight.position - transform.position ;

        Physics.Raycast(new Ray(transform.position, x), out _nightHits, 50f);
        Physics.Raycast(new Ray(transform.position, y), out _dayHits, 50f);

        litNight = _nightHits.collider.name.Equals("NightLight");
        litDay = _dayHits.collider.name.Equals("DayLight");
    }
}
