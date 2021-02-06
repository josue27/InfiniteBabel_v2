using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.NiceVibrations;
public class VibracionesControl : MonoBehaviour
{
    public static VibracionesControl instancia;
    private void Awake()
    {
        instancia = this;
    }
    public void Vibrar(TipoVibracion vibracion)
    {
        switch (vibracion)
        {
            case TipoVibracion.Seleccion:
                MMVibrationManager.Haptic(HapticTypes.LightImpact);

                break;
            case TipoVibracion.Impacto:
                MMVibrationManager.Haptic(HapticTypes.HeavyImpact);
               // MMVibrationManager.Haptic(HapticTypes.Failure);

                break;
            case TipoVibracion.Exito:
                MMVibrationManager.Haptic(HapticTypes.Success);

                break;
            case TipoVibracion.Error:
                MMVibrationManager.Haptic(HapticTypes.Failure);

                break;
        }
    }


}
[System.Serializable]
public enum TipoVibracion
{
    Seleccion,
    Impacto,
    Exito,
    Error

}