using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Brinco;
public class BrincoAcumulado : MonoBehaviour
{

    int aperturas = 0;

    

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

    public Animator sprite_anim;

    [Header("Muerte settings")]
    public Transform posicionMuerte;
    public float duracionAPosicionMuerte = 1.0f;
    public bool muerto;

    [Header("SFX")]
    public AudioSource audioJugador;
    public List<ClipSonido> clipsSonido_Jugador = new List<ClipSonido>();

    void Start()
    {
        rigid = this.GetComponent<Rigidbody>();

        Eventos_Dispatcher.eventos.JugadorPerdio += PerdioJuego;
        Eventos_Dispatcher.eventos.InicioJuego += InicioJuego;
    }

    // Update is called once per frame
    void Update()
    {


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
#if UNITY_EDITOR
     //   Brinco_PC();
        BrincoSwipePC();

#endif
  
         

        //Esta condicion solo se cumple cuando esta en su punto mas alto o en posicion inicial
        //donde  en ambas partes la velocidad inicial debe ser 0 
        if (rigid.velocity.y < 0)
        {
            rigid.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
           

        }
        //esto solo se cumple cuando la velocidad es mayor a 0 y no llego a su punto mas alto
        else if (rigid.velocity.y > 0 && !Input.GetKey(teclaSalto))
        {
            rigid.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        slider_fuerzaBrinco.value = Mathf.Lerp(slider_fuerzaBrinco.value, acumulacionFuerza, 1.0f);
    }



    private void LateUpdate()
    {
       // scoreActual.text = aperturas.ToString();
    }

    private void OnCollisionEnter(Collision collision)
    {
      
        if (collision.transform.tag == "piso")
        {
            enPiso = true;
            Camara_Control.camara.ShakeCam_Call();
            if(muerto)
               LeanTween.moveZ(this.gameObject,posicionMuerte.position.z,duracionAPosicionMuerte);

            if(this.transform.position.z != posEnPiso.position.z)
            {
                LeanTween.moveZ(this.gameObject,posEnPiso.position.z,0.3f);
            }
            ReproducirSonido_Jugador("golpePiso");

            //TODO: shake cam
        }
        print(collision.transform.name);

        

        // if(collision.transform.tag == "pared")
        // {
        //      if(!cruzandoApertura)
        //      {
        //        EmpujarJugador(new Vector3(0.0f, 1.0f, -1.0f), 50.0f);
        //      }//TODO:activar chispas de suelo, ver collisionGetContact
        // }

        //Esto no funciona si el jugador esta en el suelo
        //ContactPoint c = collision.GetContact(0);
        //if(c.normal.z < 0)
        //{
        //    print("Choco con pared");
        //    this.rigid.AddForce(Vector3.back * 500.3f);
        //    Eventos_Dispatcher.jugadorPerdio();
        //    print(c.normal);
        //}
      
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.tag == "piso")
        {
            enPiso = false;

        }
       
    }
    private void OnTriggerEnter(Collider other)
    {
        //RaycastHit hit;
        //if (other.Raycast(other.transform.position,other.transform.forward, out hit, 10))
        //{

        //}
        //TODO: Cambiar a un script independiento o EventDispatcher
        
        if (other.CompareTag("pared"))
        {
            if(!cruzandoApertura)
             {
                EmpujarJugador(new Vector3(0.0f, 1.0f, 1.0f), 50.0f);
                Eventos_Dispatcher.eventos.JugadorPerdio();

            }
            return;
        }
        if(other.CompareTag("apertura"))
        {

           // this.GetComponent<BoxCollider>().enabled = false;
           // aperturas++;
           Eventos_Dispatcher.eventos.CruceObstaculo_Call();
           EmpujarJugador(Vector3.up, empujeEnCruze);
         
            cruzandoApertura = true;
        }
    }
    private void OnTriggerExit(Collider other) {
        if(other.transform.tag == "apertura")
            {
            cruzandoApertura = false;
            

            }
        }

        ///<sumary>
        ///Brinco para PC con dedo arrastrandose
        ///</sumary>
    public void BrincoSwipePC()
    {
        if(enPiso)
        {
            if(Input.GetMouseButtonDown(0))
            {
                mousePosInit = Input.mousePosition;

            }
            if(Input.GetMouseButtonUp(0))
            {
                mousePosFinal = Input.mousePosition;
                 distanciaMouse = mousePosInit.y - mousePosFinal.y;


                

                acumulacionFuerza = Mathf.Clamp(distanciaMouse/100,0.0f,maxFuerza);


                 rigid.velocity = Vector3.up * acumulacionFuerza;
                    //print("Fuerza:" + acumulacionFuerza);
                    acumulacionFuerza = 0;
                    this.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        ReproducirSonido_Jugador("brinco");

            }
           //Debug.Log("posInicial:"+ mousePosInit.y+" posFinal:"+ mousePosFinal.y+"="+ dist);

        }
        else if(!enPiso)
        {
             if(Input.GetMouseButtonDown(0))
            {
                mousePosInit = Input.mousePosition;

            }
            if(Input.GetMouseButtonUp(0))
            {
                mousePosFinal = Input.mousePosition;
                 distanciaMouse = mousePosFinal.y - mousePosInit.y  ;


                

                acumulacionFuerza = Mathf.Clamp(distanciaMouse/100,0.0f,maxFuerza*2);


                 rigid.velocity = Vector3.down * acumulacionFuerza;
                    //print("Fuerza:" + acumulacionFuerza);
                    acumulacionFuerza = 0;
                    this.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                    ReproducirSonido_Jugador("brincoRegreso");
            }
        }
    }
    /// <summary>
    /// Logica de Brinco para PC con tecla
    /// </summary>
    private void Brinco_PC()
    {
       
            if (enPiso)
            {

                if (Input.GetKey(teclaSalto))
                {
                    print("Saltando");
                    if(acumulacionFuerza < maxFuerza/2)
                    {
                        stepAcumulacion = stepAcumulacionMax;
                    }else if(acumulacionFuerza > maxFuerza/2)
                    {
                        stepAcumulacion = stepAcumulacionMin;

                    }
                    acumulacionFuerza += stepAcumulacion;
                    acumulacionFuerza = Mathf.Clamp(acumulacionFuerza, 0, maxFuerza);

                    //TODO: podriamos hacer que
                    /*
                     * El este de acumulacion sea mayor al principio y menor casi al final
                     * o
                     * Al reves que sea menor al principio y se acelere hacia el final
                     */
                    float sizeY = Mathf.Clamp((acumulacionFuerza * 0.1f), 0.1f, 0.8f);
                    this.transform.localScale = new Vector3(1.0f, 1.0f - sizeY, 1.0f);
                }
                else if (Input.GetKeyUp(teclaSalto))
                {
                    rigid.velocity = Vector3.up * acumulacionFuerza;
                    //print("Fuerza:" + acumulacionFuerza);
                    acumulacionFuerza = 0;
                    this.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

                }
            }
        
    }
    /// <summary>
    /// Logica de Brinco para App Movil con touchHold
    /// </summary>
    private void Brinco_Movil()
    {
        if (Input.touchCount > 0)
        {

            Touch touch = Input.GetTouch(0);
            if (enPiso)
            {

                if (touch.phase == TouchPhase.Stationary)
                {
                    print("Saltando");
                    acumulacionFuerza += stepAcumulacion;
                    acumulacionFuerza = Mathf.Clamp(acumulacionFuerza, 0, maxFuerza);

                    //TODO: podriamos hacer que
                    /*
                     * El este de acumulacion sea mayor al principio y menor casi al final
                     * o
                     * Al reves que sea menor al principio y se acelere hacia el final
                     */
                    float sizeY = Mathf.Clamp((acumulacionFuerza * 0.1f), 0.1f, 0.8f);
                    this.transform.localScale = new Vector3(1.0f, 1.0f - sizeY, 1.0f);
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    rigid.velocity = Vector3.up * acumulacionFuerza;
                    print("Fuerza:" + acumulacionFuerza);
                    acumulacionFuerza = 0;
                    this.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

                }
            }
        }
    }

    private void InicioJuego()
    {
        jugando = true;
       // sprite_anim.SetTrigger("idle");

    }

    private void PerdioJuego()
    {
        jugando = false;
        sprite_anim.SetTrigger("muerto");
        //this.rigid.isKinematic = true;
        ReproducirSonido_Jugador("caidaPerdio");
        muerto = true;
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
    private void OnDestroy()
    {
        Eventos_Dispatcher.eventos.JugadorPerdio -= PerdioJuego;
        Eventos_Dispatcher.eventos.InicioJuego -= InicioJuego;
    }
}
