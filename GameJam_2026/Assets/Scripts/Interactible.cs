using UnityEngine;

public abstract class Interactible : MonoBehaviour
{
    [SerializeField] private float interactionCooldown = 0.0f;
    private float interactionTimer = 0.0f;

    public virtual void Update()
    {
        if (interactionTimer > 0)
        {
            interactionTimer -= Time.deltaTime;
        }
    }

    public bool StartInteract()
    {
        if (interactionTimer <= 0)
        {
            interactionTimer = 0;
            OnInteract();
            interactionTimer = interactionCooldown;
            return true;
        }
        return false;
    }

    //overwrite this with whatever behavior should happen on interaction
    public abstract void OnInteract();
}
