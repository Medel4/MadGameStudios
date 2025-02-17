using UnityEngine;

public class PLANTAS : MonoBehaviour, IInteractuable
{
    [SerializeField] private float tamanomaximo = 1.6f; // Tama�o m�ximo de la planta
    private float tamanoactual; // Tama�o actual de la planta
    [SerializeField] private float tamanoinicial = 0.00001f; // Tama�o inicial de la planta
    private float velocidadCrecimiento; // Velocidad de crecimiento de la planta
    [SerializeField] private bool funcionar = false; // Determina si la planta est� creciendo

    [SerializeField] private GameObject plantaCortadaPrefab; // Prefab para la planta cortada
    [SerializeField] private float tiempoAntesDeDesaparecer = 1f; // Tiempo antes de que desaparezca la planta despu�s de ser cortada
    [SerializeField] private FirstPerson fp; 
    private void Awake()
    {
        tamanomaximo = 1f;
        velocidadCrecimiento = 1.4f * Time.deltaTime;
        tamanoinicial = 0.0001f;
        transform.localScale = new Vector3(0.000001f, 0.000001f, 0.000001f);
    }

    void Update()
    {
        if (funcionar)
        {
            tamanoactual = transform.localScale.magnitude;
            if (tamanoactual < tamanomaximo)
            {
                transform.localScale += new Vector3(tamanoinicial, tamanoinicial, tamanoinicial) * velocidadCrecimiento;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("aire"))
        {
            funcionar = true;
        }
        else if (other.CompareTag("volagua"))
        {
            funcionar = false;
        }
    }

    // M�todo para cortar la planta
    public bool PuedeSerCortada()
    {
        return transform.localScale.magnitude >= tamanomaximo; // La planta puede ser cortada si ha alcanzado su tama�o m�ximo
    }

    public void CortarPlanta()
    {
        // Si la planta puede ser cortada, destruye la planta y genera efectos
        if (PuedeSerCortada())
        {
            fp.MaderaActual += 5;

            // Destruir la planta original
            Destroy(gameObject,tiempoAntesDeDesaparecer);

            // Aqu� puedes a�adir efectos adicionales, como aumentar el inventario del jugador con madera
            Debug.Log("Planta cortada y madera recolectada.");
        }
    }

    // Implementar la interfaz IInteractuable
    public void Interactuar(Transform interactuador)
    {
        // Solo se puede cortar si la planta est� lista
        if (PuedeSerCortada())
        {
            CortarPlanta(); // Cortar la planta al interactuar con ella
        }
    }
}