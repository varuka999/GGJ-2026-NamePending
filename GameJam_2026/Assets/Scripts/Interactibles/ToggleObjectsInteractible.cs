using System.Collections.Generic;
using UnityEngine;

public class ToggleObjectsInteractible : Interactible
{
    
    [SerializeField] private List<GameObject> toggleObjects;

    [SerializeField] private Sprite switchOn;
    [SerializeField] private Sprite switchOff;

    private bool isOn = false;

    public override void OnInteract()
    {
        isOn = !isOn;
        if(isOn)
        {
            GetComponent<SpriteRenderer>().sprite = switchOn;
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = switchOff;
        }
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
    }
}
