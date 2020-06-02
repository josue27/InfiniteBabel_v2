using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BrincoAcumulado : MonoBehaviour
{

    int aperturas = 0;

    

    public bool enPiso;
    public float fuerzaSalto = 1.0f;
    
    [Header("KeyBindigns")]
    public KeyCode teclaSalto;
    public Rigidbody rigid;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    public float acumulacionFuerza = 0.0f;
    public float maxFuerza = 16.5f;
    [SerializeField] private float stepAcumulacion = 0.1f;

    public bool presionada;
    public bool soltada;

    [Header("UIX")]
    public Slider slider_fuerzaBrinco;
    public Text scoreActual;

    bool jugando;

    void Start()
    {
        rigid = this.GetComponent<Rigidbody>();

        Eventos_Dispatcher.jugadorPerdio += PerdioJuego;
        Eventos_Dispatcher.inicioJuego += InicioJuego;
    }

    // Update is called once per frame
    void Update()
    {
        if (!jugando)
            return;

#if UNITY_ANDROID
        Brinco_Movil();
#endif
#if UNITY_EDITOR
        Brinco_PC();
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
        scoreActual.text = aperturas.ToString();
    }

    private void OnCollisionEnter(Collision collision)
    {
      
        if (collision.transform.tag == "piso")
        {
            enPiso = true;
        }
        print(collision.transform.name);

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
        if(other.transform.tag == "apertura")
        {
            aperturas++;
        }
        if (other.transform.tag == "pared")
        {
            EmpujarJugador(new Vector3(0.0f, 1.0f, -1.0f), 500.0f);
        }
    }

    /// <summary>
    /// Logica de Brinco para PC
    /// </summary>
    private void Brinco_PC()
    {
       
            if (enPiso)
            {

                if (Input.GetKey(teclaSalto))
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
    /// Logica de Brinco para App Movil
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
    }

    private void PerdioJuego()
    {
        jugando = false;
    }

    private void EmpujarJugador(Vector3 direccion,float fuerza)
    {
        print("Choco con pared");
        this.rigid.AddForce(direccion * fuerza);
        Eventos_Dispatcher.jugadorPerdio();
       
    }

    private void OnDestroy()
    {
        Eventos_Dispatcher.jugadorPerdio -= PerdioJuego;
        Eventos_Dispatcher.inicioJuego -= InicioJuego;
    }
}
