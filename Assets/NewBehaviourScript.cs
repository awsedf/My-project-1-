using UnityEngine;

public class AdvancedFPSController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f; // Новая переменная для скорости бега
    public float gravity = -9.81f;
    private Vector3 velocity;
    private float currentSpeed; // Текущая скорость (меняется при беге/ходьбе)

    [Header("Jump")]
    public float jumpHeight = 1.5f;
    private bool isGrounded;

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float staminaDepletionRate = 20f;
    public float staminaRegenRate = 15f;
    private float currentStamina;
    private bool canRun = true;

    [Header("Camera")]
    public float mouseSensitivity = 200f;
    public Transform playerCamera;
    private float xRotation = 0f;

    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        currentStamina = maxStamina;
        currentSpeed = walkSpeed;
    }

    void Update()
    {
        HandleGroundCheck();
        HandleCameraRotation();
        HandleRun(); // Обработка бега
        HandleMovement();
        HandleJump();
        ApplyGravity();
        UpdateStamina(); // Обновление стамины
    }

    private void HandleGroundCheck()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }

    private void HandleRun()
    {
        // Бежим только если зажат Shift, есть стамина и можем бежать
        bool wantToRun = Input.GetKey(KeyCode.LeftShift) && canRun;

        if (wantToRun && currentStamina > 0)
        {
            currentSpeed = runSpeed;
            currentStamina -= staminaDepletionRate * Time.deltaTime;
        }
        else
        {
            currentSpeed = walkSpeed;
        }
    }

    private void UpdateStamina()
    {
        // Восстанавливаем стамину, когда не бежим
        if (!Input.GetKey(KeyCode.LeftShift) && isGrounded)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        }

        // Блокировка бега при нулевой стамине
        if (currentStamina <= 0)
        {
            canRun = false;
        }
        else if (currentStamina > maxStamina * 0.3f)
        {
            canRun = true;
        }
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    private void HandleCameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void HandleMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * currentSpeed * Time.deltaTime);
    }

    private void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    // Для отображения стамины в UI (опционально)
    public float GetStaminaNormalized()
    {
        return currentStamina / maxStamina;
    }
}