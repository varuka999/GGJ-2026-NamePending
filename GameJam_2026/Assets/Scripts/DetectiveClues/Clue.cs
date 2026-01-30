using UnityEngine;

public abstract class Clue : MonoBehaviour
{
    //determines if the clue should be highlghted/clickable in detective mode
    [SerializeField] bool active = false;
    protected Material material = null;

    Renderer sprite = null;
    Collider2D hitbox = null;

    bool visibleHighlight = false;

    [SerializeField] string textWhenClicked = "";

    [SerializeField] protected bool visibleOutsideDetective = true;

    [SerializeField] bool hideAfterInteract = false;
    
    bool revealed = false;
    

    protected virtual void Awake()
    {
        GetComponent<SpriteRenderer>().material = Resources.Load<Material>("MasterShader");
        SetActive(active);
        sprite = GetComponent<Renderer>();
        hitbox = GetComponent<Collider2D>();
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
        if (active && !isActive && !hideAfterInteract)
        {
            revealed = true;
        }
        active = isActive;
        UpdateHighlight();
    }

    protected virtual void UpdateHighlight()
    {
        if (active && (visibleHighlight != GameManager.Instance.GetDetectiveView()))
        {
            material = GetComponent<SpriteRenderer>().material;
            visibleHighlight = GameManager.Instance.GetDetectiveView();
            if (visibleHighlight)
            {
                if (!visibleOutsideDetective)
                {
                    sprite.enabled = true;
                    hitbox.enabled = true;
                }
                StartHighlight();
            }
            else
            {
                EndHighlight();
                if (!visibleOutsideDetective)
                {
                    sprite.enabled = false;
                    hitbox.enabled = false;
                }
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
        material.SetInt("_Saturation",0);
    }

    protected virtual void EndHighlight()
    {
        if (revealed)
        {
            //just disable outline
            material.SetInt("_Outline",0);
        }
        else
        {
            //set back to default material
            material = GameManager.Instance.GetMaterial();
            GetComponent<SpriteRenderer>().material = material;
        }
    }

    public virtual void StartInteract()
    {
        if (textWhenClicked != "")
        {
            //Show text with UI
            Debug.Log(textWhenClicked);
        }
        OnInteract();
    }

    public abstract void OnInteract();
}
