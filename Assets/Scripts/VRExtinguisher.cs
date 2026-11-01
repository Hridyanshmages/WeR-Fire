using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class VRExtinguisher : MonoBehaviour
{
    [Header("Extinguisher Properties")]
    [SerializeField] private ExtinguisherType extinguisherType;
    
    [Header("VFX Prefab")]
    [Tooltip("Drag your Extinguisher Particle System Prefab here")]
    [SerializeField] private GameObject dischargePrefab;
    [Tooltip("Transform where the particles will spawn (e.g., the nozzle). Defaults to this object if empty.")]
    [SerializeField] private Transform nozzlePoint;

    private ParticleSystem dischargeParticles; // Private runtime instance
    private XRGrabInteractable bodyGrabInteractable;

    private void Awake()
    {
        bodyGrabInteractable = GetComponent<XRGrabInteractable>();

        // Spawn the prefab at runtime and get its Particle System
        if (dischargePrefab != null)
        {
            Transform spawnLocation = nozzlePoint != null ? nozzlePoint : transform;
            GameObject spawnedVFX = Instantiate(dischargePrefab, spawnLocation);
            
            // Ensure the spawned prefab aligns perfectly with the nozzle
            spawnedVFX.transform.localPosition = Vector3.zero;
            spawnedVFX.transform.localRotation = Quaternion.identity;
            
            dischargeParticles = spawnedVFX.GetComponentInChildren<ParticleSystem>();

            // Ensure it does not play immediately on startup
            if (dischargeParticles != null)
            {
                dischargeParticles.Stop();
            }
        }
    }

    private void OnEnable()
    {
        bodyGrabInteractable.activated.AddListener(OnTriggerSqueezed);
        bodyGrabInteractable.deactivated.AddListener(OnTriggerReleased);
    }

    private void OnDisable()
    {
        bodyGrabInteractable.activated.RemoveListener(OnTriggerSqueezed);
        bodyGrabInteractable.deactivated.RemoveListener(OnTriggerReleased);
    }

    private void OnTriggerSqueezed(ActivateEventArgs args)
    {
        if (dischargeParticles != null) dischargeParticles.Play();
    }

    private void OnTriggerReleased(DeactivateEventArgs args)
    {
        if (dischargeParticles != null) dischargeParticles.Stop();
    }
}