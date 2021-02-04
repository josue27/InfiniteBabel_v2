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
        public int casosProbables_AdReward = 3;
        public int casosFavorables_AdReward = 1;
        public GameObject panelAdReward;
        public bool mostrandoAdReward;
        public bool adRewardMostrado;
        public bool mostrandoAdIntermedio;
        public int monedasRecompensa = 10;
        [Tooltip("TRUE si el jugador compro este articulo")]
        public bool removerAdIntermedio;
        [SerializeField] private bool isInitialized;


        private void OnEnable()
        {
            Advertising.InterstitialAdCompleted += Advertising_InterstitialAdCompleted;
            Advertising.RewardedAdCompleted += Advertising_RewardedAdCompleted;
            Eventos_Dispatcher.eventos.JugadorPerdio += JugadorPerdio_Callback;
            Eventos_Dispatcher.Reinicio += Reinicio;
            Advertising.AdsRemoved += Advertising_AdsRemoved;


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
            //Esta autonicializado
            //GameServices.Init();
           
        }


        /// <summary>
        /// Eventos que ocurren cuando el jugador perdio, normalmente se decide si mostar
        /// un anuncion o no?
        /// </summary>
        private void JugadorPerdio_Callback()
        {

            //Loteria para saber si mostramos un AD Intermedio
            int probabilidad = Random.Range(0, casosProbables);
            if(probabilidad < casosFavorables)
            {
                MostarAdIntermedio();
            }

            //Loteria para saber si mostramos panel AD Reward
            probabilidad = Random.Range(0, casosProbables_AdReward);
            if(probabilidad < casosFavorables_AdReward)
            {
                //Mostrar Panel Ad Reward
                MostrarPanelAdReward();
            }


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

        public void MostrarPanelAdReward()
        {
            panelAdReward.SetActive(true);
        }

        public void MostrarPanelAdReward(bool mostrar)
        {
            panelAdReward.SetActive(mostrar);

        }

        /// <summary>
        /// Activado por UI si el jugador decide ver un AD por una recompenza de dinero,
        /// agregar sumar la cantidad de dinero por ver el Ad en el callback
        /// </summary>
        public void MostrarAdReward()
        {
            if (mostrandoAdReward)
                return;
            MostrarPanelAdReward(false);
            bool estaListo = Advertising.IsRewardedAdReady();
            if(estaListo && !mostrandoAdReward)
            {
                mostrandoAdReward = true;
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

            Debug.Log("Ad Reward mostrado");
            Score_Control.instancia.SumarMonedas(monedasRecompensa,true);
            if(this.GetComponent<Compras_Control>())
                this.GetComponent<Compras_Control>().AnimacionMonedaComprada();
        }
        #endregion 

        public void RemoverAds(bool remover)
        {
            
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

            //Duda si volver a desactivar aqui esto
            Advertising.AdsRemoved -= Advertising_AdsRemoved;


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
            MostrarPanelAdReward(false);

        }
    }
}
