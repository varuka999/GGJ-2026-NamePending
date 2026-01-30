using UnityEngine;

public abstract class Interactible : MonoBehaviour
{
    [SerializeField] protected bool active = true;
    [SerializeField] private float interactionCooldown = 0.0f;
    private float interactionTimer = 0.0f;

    [SerializeField] protected bool onlyHighlightWhenInContact = true;
    Material material = null;

    protected virtual void Update()
    {
        if (interactionTimer > 0)
        {
            interactionTimer -= Time.deltaTime;
        }
        if (GameManager.Instance.GetDetectiveView())
        {
            material.SetInt("_Saturation",1);
        }
        else
        {
            material.SetInt("_Saturation",0);
        }
    }

    public void InitMaterial()
    {
        material = GetComponent<SpriteRenderer>().material;
        SetActive(active);
    }

    public void SetActive(bool isActive)
    {
        active = isActive;
        if (active && !onlyHighlightWhenInContact)
        {
            StartHighlight();
        }
    }

    public bool GetActive()
    {
        return active;
    }

    public bool StartInteract()
    {
        if (interactionTimer <= 0 && active)
        {
            interactionTimer = 0;
            OnInteract();
            interactionTimer = interactionCooldown;
            return true;
        }
        return false;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" && active  && onlyHighlightWhenInContact)
        {
            StartHighlight();
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Player" && active && onlyHighlightWhenInContact)
        {
            EndHighlight();
        }
    }

    protected virtual void StartHighlight()
    {
        //can override this to have more interesting highlight effects
        //just make sure to also override EndHighlight to disable them
        material.SetInt("_Outline",1);
    }

    protected virtual void EndHighlight()
    {
        material.SetInt("_Outline",0);
    }

    //overwrite this with whatever behavior should happen on interaction
    public abstract void OnInteract();
}
