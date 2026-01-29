using UnityEngine;

public abstract class Clue : MonoBehaviour
{
    //determines if the clue should be highlghted/clickable in detective mode
    [SerializeField] bool active = false;
    protected Material material = null;

    bool visibleHighlight = false;

    protected virtual void Awake()
    {
        GetComponent<SpriteRenderer>().material = Resources.Load<Material>("MasterShader");
        material = GetComponent<SpriteRenderer>().material;
        SetActive(active);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        UpdateHighlight();
    }

    public bool GetActive()
    {
        return active;
    }

    public void SetActive(bool isActive)
    {
        active = isActive;
        UpdateHighlight();
    }

    protected virtual void UpdateHighlight()
    {
        if (active && (visibleHighlight != GameManager.Instance.GetDetectiveView()))
        {
            visibleHighlight = GameManager.Instance.GetDetectiveView();
            if (visibleHighlight)
            {
                StartHighlight();
            }
            else
            {
                EndHighlight();
            }
        }
        else if (!active)
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

    public abstract void OnInteract();
}
