using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JugadorControl : MonoBehaviour
{
    public static JugadorControl _jugador;
    public float velocidad =0.1f;
    public bool enPiso;
    public float fuerzaSalto = 1.0f;
    public float distanciaDash = 5.0f;
    public float velocidadDash = 5.0f;
    public Rigidbody rigid;
    [Header("KeyBindigns")]
    public KeyCode teclaSalto;
    public KeyCode teclaDash;

    public float velocidadTotal;
    public float rateDesaceleracion = 0.1f;
    // Start is called before the first frame update
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;



    public bool _puedeMover;
    void Start()
    {
        _jugador = this;
        rigid = this.GetComponent<Rigidbody>();
        velocidadTotal = velocidad;
    }

    // Update is called once per frame
    void Update()
    {
        if(_puedeMover)
            Mover();


        //Brinco 
        //Evitando Double jump
        if (enPiso)
        {
            if (Input.GetKeyDown(teclaSalto))
            {
                print("Saltando");
                // rigid.AddForce(Vector3.up * fuerzaSalto);
                rigid.velocity = Vector3.up * fuerzaSalto;

            }
        }

        //Esta condicion solo se cumple cuando esta en su punto mas alto o en posicion inicial
        //donde  en ambas partes la velocidad inicial debe ser 0 
        if (rigid.velocity.y < 0)
        {
            rigid.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;

        }
        //esto solo se cumple cuando la velocidad es mayor a 0 y no llego a su punto mas alto
        else if(rigid.velocity.y > 0 && !Input.GetKey(teclaSalto))
        {
            rigid.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
       


    }

    public void Mover()
    {
        transform.Translate(Vector3.forward * (Time.deltaTime * velocidadTotal));
        if (velocidadTotal > velocidad)
        {
            velocidadTotal -= rateDesaceleracion;
        }

        if (Input.GetKeyDown(teclaDash))
        {
            print("Dash");
            // transform.position =  Vector3.MoveTowards(this.transform.position,new Vector3(this.transform.position.x, this.transform.position.y, distanciaDash),Time.deltaTime + velocidad);
            velocidadTotal = velocidadDash;

        }

        if (rigid.velocity.y > 0.1f)
        {
            enPiso = false;
        }
        else if (rigid.velocity.y < 0.0f)
        {
            enPiso = true;
        }
        print(rigid.velocity.y);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "piso")
        {
            enPiso = true;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.tag == "piso")
        {
            enPiso = false;
        }
    }
}
