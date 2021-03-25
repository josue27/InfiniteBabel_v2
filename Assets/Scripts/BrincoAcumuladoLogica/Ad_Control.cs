using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using EasyMobile;


namespace Brinco
{
    public class Ad_Control : MonoBehaviour
    {

        ConsentStatus moduleConsent;

        [Header("Probabilidad para mostrar Ad Intermedio")]
        public int casosProbables = 3;
        public int casosFavorables = 1;
        [Header("Probabilidad para mostrar Panel Ad Reward")]
        public int casosProbables_PanelRevivir = 3;
        public int casosFavorables_PanelRevivir = 1;
        public GameObject panelRevivir;
        [SerializeField] private GameObject botonAdReward;//reward por cafes
        public bool mostrandoAdRevivir;
        public bool adRewardMostrado;
        public bool mostrandoAdIntermedio;
        public int cafeRecompensa = 5;
        [Tooltip("TRUE si el jugador compro este articulo")]
        public bool removerAdIntermedio;
        [SerializeField] private bool isInitialized;

        public bool reviviendo;

        private void OnEnable()
        {
            Advertising.InterstitialAdCompleted += Advertising_InterstitialAdCompleted;
            Advertising.RewardedAdCompleted += Advertising_RewardedAdCompleted;
            Eventos_Dispatcher.eventos.JugadorPerdio += JugadorPerdio_Callback;
            Eventos_Dispatcher.Reinicio += Reinicio;
            Eventos_Dispatcher.eventos.InicioJuego += InicioJuego;
            Eventos_Dispatcher.eventos.Revivir += Revivir;
            Advertising.AdsRemoved += Advertising_AdsRemoved;
            Eventos_Dispatcher.eventos.OcultarBannerAdd += OcultarBanner;


        }

        /// <summary>
        /// Llamado cuando los ads son removidos
        /// </summary>
        private void Advertising_AdsRemoved()
        {
            //NativeUI.Alert("Ads", "Ads removidos");
            Advertising.AdsRemoved -= Advertising_AdsRemoved;
        }

        private void Awake()
        {
            ObtenerConsentimiento();

        }
        void Start()
        {
            isInitialized = InAppPurchasing.IsInitialized();
            Debug.Log("AdControl: isInitialized" + isInitialized);
            SeComproNoAds();
         
            
           
        }

        private void InicioJuego()
        {

        }

        /// <summary>
        /// Llamado por eventdispatcher para aparecer u ocultar el banner
        /// (normalmente llamado cuando aparece un menu)
        /// </summary>
        /// <param name="ocultar"></param>
        private void OcultarBanner(bool ocultar)
        {
            #if UNITY_IOS
            if(ocultar)
            { 
                
                Advertising.HideBannerAd();
            }else
            {

                MostrarAdBanner();
            }
            #endif

        }

        /// <summary>
        /// Eventos que ocurren cuando el jugador perdio, normalmente se decide si mostar
        /// un anuncion o no?
        /// </summary>
        private void JugadorPerdio_Callback()
        {

            //Loteria para saber si mostramos un AD Intermedio
            int probabilidad = Random.Range(0, casosProbables);
            //if(probabilidad < casosFavorables)
            //{
            //    MostarAdIntermedio();
            //}

            //Loteria para saber si mostramos panel AD Revivir
            probabilidad = Random.Range(0, casosProbables_PanelRevivir);
            if(probabilidad < casosFavorables_PanelRevivir)
            {
                //Mostrar Panel Ad Reward
                MostrarPanelRevivir();
            }

            //Mostrar Banner
            MostrarAdBanner();


        }

        void MostrarAdBanner()
        {
            
            if (removerAdIntermedio)
            {
                Debug.Log("El jugador compra NO ADS, descartando...");
                return;
            }

            Advertising.ShowBannerAd(BannerAdPosition.Bottom);
            Debug.Log("Mostrando Banner");
        }

       
        void ObtenerConsentimiento()
        {
            // Grants the module-level consent for the Advertising module.
            Advertising.GrantDataPrivacyConsent();

            // Revokes the module-level consent of the Advertising module.
           // Advertising.RevokeDataPrivacyConsent();

            // Reads the current module-level consent of the Advertising module.
            moduleConsent = Advertising.DataPrivacyConsent;
        }

        /// <summary>
        /// Si se decide mostrar un AdIntermedio skipeable se muestra aqui
        /// </summary>
        void MostarAdIntermedio()
        {
            if (removerAdIntermedio)
            {
                Debug.Log("El jugador compra NO ADS, descartando...");
                return;
            }

            if (mostrandoAdIntermedio)
                return;

            
            Debug.Log("Mostrando Ad Intermedio");
            bool estaListo = Advertising.IsInterstitialAdReady();

            if (estaListo)
            {
                Advertising.ShowInterstitialAd();
                mostrandoAdIntermedio = true;
            }
        }

        private void Advertising_InterstitialAdCompleted(InterstitialAdNetwork arg1, AdPlacement arg2)
        {
            Debug.Log("Ad Intermedio mostrado");
            //Logros_Control.instancia.LogroIncremental(EM_GPGSIds.achievement_no_quitters, 1);
            //GameServices.ReportAchievementProgress(EM_GPGSIds.achievement_no_quitters, 1);
            //Debug.Log("Achievement incremental...");

            Logros_Control.instancia.LogroIncremental(EM_GameServicesConstants.Achievement_No_Quitters,1);

        }

        #region AdReward

        public void MostrarPanelRevivir()
        {
            panelRevivir.SetActive(true);
        }

        public void MostrarPanelRevivir(bool mostrar)
        {
            panelRevivir.SetActive(mostrar);

        }

        /// <summary>
        /// Activado por UI si el jugador decide ver un AD por una RECOMPENSA de cafe,
        /// agregar sumar la cantidad de cafe por ver el Ad en el callback
        /// </summary>
        public void MostrarAdReward()
        {
            if (mostrandoAdRevivir)
                return;
            MostrarPanelRevivir(false);
            bool estaListo = Advertising.IsRewardedAdReady();
            if(estaListo && !mostrandoAdRevivir)
            {
                mostrandoAdRevivir = true;
                Advertising.ShowRewardedAd();

            }
        }

        /// <summary>
        /// Activado por UI si el jugador decide ver AD para REVIVIR
        /// </summary>
        public void MostrarAd_Revivir()
        {
            if (mostrandoAdRevivir)
                return;
            MostrarPanelRevivir(false);
            bool estaListo = Advertising.IsRewardedAdReady();
            if (estaListo && !mostrandoAdRevivir)
            {
                reviviendo = true;
                mostrandoAdRevivir = true;
                Advertising.ShowRewardedAd();

            }
        }


        /// <summary>
        /// Callback que se activa cuando el AdReward ya termino, se deberia implementar
        /// el agregar la recompensa aqui
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        private void Advertising_RewardedAdCompleted(RewardedAdNetwork arg1, AdPlacement arg2)
        {
            if (adRewardMostrado)
                return;

            adRewardMostrado = true;

            if (reviviendo)
            {
                Debug.Log("Ad Reward Revivir mostrado");
                Master_Level._masterBrinco.Revivir();

                //mostrandoAdRevivir = false;
                reviviendo = false;
                adRewardMostrado=false;
                return;
            }
            else
            {


                Debug.Log("Ad Reward mostrado");
                Score_Control.instancia.SumarMonedas(cafeRecompensa, true);
                if (this.GetComponent<Compras_Control>())
                    this.GetComponent<Compras_Control>().AnimacionMonedaComprada();
                mostrandoAdRevivir = false;
                adRewardMostrado = false;
                botonAdReward.SetActive(false);

            }

        }
        #endregion 

        public void RemoverAds(bool remover)
        {
            //Al parecer solo con RemoveAds() basta
            removerAdIntermedio = remover;
            if(remover)
            {
                Advertising.RemoveAds();
                //Advertising.RemoveAds(true);

            }
            Debug.Log("El jugador tiene comprado no ADS");
        }

        private void OnDisable()
        {
            Advertising.InterstitialAdCompleted -= Advertising_InterstitialAdCompleted;
            Advertising.RewardedAdCompleted -= Advertising_RewardedAdCompleted;
            Eventos_Dispatcher.eventos.JugadorPerdio -= JugadorPerdio_Callback;
            Eventos_Dispatcher.Reinicio -= Reinicio;
            Eventos_Dispatcher.eventos.Revivir -= Revivir;
            //Duda si volver a desactivar aqui esto
            Advertising.AdsRemoved -= Advertising_AdsRemoved;
            Eventos_Dispatcher.eventos.OcultarBannerAdd += OcultarBanner;



        }

        //Reviza si el usuario compro el producto de remover ads intermedios y se lo manda a Ad_Control.cs
        public void SeComproNoAds()
        {
            Debug.Log("AdControl: revizando compra noAds");
            if (isInitialized)
            {
                bool remover = InAppPurchasing.IsProductOwned(EM_IAPConstants.Product_RemoverAds);

                RemoverAds(remover);
                Debug.Log("AdControl: noAds = "+remover);

            }

        }
        public void Reinicio()
        {
            MostrarPanelRevivir(false);
            Advertising.HideBannerAd();
            adRewardMostrado = false;
            botonAdReward.SetActive(true);

        }


        public void Revivir()
        {
            adRewardMostrado = false;
            MostrarPanelRevivir(false);
            //Advertising.HideBannerAd();
            botonAdReward.SetActive(true);


        }
    }
}
