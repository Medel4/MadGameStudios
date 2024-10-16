using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPSController : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        // Obtener la entrada de movimiento
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // Crear el vector de movimiento
        Vector3 movement = new Vector3(moveHorizontal, 0, moveVertical).normalized;

        // Aplicar el movimiento
        rb.velocity = movement * moveSpeed;
    }
}
