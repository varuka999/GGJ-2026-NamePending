using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class TimerButtonInteractible : Interactible
{
    [SerializeField] bool isMain;

    [SerializeField] List<TimerButtonInteractible> otherButtons;
    [SerializeField] float timeToPressAllButtons;
    protected float timer = 0.0f;

    bool pressed = false;
    public bool Pressed {get {return pressed;}}

    [SerializeField] private Sprite switchOn;
    [SerializeField] private Sprite switchOff;

    [SerializeField] private List<GameObject> toggleObjects;

    protected void Awake()
    {
        if (isMain)
        {
            otherButtons.Add(this);
        }
    }

    protected override void Update()
    {
        base.Update();
        if (isMain)
        {
            bool allButtonsPressed = true;
            bool noButtonsPressed = true;
            foreach (TimerButtonInteractible button in otherButtons)
            {
                if(button.pressed)
                {
                    noButtonsPressed = false;
                }
                else
                {
                    allButtonsPressed = false;
                }
            }
            if (allButtonsPressed)
            {
                OnCompleted();
            }
            else if (!noButtonsPressed)
            {
                if (timer <= 0)
                {
                    timer = timeToPressAllButtons;
                }
                else
                {
                    timer -= Time.deltaTime;
                    if (timer <= 0)
                    {
                        OnFailure();
                    }
                }
            }
        }

    }

    public override void OnInteract()
    {
        pressed = true;
        GetComponent<SpriteRenderer>().sprite = switchOn;
    }

    protected virtual void OnCompleted()
    {
        foreach (GameObject i in toggleObjects)
        {
            Renderer sprite = i.GetComponent<Renderer>();
            if (sprite != null)
            {
                sprite.enabled = !sprite.enabled;
            }
            Collider2D[] colliders = i.GetComponents<Collider2D>();
            foreach (Collider2D c2d in colliders)
            {
                c2d.enabled = !c2d.enabled;
            }
        }
        foreach (TimerButtonInteractible button in otherButtons)
        {
            button.timer = timeToPressAllButtons;
            button.pressed = false;
            button.GetComponent<SpriteRenderer>().sprite = switchOff;
            if (!button.CheckPlayerInBounds())
            {
                button.EndHighlight();
            }
        }
    }

    protected virtual void OnFailure()
    {
        foreach (TimerButtonInteractible button in otherButtons)
        {
            button.timer = timeToPressAllButtons;
            button.pressed = false;
            button.GetComponent<SpriteRenderer>().sprite = switchOff;
            if (!button.CheckPlayerInBounds())
            {
                button.EndHighlight();
            }
        }
        
    }

    bool CheckPlayerInBounds()
    {
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        Vector2 point = transform.position;
        point += box.offset;
        Vector2 size = box.size;

        Collider2D[] objectsHit = Physics2D.OverlapBoxAll(point,size,0);

        foreach (Collider2D i in objectsHit)
        {
            if (i.tag == "Player")
            {
                return true;
            }
        }
        return false;
    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Player" && active && onlyHighlightWhenInContact && !pressed)
        {
            EndHighlight();
        }
    }
}
