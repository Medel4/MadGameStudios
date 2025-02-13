using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class MissionsUIDisplay : MonoBehaviour
{
    [SerializeField] private SistemaMisiones sistemaMisiones; // Referencia al sistema de misiones.
    [SerializeField] private ToggleMision[] toggleMisiones; // Referencia a los toggles de misiones en la UI.

    private void OnEnable()
    {
        sistemaMisiones.eventManager.OnNuevaMision += ActivarToggleMision;
        sistemaMisiones.eventManager.OnActualizarMision += ActualizarToggle;
        sistemaMisiones.eventManager.OnTerminarMision += DesactivarToggle;
    }

    private void OnDisable()
    {
        sistemaMisiones.eventManager.OnNuevaMision -= ActivarToggleMision;
        sistemaMisiones.eventManager.OnActualizarMision -= ActualizarToggle;
        sistemaMisiones.eventManager.OnTerminarMision -= DesactivarToggle;
    }

    private void ActivarToggleMision(MisionSO mision)
    {
        if (mision.indiceMision >= 0 && mision.indiceMision < toggleMisiones.Length)
        {
            ToggleMision toggle = toggleMisiones[mision.indiceMision];
            toggle.TextoMision.text = mision.ordenInicial;
            if (mision.repetir)
            {
                toggle.TextoMision.text += " (" + mision.estadoActual + "/" + mision.repeticionesTotales + ")";
            }
            toggle.gameObject.SetActive(true);
        }
    }

    private void ActualizarToggle(MisionSO mision)
    {
        if (mision.indiceMision >= 0 && mision.indiceMision < toggleMisiones.Length)
        {
            ToggleMision toggle = toggleMisiones[mision.indiceMision];
            toggle.TextoMision.text = mision.ordenInicial;
            if (mision.repetir)
            {
                toggle.TextoMision.text += " (" + mision.estadoActual + "/" + mision.repeticionesTotales + ")";
            }
        }
    }

    private void DesactivarToggle(MisionSO mision)
    {
        if (mision.indiceMision >= 0 && mision.indiceMision < toggleMisiones.Length)
        {
            ToggleMision toggle = toggleMisiones[mision.indiceMision];
            toggle.Toggle.isOn = true;
            toggle.TextoMision.text = mision.ordenFinal;
        }
    }
}
