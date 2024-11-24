using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintMultiplier = 1.25f; // Ускорение при беге
    [SerializeField] private float jumpForce = 5f;

    [Header("Camera Settings")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float cameraRotationSpeed = 3f;
    [SerializeField] private float mouseSensitivity = 360;
    [SerializeField] private float maxLookAngle = 80f;
    [SerializeField] private float cameraDistance = -2.3f;
    [SerializeField] private float cameraHeight = 1.7f;
    [SerializeField] private float sprintCameraOffset = -0.5f; // Увеличение дистанции камеры при беге

    [Header("Vertical Camera Movement")]
    [SerializeField] private float minVerticalAngle = -1f;
    [SerializeField] private float maxVerticalAngle = 1f;

    [Header("Animator Settings")]
    [SerializeField] private Animator animator;

    [Header("Dialog Settings")]
    [SerializeField] private OpenDialog openDialog; // Скрипт для диалога

    private Rigidbody rb;
    private Vector3 moveDirection;
    private bool isGrounded = true;
    private bool isInMomDialogTrigger = false; // Флаг нахождения в триггере

    private float rotationX = 0f;
    private float rotationY = 0f;

    [Header("Smooth Movement Settings")]
    [SerializeField] private float moveSmoothTime = 0.1f;
    [SerializeField] private float cameraSmoothTime = 0.1f;

    private Vector3 currentVelocity = Vector3.zero;
    private Vector3 cameraVelocity = Vector3.zero;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (cameraTransform == null) Debug.LogError("Camera Transform not assigned!");
        if (animator == null) Debug.LogError("Animator not assigned!");

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleCamera();
        HandleMovementInput();
        UpdateAnimator();

        // Проверяем нажатие кнопки E, когда персонаж находится в триггере
        if (isInMomDialogTrigger && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Е нажата");
            openDialog.DialogWithMom(); // Открываем диалог
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            Debug.Log("N нажата");
            PlayerPrefs.DeleteKey("TalkedWithMom");
            PlayerPrefs.Save(); 
        }
    }

    private void FixedUpdate()
    {
        ApplyMovement();
    }

    private void OnCollisionStay(Collision collision)
    {
        isGrounded = collision.gameObject.CompareTag("Ground");
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground")) isGrounded = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MomDialog"))
        {
            Debug.Log("Игрок вошел в триггер");
            isInMomDialogTrigger = true; // Устанавливаем флаг
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MomDialog"))
        {
            isInMomDialogTrigger = false; // Сбрасываем флаг
        }
    }

    private void HandleMovementInput()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = right.y = 0f;

        float currentSpeed = moveSpeed;

        // Ускорение при нажатии Shift
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed *= sprintMultiplier;
        }

        moveDirection = (forward.normalized * moveZ + right.normalized * moveX) * currentSpeed;

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void ApplyMovement()
    {
        Vector3 velocity = new Vector3(moveDirection.x, rb.velocity.y, moveDirection.z);
        rb.velocity = Vector3.SmoothDamp(rb.velocity, velocity, ref currentVelocity, moveSmoothTime);
    }

    private void HandleCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        rotationX += mouseX * cameraRotationSpeed;
        rotationY = Mathf.Clamp(rotationY - mouseY * cameraRotationSpeed, minVerticalAngle, maxVerticalAngle);

        // Изменение дистанции камеры при ускорении
        float targetCameraDistance = Input.GetKey(KeyCode.LeftShift) ? cameraDistance + sprintCameraOffset : cameraDistance;

        float angleRad = rotationX * Mathf.Deg2Rad;
        Vector3 targetPosition = new Vector3(
            Mathf.Sin(angleRad) * targetCameraDistance,
            cameraHeight + Mathf.Sin(rotationY * Mathf.Deg2Rad) * 2f,
            Mathf.Cos(angleRad) * targetCameraDistance
        ) + transform.position;

        cameraTransform.position = Vector3.SmoothDamp(cameraTransform.position, targetPosition, ref cameraVelocity, cameraSmoothTime);

        Vector3 targetLookAt = new Vector3(transform.position.x, cameraTransform.position.y, transform.position.z);
        cameraTransform.LookAt(targetLookAt);

        transform.rotation = Quaternion.Euler(0f, rotationX, 0f);
    }

    private void UpdateAnimator()
    {
        bool isWalking = moveDirection.magnitude > 0.1f;
        bool isRunning = isWalking && Input.GetKey(KeyCode.LeftShift);

        animator.SetBool("isWalk", isWalking);
        animator.SetBool("isRun", isRunning);
    }
}
