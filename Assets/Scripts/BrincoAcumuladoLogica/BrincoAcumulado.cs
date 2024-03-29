﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Brinco;
using Lean.Touch;
using Lean.Common;
using MoreMountains.NiceVibrations;

public class BrincoAcumulado : MonoBehaviour
{


    public bool enPiso;
    public float fuerzaSalto = 1.0f;
    public Transform posEnPiso;
    [Header("KeyBindigns")]
    public KeyCode teclaSalto;
    public Rigidbody rigid;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    public float acumulacionFuerza = 0.0f;
    public float maxFuerza = 16.5f;
    public float stepAcumulacion = 0.1f;

    public float stepAcumulacionMin = 0.1f;
    public float stepAcumulacionMax = 0.5f;

    public float empujeEnCruze = 1.0f;

    public bool presionada;
    public bool soltada;

    [Header("UIX")]
    public Slider slider_fuerzaBrinco;
    public Text scoreActual;

    bool jugando;

    public bool cruzandoApertura;
    public float duracionShake;
    public float intencidadShake;
    public Vector2 mousePosInit = Vector2.zero;
    public Vector2 mousePosFinal = Vector2.zero;
    public float distanciaMouse;

   // public Animator sprite_anim;
    public GameObject sprite_golpe;

    public LineRenderer trazoDedo;

    [Header("Muerte settings")]
    public Transform posicionMuerte;
    public float duracionAPosicionMuerte = 1.0f;
    public bool muerto;

    [Header("SFX")]
    public AudioSource audioJugador;
    public List<ClipSonido> clipsSonido_Jugador = new List<ClipSonido>();
    [Header("VFX")]
    public GameObject humofx;
    public ParticleSystem shadow_fx;
    
    public GameObject imagenDebug;

    public bool inmortal;
    public float multiplicadorSensibilidad  = 1.2f;

    public Vector3 Ubicacion{ get; set;}

    public Vector3 mousePos;

   public Sprite_Animador spriteAnim;
    public GameObject spritePersonaje;
    
    public bool puedeBrincar;

    
    public bool enTrigger;

    [SerializeField] int toques = 0;

  
    void Start()
    {
        rigid = this.GetComponent<Rigidbody>();

       // Eventos_Dispatcher.eventos.JugadorPerdio += PerdioJuego;
        Eventos_Dispatcher.eventos.InicioJuego += InicioJuego;
        Eventos_Dispatcher.Reinicio += Reiniciar;
        Eventos_Dispatcher.eventos.Revivir += Revivir;
        LeanTouch.OnFingerDown+= DedoDown;
        LeanTouch.OnFingerUp +=DedoArriba;

        spriteAnim?.CambioAnimacion("idle",true);


    }

    // Update is called once per frame
    void Update()
    {
       // Debug.Log("Screen"+Screen.width+"w "+Screen.height+" h");


            if(imagenDebug)
            {
                    imagenDebug.transform.position = Input.mousePosition;
            }




        if(Input.GetKeyDown(KeyCode.S))
        {
          //  CameraShake.Shake(duracionShake,intencidadShake);

        }
        if(Input.GetKeyUp(KeyCode.Space))
        {
            EmpujarJugador(Vector3.up, empujeEnCruze);

        }
        if (!jugando)
            return;

#if UNITY_ANDROID
        // Brinco_Movil();
#endif
#if UNITY_EDITOR || UNITY_WEBGL
        //   Brinco_PC();

#endif
        if (enPiso)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                toques++;
            }
        }
         
        //BrincoSwipePC();

        //Esta condicion solo se cumple cuando esta en su punto mas alto o en posicion inicial
        //donde  en ambas partes la velocidad inicial debe ser 0 
      
    }

    /// <summary>
    /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    /// </summary>
    void FixedUpdate()
    {
        //MEJORAR ESTO CON EL SISTEMA DE PERSONAJE DE CATLIKECODING
          if (rigid.velocity.y < 0)
        {
            rigid.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            if(!muerto && !enPiso)
            {
                //  ReproducirAnimacion("caida");
                spriteAnim.CambioAnimacion("caida",false);
                if(shadow_fx.isPlaying) shadow_fx.Stop();
            }

        }
        //esto solo se cumple cuando la velocidad es mayor a 0 y no llego a su punto mas alto
        else if (rigid.velocity.y > 0 && !Input.GetKey(teclaSalto))
        {
            rigid.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        slider_fuerzaBrinco.value = Mathf.Lerp(slider_fuerzaBrinco.value, acumulacionFuerza, 1.0f);
    }
  

    private void OnCollisionEnter(Collision collision)
    {
      
        if (collision.transform.tag == "piso")
        {
            enPiso = true;

            Camara_Control.camara.ShakeCam_Call();
            
            if(muerto)
            {
              // LeanTween.moveZ(this.gameObject,posicionMuerte.position.z,duracionAPosicionMuerte);
             //  ReproducirAnimacion("muerto");
             spriteAnim.CambioAnimacion("muerte",false);
            }else if(!muerto)
            {
                if(Master_Level._masterBrinco.estadoJuego == EstadoJuego.inicio)
                {
                    // ReproducirAnimacion("idle");
                     spriteAnim.CambioAnimacion("idle",true);

                }
                else
                {
                     spriteAnim.CambioAnimacion("corriendo",true);

                 //   ReproducirAnimacion("corriendo");
                }
                //  ReproducirAnimacion("corriendo",true);
                MMVibrationManager.Haptic(HapticTypes.LightImpact);

            }

            if(this.transform.position.z != posEnPiso.position.z && !muerto)
            {
                LeanTween.moveZ(this.gameObject,posEnPiso.position.z,0.3f);
            }
            
            ReproducirSonido_Jugador("golpePiso");
            StartCoroutine(HumoFX());

            //TODO: shake cam
        }
       
        print(collision.transform.name);

        

      
      
    }

    
    public void Display(Vector3 value)
	{
		//Display(value.ToString());
		
    }
    public void DedoDown(LeanFinger dedo)
    {
        if(Master_Level._masterBrinco.estadoJuego == EstadoJuego.inicio || Master_Level._masterBrinco.estadoJuego == EstadoJuego.perdio ||
            !puedeBrincar)
            return;
       // Debug.Log("Dedo abajo: "+dedo.GetWorldPosition(10,Camera.main));
        mousePosInit = dedo.GetWorldPosition(10,Camera.main);

        StopCoroutine(DesctivarTrail());
        trazoDedo.transform.GetComponent<TrailRenderer>().enabled = true;
        trazoDedo.transform.position = dedo.GetWorldPosition(1,Camera.main);

        MMVibrationManager.Haptic(HapticTypes.Selection);

    }
    public void DedoArriba(LeanFinger dedo)
    {
        //if(Master_Level._masterBrinco.estadoJuego != EstadoJuego.jugando)
        // return;
        if (Master_Level._masterBrinco.estadoJuego == EstadoJuego.inicio || Master_Level._masterBrinco.estadoJuego == EstadoJuego.perdio ||
            !puedeBrincar)
            return;

        if (enPiso)
        { 
            // Debug.Log("Dedo arriba: "+dedo.GetWorldPosition(10,Camera.main));
            
            mousePosFinal = dedo.GetWorldPosition(10,Camera.main);
            //Solo queremos saber la distancia en Y
            // distanciaMouse = Vector3.Distance( mousePosFinal , mousePosInit ) * multiplicadorSensibilidad;
            distanciaMouse = ( mousePosInit.y - mousePosFinal.y) * multiplicadorSensibilidad;
           // Debug.Log("Distancia dedo: "+ distanciaMouse);
            //trazoDedo.SetPosition(0,dedo.StartScreenPosition);
            StartCoroutine(DesctivarTrail());
            trazoDedo.transform.position = dedo.GetWorldPosition(1,Camera.main);

            // acumulacionFuerza = Mathf.Clamp(distanciaMouse/100,0.0f,maxFuerza);
            acumulacionFuerza = Mathf.Clamp(distanciaMouse,0f,maxFuerza);
                    
            shadow_fx.Play();
            if(rigid){
                rigid.velocity = Vector3.up * acumulacionFuerza;
            }
            //print("Fuerza:" + acumulacionFuerza);
            acumulacionFuerza = 0;
            this.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            //if(Master_Level._masterBrinco.estadoJuego == EstadoJuego.tutorial)
            //{
            //    Master_Level._masterBrinco.InstruccionesCompletada("brinco");
            //}

        }else if(!enPiso)
        {
            mousePosFinal = dedo.GetWorldPosition(10,Camera.main);
            distanciaMouse = (  mousePosFinal.y - mousePosInit.y ) * multiplicadorSensibilidad;

            StartCoroutine(DesctivarTrail());
            trazoDedo.transform.position = dedo.GetWorldPosition(1,Camera.main);

      
            acumulacionFuerza = Mathf.Clamp(distanciaMouse,0f,maxFuerza);
            shadow_fx.Play();
            
            if(rigid)
                rigid.velocity = Vector3.down * acumulacionFuerza;
            
            //print("Fuerza:" + acumulacionFuerza);
            acumulacionFuerza = 0;
            this.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            //if (Master_Level._masterBrinco.estadoJuego == EstadoJuego.tutorial)
            //{
            //    Master_Level._masterBrinco.InstruccionesCompletada("regreso");
            //}

        }
       
    }
   
    IEnumerator DesctivarTrail()
    {
        yield return new WaitForSeconds(0.5f);
        trazoDedo.transform.GetComponent<TrailRenderer>().enabled = false;
    }
    /// <summary>
    /// OnCollisionStay is called once per frame for every collider/rigidbody
    /// that is touching rigidbody/collider.
    /// </summary>
    /// <param name="other">The Collision data associated with this collision.</param>
    
    //void OnCollisionStay(Collision other)
    //{
    //    if(other.transform.CompareTag("piso"))
    //    {
    //        if(Master_Level._masterBrinco.estadoJuego == EstadoJuego.jugando)
    //        {
    //            //ReproducirAnimacion("corriendo");

    //        }
    //    }
    //}
   
    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.tag == "piso")
        {
            enPiso = false;
            //ReproducirAnimacion("brinco");
            spriteAnim.CambioAnimacion("brinco",false);
        }
       
    }
    private void OnTriggerEnter(Collider other)
    {

        //TODO: Cambiar a un script independiento o EventDispatcher
        //BUG:no esta detectando las TNT

        if (enTrigger)
            return;

    
        switch (other.tag)
        {
            case "pared":
                if (!cruzandoApertura)
                {
                    enTrigger = true;
                    EmpujarJugador(new Vector3(0.0f, 1.0f, 1.0f), 50.0f);
                    //Eventos_Dispatcher.eventos.JugadorPerdio();
                    PerdioJuego();
                    RotarSprite();
                    ReproducirAnimacion("muerto");
                    VibracionGolpe();   
                }
                break;
            case "paredTNT":
                if (!cruzandoApertura)
                {
                    enTrigger = true;
                    EmpujarJugador(new Vector3(0.0f, 1.0f, 1.0f), 150.0f);
                    //Eventos_Dispatcher.eventos.JugadorPerdio();
                    PerdioJuego();

                    ReproducirAnimacion("muerto");
                    RotarSprite();
                    other.gameObject.SetActive(false);
                    other.transform.parent.parent.GetComponent<ObstaculoControl>().ExplotarTNT(other.transform);
                    Debug.Log("Jugador KABOOOOMM!!!");
                    VibracionGolpe();

                }
                break;
            case "pisoRoto":
                if (!cruzandoApertura)
                {
                    enTrigger = true;
                    ReproducirAnimacion("muerto");
                    GetComponent<CapsuleCollider>().enabled = false;
                    rigid.useGravity = false;
                    Vector3 posFinalCaida = other.transform.position + new Vector3(0f, -0.2f, 0f);
                    LeanTween.move(this.gameObject, posFinalCaida, 0.1f).setOnComplete(()=> 
                        {
                            StartCoroutine(HumoFX());
                        });


                    //Eventos_Dispatcher.eventos.JugadorPerdio();
                    PerdioJuego();

                    Debug.Log("Jugador entro a piso roto:" + other.transform.name);
                    VibracionGolpe();

                }
                break;
            case "apertura":
                Eventos_Dispatcher.eventos.CruceObstaculo_Call();
                EmpujarJugador(Vector3.up, empujeEnCruze);

                cruzandoApertura = true;
                enTrigger = false;

                break;
            default:
                Debug.Log("Colision no encontrada contra:" + other.tag);
                break;
        }


        

    }
    private void VibracionGolpe()
    {
        VibracionesControl.instancia.Vibrar(TipoVibracion.Impacto);
    }
    private void OnTriggerExit(Collider other) {
        if(other.transform.tag == "apertura")
            {
                 cruzandoApertura = false;
            

            }
        enTrigger = false;

    }
    ///

    private void InicioJuego()
    {
        jugando = true;
        spriteAnim.CambioAnimacion("corriendo",true);
       //sprite_anim.SetTrigger("idle");

    }

    

    private void PerdioJuego()
    {
        jugando = false;
        // ReproducirAnimacion("muerto");
        spriteAnim.CambioAnimacion("muerte",false);
        //this.rigid.isKinematic = true;
        ReproducirSonido_Jugador("caidaPerdio");
        muerto = true;
        StartCoroutine(PerdioJuego_Rutina());
    }
    private IEnumerator PerdioJuego_Rutina()
    {
        yield return new WaitForSeconds(1.0f);
        Eventos_Dispatcher.eventos.JugadorPerdio();

    }

    /// <summary>
    /// Activa un trigger dentro del animator del personaje
    /// </summary>
    /// <param name="trigger">idle,muerto,caida,brinco,corriendo</param>
    private void ReproducirAnimacion(string trigger)
    {
        sprite_golpe.SetActive(true);
        
       // sprite_anim.SetTrigger(trigger);
        spriteAnim.CambioAnimacion(trigger);
    }
  
    /// <summary>
    /// Activa un bool dentro del animator del personaje
    /// </summary>
    /// <param name="nombre">corriendo</param>
    /// <param name="estado">true/false</param>
    private void ReproducirAnimacion(string nombre,bool estado)
    {
        //sprite_anim.SetBool(nombre,estado);
        spriteAnim.CambioAnimacion(nombre);

    }

    private void EmpujarJugador(Vector3 direccion,float fuerza)
    {
        print("Choco con pared");
        this.rigid.AddForce(direccion * fuerza);
       
    }

    //cgs-32 o cgs-10 golpe en piso
    /// <summary>
    /// Reproduce un sonido a un audio source buscandolo por su nombre
    /// </summary>
    /// <param name="nombrePista">brinco,brincoRegreso,caidaPerdio,golpePiso</param>
    public void ReproducirSonido_Jugador(string nombrePista)
    {
          foreach(ClipSonido clipSonido in clipsSonido_Jugador)
        {
            if(clipSonido.nombre.Equals(nombrePista))
            {
                audioJugador.clip = clipSonido.clip;
                audioJugador.Play();
                
                break;
            }
        }
    }

    public void RotarSprite()
    {
        //LeanTween.rotateZ(spritePersonaje, 359, 0.5f);
        LeanTween.rotateAroundLocal(spritePersonaje, Vector3.forward , -360f, 0.5f);
    }

    private IEnumerator HumoFX(){
        if(humofx.activeInHierarchy)
            humofx.SetActive(false);
        humofx.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        humofx.SetActive(false);
    }
    private void OnDestroy()
    {
        //Eventos_Dispatcher.eventos.JugadorPerdio -= PerdioJuego;
        Eventos_Dispatcher.eventos.InicioJuego -= InicioJuego;
        LeanTouch.OnFingerDown -= DedoDown;
        LeanTouch.OnFingerUp -= DedoArriba;
        Eventos_Dispatcher.eventos.Revivir -= Revivir;

        Eventos_Dispatcher.Reinicio -= Reiniciar;

    }

    public void Reiniciar()
    {
      
        ReproducirAnimacion("idle");
        
        gameObject.transform.position = new Vector3(0f, 0f, -3.05f);
        //gameObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        //reorientamos el sprite porque le aplicamos un efecto cuando choca de rotacion
        spritePersonaje.transform.rotation = Quaternion.Euler(0f, -90f, 0f);
        // no aseguramos de devolverlo a su posicion original
        LeanTween.moveZ(this.gameObject, posEnPiso.position.z, 0.3f);
        puedeBrincar = false;
        muerto = false;
    }

    /// <summary>
    /// Invocado por evento para continuar el juego, ajusta la animacion
    /// y la posicion
    /// </summary>
    public void Revivir()
    {
        ReproducirAnimacion("idle");
        
        gameObject.transform.position = new Vector3(0f, 0f, -3.05f);
        //gameObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        //reorientamos el sprite porque le aplicamos un efecto cuando choca de rotacion
        spritePersonaje.transform.rotation = Quaternion.Euler(0f, -90f, 0f);
        // no aseguramos de devolverlo a su posicion original
        LeanTween.moveZ(this.gameObject, posEnPiso.position.z, 0.3f);
        puedeBrincar = true;
        muerto = false;
    }

    /// <summary>
    /// Lamado(Master.cs) para acomodar el personaje, pero no puede brincar
    /// hasta el event Revivir sea invocado
    /// </summary>
    public void PrepararParaRevivir()
    {
        ReproducirAnimacion("idle");

        gameObject.transform.position = new Vector3(0f, 0f, -3.05f);
        //gameObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        //reorientamos el sprite porque le aplicamos un efecto cuando choca de rotacion
        spritePersonaje.transform.rotation = Quaternion.Euler(0f, -90f, 0f);
        // no aseguramos de devolverlo a su posicion original
        LeanTween.moveZ(this.gameObject, posEnPiso.position.z, 0.3f);
        muerto = false;

    }

}
