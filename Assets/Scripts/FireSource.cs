using UnityEngine;

public enum FireClass { ClassA, ClassB, ClassC }

public class FireSource : MonoBehaviour
{
    [SerializeField] private FireClass fireType = FireClass.ClassA;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private ParticleSystem fireParticles;
    
    private float currentHealth;

    public FireClass Type => fireType;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void ApplyExtinguisher(ExtinguisherType agentType, float dpsRate)
    {
        bool isEffective = IsEffectiveAgent(agentType);

        if (isEffective)
        {
            currentHealth -= dpsRate * Time.deltaTime;
            currentHealth = Mathf.Max(0, currentHealth);
            
            // Scale particle effect intensity with remaining health
            var main = fireParticles.main;
            main.startSize = Mathf.Lerp(0.1f, 1.5f, currentHealth / maxHealth);

            if (currentHealth <= 0)
            {
                Extinguish();
            }
        }
        else
        {
            // Adverse reaction: wrong extinguisher used
            HandleWrongAgent(agentType);
        }
    }

    private bool IsEffectiveAgent(ExtinguisherType agent)
    {
        return fireType switch
        {
            FireClass.ClassA => agent == ExtinguisherType.Water || agent == ExtinguisherType.ABC_DryChemical,
            FireClass.ClassB => agent == ExtinguisherType.CO2 || agent == ExtinguisherType.ABC_DryChemical || agent == ExtinguisherType.Foam,
            FireClass.ClassC => agent == ExtinguisherType.CO2 || agent == ExtinguisherType.ABC_DryChemical,
            _ => false
        };
    }

    private void HandleWrongAgent(ExtinguisherType agent)
    {
        // Example: spraying water on electrical or oil fire causes flair-up
        if (agent == ExtinguisherType.Water && (fireType == FireClass.ClassB || fireType == FireClass.ClassC))
        {
            currentHealth = Mathf.Min(maxHealth * 1.5f, currentHealth + 10f * Time.deltaTime);
            // Trigger feedback (sparks/flame explosion/audio cue)
        }
    }

    private void Extinguish()
    {
        if (fireParticles != null)
        {
            fireParticles.Stop();
        }
        
        // Disable the collider so the extinguisher stops registering hits
        var col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }
    }
}