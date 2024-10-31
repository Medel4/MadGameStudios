using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class adadad : MonoBehaviour
{
    [SerializeField] private float velocidadMovimiento;
    private CharacterController cc;
    private Camera cam;

    public float range = 5f; // Rango de detección


    [Header("Configuración para Colocación de Objetos")]
    public GameObject objetoOriginal;
    public GameObject objetoFinal;
    public float distanciaMaxima = 5f;
    public LayerMask layerInteractuable;

    [Header("Materiales de Previsualización")]
    public Material materialValido;
    public Material materialInvalido;

    private GameObject objetoPreview;
    private bool previsualizando = false;

    [Header("Configuración Gravedad")]
    [SerializeField] private Vector3 MovimientoVertical;
    [SerializeField] private float escalaGravedad = -9.81f;
    [SerializeField] private float alturaSalto = 3f;
    [SerializeField] private Transform Pies;
    [SerializeField] private float radioDeteccion = 0.3f;
    [SerializeField] private LayerMask queEsSuelo;






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
            Vector3 movimiento = Quaternion.Euler(0, anguloRotacion, 0) * Vector3.forward;

            cc.Move(movimiento * velocidadMovimiento * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.P)) // Activa o desactiva la previsualización con la tecla 'P'
        {
            previsualizando = !previsualizando;
            objetoPreview.SetActive(previsualizando);
        }

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
                // Verificamos si el objeto que colisiona tiene el tag "Destructible"
                if (hit.collider.CompareTag("Destructible"))
                {
                    Destroy(hit.collider.gameObject); // Destruye el objeto
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
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, distanciaMaxima, layerInteractuable))
        {
            Vector3 center = objetoPreview.GetComponent<Collider>().bounds.center;
            hit.point = new Vector3(hit.point.x, center.y, hit.point.z);
            objetoPreview.transform.position = hit.point;
            objetoPreview.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

            if (LugarEsValido(hit))
            {
                CambiarMaterialPreview(materialValido);

                if (Input.GetMouseButtonDown(0))
                {
                    Instantiate(objetoFinal, hit.point, objetoPreview.transform.rotation);
                }
            }
            else
            {
                CambiarMaterialPreview(materialInvalido);
            }
        }
    }

    bool LugarEsValido(RaycastHit hit)
    {
        return hit.collider != null && hit.collider.tag != "Obstaculo";
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






