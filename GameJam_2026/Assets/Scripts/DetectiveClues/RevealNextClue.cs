using System.Collections.Generic;
using UnityEngine;

public class RevealNextClue : Clue
{
    [SerializeField] List<Clue> nextClues = new List<Clue>();

    public override void OnInteract()
    {
        foreach (Clue clue in nextClues)
        {
            clue.SetActive(true);
        }
        SetActive(false);
    }
}
