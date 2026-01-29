using System.Collections.Generic;
using UnityEngine;

public class ToggleObjectClue : Clue
{
    [SerializeField] List<GameObject> toggleObjects;

    public override void OnInteract()
    {
        foreach (GameObject i in toggleObjects)
        {
            i.SetActive(!i.activeSelf);
        }
        SetActive(false);
    }
}
