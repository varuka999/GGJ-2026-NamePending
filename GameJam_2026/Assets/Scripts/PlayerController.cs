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
    [SerializeField] float moveSpeed = 8.0f;
    private Rigidbody2D rb = null;
    private Animator animator = null;
    private Vector2 animatorDirection = Vector2.down;
    private List<Interactible> interactibles = new List<Interactible>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        input = new PlayerInput();
        moveAction = input.Player.Move;
        interactAction = input.Player.Interact;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        animator.SetFloat("X", 0);
        animator.SetFloat("Y", 0);
        animator.SetBool("isMoving", false);
        interactAction.performed += OnInteract;
    }

    void OnEnable()
    {
        input.Enable();
        moveAction.Enable();
        interactAction.Enable();
    }

    void OnDisable()
    {
        input.Disable();
        moveAction.Disable();
        interactAction.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        //Movement code
        Vector2 moveInput = moveAction.ReadValue<Vector2>();

        ChangeAnimationDirection(moveInput);
        

        Vector3 direction = moveInput.normalized;
        rb.linearVelocity = direction * moveSpeed;

    }



    void ChangeAnimationDirection(Vector2 moveInput) 
    {
        if (moveInput.y == 1 || moveInput.y == -1) 
        {
            Vector3 direction = Vector3.zero;
            direction.y += moveInput.y;
            animatorDirection = direction;
        }

        else if (moveInput.x == 1 || moveInput.x == -1) 
        {
            Vector3 direction = Vector3.zero;
            direction.x += moveInput.x;
            animatorDirection = direction;
        }

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
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("trigger entered");
        Interactible interactible = collision.gameObject.GetComponent<Interactible>();
        if (interactible != null)
        {
            interactibles.Add(interactible);
            Debug.Log("interactible found");
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("trigger exited");
        Interactible interactible = collision.gameObject.GetComponent<Interactible>();
        if (interactible != null)
        {
            interactibles.Remove(interactible);
        }
    }

    void OnInteract(InputAction.CallbackContext context)
    {
        Debug.Log("interact");
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
            //start interact animation here
        }
    }
}
