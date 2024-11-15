using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovementController : MonoBehaviour
{
    public float baseMoveSpeed = 5f; // Vitesse de déplacement de base
    private Rigidbody rb;
    private Animator animator;
    private Vector3 movementInput;

    private ImprovementManager improvementManager; // Référence au système d'améliorations

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        // Empêche les rotations accidentelles
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        // Récupère l'ImprovementManager dans la scène
        improvementManager = FindObjectOfType<ImprovementManager>();
        if (improvementManager == null)
        {
            Debug.LogError("ImprovementManager non trouvé dans la scène.");
        }
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
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

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
        // Calcule la vitesse totale (base + bonus d'amélioration)
        float totalMoveSpeed = baseMoveSpeed + (improvementManager?.moveSpeedBonus ?? 0f);

        Vector3 moveDirection = movementInput * totalMoveSpeed;
        rb.velocity = new Vector3(moveDirection.x, rb.velocity.y, moveDirection.z);

        if (movementInput != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movementInput);
            rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 10f));
        }
    }
}
