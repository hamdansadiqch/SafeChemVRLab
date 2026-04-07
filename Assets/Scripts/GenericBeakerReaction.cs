using UnityEngine;

public class GenericBeakerReaction : MonoBehaviour
{
    [Header("Visual & Sound Effects")]
    [Tooltip("Drag the smoke/fumes particle system here.")]
    public ParticleSystem fumesVFX;
    
    [Tooltip("Drag the bubbles particle system here.")]
    public ParticleSystem bubblesVFX;

    [Tooltip("Drag the Audio Source component from THIS object here.")]
    public AudioSource audioSource;

    [Tooltip("Drag the short reaction sound file (e.g., fizzing) here.")]
    public AudioClip reactionClip;

    [Header("Settings")]
    [Tooltip("Time in seconds before another reaction can be triggered.")]
    public float reactionCooldown = 1.0f;
    
    private float nextReactionTime = 0f;

    private void Start()
    {
        // Safety check to ensure an AudioSource is available
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Don't trigger if we are on cooldown
        if (Time.time < nextReactionTime) return;

        // Check the tag of the object falling into the beaker
        if (other.CompareTag("Acid") || other.CompareTag("Base"))
        {
            TriggerReactionEffects();
            nextReactionTime = Time.time + reactionCooldown;
        }
    }

    private void TriggerReactionEffects()
    {
        // 1. Play the Particle Effects
        if (fumesVFX != null) fumesVFX.Play();
        if (bubblesVFX != null) bubblesVFX.Play();

        // 2. Play the Sound Effect once
        if (audioSource != null && reactionClip != null)
        {
            audioSource.PlayOneShot(reactionClip);
        }
    }
}