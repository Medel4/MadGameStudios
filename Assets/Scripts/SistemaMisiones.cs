using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SistemaMisiones : MonoBehaviour
{
    [SerializeField] public EventManagerSO eventManager;
    [SerializeField] private ToggleMision[] misionesToggle;
    [SerializeField] private VolumenAgua controladorAgua;

    private void OnEnable()
    {
        eventManager.OnNuevaMision += ActivarMision;
        eventManager.OnActualizarMision += ActualizarMision;
        eventManager.OnTerminarMision += CompletarMision;
    }

    private void ActivarMision(MisionSO mision)
    {
        misionesToggle[mision.indiceMision].TextoMision.text = mision.ordenInicial;
        if (mision.repetir)
        {
            misionesToggle[mision.indiceMision].TextoMision.text += "(" + mision.estadoActual + "/" + mision.repeticionesTotales + ")";
        }
        misionesToggle[mision.indiceMision].gameObject.SetActive(true);
    }

    private void ActualizarMision(MisionSO mision)
    {
        misionesToggle[mision.indiceMision].TextoMision.text = mision.ordenInicial;
        misionesToggle[mision.indiceMision].TextoMision.text += "(" + mision.estadoActual + "/" + mision.repeticionesTotales + ")";
    }

    private void CompletarMision(MisionSO mision)
    {
        misionesToggle[mision.indiceMision].Toggle.isOn = true;
        misionesToggle[mision.indiceMision].TextoMision.text = mision.ordenFinal;

        // Comprobamos si es la primera misión y activar el descenso de agua si corresponde
        if (mision.indiceMision == 0 && controladorAgua != null)
        {
            controladorAgua.IniciarDescensoAgua();
        }

        // Desbloquear la siguiente misión
        if (mision.indiceMision + 1 < misionesToggle.Length)
        {
            misionesToggle[mision.indiceMision + 1].gameObject.SetActive(true);
        }
    }
}
