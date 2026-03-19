using UnityEngine;
using UnityEngine.AI;

public class AssistantController : MonoBehaviour
{
    public Transform playerTarget; // This is now your Assistant_Destination point
    public Transform vrHeadset;    // We will drag your Main Camera here!
    private NavMeshAgent agent;
    private Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (playerTarget != null)
        {
            agent.SetDestination(playerTarget.position);
            animator.SetBool("isWalking", true);
        }
    }

    void Update()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            animator.SetBool("isWalking", false); 
            
            // Look at the VR Headset instead of the floor target
            if (vrHeadset != null)
            {
                Vector3 lookDirection = vrHeadset.position - transform.position;
                lookDirection.y = 0; 
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection), Time.deltaTime * 5f);
            }
        }
    }
}