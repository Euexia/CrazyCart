using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovementController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody rb;
    private Animator animator;
    private Vector3 movementInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void Update()
    {
        GetMovementInput();
        UpdateAnimation();
    }

    void FixedUpdate()
    {
        MoveCharacter();
    }

    private void GetMovementInput()
    {
        float moveX = 0;
        float moveZ = 0;

        if (Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.UpArrow)) moveZ = 1;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) moveZ = -1;
        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftArrow)) moveX = -1;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) moveX = 1;

        Vector3 rawInput = new Vector3(moveX, 0, moveZ).normalized;
        movementInput = IsoVectorConvert(rawInput);
    }

    private Vector3 IsoVectorConvert(Vector3 vector)
    {
        Quaternion rotation = Quaternion.Euler(0, 45f, 0);
        Matrix4x4 isoMatrix = Matrix4x4.Rotate(rotation);
        return isoMatrix.MultiplyPoint3x4(vector);
    }

    private void UpdateAnimation()
    {
        bool isWalking = movementInput != Vector3.zero;
        animator.SetBool("isWalking", isWalking);
    }


    private void MoveCharacter()
    {
        Vector3 moveDirection = movementInput * moveSpeed;
        rb.velocity = new Vector3(moveDirection.x, rb.velocity.y, moveDirection.z);

        if (movementInput != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movementInput);
            rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 10f));
        }
    }
}
