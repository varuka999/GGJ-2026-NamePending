using UnityEngine;

public class ButtonInteractable : Interactible
{
    public override void OnInteract()
    {
        Debug.Log("Button Pressed");
    }
}
