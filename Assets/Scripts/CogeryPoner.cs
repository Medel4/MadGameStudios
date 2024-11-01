using UnityEngine;

public class CogeryPoner : MonoBehaviour
{
    [SerializeField] private float velocidadMovimiento;
    private CharacterController cc;
    private Camera cam;

    public float range = 5f;

    [Header("Configuración Gravedad")]
    [SerializeField] private Vector3 MovimientoVertical;
    [SerializeField] private float escalaGravedad = -9.81f;
    [SerializeField] private float alturaSalto = 3f;
    [SerializeField] private Transform Pies;
    [SerializeField] private float radioDeteccion = 0.3f;
    [SerializeField] private LayerMask queEsSuelo;

    [Header("Configuración para Colocación de Objetos")]
    [SerializeField] private float distanciaMaxima = 5f;
    [SerializeField] private LayerMask layerInteractuable;

    [Header("Materiales de Previsualización")]
    [SerializeField] private Material materialValido;
    [SerializeField] private Material materialInvalido;

    [Header("Restricción de Agua")]
    [SerializeField] private LayerMask layerAgua; // Capa que representa el agua
    [SerializeField] private float alturaMaximaAgua = 1.0f; // Altura máxima permitida en el agua
    [SerializeField] private float fuerzaEmpujeAgua = 2.0f; // Fuerza de empuje en el agua

    private GameObject objetoRecogido;
    private GameObject objetoPreview;
    private bool previsualizando = false;
    private bool enAgua = false; // Verifica si el jugador está en el agua

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        cc = GetComponent<CharacterController>();
        cam = Camera.main;
    }

    void Update()
    {
        CogerObjetos();
        MovimientoJugador();

        if (enAgua)
        {
            ProfundidadAgua();
        }
        else
        {
            AplicarGravedad();
            DeteccionSuelo();
        }

        if (Input.GetKeyDown(KeyCode.P) && objetoRecogido != null)
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

        if (previsualizando)
        {
            ColocarObjetos();
        }
    }

    private void MovimientoJugador()
    {
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
    }

    private void AplicarGravedad()
    {
        MovimientoVertical.y += escalaGravedad * Time.deltaTime;
        cc.Move(MovimientoVertical * Time.deltaTime);
    }

    private void DeteccionSuelo()
    {
        Collider[] collsDetectados = Physics.OverlapSphere(Pies.position, radioDeteccion, queEsSuelo);
        if (collsDetectados.Length > 0)
        {
            MovimientoVertical.y = 0;
            Saltar();
        }
    }

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
                    objetoRecogido = hit.collider.gameObject;
                    objetoRecogido.SetActive(false);
                }
            }
        }
    }

    void IniciarPrevisualizacion()
    {
        if (objetoRecogido != null)
        {
            objetoPreview = Instantiate(objetoRecogido);
            objetoPreview.SetActive(true);

            CambiarMaterialPreview(materialInvalido);
        }
    }

    void FinalizarPrevisualizacion()
    {
        if (objetoPreview != null)
        {
            Destroy(objetoPreview);
            objetoPreview = null;
        }
    }

    void ColocarObjetos()
    {
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, distanciaMaxima, layerInteractuable))
        {
            objetoPreview.transform.position = hit.point;
            objetoPreview.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

            if (LugarEsValido(hit))
            {
                CambiarMaterialPreview(materialValido);

                if (Input.GetMouseButtonDown(0))
                {
                    GameObject nuevoObjeto = Instantiate(objetoRecogido, objetoPreview.transform.position, objetoPreview.transform.rotation);
                    nuevoObjeto.SetActive(true);
                    objetoRecogido = null;
                    FinalizarPrevisualizacion();
                    previsualizando = false;
                }
            }
            else
            {
                CambiarMaterialPreview(materialInvalido);
            }
        }
        else
        {
            CambiarMaterialPreview(materialInvalido);
        }
    }

    bool LugarEsValido(RaycastHit hit)
    {
        return hit.collider != null && ((1 << hit.collider.gameObject.layer) & layerInteractuable) != 0;
    }

    void CambiarMaterialPreview(Material material)
    {
        Renderer[] renderers = objetoPreview.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material = material;
        }
    }

    private void ProfundidadAgua()
    {
        if (transform.position.y < alturaMaximaAgua)
        {
            // Si el jugador está más allá de la altura permitida, aplica un empuje hacia arriba
            MovimientoVertical.y = fuerzaEmpujeAgua;
        }
        else
        {
            // Mantiene la velocidad vertical en cero para evitar que la gravedad actúe mientras está en el agua
            MovimientoVertical.y = 0;
        }

        cc.Move(MovimientoVertical * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & layerAgua) != 0)
        {
            enAgua = true;
            MovimientoVertical.y = 0; // Restablecer la velocidad vertical al entrar en el agua
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & layerAgua) != 0)
        {
            enAgua = false;
        }
    }

    private void OnDestroy()
    {
        FinalizarPrevisualizacion();
    }
}
