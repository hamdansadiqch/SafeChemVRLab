using System.Collections; 
using UnityEngine;
using UnityEngine.AI;

public class AssistantController : MonoBehaviour
{
    [Header("Destinations")]
    [Tooltip("Where he walks right when the game starts")]
    public Transform initialDestination;
    
    [Tooltip("Drag the Main Camera (from XR Origin) here")]
    public Transform playerCamera;

    [Header("Audio")]
    public AudioSource assistantVoice; 
    public AudioClip initialGreeting;
    
    [Tooltip("Drag the second audio file here (plays right after the first)")]
    public AudioClip secondGreeting; 

    [Tooltip("Drag the third audio file here (plays right after the second)")]
    public AudioClip thirdGreeting; 

    [Tooltip("Drag the fourth audio file here (plays right after the third)")]
    public AudioClip fourthGreeting; 

    private NavMeshAgent agent;
    private Animator animator;
    
    private bool hasStartedWalking = false; 
    private bool hasPlayedGreeting = false; 

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (initialDestination != null)
        {
            StartCoroutine(WaitThenWalk());
        }
    }

    IEnumerator WaitThenWalk()
    {
        yield return new WaitForSeconds(5f);
        MoveToStation(initialDestination);
    }

    void Update()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            // Stop the walking animation
            animator.SetBool("isWalking", false);
            
            // Look at the player
            FacePlayer(); 
            
            // If he arrived, and hasn't spoken yet, start the audio sequence!
            if (hasStartedWalking && !hasPlayedGreeting && assistantVoice != null)
            {
                hasPlayedGreeting = true; 
                StartCoroutine(PlayAudioSequence()); 
            }
        }
    }

    IEnumerator PlayAudioSequence()
    {
        // --- ADDED THIS: Force the animator straight into your "talking" state ---
        animator.Play("talking");

        // Play the first audio
        if (initialGreeting != null)
        {
            assistantVoice.clip = initialGreeting;
            assistantVoice.Play();
            yield return new WaitForSeconds(initialGreeting.length);
        }

        // Play the second audio
        if (secondGreeting != null)
        {
            assistantVoice.clip = secondGreeting;
            assistantVoice.Play();
            yield return new WaitForSeconds(secondGreeting.length);
        }

        // Play the third audio 
        if (thirdGreeting != null)
        {
            assistantVoice.clip = thirdGreeting;
            assistantVoice.Play();
            yield return new WaitForSeconds(thirdGreeting.length); 
        }

        // Play the fourth audio 
        if (fourthGreeting != null)
        {
            assistantVoice.clip = fourthGreeting;
            assistantVoice.Play();
            
            // --- ADDED THIS: Wait for the 4th audio to actually finish ---
            yield return new WaitForSeconds(fourthGreeting.length);
        }

        // --- ADDED THIS: All audio is done, force him back to your "idle" state ---
        animator.Play("idle");
    }

    public void MoveToStation(Transform newTarget)
    {
        agent.SetDestination(newTarget.position);
        animator.SetBool("isWalking", true); 
        hasStartedWalking = true; 
    }

    void FacePlayer()
    {
        if (playerCamera != null)
        {
            Vector3 direction = (playerCamera.position - transform.position).normalized;
            direction.y = 0; 
            
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }
}