using UnityEditor.UI;
using UnityEngine;

public class MaskPickup : Interactible
{
    [SerializeField] MaskType maskType;

    public override void OnInteract()
    {
        FindFirstObjectByType<PlayerController>().ObtainMask(maskType);
        FindFirstObjectByType<PlayerController>().ChangeMaskOnPickup(maskType);
        FindAnyObjectByType<PlayerController>().GetComponent<Animator>().SetTrigger("G_FirstEquip");
        GetComponent<Collider2D>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;

        UIManager.Instance.UpdateControlsText((int)maskType);
    }
}
