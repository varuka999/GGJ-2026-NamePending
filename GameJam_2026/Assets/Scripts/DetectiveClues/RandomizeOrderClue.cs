using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using UnityEngine;

public class RandomizeOrderClue : Clue
{

    [SerializeField] RandomizeOrderClue main;

    int orderIndex=0;

    bool pressed = false;

    bool failed = false;

    [SerializeField] SpriteRenderer numberSprite;
    [SerializeField] List<Sprite> numbers;

    [SerializeField] bool isLast;
    [SerializeField] bool isMain;
    [SerializeField] List<RandomizeOrderClue> allClues;
    [SerializeField] List<GameObject> toggleObjects;
    protected override void Update()
    {
        base.Update();
        if (isMain)
        {
            bool allCluesPressed = true;
            foreach(RandomizeOrderClue clue in allClues)
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
        foreach(RandomizeOrderClue clue in allClues)
        {
            clue.failed = false;
            clue.pressed = false;
        }
    }

    void ResetAllClues()
    {
        List<int> indexes = new List<int>();
        for (int i = 0; i < allClues.Count-1; i++)
        {
            indexes.Add(i);
        }
        indexes = indexes.OrderBy( x => UnityEngine.Random.value ).ToList( );
        int indexIndex = 0;
        foreach(RandomizeOrderClue clue in allClues)
        {
            clue.failed = false;
            clue.SetActive(true);
            clue.StartHighlight();
            clue.pressed = false;
            if(clue.isLast)
            {
                clue.orderIndex = allClues.Count - 1;
            }
            else
            {
                clue.orderIndex = indexes[indexIndex];
                numberSprite.sprite = numbers[clue.orderIndex];
                indexIndex++;
            }
        }
    }

    public override void OnInteract()
    {
        if (orderIndex == 0)
        {
            Success();            
        }
        else
        {
            int previousIndex = orderIndex - 1;
            RandomizeOrderClue previousClue = main.getByIndex(previousIndex);
            if (previousClue.pressed)
            {
                Success();
            }
            else
            {
                Fail();
            }
        }  
    }

    RandomizeOrderClue getByIndex(int index)
    {
        foreach(RandomizeOrderClue clue in allClues)
        {
            if(clue.orderIndex == index)
            {
                return clue;
            }
        }
        Debug.Log("no clue was assigned this index");
        return null;
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
