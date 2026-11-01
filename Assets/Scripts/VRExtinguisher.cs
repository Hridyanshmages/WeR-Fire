using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class VRExtinguisher : MonoBehaviour
{
    [Header("Extinguisher Properties")]
    [SerializeField] private ExtinguisherType extinguisherType;
    [SerializeField] private float suppressionPower = 25f;
    [SerializeField] private float maxSprayDistance = 5f;
    
    [Header("VFX Prefab")]
    [Tooltip("Drag your Extinguisher Particle System Prefab here")]
    [SerializeField] private GameObject dischargePrefab;
    [Tooltip("Transform where the raycast and particles will spawn (e.g., the nozzle). Defaults to this object if empty.")]
    [SerializeField] private Transform nozzlePoint;

    private ParticleSystem dischargeParticles;
    private XRGrabInteractable bodyGrabInteractable;
    private bool isSpraying = false;

    private void Awake()
    {
        bodyGrabInteractable = GetComponent<XRGrabInteractable>();

        if (dischargePrefab != null)
        {
            Transform spawnLocation = nozzlePoint != null ? nozzlePoint : transform;
            GameObject spawnedVFX = Instantiate(dischargePrefab, spawnLocation);
            
            spawnedVFX.transform.localPosition = Vector3.zero;
            spawnedVFX.transform.localRotation = Quaternion.identity;
            
            dischargeParticles = spawnedVFX.GetComponentInChildren<ParticleSystem>();

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
        isSpraying = true;
        if (dischargeParticles != null) dischargeParticles.Play();
    }

    private void OnTriggerReleased(DeactivateEventArgs args)
    {
        isSpraying = false;
        if (dischargeParticles != null) dischargeParticles.Stop();
    }

    private void Update()
    {
        if (!isSpraying) return;

        Transform rayOrigin = nozzlePoint != null ? nozzlePoint : transform;

        // Perform the raycast
        if (Physics.Raycast(rayOrigin.position, rayOrigin.forward, out RaycastHit hit, maxSprayDistance))
        {
            Debug.Log($"[VRExtinguisher] Raycast hit: {hit.collider.name}");
            
            // Check if we hit a fire
            if (hit.collider.TryGetComponent<FireSource>(out var fire))
            {
                Debug.Log($"[VRExtinguisher] Raycast hit FireSource. Applying extinguisher.");
                fire.ApplyExtinguisher(extinguisherType, suppressionPower);
            }
        }
    }
}