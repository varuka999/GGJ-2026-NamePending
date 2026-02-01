using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;

public class SpecificOrderClue : Clue
{

    [SerializeField] SpecificOrderClue previousClue = null;

    bool pressed = false;

    bool failed = false;

    [SerializeField] bool isMain;
    [SerializeField] List<SpecificOrderClue> allClues;
    [SerializeField] List<GameObject> toggleObjects;
    protected override void Update()
    {
        base.Update();
        if (isMain)
        {
            bool allCluesPressed = true;
            foreach(SpecificOrderClue clue in allClues)
            {
                if(clue.failed)
                {
                    ResetAllClues();
                    allCluesPressed = false;
                    break;
                }
                else if (!clue.pressed)
                {
                    allCluesPressed = false;
                }
            }
            if (allCluesPressed)
            {
                OnCompleted();
            }
        }
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
        foreach(SpecificOrderClue clue in allClues)
        {
            clue.failed = false;
            clue.pressed = false;
        }
    }

    void ResetAllClues()
    {
        foreach(SpecificOrderClue clue in allClues)
        {
            clue.failed = false;
            clue.SetActive(true);
            clue.StartHighlight();
            clue.pressed = false;
        }
    }

    public override void OnInteract()
    {
        if (previousClue == null)
        {
            Success();
        }
        else if (previousClue.pressed)
        {
            Success();
        }
        else
        {
            Fail();
        }
    }

    void Success()
    {
        pressed = true;
        SetActive(false);
    }

    void Fail()
    {
        failed = true;
    }
}
