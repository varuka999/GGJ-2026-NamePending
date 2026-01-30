using System.Collections.Generic;
using UnityEngine;

public class ToggleObjectClue : Clue
{
    [SerializeField] List<GameObject> toggleObjects;

    public override void OnInteract()
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
        SetActive(false);
    }
}
