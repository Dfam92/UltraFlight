using Fusion;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    private Vector3 _velocity;
    private bool _jumpPressed;

    private CharacterController _controller;

    public float MaxForwardSpeed = 10f;
    public float LateralSpeed = 2f;
    public float TiltAmount = 15f;

    public float Acceleration = 1f;
    public float Deceleration = 2f;

    public float JumpForce = 5f;
    public float GravityValue = -9.81f;

    public bool AllowBackwardMovement = false; // 🔧 Debug flag

    public CinemachineCamera CinemachinePlayerCamera;

    private float currentForwardSpeed = 0f;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            _jumpPressed = true;
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (Input.GetKeyDown(KeyCode.B))
        {
            AllowBackwardMovement = !AllowBackwardMovement;
            Debug.Log("AllowBackwardMovement = " + AllowBackwardMovement);
        }
#endif
    }

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            CinemachinePlayerCamera = GameObject.FindWithTag("PlayerCamera").GetComponent<CinemachineCamera>();
            CinemachinePlayerCamera.Follow = transform;
            CinemachinePlayerCamera.LookAt = transform;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (_controller.isGrounded)
        {
            _velocity = new Vector3(0, -1, 0);
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Aceleração e desaceleração
        if (vertical > 0)
        {
            currentForwardSpeed += Acceleration * Runner.DeltaTime;
        }
        else if (vertical < 0)
        {
            currentForwardSpeed -= Deceleration * Runner.DeltaTime;
        }
        else
        {
            // Desacelera suavemente se nada pressionado
            currentForwardSpeed -= Deceleration * Runner.DeltaTime * 0.5f;
        }

        // Clamp dependendo da flag
        if (AllowBackwardMovement)
        {
            currentForwardSpeed = Mathf.Clamp(currentForwardSpeed, -MaxForwardSpeed, MaxForwardSpeed);
        }
        else
        {
            currentForwardSpeed = Mathf.Clamp(currentForwardSpeed, 0f, MaxForwardSpeed);
        }

        Vector3 move = new Vector3(horizontal * LateralSpeed, 0, currentForwardSpeed) * Runner.DeltaTime;

        _velocity.y += GravityValue * Runner.DeltaTime;
        if (_jumpPressed && _controller.isGrounded)
        {
            _velocity.y += JumpForce;
        }

        _controller.Move(move + _velocity * Runner.DeltaTime);

        // Inclinação no eixo Z
        float targetZRotation = -horizontal * TiltAmount;
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetZRotation);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Runner.DeltaTime * 5f);

        _jumpPressed = false;
    }
}
