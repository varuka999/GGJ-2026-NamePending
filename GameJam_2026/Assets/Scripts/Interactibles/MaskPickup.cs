using UnityEditor.UI;
using UnityEngine;

public class MaskPickup : Interactible
{
    [SerializeField] MaskType maskType;

    public override void OnInteract()
    {
        FindFirstObjectByType<PlayerController>().ObtainMask(maskType);
        GetComponent<Collider2D>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
    }
}
