using Fusion;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    private Vector3 _velocity;
    public bool _jumpPressed;

    private CharacterController _controller;

    public float MaxForwardSpeed = 10f;
    private float _defaultMaxForwardSpeed = 0f;
    public float LateralSpeed = 2f;
    public float TiltAmount = 15f;

    public float Acceleration = 1f;
    private float _defaultAcceleration = 0f;    
    public float Deceleration = 2f;

    public float JumpForce = 5f;
    public float GravityValue = -9.81f;
    private float _defaultGravity = 0;

    public bool AllowBackwardMovement = false; // 🔧 Debug flag

    public CinemachineCamera CinemachinePlayerCamera;

    private float currentForwardSpeed = 0f;
    public float ExtraFallGravityMultiplier = 2f; // Multiplicador de gravidade extra quando o jogador quiser descer mais rápido

    [SerializeField] private Transform shipVisual; // arraste o ShipSelected aqui no Inspector

    [Range(0f, 90f)] public float MaxTiltZ = 15f;
    public float TiltLerpSpeed = 5f;
    private Quaternion _groundAlignedRotation = Quaternion.identity;


    private float currentXTilt = 0f;
    public float MaxTiltX = 15f;
    public float TiltXSpeed = 5f;

    private bool _inputBlocked = false;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        if (shipVisual == null)
        {
            shipVisual = transform.Find("ShipSelected");
        }

        _defaultGravity = GravityValue;
        _defaultMaxForwardSpeed = MaxForwardSpeed;
        _defaultAcceleration = Acceleration;
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
        if (_inputBlocked) return;
        if (_controller.isGrounded)
        {
            _velocity.y = -1f; // Reset do pulo ao tocar o chão
            GravityValue = _defaultGravity;
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
            GravityValue = GravityValue * 0.5f;
        }

        _controller.Move(move + _velocity * Runner.DeltaTime);

        _jumpPressed = false;

        AlignToGroundNormal();
        ApplyTiltRotation(horizontal);
        ApplyJumpTilt();
    }

    private void ApplyTiltRotation(float horizontalInput)
    {
        float targetZRotation = -horizontalInput * TiltAmount;

        Quaternion tiltOffset = Quaternion.Euler(0f, 0f, targetZRotation);
        Quaternion finalRotation = _groundAlignedRotation * tiltOffset;

        shipVisual.rotation = Quaternion.Slerp(shipVisual.rotation, finalRotation, Runner.DeltaTime * 5f);
    }

    private void AlignToGroundNormal()
    {
        if (_controller.isGrounded)
        {
            Ray ray = new Ray(transform.position + Vector3.up * 0.5f, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 2f))
            {
                Vector3 groundNormal = hitInfo.normal;
                Vector3 projectedForward = Vector3.ProjectOnPlane(transform.forward, groundNormal).normalized;
                Quaternion targetRotation = Quaternion.LookRotation(projectedForward, groundNormal);

                Vector3 euler = targetRotation.eulerAngles;
                euler.x = 0f;
                targetRotation = Quaternion.Euler(euler);

                _groundAlignedRotation = Quaternion.Slerp(_groundAlignedRotation, targetRotation, Runner.DeltaTime * 10f);
            }
        }
        else
        {
            // Voltar ao alinhamento reto (sem slope)
            Quaternion uprightRotation = Quaternion.LookRotation(transform.forward, Vector3.up);

            Vector3 euler = uprightRotation.eulerAngles;
            euler.x = 0f;
            uprightRotation = Quaternion.Euler(euler);

            _groundAlignedRotation = Quaternion.Slerp(_groundAlignedRotation, uprightRotation, Runner.DeltaTime * 5f);
        }
    }

    private void ApplyJumpTilt()
    {
        float targetXTilt = 0f;

        if (!_controller.isGrounded)
        {
            if (_velocity.y > 0.1f)
            {
                targetXTilt = -MaxTiltX; // inclinando para trás ao subir
            }
            else if (_velocity.y < -0.1f)
            {
                targetXTilt = MaxTiltX; // inclinando para frente ao cair
            }
        }

        // Suavizar a transição
        currentXTilt = Mathf.Lerp(currentXTilt, targetXTilt, Runner.DeltaTime * TiltXSpeed);

        // Aplicar ao visual (sem afetar tilt lateral)
        Quaternion currentRotation = shipVisual.localRotation;
        Vector3 euler = currentRotation.eulerAngles;

        // Garantir que o ângulo esteja entre -180 e 180
        if (euler.x > 180f) euler.x -= 360f;

        euler.x = currentXTilt;
        shipVisual.localRotation = Quaternion.Euler(euler);
    }

    public void ResetMovementState()
    {
        currentForwardSpeed = 0f;
        _velocity = Vector3.zero;
        GravityValue = _defaultGravity;
        BlockInput(0.1f);
    }

    public void ResetMaxSpeed()
    {
        MaxForwardSpeed = _defaultMaxForwardSpeed;
        Acceleration = _defaultAcceleration;
    }

    public void BlockInput(float duration)
    {
        _inputBlocked = true;
        Invoke(nameof(UnblockInput), duration);
    }

    private void UnblockInput()
    {
        _inputBlocked = false;
    }

    public void OnTriggerAccelerationFloor()
    {
        Debug.Log("Boosting");
        Acceleration = Acceleration + 20;
        MaxForwardSpeed = MaxForwardSpeed + 200;
        currentForwardSpeed += 50;
        Invoke(nameof(ResetMaxSpeed), 1);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("BoostFloor"))
        {
            OnTriggerAccelerationFloor();
        }
    }

}