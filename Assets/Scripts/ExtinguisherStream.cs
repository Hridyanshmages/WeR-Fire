using UnityEngine;

public enum ExtinguisherType { Water, Foam, CO2, ABC_DryChemical }

public class ExtinguisherStream : MonoBehaviour
{
    [SerializeField] private ExtinguisherType streamType;
    [SerializeField] private float suppressionPower = 25f;

    private void OnParticleCollision(GameObject other)
    {
        if (other.TryGetComponent<FireSource>(out var fire))
        {
            fire.ApplyExtinguisher(streamType, suppressionPower);
        }
    }
}