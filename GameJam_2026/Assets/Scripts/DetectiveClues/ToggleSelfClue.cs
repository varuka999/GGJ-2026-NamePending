using UnityEngine;

public class ToggleSelfClue : Clue
{
    public override void OnInteract()
    {
        if (!visibleOutsideDetective)
        {
            visibleOutsideDetective = true;
        }
        else
        {
            gameObject.SetActive(false);
        }
        SetActive(false);
    }
}
