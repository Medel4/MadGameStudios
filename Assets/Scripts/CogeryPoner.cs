using UnityEngine;

public class CogeryPoner : MonoBehaviour
{
    [SerializeField] private float velocidadMovimiento;
    private CharacterController cc;
    private Camera cam;

    public float range = 5f;

    [Header("Configuración Gravedad")]
    [SerializeField] private Vector3 MovimientoVertical;
    [SerializeField] private float escalaGravedad;
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

    private GameObject objetoRecogido; // Objeto que el jugador ha recogido
    private GameObject objetoPreview;
    private bool previsualizando = false;

    public float EscalaGravedad { get => escalaGravedad; set => escalaGravedad = value; }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        cc = GetComponent<CharacterController>();
        cam = Camera.main;
    }

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
                    objetoRecogido.SetActive(false); // Ocultar el objeto en la escena
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
            objetoPreview = null; // Aseguramos que se resetee
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
                    nuevoObjeto.SetActive(true); // Activamos el objeto instanciado
                    objetoRecogido = null; // Limpiamos el objeto recogido
                    FinalizarPrevisualizacion(); // Finalizamos la previsualización después de colocar
                    previsualizando = false; // Aseguramos que la previsualización se detiene
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

    private void OnDestroy()
    {
        FinalizarPrevisualizacion();
    }
}
