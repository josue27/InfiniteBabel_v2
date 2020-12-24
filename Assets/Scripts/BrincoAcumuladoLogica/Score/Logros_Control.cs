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
        
       
        switch (logro)
        {
            case EM_GPGSIds.achievement_work_with_style:
                GameServices.UnlockAchievement(EM_GPGSIds.achievement_work_with_style, (bool exito) =>
                {
                    Debug.Log("Logor desbloqueado:" + exito);

                });
                break;
            case EM_GPGSIds.achievement_down_the_system:
                GameServices.UnlockAchievement(EM_GPGSIds.achievement_down_the_system, (bool exito) =>
                {
                    Debug.Log("Logor desbloqueado:" + exito);

                });
                break;
            case EM_GPGSIds.achievement_looking_that_promotion:
                GameServices.UnlockAchievement(EM_GPGSIds.achievement_looking_that_promotion, (bool exito) =>
                {
                    Debug.Log("Logor desbloqueado:" + exito);

                });
                break;
            case EM_GPGSIds.achievement_make_it_rain:
                GameServices.UnlockAchievement(EM_GPGSIds.achievement_make_it_rain, (bool exito) =>
                {
                    Debug.Log("Logor desbloqueado:" + exito);

                });
                break;
            case EM_GPGSIds.achievement_more_coffee_please:
                GameServices.UnlockAchievement(EM_GPGSIds.achievement_more_coffee_please, (bool exito) =>
                {
                    Debug.Log("Logor desbloqueado:" + exito);

                });
                break;
           
            case EM_GPGSIds.achievement_no_shame:
                GameServices.UnlockAchievement(EM_GPGSIds.achievement_no_shame, (bool exito) =>
                {
                    Debug.Log("Logor desbloqueado:" + exito);

                });
                break;
            case EM_GPGSIds.achievement_rookie_moves:
                GameServices.UnlockAchievement(EM_GPGSIds.achievement_rookie_moves, (bool exito) =>
                {
                    Debug.Log("Logor desbloqueado:" + exito);

                });
                break;
            case EM_GPGSIds.achievement_senior_moves:
                GameServices.UnlockAchievement(EM_GPGSIds.achievement_senior_moves, (bool exito) =>
                {
                    Debug.Log("Logor desbloqueado:" + exito);

                });
                break;
            case EM_GPGSIds.achievement_welcome_to_the_late_shift:
                GameServices.UnlockAchievement(EM_GPGSIds.achievement_welcome_to_the_late_shift, (bool exito) =>
                {
                    Debug.Log("Logor desbloqueado:" + exito);

                });
                break;
            default:
                Debug.Log("No se encontro logro");
                break;
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

        if (logro == EM_GPGSIds.achievement_no_quitters)
        {
            GameServices.ReportAchievementProgress(EM_GPGSIds.achievement_no_quitters, cantidad, (exito) =>
            {

                Debug.Log("Logro incrementado:" + exito);
            });
        }

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
