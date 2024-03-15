using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{
    [Header("Preferences")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform bodyTransform;
    [SerializeField] private Rigidbody2D rb;

    [Header("Settings")]
    [SerializeField] private float movementSpeed = 4f;
    [SerializeField] private float tunningRate = 100f;

    private Vector2 previousMovementInput;


    // Start is called before the first frame update

    public override void OnNetworkSpawn()
    {
        if(!IsOwner)
        {
            return;
        }

        inputReader.MoveEvent += HandleMove;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner)
        {
            return;
        }

        inputReader.MoveEvent -= HandleMove;
    }

    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        float zRotation = previousMovementInput.x * -tunningRate * Time.deltaTime;
        bodyTransform.Rotate(0f, 0f, zRotation);


        
    }

    private void FixedUpdate()
    {   
        if (!IsOwner) 
        {
            return;
        }

        rb.velocity = (Vector2)bodyTransform.up * previousMovementInput.y * movementSpeed;
    }
    private void HandleMove(Vector2 movementInput)
    {
        previousMovementInput = movementInput;
    }


}
