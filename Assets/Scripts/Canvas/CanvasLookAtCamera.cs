using UnityEngine;

public class CanvasLookAtCamera : MonoBehaviour
{
    private Canvas _canvas;
    private Transform _mainCam;
    
    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        _canvas.worldCamera = Camera.main;
        _mainCam = Camera.main.transform;
    }

    private void LateUpdate()
    {
        _canvas.transform.rotation = Quaternion.LookRotation(_mainCam.forward, _mainCam.up);
    }
}
