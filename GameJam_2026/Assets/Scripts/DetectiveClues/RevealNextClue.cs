using System.Collections.Generic;
using UnityEngine;

public class RevealNextClue : Clue
{
    [SerializeField] List<Clue> nextClues = new List<Clue>();


    [SerializeField] bool deleteSelf = false;

    public override void OnInteract()
    {
        foreach (Clue clue in nextClues)
        {
            clue.SetActive(true);
        }
        SetActive(false);
        if (deleteSelf)
        {
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<Collider2D>().enabled = false;
            revealed = false;
        }
        
    }
}
