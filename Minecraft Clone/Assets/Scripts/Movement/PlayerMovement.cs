using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // CACHE (PUBLIC)
        public CharacterController controller;
        public MouseLook cam;
        public Transform groundCheck;
        public LayerMask ground;

        // CONFIGS (PUBLIC)
        public float jumpForce = 2f;
        public float groundDistance;
        public float runSpeed = 12f;
        public float walkSpeed = 6f;
        public float gravity = -9.81f;

        // PRIVATE
        private PlayerState playerState = PlayerState.Idle;

        private float forwardSpeed = 0;

        private Vector3 _dir;
        private Vector3 _velocity;
        private Vector2 _input;
        private bool _isGrounded;
        private bool _canLook;

        private void Update()
		{
			float currentSpeed = 0f;

			// gettin the x and z input 
			float horizontal = Input.GetAxisRaw("Horizontal");
			float vertical = Input.GetAxisRaw("Vertical");

			// setting the _input Vector 2 to the horizontal and vertical values
			_input = new Vector2(horizontal, vertical);

			currentSpeed = SetSpeedAndState(currentSpeed);
			
			_dir = transform.right * horizontal + transform.forward * vertical;

			controller.Move(_dir * currentSpeed * Time.deltaTime);

			if (Input.GetKeyDown(KeyCode.Space) && _isGrounded)
			{
				_velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
			}

			controller.Move(_velocity * Time.deltaTime);
		}

		private float SetSpeedAndState(float speed)
		{
			if (Input.GetKey(KeyCode.LeftShift) && _input.magnitude > 0)
			{
				playerState = PlayerState.Running;
				forwardSpeed += 0.75f * Time.deltaTime;
				if (forwardSpeed > 0.5f) forwardSpeed = 0.5f;
				speed = runSpeed;
			}
			else if (Input.GetKey(KeyCode.LeftShift) == false && _input.magnitude > 0)
			{
				playerState = PlayerState.Walking;
				forwardSpeed += 0.75f * Time.deltaTime;
				if (forwardSpeed > 0.25f) forwardSpeed = 0.25f;
				speed = walkSpeed;
			}
			else
			{
				playerState = PlayerState.Idle;
				forwardSpeed -= 0.75f * Time.deltaTime;
				if (forwardSpeed < 0f) forwardSpeed = 0f;
			}

			return speed;
		}

		private void FixedUpdate()
		{
            if (_isGrounded && _velocity.y < 0f)
            {
                _velocity.y = -2f;
            }

            _isGrounded = Physics.Raycast(groundCheck.position, -groundCheck.transform.up, groundDistance, ground);
            _velocity.y += gravity * Time.deltaTime;
        }
}
public enum PlayerState
{
	Idle,
	Walking, 
	Running
}
