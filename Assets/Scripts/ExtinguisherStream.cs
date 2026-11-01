using UnityEngine;

public enum ExtinguisherType { Water, Foam, CO2, ABC_DryChemical }

public class ExtinguisherStream : MonoBehaviour
{
    [SerializeField] private ExtinguisherType streamType;
    [SerializeField] private float suppressionPower = 25f;

    private void OnParticleCollision(GameObject other)
    {
        // Log every object the particles hit
        Debug.Log($"[ExtinguisherStream] Particle hit: {other.name}");

        if (other.TryGetComponent<FireSource>(out var fire))
        {
            Debug.Log($"[ExtinguisherStream] Found FireSource on {other.name}. Applying extinguisher.");
            fire.ApplyExtinguisher(streamType, suppressionPower);
        }
        else
        {
            Debug.Log($"[ExtinguisherStream] {other.name} does NOT have a FireSource script attached.");
        }
    }
}