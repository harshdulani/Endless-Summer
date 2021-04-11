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

    private RaycastHit[] _nightHits = new RaycastHit[1];
    private RaycastHit[] _dayHits = new RaycastHit[1];

    private void Update()
    {
        var x = nightLight.position - transform.position;
        var y = dayLight.position - transform.position ;

        Physics.RaycastNonAlloc(new Ray(transform.position, x), _nightHits);
        Physics.RaycastNonAlloc(new Ray(transform.position, y), _dayHits);

        litNight = _nightHits[0].collider.name.Equals("NightLight");
        litDay = _dayHits[0].collider.name.Equals("DayLight");
    }
}
