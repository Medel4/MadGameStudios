using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPerson : MonoBehaviour
{
    [SerializeField] private float velocidadMovimiento;
    private CharacterController cc;
    private Camera cam;

    public float range = 5f; // Rango de detección

    [Header("Configuración Gravedad")]
    [SerializeField] private Vector3 MovimientoVertical;
    [SerializeField] private float escalaGravedad = -9.81f;
    [SerializeField] private float alturaSalto = 3f;
    [SerializeField] private Transform Pies;
    [SerializeField] private float radioDeteccion = 0.3f;
    [SerializeField] private LayerMask queEsSuelo;

    [Header("Configuración para Colocación de Objetos")]
    [SerializeField] private GameObject objetoOriginal;
    [SerializeField] private GameObject objetoFinal;
    [SerializeField] private float distanciaMaxima = 5f;
    [SerializeField] private LayerMask layerInteractuable;

    [Header("Materiales de Previsualización")]
    [SerializeField] private Material materialValido;
    [SerializeField] private Material materialInvalido;


    private GameObject objetoPreview;
    private bool previsualizando = false;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        cc = GetComponent<CharacterController>();
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        CogerObjetos();

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector2 input = new Vector2(h, v).normalized;
        transform.rotation = Quaternion.Euler(0, cam.transform.eulerAngles.y, 0);

        if (input.sqrMagnitude > 0)
        {
            float anguloRotacion = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
            transform.eulerAngles = new Vector3(0, anguloRotacion, 0);
            Vector3 movimiento = Quaternion.Euler(0, anguloRotacion, 0) * Vector3.forward;
            cc.Move(movimiento * velocidadMovimiento * Time.deltaTime);
            
        }
        

        AplicarGravedad();
        DeteccionSuelo();

        

        // Activar o desactivar previsualización con la tecla P
        if (Input.GetKeyDown(KeyCode.P))
        {
            previsualizando = !previsualizando;

            if (previsualizando)
            {
                IniciarPrevisualizacion();
            }
            else
            {
                FinalizarPrevisualizacion();
            }
        }

        // Actualizar previsualización si está activada
        if (previsualizando)
        {
            ColocarObjetos();
        }
    }
    private void AplicarGravedad()
    {
        //mi movimiento vertical en la Y va aumentando a cierta escala por segundo
        MovimientoVertical.y += escalaGravedad * Time.deltaTime;
        cc.Move(MovimientoVertical * Time.deltaTime);
    }

    private void DeteccionSuelo()
    {
        Collider[] collsDetectados = Physics.OverlapSphere(Pies.position, radioDeteccion, queEsSuelo);
        //Si existe un collider bajo mis pies
        if (collsDetectados.Length > 0)
        {
            MovimientoVertical.y = 0;
            Saltar();
        }
    }

    //sirve para dibujar figuras en la escena
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(Pies.position, radioDeteccion);
    }

    private void Saltar()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            MovimientoVertical.y = Mathf.Sqrt(-2 * escalaGravedad * alturaSalto);
        }
    }
    void CogerObjetos()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, range))
            {
                if (hit.collider.CompareTag("Destructible"))
                {
                    Destroy(hit.collider.gameObject);
                }
            }
        }
    }

    void IniciarPrevisualizacion()
    {
        objetoPreview = Instantiate(objetoOriginal);
        objetoPreview.SetActive(true);

        CambiarMaterialPreview(materialInvalido);
    }

    void FinalizarPrevisualizacion()
    {
        if (objetoPreview != null)
        {
            Destroy(objetoPreview);
        }
    }
    void ColocarObjetos()
    {
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2)); // Ray from the center of the screen

        // Perform Raycast in the current camera direction
        if (Physics.Raycast(ray, out hit, distanciaMaxima, layerInteractuable))
        {
            // Get the hit point of the raycast
            Vector3 posicion = hit.point;

            // Adjust the position to make the object's base touch the surface
            float alturaBaseObjeto = objetoPreview.GetComponent<Collider>().bounds.extents.y;
            posicion.y = hit.point.y + alturaBaseObjeto; // Position the object at the surface

            // Snap the object to the ground
            posicion.y = Mathf.Max(posicion.y, hit.collider.bounds.min.y + alturaBaseObjeto);

            objetoPreview.transform.position = posicion;
            objetoPreview.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

            // Check if the location is valid
            if (LugarEsValido())
            {
                CambiarMaterialPreview(materialValido);

                if (Input.GetMouseButtonDown(0)) // Place the object on left click
                {
                    Instantiate(objetoFinal, objetoPreview.transform.position, objetoPreview.transform.rotation);
                }
            }
            else
            {
                CambiarMaterialPreview(materialInvalido);
            }
        }
        else
        {
            CambiarMaterialPreview(materialInvalido); // Change to red material if placement is not possible
        }
    }



    bool LugarEsValido()
    {
        Collider[] colliders = Physics.OverlapBox(objetoPreview.transform.position, objetoPreview.GetComponent<Collider>().bounds.extents, objetoPreview.transform.rotation, layerInteractuable);
        return colliders.Length == 0; // Devuelve falso si hay colisiones
    }

    void CambiarMaterialPreview(Material material)
    {
        Renderer[] renderers = objetoPreview.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material = material;
        }
    }

    private void OnDestroy()
    {
        FinalizarPrevisualizacion();
    }
}
