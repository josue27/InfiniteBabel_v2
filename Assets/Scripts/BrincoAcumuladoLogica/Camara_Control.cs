using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        Camera cam;
        bool moverCamara;

        private void Awake()
        {
            cam = GetComponent<Camera>();
        }
        void Start()
        {
            camara = this;
            Eventos_Dispatcher.eventos.InicioJuego += InicioJuego;

        }
        public void MePudeVer()
        {
            print(this.transform.name + "me pueden ver");
        }
        // Update is called once per frame
        void Update()
        {
            if(moverCamara)
            {
                MoverCamaraInicio();
            }
            if(Input.GetKeyDown(KeyCode.S))
            {
                StartCoroutine(ShakeCam(duracionShake, magnitudShake));
            }
        }
        public void ShakeCam_Call()
        {
             StartCoroutine(ShakeCam(duracionShake, magnitudShake));

        }
        IEnumerator ShakeCam(float duracion, float magnitud)
        {
            Vector3 posActual = this.transform.position;

            float tiempoPasado = 0.0f;
            while(tiempoPasado<duracion)
            {
                float x = Random.Range(-1.0f, 1.0f) * magnitud;
                float y = Random.Range(-1.0f, 1.0f) * magnitud;


                 Vector3 nuevaPos =  new Vector3(this.transform.position.x + x, this.transform.position.y + y, posActual.z);
                this.transform.position = Vector3.Lerp(this.transform.position, nuevaPos, Time.deltaTime * 5.0f);
                tiempoPasado += Time.deltaTime;

                yield return null;
            }
            while (this.transform.position.x != posActual.x)
            {


                this.transform.position = Vector3.Lerp(this.transform.position, posActual, Time.deltaTime * 1.0f);
                yield return null;
            }
        }

        void InicioJuego()
        {
            moverCamara = true;
        }

        void MoverCamaraInicio()
        {
            this.transform.position = Vector3.Lerp(this.transform.position, posJuego.position, Time.deltaTime * velocidad);
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, zoomJuego, Time.deltaTime * velocidad);
        }
        private void OnDestroy()
        {
            Eventos_Dispatcher.eventos.InicioJuego -= InicioJuego;

        }

    }
}
