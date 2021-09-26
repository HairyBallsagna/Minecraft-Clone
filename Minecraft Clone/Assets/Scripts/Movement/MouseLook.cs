using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float sensitivity = 100f;
    public float xClamp = -90f;
    public float yClamp = 90f;

    public Transform player = null;
    
    private float xRotation = 0f;

    private Vector3 rot;

    private bool _canLook = true;

    private void Start()
    {
        SetCursorState(true);
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, xClamp, yClamp);
        
        if (_canLook)
        {
            transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
            player.Rotate(Vector3.up * mouseX);
        }
    }

    public void SetCursorState(bool _value)
    {
        if (_value)
        {
            Cursor.lockState = CursorLockMode.Locked;
            _canLook = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            _canLook = false;
        }
    }
}
