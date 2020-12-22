using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using EasyMobile;
public class Logros_Control : MonoBehaviour
{
    public static Logros_Control instancia;
    // Start is called before the first frame update
    void Start()
    {
        instancia = this; 
    }

    public void DesbloquearLogro(string logro)
    {
        if(!GameServices.IsInitialized())
        {
            Debug.Log("No se inicializo gameservices");
            return;
        }
        if (logro == EM_GPGSIds.achievement_work_with_style)
        {
            GameServices.UnlockAchievement(EM_GPGSIds.achievement_work_with_style, (bool exito) =>
            {
                Debug.Log("Logor desbloqueado:" + exito);

            });
        }


    }

    /// <summary>
    /// Trata de registrar un incremento en un logro
    /// </summary>
    /// <param name="logro">el id del logor (EM_GPGSIds)</param>
    /// <param name="cantidad">la cantidad que querramos de acuerdo a los setting de GooglePlay</param>
    public void LogroIncremental(string logro,double cantidad)
    {
        if (!GameServices.IsInitialized())
        {
            Debug.Log("No se inicializo gameservices");
            return;
        }

        GameServices.ReportAchievementProgress(logro, cantidad, (exito) =>
        {

            Debug.Log("Logro incrementado:" + exito);
        });
    }
    

    /// <summary>
    /// Llamado por Interfa de Usuario en los botones superiores muestra nativamente los logros
    /// </summary>
    public void MostrarGoogleAchievemnts()
    {


        if (GameServices.IsInitialized())
        {

            GameServices.ShowAchievementsUI();
        }
        else
        {
#if UNITY_ANDROID
            GameServices.Init();    // start a new initialization process
#elif UNITY_IOS
            Debug.Log("Cannot show achievements UI: The user is not logged in to Game Center.");
#endif
        }
    }



}
