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

        [Tooltip("TRUE si el jugador compro este articulo")]
        public bool noAdIntermedio;
        private void OnEnable()
        {
            Advertising.InterstitialAdCompleted += Advertising_InterstitialAdCompleted;
            Advertising.RewardedAdCompleted += Advertising_RewardedAdCompleted;
            Eventos_Dispatcher.eventos.JugadorPerdio += JugadorPerdio_Callback;


        }
        private void Awake()
        {
            ObtenerConsentimiento();

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

        void Start()
        {

        }



        void ObtenerConsentimiento()
        {
            // Grants the module-level consent for the Advertising module.
            Advertising.GrantDataPrivacyConsent();

            // Revokes the module-level consent of the Advertising module.
            Advertising.RevokeDataPrivacyConsent();

            // Reads the current module-level consent of the Advertising module.
            moduleConsent = Advertising.DataPrivacyConsent;
        }

        /// <summary>
        /// Si se decide mostrar un AdIntermedio skipeable se muestra aqui
        /// </summary>
        void MostarAdIntermedio()
        {
            if (noAdIntermedio)
            {
                Debug.Log("El jugador compra NO ADS, descartando...");
                return;
            }
            Debug.Log("Mostrando Ad Intermedio");
            bool estaListo = Advertising.IsInterstitialAdReady();

            if (estaListo)
            {
                Advertising.ShowInterstitialAd();
            }
        }

        private void Advertising_InterstitialAdCompleted(InterstitialAdNetwork arg1, AdPlacement arg2)
        {
            Debug.Log("Ad Intermedio mostrado");
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
            Debug.Log("Ad Reward mostrado");
            Score_Control.instancia.SumarMonedas(10);
        }
        #endregion 



        private void OnDisable()
        {
            Advertising.InterstitialAdCompleted -= Advertising_InterstitialAdCompleted;

        }
    }
}
