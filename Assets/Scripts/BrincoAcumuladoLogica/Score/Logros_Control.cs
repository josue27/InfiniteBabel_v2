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
        GameServices.Init();
    }

    
    public void DesbloquearLogro(string logro)
    {
        if(!GameServices.IsInitialized())
        {
            Debug.Log("No se inicializo gameservices");
            return;
        }

        GameServices.UnlockAchievement(logro, (bool exito) =>
        {
            Debug.Log("Logor desbloqueado:" + exito);

        });
        

        /*
         * DEPRECATED: Al parecer solo con el string es el codigo que pide Google
        switch (logro)
        {
            case EM_GameServicesConstants.Achievement_WorkWithStyle:
                GameServices.UnlockAchievement(EM_GameServicesConstants.Achievement_WorkWithStyle, (bool exito) =>
                {
                    Debug.Log("Logor desbloqueado:" + exito);

                });
                break;
            case EM_GameServicesConstants.Achievement_DownTheSystem:
                GameServices.UnlockAchievement(EM_GameServicesConstants.Achievement_DownTheSystem, (bool exito) =>
                {
                    Debug.Log("Logor desbloqueado:" + exito);

                });
                break;
            case EM_GameServicesConstants.Achievement_Looking_Promotion:
                GameServices.UnlockAchievement(EM_GameServicesConstants.Achievement_Looking_Promotion, (bool exito) =>
                {
                    Debug.Log("Logor desbloqueado:" + exito);

                });
                break;
            case EM_GameServicesConstants.Achievement_MakeItRain:
                GameServices.UnlockAchievement(EM_GameServicesConstants.Achievement_MakeItRain, (bool exito) =>
                {
                    Debug.Log("Logor desbloqueado:" + exito);

                });
                break;
            case EM_GameServicesConstants.Achievement_MoreCoffee:
                GameServices.UnlockAchievement(EM_GameServicesConstants.Achievement_MoreCoffee, (bool exito) =>
                {
                    Debug.Log("Logor desbloqueado:" + exito);

                });
                break;
           
            case EM_GameServicesConstants.Achievement_NoShame:
                GameServices.UnlockAchievement(EM_GameServicesConstants.Achievement_NoShame, (bool exito) =>
                {
                    Debug.Log("Logor desbloqueado:" + exito);

                });
                break;
            case EM_GameServicesConstants.Achievement_Rookie_moves:
                GameServices.UnlockAchievement(EM_GameServicesConstants.Achievement_Rookie_moves, (bool exito) =>
                {
                    Debug.Log("Logor desbloqueado:" + exito);

                });
                break;
            case EM_GameServicesConstants.Achievement_Senior_Moves:
                GameServices.UnlockAchievement(EM_GameServicesConstants.Achievement_Senior_Moves, (bool exito) =>
                {
                    Debug.Log("Logor desbloqueado:" + exito);

                });
                break;
            case EM_GameServicesConstants.Achievement_FirstRun:
                GameServices.UnlockAchievement(EM_GameServicesConstants.Achievement_FirstRun, (bool exito) =>
                {
                    Debug.Log("Logor desbloqueado:" + exito);

                });
                break;
            default:
                Debug.Log("No se encontro logro");
                break;
        }
        */

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

        if (logro == EM_GameServicesConstants.Achievement_No_Quitters)
        {
            GameServices.ReportAchievementProgress(EM_GameServicesConstants.Achievement_No_Quitters, cantidad, (exito) =>
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
