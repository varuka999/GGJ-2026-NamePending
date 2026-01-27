using System.Collections.Generic;
using UnityEngine;

public class ToggleObjectsInteractible : Interactible
{
    
    [SerializeField] private List<GameObject> toggleObjects;

    public override void OnInteract()
    {
        
        foreach (GameObject i in toggleObjects)
        {
            i.SetActive(!i.activeSelf);
        }
    }
}
