using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables; // Required for XRI 3.x

[RequireComponent(typeof(XRGrabInteractable))]
public class FireExtinguisher : MonoBehaviour
{
    public ParticleSystem extinguisherParticles;

    [Header("Audio")]
    public AudioSource extinguisherAudio;

    private XRGrabInteractable grabInteractable;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
    }

    private void OnEnable()
    {
        // Subscribe to XRI events
        grabInteractable.activated.AddListener(OnTriggerSqueezed);     // Squeeze trigger
        grabInteractable.deactivated.AddListener(OnTriggerReleased);   // Release trigger
        grabInteractable.selectExited.AddListener(OnDropped);          // Drop object
    }

    private void OnDisable()
    {
        // Unsubscribe to prevent memory leaks
        grabInteractable.activated.RemoveListener(OnTriggerSqueezed);
        grabInteractable.deactivated.RemoveListener(OnTriggerReleased);
        grabInteractable.selectExited.RemoveListener(OnDropped);
    }

    private void OnTriggerSqueezed(ActivateEventArgs args)
    {
        extinguisherParticles.Play();

        if (extinguisherAudio != null && !extinguisherAudio.isPlaying)
        {
            extinguisherAudio.Play();
        }

        Debug.Log("Trigger squeezed, playing particles and sound.");
    }

    private void OnTriggerReleased(DeactivateEventArgs args)
    {
        StopExtinguisher();
        Debug.Log("Trigger released, stopping particles and sound.");
    }

    private void OnDropped(SelectExitEventArgs args)
    {
        // Ensure everything stops if the user drops the extinguisher while shooting
        StopExtinguisher();
    }

    private void StopExtinguisher()
    {
        if (extinguisherParticles.isPlaying)
        {
            extinguisherParticles.Stop();
        }

        if (extinguisherAudio != null && extinguisherAudio.isPlaying)
        {
            extinguisherAudio.Stop();
        }
    }
}