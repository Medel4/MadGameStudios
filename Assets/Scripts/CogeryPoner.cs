using UnityEngine;

public class CogeryPoner : MonoBehaviour
{
    #region Configuración Extra Llamamiento
    
    private CharacterController cc;
    private Camera cam;
    #endregion

    #region Configuración de Movimiento
    [Header("Configuración Movimiento")]
    [SerializeField] private float velocidadMovimiento;
    [SerializeField] private Vector3 MovimientoVertical;
    [SerializeField] private float escalaGravedad = -9.81f;
    [SerializeField] private float alturaSalto = 3f;
    [SerializeField] private Transform Pies;
    [SerializeField] private float radioDeteccion = 0.3f;
    [SerializeField] private LayerMask queEsSuelo;
    #endregion

    #region Configuración Detección de Agua
    [Header("Configuración para Detección de Agua")]
    [SerializeField] private LayerMask capaAgua;
    [SerializeField] private float radioEsfera = 0.5f;
    [SerializeField] private float distanciaEsfera = 1.5f;
    [SerializeField] private float alturaEsfera = 1.0f;
    [SerializeField] private float distanciaRaycast = 1.5f;
    [SerializeField] private Vector3 tamanoCajaCintura = new Vector3(1.0f, 0.5f, 1.0f);
    [SerializeField] private float alturaCintura = 1.0f;
    private bool enAgua = false;
    #endregion

    #region Configuración Colocación de Objetos
    [Header("Configuración para Colocación de Objetos")]
    [SerializeField] private float distanciaInteraccion;
    [SerializeField] private GameObject objetoOriginal;
    [SerializeField] private GameObject objetoFinal;
    [SerializeField] private float distanciaMaxima = 5f;
    [SerializeField] private LayerMask layerInteractuable;
    private GameObject objetoRecogido;
    private GameObject objetoPreview;
    #endregion

    #region Configuración de Previsualización
    [Header("Materiales de Previsualización")]
    [SerializeField] private Material materialValido;
    [SerializeField] private Material materialInvalido;
    private bool previsualizando = false;
    
    #endregion



    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        cc = GetComponent<CharacterController>();
        cam = Camera.main;
    }

    void Update()
    {
        // Detección de objetos recogibles
        CogerObjetos();

        // Movimiento del jugador
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector2 input = new Vector2(h, v).normalized;

        transform.rotation = Quaternion.Euler(0, cam.transform.eulerAngles.y, 0);

        if (input.sqrMagnitude > 0)
        {
            float anguloRotacion = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
            transform.eulerAngles = new Vector3(0, anguloRotacion, 0);
            Vector3 movimiento = Quaternion.Euler(0, anguloRotacion, 0) * Vector3.forward;

            if (!DetectarAguaYSinSuelo(movimiento) || PuedeMoverseEnAgua(movimiento))
            {
                cc.Move(movimiento * velocidadMovimiento * Time.deltaTime);
            }
        }

        // Aplicar gravedad y detectar suelo
        AplicarGravedad();
        DeteccionSuelo();

        // Previsualización y colocación de objetos
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

    #region Movimiento
    private void AplicarGravedad()
    {
        MovimientoVertical.y += escalaGravedad * Time.deltaTime;
        cc.Move(MovimientoVertical * Time.deltaTime);
    }

    private void DeteccionSuelo()
    {
        Collider[] collsDetectados = Physics.OverlapSphere(Pies.position, radioDeteccion, queEsSuelo);
        if (collsDetectados.Length > 0 && !enAgua)
        {
            MovimientoVertical.y = 0;
            Saltar();
        }
    }

    private void Saltar()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !enAgua && Physics.CheckSphere(Pies.position, radioDeteccion, queEsSuelo))
        {
            MovimientoVertical.y = Mathf.Sqrt(-2 * escalaGravedad * alturaSalto);
        }
    }
    #endregion

    #region Detección de Agua
    private bool DetectarAguaYSinSuelo(Vector3 direccion)
    {
        bool contactoConAguaEsfera = DetectarAguaEsfera(direccion);
        bool contactoConAguaCaja = DetectarAguaCaja();

        enAgua = contactoConAguaEsfera || contactoConAguaCaja;

        return enAgua;
    }

    private bool DetectarAguaEsfera(Vector3 direccion)
    {
        Vector3 posicionEsfera = transform.position + Vector3.up * alturaEsfera + direccion.normalized * distanciaEsfera;

        bool contactoConAgua = Physics.OverlapSphere(posicionEsfera, radioEsfera, capaAgua).Length > 0;

        if (contactoConAgua)
        {
            if (!Physics.Raycast(posicionEsfera, Vector3.down, distanciaRaycast, queEsSuelo))
            {
                return true;
            }
        }

        return false;
    }

    private bool DetectarAguaCaja()
    {
        Vector3 centroCaja = transform.position + Vector3.up * alturaCintura;
        Collider[] colisionadores = Physics.OverlapBox(centroCaja, tamanoCajaCintura / 2, Quaternion.identity, capaAgua);

        if (colisionadores.Length > 0)
        {
            return true;
        }

        return false;
    }

    private bool PuedeMoverseEnAgua(Vector3 direccion)
    {
        Vector3 posicionEsfera = transform.position + Vector3.up * alturaEsfera + direccion.normalized * distanciaEsfera;

        return Physics.Raycast(posicionEsfera, Vector3.down, distanciaRaycast, queEsSuelo);
    }
    #endregion

    #region Colocación de Objetos
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

            Collider collider = objetoPreview.GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = false;
            }

            Rigidbody rigidbody = objetoPreview.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.isKinematic = true;
            }

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

    private void ColocarObjetos()
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
                    if (collider != null)
                    {
                        collider.enabled = true;
                    }

                    Rigidbody rigidbody = objetoPreview.GetComponent<Rigidbody>();
                    if (rigidbody != null)
                    {
                        rigidbody.isKinematic = false;
                    }

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
            if (col.gameObject == objetoIgnorar)
            {
                continue;
            }

            if (col.CompareTag("Destructible"))
            {
                return true;
            }
        }

        return false;
    }

    private bool LugarEsValido(RaycastHit hit)
    {
        if (hit.collider != null && ((1 << hit.collider.gameObject.layer) & layerInteractuable) != 0)
        {
            return true;
        }

        return false;
    }

    private void CambiarMaterialPreview(Material material)
    {
        Renderer[] renderers = objetoPreview.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material = material;
        }
    }
    #endregion

    #region Visualización
    private void OnDrawGizmos()
    {
        Vector3 direccion = transform.forward;
        Vector3 posicionEsfera = transform.position + Vector3.up * alturaEsfera + direccion.normalized * distanciaEsfera;

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(posicionEsfera, radioEsfera);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(posicionEsfera, Vector3.down * distanciaRaycast);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(Pies.position, radioDeteccion);

        Vector3 centroCaja = transform.position + Vector3.up * alturaCintura;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(centroCaja, tamanoCajaCintura);
    }

    private void OnDestroy()
    {
        FinalizarPrevisualizacion();
    }
    #endregion
}
