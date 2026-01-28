using UnityEngine;

public abstract class Clue : MonoBehaviour
{
    //determines if the clue should be highlghted/clickable in detective mode
    [SerializeField] bool active = false;
    protected Material material = null;

    public virtual void Awake()
    {
        material = GetComponent<SpriteRenderer>().material;
        SetActive(active);
    }

    // Update is called once per frame
    public virtual void Update()
    {
        
    }

    public bool GetActive()
    {
        return active;
    }

    public void SetActive(bool isActive)
    {
        active = isActive;
        if (active)
        {
            BeginActive();
        }
        else
        {
            EndActive();
        }
    }

    public virtual void BeginActive()
    {
        //can override this to have more interesting highlight effects
        //just make sure to also override EndActive to disable them
        material.SetInt("_Outline",1);
    }

    public virtual void EndActive()
    {
        material.SetInt("_Outline",0);
    }

    public abstract void OnInteract();
}
