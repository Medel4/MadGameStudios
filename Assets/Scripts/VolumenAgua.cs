using System.Collections;
using UnityEngine;

public class VolumenAgua : MonoBehaviour
{
    [SerializeField] private Transform planoAgua; // El objeto que representa el plano del agua.
    [SerializeField] private float velocidadDescenso = 0.5f; // Velocidad a la que baja el agua.
    [SerializeField] private SistemaMisiones sistemaMisiones; // Referencia al sistema de misiones.
    [SerializeField] private EventManagerSO gestorEventos; // Referencia al sistema de misiones.

    [Header("Posiciones de Descenso")]
    [SerializeField] private Transform posicionMision1; // Primer objeto para la posici�n del agua en la misi�n 1.
    [SerializeField] private Transform posicionMision2; // Segundo objeto para la posici�n del agua en la misi�n 2.

    private int indiceMisionActual = 0; // �ndice de la misi�n actual.

    private void OnEnable()
    {
        // Suscribirse a la finalizaci�n de misiones.
        sistemaMisiones.eventManager.OnTerminarMision += ManejarMisionCompletada;
    }

    private void OnDisable()
    {
        // Desuscribirse para evitar errores.
        sistemaMisiones.eventManager.OnTerminarMision -= ManejarMisionCompletada;
    }

    private void ManejarMisionCompletada(MisionSO mision)
    {
        if (mision.indiceMision == indiceMisionActual)
        {
            // Determinar la posici�n del agua dependiendo de la misi�n actual
            Transform posicionObjetivo = null;

            if (indiceMisionActual == 0)
                posicionObjetivo = posicionMision1; // Usar la posici�n de la primera misi�n.
            else if (indiceMisionActual == 1)
                posicionObjetivo = posicionMision2; // Usar la posici�n de la segunda misi�n.

            if (posicionObjetivo != null)
            {
                StartCoroutine(DescenderAgua(posicionObjetivo));
                indiceMisionActual++; // Avanzar a la siguiente misi�n.
            }
        }
    }

    private IEnumerator DescenderAgua(Transform objetivo)
    {
        Vector3 posicionInicial = planoAgua.position;
        Vector3 posicionObjetivo = new Vector3(posicionInicial.x, objetivo.position.y, posicionInicial.z); // Usar la Y del objeto objetivo

        while (planoAgua.position.y > posicionObjetivo.y)
        {
            planoAgua.position = Vector3.MoveTowards(planoAgua.position, posicionObjetivo, velocidadDescenso * Time.deltaTime);
            yield return null;
        }

        planoAgua.position = posicionObjetivo; // Asegurar que termine exactamente en la altura deseada.
    }

    public void IniciarDescensoAgua()
    {
        if (indiceMisionActual == 0) // Iniciar solo si es la primera misi�n.
        {
            ManejarMisionCompletada(new MisionSO { indiceMision = 0 });
        }
    }
}
