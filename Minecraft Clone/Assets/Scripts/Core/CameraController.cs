using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float sensitivity = 300f;
    public Transform playerBody = null;
    public PlayerInput playerInput;

    private float verticalRotation = 0f;

    private void Awake()
    {
        playerInput = GetComponentInParent<PlayerInput>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        float mouseX = playerInput.MousePosition.x * sensitivity * Time.deltaTime;
        float mouseY = playerInput.MousePosition.y * sensitivity * Time.deltaTime;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
        
        transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
