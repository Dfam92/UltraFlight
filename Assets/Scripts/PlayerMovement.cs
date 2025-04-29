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
    public float ExtraFallGravityMultiplier = 2f; // Multiplicador de gravidade extra quando o jogador quiser descer mais rápido

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
            _velocity.y = -1f; // Reset do pulo ao tocar o chão
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Aceleração e desaceleração contínua
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
            currentForwardSpeed -= Deceleration * Runner.DeltaTime * 0.5f;
        }

        // Clamp de velocidade (incluindo debug para trás)
        currentForwardSpeed = AllowBackwardMovement
            ? Mathf.Clamp(currentForwardSpeed, -MaxForwardSpeed, MaxForwardSpeed)
            : Mathf.Clamp(currentForwardSpeed, 0f, MaxForwardSpeed);

        Vector3 move = new Vector3(horizontal * LateralSpeed, 0, currentForwardSpeed) * Runner.DeltaTime;

        // 🔽 Descida rápida (fast fall)
        bool isFallingFast = !_controller.isGrounded && (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow));

        float gravityThisFrame = GravityValue;

        if (isFallingFast)
        {
            gravityThisFrame *= ExtraFallGravityMultiplier;
        }

        _velocity.y += gravityThisFrame * Runner.DeltaTime;

        // Aplicação do pulo
        if (_jumpPressed && _controller.isGrounded)
        {
            _velocity.y = JumpForce;
        }

        _controller.Move(move + _velocity * Runner.DeltaTime);

        // Inclinação no eixo Z (movimento lateral)
        float targetZRotation = -horizontal * TiltAmount;

        // Inclinação no eixo X (pulo)
        float targetXRotation = 0f;
        if (!_controller.isGrounded)
        {
            targetXRotation = _velocity.y > 0 ? -10f : 10f;
        }

        // Combina ambas rotações
        Quaternion targetRotation = Quaternion.Euler(targetXRotation, 0, targetZRotation);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Runner.DeltaTime * 5f);

        _jumpPressed = false;
    }
}
