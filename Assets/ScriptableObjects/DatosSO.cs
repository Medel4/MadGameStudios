using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Datos del Player")]
public class DatosSO : ScriptableObject
{
    public int macetas;
    public GameObject objetoMacetas;
    public int pelotas;
    public GameObject objetoPelotas;
    public int cuadrados;
    public GameObject objetoCuadrados;
    public int capsulas;
    public GameObject objetoCapsula;
    public int[] objetosEnInventario;
    public int huecosEnInventario;
    public int cantidadApilable;
    public float distanciaInteraccion;
    public int espacioinventario;
    public float distanciaMaxima;

}