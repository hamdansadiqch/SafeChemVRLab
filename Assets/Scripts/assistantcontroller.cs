using UnityEngine;
using UnityEngine.AI;

public class AssistantController : MonoBehaviour
{
    [Header("Destinations")]
    [Tooltip("Where he walks right when the game starts")]
    public Transform initialDestination;
    
    [Tooltip("Drag the Main Camera (from XR Origin) here")]
    public Transform playerCamera;

    private NavMeshAgent agent;
    private Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // 1. Instantly walk to the starting point to greet the user
        if (initialDestination != null)
        {
            MoveToStation(initialDestination);
        }
    }

    void Update()
    {
        // 2. Check if he has reached wherever he is currently going
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            // Stop the walking animation
            animator.SetBool("isWalking", false);
            
            // Look at the player
            FacePlayer(); 
        }
    }

    // 3. The Teleport Pad will trigger this!
    public void MoveToStation(Transform newTarget)
    {
        agent.SetDestination(newTarget.position);
        
        // Trigger the walking animation
        animator.SetBool("isWalking", true); 
    }

    void FacePlayer()
    {
        if (playerCamera != null)
        {
            Vector3 direction = (playerCamera.position - transform.position).normalized;
            direction.y = 0; // Keep him standing straight
            
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }
}