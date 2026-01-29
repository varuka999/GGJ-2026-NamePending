using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum MaskType
{
    None,
    Ghost,
    Detective,
}

public class PlayerController : MonoBehaviour
{
    //input
    private PlayerInput input = null;
    private InputAction moveAction = null;
    private InputAction interactAction = null;
    private InputAction abilityAction = null;
    private InputAction cycleMaskAction = null;
    private InputAction clickAction = null;


    [SerializeField] float moveSpeed = 8.0f;

    //components
    private Rigidbody2D rb = null;
    private Animator animator = null;
    private BoxCollider2D box = null;


    private Vector3 animatorDirection = Vector2.down;
    private List<Interactible> interactibles = new List<Interactible>();

    private List<MaskType> ownedMasks = new List<MaskType>();
    private int currentMaskIndex = 0;

    //detective mode stuff
    [SerializeField] private bool inDetectiveMode = false;

    //ghost dash stuff
    [SerializeField] float dashDistance = 3.5f;
    [SerializeField] float dashSpeed = 12.0f;
    private Vector3 dashDestination = new Vector3(0, 0, -1);


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        input = new PlayerInput();
        moveAction = input.Player.Move;
        interactAction = input.Player.Interact;
        abilityAction = input.Player.Ability;
        cycleMaskAction = input.Player.CycleMask;
        clickAction = input.Player.Click;

        rb = GetComponent<Rigidbody2D>();
        box = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        animator.SetFloat("X", 0);
        animator.SetFloat("Y", 0);
        animator.SetBool("isMoving", false);
        interactAction.performed += OnInteract;
        abilityAction.performed += OnAbility;
        cycleMaskAction.performed += OnCycleMask;
        clickAction.performed += OnClick;
        ownedMasks.Add(MaskType.None);

        //just for testing
        ObtainMask(MaskType.Ghost);
        ObtainMask(MaskType.Detective);
    }

    void OnEnable()
    {
        input.Enable();
        moveAction.Enable();
        interactAction.Enable();
        abilityAction.Enable();
        cycleMaskAction.Enable();
        clickAction.Enable();
    }

    void OnDisable()
    {
        input.Disable();
        moveAction.Disable();
        interactAction.Disable();
        abilityAction.Disable();
        cycleMaskAction.Disable();
        clickAction.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsDashing())
        {
            rb.linearVelocity = Vector2.zero;
            transform.position = Vector3.MoveTowards(transform.position, dashDestination, dashSpeed*Time.deltaTime);

            if (transform.position == dashDestination)
            {
                //end dash
                dashDestination = new Vector3(0, 0, -1);
                rb.simulated = true;
            }

        }
        else if (inDetectiveMode)
        {
            //Detective code
            rb.linearVelocity = Vector2.zero;

        }
        else
        {

            //Movement code
            Vector2 moveInput = moveAction.ReadValue<Vector2>();

            AnimationDirectionCheck("Move");

            Vector3 direction = moveInput.normalized; // where player is 
            rb.linearVelocity = direction * moveSpeed;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Interactible interactible = collision.gameObject.GetComponent<Interactible>();
        if (interactible != null)
        {
            interactibles.Add(interactible);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        Interactible interactible = collision.gameObject.GetComponent<Interactible>();
        if (interactible != null)
        {
            interactibles.Remove(interactible);
        }
    }

    void OnInteract(InputAction.CallbackContext context)
    {
        bool interacted = false;
        foreach (Interactible interactible in interactibles)
        {
            if (interactible.StartInteract())
            {
                interacted = true;
            }
        }

        if (interacted)
        {
          AnimationDirectionCheck("Interact");
        }
    }



    public bool IsDashing()
    {
        return !(dashDestination.z == -1);
    }

    void OnAbility(InputAction.CallbackContext context)
    {
        if (GetCurrentMask() == MaskType.Ghost && !IsDashing())
        {
            Vector2 point = transform.position + (dashDistance * animatorDirection);
            point += box.offset;
            Vector2 hitboxSize = box.size - new Vector2(0.05f, 0.05f);
            if (!Physics2D.OverlapBox(point, hitboxSize, 0.0f))
            {
                //start dash
                AnimationDirectionCheck("Dash");
                point -= box.offset;
                dashDestination.x = point.x;
                dashDestination.y = point.y;
                dashDestination.z = 0;

                rb.linearVelocity = Vector2.zero;
                rb.simulated = false;
            }
            else
            {
                //destination is not valid
            }
        }
        else if (GetCurrentMask() == MaskType.Detective)
        {
            ChangeDetectiveMode(!inDetectiveMode);
        }
    }

    void ChangeDetectiveMode(bool mode)
    {
        inDetectiveMode = mode;
        GameManager.Instance.SetDetectiveView(mode);
    }

    MaskType GetCurrentMask()
    {
        return ownedMasks[currentMaskIndex];
    }

    void ObtainMask(MaskType mask)
    {
        if (ownedMasks[0] == MaskType.None)
        {
            ownedMasks[0] = mask;
        }
        else
        {
            ownedMasks.Add(mask);
        }
    }

    void OnCycleMask(InputAction.CallbackContext context)
    {
        if (!IsDashing())
        {
            ChangeDetectiveMode(false);
            currentMaskIndex = (currentMaskIndex + 1) % ownedMasks.Count;
            Debug.Log(GetCurrentMask());
        }
    }

    void OnClick(InputAction.CallbackContext context)
    {
        if (inDetectiveMode)
        {
            Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(mouseScreenPos);

            Collider2D[] objectsHit = Physics2D.OverlapPointAll(mousePos);

            foreach (Collider2D collider in objectsHit)
            {
                Clue clue = collider.gameObject.GetComponent<Clue>();
                if (clue != null)
                {
                    if (clue.GetActive())
                    {
                        clue.OnInteract();
                    }
                }
            }
        }
    }

    void AnimationDirectionCheck(string animationName) 
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>(); // find players direction

        if (moveInput.y == 1 || moveInput.y == -1) // up or down
        {
            Vector3 direction = Vector3.zero;
            direction.y += moveInput.y;
            animatorDirection = direction;
        } 

        else if (moveInput.x == 1 || moveInput.x == -1) // left or right
        {
            Vector3 direction = Vector3.zero;
            direction.x += moveInput.x;
            animatorDirection = direction;
        } 

        switch (animationName) 
        {
            case "Interact":
                animator.SetFloat("X", animatorDirection.x);
                animator.SetFloat("Y", animatorDirection.y);
                animator.SetTrigger("Touch");
                break;

            case "Dash":
                animator.SetFloat("X", animatorDirection.x);
                animator.SetFloat("Y", animatorDirection.y);
                animator.SetTrigger("Dash");
                break;

             case "Move":

                if (moveInput == Vector2.zero)
                {
                    animator.SetBool("isMoving", false);
                }
                else
                {
                    animator.SetBool("isMoving", true);
                }

                animator.SetFloat("X", animatorDirection.x);
                animator.SetFloat("Y", animatorDirection.y);


                break;


            default:
                break;
        }
    }

}
