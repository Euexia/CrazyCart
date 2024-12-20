using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Mouvement")]
    public float speed = 5f;
    public float sensitivity = 2f;

    private Rigidbody rb;
    private Transform cameraTransform;

    private float xRotation = 0f;

    [Header("Contrôles")]
    public bool canLook = true;
    private bool isRotating = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        if (canLook)
        {
            RotatePlayer();
            UpdatePlayerRotationWithKeys();
        }
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        if (isRotating)
        {
            moveX = 0f;
        }

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        rb.MovePosition(rb.position + move * speed * Time.fixedDeltaTime);
    }

    void RotatePlayer()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        transform.Rotate(Vector3.up * mouseX);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    void UpdatePlayerRotationWithKeys()
    {
        float rotateInput = 0f;
        float keySensitivity = 90f;
        isRotating = false;

        if (Input.GetKey(KeyCode.A))
        {
            rotateInput = -1f;
            isRotating = true;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            rotateInput = 1f;
            isRotating = true;
        }

        transform.Rotate(Vector3.up * rotateInput * keySensitivity * Time.deltaTime);
    }

    public void LockCamera(bool lockCamera)
    {
        canLook = !lockCamera;
    }
}
