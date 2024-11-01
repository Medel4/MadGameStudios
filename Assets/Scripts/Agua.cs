using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agua : MonoBehaviour
{
    public float swimSpeed = 3f; // Velocidad de movimiento en el agua
    public float maxDepth = 0.5f; // Profundidad máxima permitida desde la superficie del agua
    public float aguaEscalaGravedad = -4f; // Gravedad reducida para el agua

    private CharacterController playerController;
    private Transform playerTransform;

    [SerializeField] private bool isInWater = false;
    private float waterSurfaceY;

    // Guardar la referencia del script CogeryPoner del jugador
    private CogeryPoner cp;

    [SerializeField] private float tierraEscalaGravedad; // Escala de gravedad original en tierra
    [SerializeField] private float actualEscalaGravedad; // Variable para monitorear la gravedad actual del jugador

    void Update()
    {
        if (isInWater && playerController != null)
        {
            // Movimiento horizontal en el agua
            Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            Vector3 swimDirection = transform.TransformDirection(input) * swimSpeed;

            // Limitar la altura Y del jugador
            float targetY = waterSurfaceY - maxDepth;
            if (playerTransform.position.y < targetY)
            {
                playerTransform.position = new Vector3(playerTransform.position.x, targetY, playerTransform.position.z);
            }

            // Aplicar el movimiento horizontal del jugador usando el CharacterController
            playerController.Move(swimDirection * Time.deltaTime);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter activado con: " + other.gameObject.name);
        if (other.CompareTag("Player")) // Asegúrate de que el jugador tenga el tag "Player"
        {
            isInWater = true;
            Debug.Log("Player ha entrado en el agua");
            // ... resto del código
        }
        playerController = other.GetComponent<CharacterController>();
            playerTransform = other.transform;
            waterSurfaceY = transform.position.y; // Y de la superficie del agua

            // Cambiar la escala de gravedad al entrar en el agua
            cp = other.GetComponent<CogeryPoner>();
            if (cp != null)
            {
                tierraEscalaGravedad = cp.EscalaGravedad;
                cp.EscalaGravedad = aguaEscalaGravedad;
                actualEscalaGravedad = cp.EscalaGravedad;

                Debug.Log("Gravedad cambiada al entrar en el agua: " + actualEscalaGravedad);
            }
        
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInWater = false;
            Debug.Log("Player ha salido del agua");

            // Restaurar la escala de gravedad al salir del agua
            if (cp != null)
            {
                cp.EscalaGravedad = tierraEscalaGravedad;
                actualEscalaGravedad = cp.EscalaGravedad;

                Debug.Log("Gravedad restaurada al salir del agua: " + actualEscalaGravedad);
            }
        }
    }
}
