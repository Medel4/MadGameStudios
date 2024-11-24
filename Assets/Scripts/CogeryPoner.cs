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

    [Header("Configuración para Detección de Agua")]
    [SerializeField] private LayerMask capaAgua;
    [SerializeField] private float radioEsfera = 0.5f;
    [SerializeField] private float distanciaEsfera = 1.5f;
    [SerializeField] private float distanciaRaycast = 1.5f;
    [SerializeField] private float alturaEsfera = 1.0f;

    [Header("Configuración para Colocación de Objetos")]
    [SerializeField] private float distanciaInteraccion;
    [SerializeField] private GameObject objetoOriginal;
    [SerializeField] private GameObject objetoFinal;
    [SerializeField] private float distanciaMaxima = 5f;
    [SerializeField] private LayerMask layerInteractuable;

    [Header("Materiales de Previsualización")]
    [SerializeField] private Material materialValido;
    [SerializeField] private Material materialInvalido;

    private GameObject objetoRecogido;
    private GameObject objetoPreview;
    private bool previsualizando = false;

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

        // Rotación basada en la cámara original
        transform.rotation = Quaternion.Euler(0, cam.transform.eulerAngles.y, 0);

        if (input.sqrMagnitude > 0)
        {
            float anguloRotacion = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
            transform.eulerAngles = new Vector3(0, anguloRotacion, 0);
            Vector3 movimiento = Quaternion.Euler(0, anguloRotacion, 0) * Vector3.forward;

            // Detectar agua y falta de suelo para bloquear movimiento
            if (!DetectarAguaYSinSuelo(movimiento))
            {
                cc.Move(movimiento * velocidadMovimiento * Time.deltaTime);
            }
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

    private bool DetectarAguaYSinSuelo(Vector3 direccion)
    {
        // Calcular la posición de la esfera basada en la dirección del movimiento y la altura
        Vector3 posicionEsfera = transform.position + Vector3.up * alturaEsfera + direccion.normalized * distanciaEsfera;

        // Verificar si la esfera está en contacto con agua
        bool contactoConAgua = Physics.OverlapSphere(posicionEsfera, radioEsfera, capaAgua).Length > 0;

        if (contactoConAgua)
        {
            // Raycast hacia abajo desde la posición de la esfera
            if (!Physics.Raycast(posicionEsfera, Vector3.down, distanciaRaycast, queEsSuelo))
            {
                return true; // Bloquear movimiento
            }
        }

        return false; // Permitir movimiento
    }

    private void DeteccionSuelo()
    {
        // Detectar si el personaje está tocando el suelo
        Collider[] collsDetectados = Physics.OverlapSphere(Pies.position, radioDeteccion, queEsSuelo);
        if (collsDetectados.Length > 0)
        {
            MovimientoVertical.y = 0; // Reiniciar la velocidad vertical cuando estamos en el suelo
            Saltar();
        }
    }

    private void OnDrawGizmos()
    {
        // Visualizar la esfera de detección de agua
        Vector3 direccion = transform.forward; // Dirección del personaje
        Vector3 posicionEsfera = transform.position + Vector3.up * alturaEsfera + direccion.normalized * distanciaEsfera;

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(posicionEsfera, radioEsfera);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(posicionEsfera, Vector3.down * distanciaRaycast);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(Pies.position, radioDeteccion);
    }

    private void Saltar()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Physics.CheckSphere(Pies.position, radioDeteccion, queEsSuelo))
        {
            MovimientoVertical.y = Mathf.Sqrt(-2 * escalaGravedad * alturaSalto);
        }
    }

    private void CogerObjetos()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit PointInfo, distanciaInteraccion))
            {
                if (PointInfo.transform.CompareTag("Destructible"))
                {
                    objetoRecogido = PointInfo.collider.gameObject;
                    objetoRecogido.SetActive(false);
                }
            }
        }
    }

    private void IniciarPrevisualizacion()
    {
        if (objetoRecogido != null)
        {
            objetoPreview = Instantiate(objetoRecogido);
            objetoPreview.SetActive(true);

            // Desactivar el collider y el rigidbody para evitar empujones
            Collider collider = objetoPreview.GetComponent<Collider>();
            if (collider != null) collider.enabled = false;

            Rigidbody rigidbody = objetoPreview.GetComponent<Rigidbody>();
            if (rigidbody != null) rigidbody.isKinematic = true;

            CambiarMaterialPreview(materialInvalido);
        }
    }

    private void FinalizarPrevisualizacion()
    {
        if (objetoPreview != null)
        {
            Destroy(objetoPreview);
            objetoPreview = null;
        }
    }

    private void AplicarGravedad()
    {
        MovimientoVertical.y += escalaGravedad * Time.deltaTime;
        cc.Move(MovimientoVertical * Time.deltaTime);
    }

    void ColocarObjetos()
    {
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit PointInfo, distanciaMaxima, layerInteractuable))
        {
            objetoPreview.transform.position = PointInfo.point;
            objetoPreview.transform.rotation = Quaternion.FromToRotation(Vector3.up, PointInfo.normal);

            if (LugarEsValido(PointInfo) && !HayOverlapDestructible(PointInfo.point, objetoPreview))
            {
                CambiarMaterialPreview(materialValido);

                if (Input.GetMouseButtonDown(0))
                {
                    Collider collider = objetoPreview.GetComponent<Collider>();
                    if (collider != null) collider.enabled = true;

                    Rigidbody rigidbody = objetoPreview.GetComponent<Rigidbody>();
                    if (rigidbody != null) rigidbody.isKinematic = false;

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

    private bool HayOverlapDestructible(Vector3 posicion, GameObject objetoIgnorar)
    {
        float radioOverlap = 0.5f;
        Collider[] objetosSolapados = Physics.OverlapSphere(posicion, radioOverlap);

        foreach (Collider col in objetosSolapados)
        {
            if (col.gameObject == objetoIgnorar) continue;

            if (col.CompareTag("Destructible"))
            {
                return true;
            }
        }

        return false;
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
