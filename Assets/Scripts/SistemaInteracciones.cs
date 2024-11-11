using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SistemaInteracciones : MonoBehaviour
{
    private Camera cam;
    [SerializeField] private float distanciaInteraccion;
    [SerializeField] private Transform interactuableActual;
    void Start()
    {
        cam = Camera.main;
    }
    void Update()
    {
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit PointInfo, distanciaInteraccion))
        {
            if (PointInfo.transform.CompareTag("Destructible"))
            {
                interactuableActual = PointInfo.transform;
                interactuableActual.GetComponent<Outline>().enabled = true;

            }
            
        }
        else if (interactuableActual)
        {
            interactuableActual.GetComponent<Outline>().enabled = false;
            interactuableActual = null;
        }
    }
}
