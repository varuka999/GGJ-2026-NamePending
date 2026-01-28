using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerInput input = null;
    private InputAction moveAction = null;
    private InputAction interactAction = null;
    private InputAction dashAction = null;
    [SerializeField] float moveSpeed = 8.0f;
    private Rigidbody2D rb = null;
    private Animator animator = null;
    private BoxCollider2D box = null;

    private Vector3 animatorDirection = Vector2.down;
    private List<Interactible> interactibles = new List<Interactible>();
    [SerializeField] private bool detectiveUnlocked = false;

    //ghost dash stuff
    [SerializeField] private bool dashUnlocked = false;
    [SerializeField] float dashDistance = 3.5f;
    [SerializeField] float dashSpeed = 12.0f;
    private Vector3 dashDestination = new Vector3(0,0,-1);


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        input = new PlayerInput();
        moveAction = input.Player.Move;
        interactAction = input.Player.Interact;
        dashAction = input.Player.Dash;

        rb = GetComponent<Rigidbody2D>();
        box = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        animator.SetFloat("X", 0);
        animator.SetFloat("Y", 0);
        animator.SetBool("isMoving", false);
        interactAction.performed += OnInteract;
        dashAction.performed += OnDash;
    }

    void OnEnable()
    {
        input.Enable();
        moveAction.Enable();
        interactAction.Enable();
        dashAction.Enable();
    }

    void OnDisable()
    {
        input.Disable();
        moveAction.Disable();
        interactAction.Disable();
        dashAction.Disable();
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
                dashDestination = new Vector3(0,0,-1);
                rb.simulated = true;
            }

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

    void OnDash(InputAction.CallbackContext context)
    {
        if (dashUnlocked && !IsDashing())
        {            
            Vector2 point = transform.position + (dashDistance*animatorDirection);
            point += box.offset;
            Vector2 hitboxSize = box.size - new Vector2(0.05f,0.05f);
            if(!Physics2D.OverlapBox(point,hitboxSize,0.0f))
            {
                //start dash
                AnimationDirectionCheck("Dash");

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
    }


    // -------------------------------------------------------------------------- //

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
