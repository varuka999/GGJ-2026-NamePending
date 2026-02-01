using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public enum MaskType
{
    None = -1,
    Detective = 0,
    Ghost = 1,
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
    private InputAction uiToggleAction = null;

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

    private bool isChangingMask = false;

    // particle stuff 
    [SerializeField] private ParticleSystem maskChangeVFX;
    

    public void PlayMaskVFX()
    {
        if (maskChangeVFX != null)
        {
            maskChangeVFX.Play();
        }
    }

    Vector3 checkpointPos = Vector3.zero;

    public void Initialize(GameObject cinemachinePrefab)
    {
        input = new PlayerInput();
        moveAction = input.Player.Move;
        interactAction = input.Player.Interact;
        abilityAction = input.Player.Ability;
        cycleMaskAction = input.Player.CycleMask;
        clickAction = input.Player.Click;
        uiToggleAction = input.Player.UIToggle;

        rb = GetComponent<Rigidbody2D>();
        box = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        interactAction.performed += OnInteract;
        abilityAction.performed += OnAbility;
        cycleMaskAction.performed += OnCycleMask;
        clickAction.performed += OnClick;
        uiToggleAction.performed += OnUIToggle;
        ownedMasks.Add(MaskType.None);

        checkpointPos = transform.position;

        this.transform.gameObject.SetActive(true);

        //just for testing
        //ObtainMask(MaskType.Ghost);
        //ObtainMask(MaskType.Detective);

        GameObject cinemachine = Instantiate(cinemachinePrefab);
        cinemachine.GetComponent<CinemachineCamera>().Follow = this.transform; // Set camera to follow the player
    }

    void OnEnable()
    {
        animator.SetFloat("X", 0);
        animator.SetFloat("Y", 0);
        animator.SetBool("isMoving", false);

        input.Enable();
        moveAction.Enable();
        interactAction.Enable();
        abilityAction.Enable();
        cycleMaskAction.Enable();
        clickAction.Enable();
        uiToggleAction.Enable();
    }

    void OnDisable()
    {
        input.Disable();
        moveAction.Disable();
        interactAction.Disable();
        abilityAction.Disable();
        cycleMaskAction.Disable();
        clickAction.Disable();
        uiToggleAction.Disable();
    }

    // Update is called once per frame
    void Update()
    {

        if (IsDashing())
        {
            rb.linearVelocity = Vector2.zero;
            transform.position = Vector3.MoveTowards(transform.position, dashDestination, dashSpeed * Time.deltaTime);

            if (transform.position == dashDestination)
            {
                // End dash
                dashDestination = new Vector3(0, 0, -1);
                rb.simulated = true;
            }
        }
        else if (isChangingMask)
        {
            rb.linearVelocity = Vector2.zero;
        }
        else if (inDetectiveMode)
        {
            // Detective code
            rb.linearVelocity = Vector2.zero;
        }
        else
        {
            // Movement code
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
        if (collision.tag == "Checkpoint")
        {
            checkpointPos = collision.transform.position;
        }
        if (collision.tag == "Trap")
        {
            transform.position = checkpointPos;
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
            bool hit = false;
            Collider2D[] results = Physics2D.OverlapBoxAll(point, hitboxSize, 0.0f);
            foreach (Collider2D c2d in results)
            {
                if (!c2d.isTrigger)
                {
                    hit = true;
                }
            }
            if (hit)
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

        // keep animator in sync 
        if (animator != null)
        {
            animator.SetBool("DetectiveMode", mode);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetDetectiveView(mode);
        }
    }

    MaskType GetCurrentMask()
    {
        return ownedMasks[currentMaskIndex];
    }

    public void ObtainMask(MaskType mask)
    {

        if (ownedMasks[0] == MaskType.None)
        {
            ownedMasks[0] = mask;
            AnimationDirectionCheck("MaskSwitch");

        }
        else
        {
            ownedMasks.Add(mask);
            currentMaskIndex = (currentMaskIndex + 1) % ownedMasks.Count;
            AnimationDirectionCheck("MaskSwitch");
        }

        ChangeMaskOnPickup(mask);
    }

    private void ChangeMaskOnPickup(MaskType mask)
    {
        UIManager.Instance.UpdateControlsText(mask);
        AnimationDirectionCheck("MaskSwitch");
    }

    void OnCycleMask(InputAction.CallbackContext context)
    {
        if (!IsDashing() && !isChangingMask)
        {
            ChangeDetectiveMode(false);

            int tempIndex = currentMaskIndex;
            currentMaskIndex = (currentMaskIndex + 1) % ownedMasks.Count;
            if (currentMaskIndex == tempIndex)
            {
                return;
            }

            Debug.Log(GetCurrentMask());
            Debug.Log(currentMaskIndex);

            UIManager.Instance.UpdateControlsText(GetCurrentMask());

            AnimationDirectionCheck("MaskSwitch");
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
                        clue.StartInteract();
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

            case "MaskSwitch":

                if (GetCurrentMask() == MaskType.Detective) // jason mask
                {
                    Debug.Log("Detective Mask active");
                    animator.SetTrigger("Clue");
                }

                else if (GetCurrentMask() == MaskType.Ghost) // ghost mask
                {
                    Debug.Log("Ghost Mask active");
                    animator.SetTrigger("Ghost");
                }

                break;

            case "DetectiveMode":
                animator.SetFloat("X", animatorDirection.x);
                animator.SetFloat("Y", animatorDirection.y);
                break;

            default:
                break;
        }
    }

    private void OnUIToggle(InputAction.CallbackContext context)
    {
        // Toggle UI elements for controls on/off
        UIManager.Instance.ToggleUIControls(currentMaskIndex);
    }

    private void UpdateChangingMask()
    {
        isChangingMask = !isChangingMask;
    }
}
