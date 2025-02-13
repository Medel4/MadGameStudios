using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Mision")]
public class MisionSO : ScriptableObject
{
    public string ordenInicial;
    public string ordenFinal;

    public bool repetir;
    public int repeticionesTotales;

    [NonSerialized]public int estadoActual=0;

    public int indiceMision;
}
