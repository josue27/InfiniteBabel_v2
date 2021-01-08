using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace Brinco
{

    public class Camara_Control : MonoBehaviour
    {
        public static Camara_Control camara;
        public Transform posJuego;
        public Transform posInicio;
        public float zoomInicial,zoomJuego;

        public float duracionShake = 1.0f;

        public float magnitudShake = 0.01f;

        public float velocidad;
        [SerializeField]
        private float duracion =1.5f;
        Camera cam;
        bool moverCamara;
        [SerializeField]
        private CinemachineVirtualCamera cmVirtualCamera;

        CinemachineBasicMultiChannelPerlin cmBasicMultiChannelPerlin;


        float shakeTimer;
        float shakeTimerTotal;
        float startingIntensity;
        private void Awake()
        {
            cam = GetComponent<Camera>();
        }
        void Start()
        {
            camara = this;
            Eventos_Dispatcher.eventos.InicioJuego += InicioJuego;
            Eventos_Dispatcher.eventos.JugadorPerdio += FinJuego;
            Eventos_Dispatcher.Reinicio += Reinicio;
           // StartCoroutine(ShakeCam(duracionShake, magnitudShake));

        }
       
       
        [EasyButtons.Button]
        public void CamaraJuego()
        {
            //moverCamara = true;
            MoverCamaraJuego();


        }
        [EasyButtons.Button]
        public void CamaraInicio()
        {
            moverCamara = false;
            MoverCamaraAInicio();

        }
        public void ShakeCam_Call()
        {
            
            ShakeCamera(duracionShake, magnitudShake);

        }

        //DEPRECATED(08/07/2021) Se utiliza ahora un script junto a Cinemachine
        //IEnumerator ShakeCam(float duracion, float magnitud)
        //{
        //    //if (Master_Level._masterBrinco.estadoJuego != EstadoJuego.jugando)
        //    //    yield break;

        //    Vector3 posActual = this.transform.position;

        //    float tiempoPasado = 0.0f;
        //    while(tiempoPasado<duracion)
        //    {
        //        float x = Random.Range(-1.0f, 1.0f) * magnitud;
        //        float y = Random.Range(-1.0f, 1.0f) * magnitud;


        //         Vector3 nuevaPos =  new Vector3(this.transform.position.x + x, this.transform.position.y + y, posActual.z);
        //        this.transform.position = Vector3.Lerp(this.transform.position, nuevaPos, Time.deltaTime * 5.0f);
        //        tiempoPasado += Time.deltaTime;

        //        yield return null;
        //    }
        //    while (this.transform.position.x != posActual.x)
        //    {


        //        this.transform.position = Vector3.Lerp(this.transform.position, posActual, Time.deltaTime * 1.0f);
        //        yield return null;
        //    }
        //}


        public void ShakeCamera(float duracion, float magnitud)
        {
             cmBasicMultiChannelPerlin = cmVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            cmBasicMultiChannelPerlin.m_AmplitudeGain = magnitud;

            shakeTimer = duracion;
            shakeTimerTotal = duracion;
            startingIntensity = magnitud;

           // StartCoroutine(ShakeCameraParar(duracion));
        }
        

        private void Update()
        {
            if(shakeTimer>0)
            {
                shakeTimer -= Time.deltaTime;
                cmBasicMultiChannelPerlin.m_AmplitudeGain = Mathf.Lerp(startingIntensity, 0f, 1-(shakeTimer / shakeTimerTotal));
            }
        }
        void InicioJuego()
        {
            // moverCamara = true;
            MoverCamaraJuego();

        }
        void FinJuego()
        {
            moverCamara = false;
            
        }

        void MoverCamaraJuego()
        {
            LeanTween.move(cmVirtualCamera.gameObject, posJuego.position, duracion).setEaseOutSine();
            Debug.Log("Moviendo camara a Juego");
         
        }
        void MoverCamaraAInicio()
        {
        
            cmVirtualCamera.transform.position = posInicio.position;
            cam.orthographicSize = zoomInicial;
        }
        private void OnDestroy()
        {
            Eventos_Dispatcher.Reinicio -= Reinicio;
            Eventos_Dispatcher.eventos.InicioJuego -= InicioJuego;

        }
        void Reinicio()
        {
           // moverCamara = false;
            MoverCamaraAInicio();
        }
    }
}
