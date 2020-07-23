using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEscenario : MonoBehaviour
{
    //TODO: Crear scriptble object ElementoEscenario
    public List<Elemento> elementos = new List<Elemento>();

    public bool spawnear;
    public Dictionary<string,Queue<GameObject>> elementosDictionary = new Dictionary<string, Queue<GameObject>>();
    /// <summary>
    /// Called when the script is loaded or a value is changed in the
    /// inspector (Called in the editor only).
    /// </summary>
    void OnValidate()
    {
        foreach(Elemento elementoEnLista in elementos)
        {
            elementoEnLista.Set();
        }
    }
    public void Start()
    {
        SpawnElementos();
        Eventos_Dispatcher.eventos.InicioJuego +=   InicioJuego; 
    }
 
    void Update()
    {
        if(spawnear)
        {
            for (int i = 0; i < elementos.Count; i++)
            {   
                if(Time.time>= elementos[i].sigSpawn)
                {
                  elementos[i].sigSpawn = Time.time+elementos[i].rateSpawn;
                  ActivarObjeto(elementos[i].nombre); 
                }
            }
        }
    }
    public void InicioJuego()
    {
        //InvokeRepeating("ActivarObjeto",1.0f,3.0f);
        // for (int i = 0; i < elementos.Count; i++)
        // {
        //     elementos[i].sigSpawn = Time.time+elementos[i].rateSpawn;
        // }
        ActivarObjetoInicial("paredIntermedia");
        spawnear = true;
    }
    public void SpawnElementos()
    {
        // foreach(ElementoEscenario elemento in elementosEscenario)
        // {
        //     Queue<GameObject> objetos = new Queue<GameObject>();

        //     for(int i = 0;i<elemento.cantidadSpawn;i++)
        //     {
        //         GameObject objeto = Instantiate(elemento.prefab,elemento.posInicial.position,elemento.posInicial.rotation);
        //         objeto.gameObject.SetActive(false);
        //         objeto.transform.name = elemento.nombre+"_"+i.ToString("00");
        //         objetos.Enqueue(objeto);

        //     }
        //     elementosDictionary.Add(elemento.nombre,objetos);
        // }

        foreach(Elemento elementoEnLista in elementos)
        {
            elementoEnLista.nombre = elementoEnLista.elemento.nombre;
            elementoEnLista.objetos = new Queue<GameObject>();
            for(int i = 0; i<elementoEnLista.elemento.cantidadSpawn; i++)
            {
                GameObject objeto = Instantiate(elementoEnLista.elemento.prefab,elementoEnLista.posInicial.position,elementoEnLista.posInicial.rotation);
                objeto.gameObject.SetActive(false);
                objeto.gameObject.name = elementoEnLista.nombre+"_"+i.ToString("00");
                elementoEnLista.objetos.Enqueue(objeto);

            }
        }
    }

    public void ActivarObjeto()
    {
        ActivarObjeto("vigaVertical");
    }

    public void ActivarObjeto(string nombreObjeto)
    {
        GameObject objetoActivar;
        foreach(Elemento elemenoEnLista in elementos){

            if(elemenoEnLista.nombre.Equals(nombreObjeto))
            {
                objetoActivar = elemenoEnLista.objetos.Dequeue();
                objetoActivar.transform.position = elemenoEnLista.posInicial.position;
                objetoActivar.gameObject.SetActive(true);
                objetoActivar.GetComponent<ElementoEscenario_Control>().Mover(elemenoEnLista.posFinal.position,elemenoEnLista.duracionRecorrido);
                elemenoEnLista.objetos.Enqueue(objetoActivar);
                break;
            }
        }
        
    }

    public void ActivarObjetoInicial(string nombreObjeto)
    {
        GameObject objetoActivar;
        foreach(Elemento elemenoEnLista in elementos){

            if(elemenoEnLista.nombre.Equals(nombreObjeto))
            {
                if(!elemenoEnLista.objetoInical)
                    return;
                objetoActivar = elemenoEnLista.objetoInical;
                //objetoActivar.transform.position = elemenoEnLista.posInicial.position;
                objetoActivar.gameObject.SetActive(true);
                
                float distanciaPos =   elemenoEnLista.posFinal.position.z-elemenoEnLista.posInicial.position.z;
                Debug.Log("Distancia:"+distanciaPos);
                Vector3 posFinalInicial = objetoActivar.transform.position;
                posFinalInicial.z += distanciaPos;
                objetoActivar.GetComponent<ElementoEscenario_Control>().Mover(posFinalInicial,elemenoEnLista.duracionRecorrido);
                //elemenoEnLista.objetos.Enqueue(objetoActivar);
                Debug.Log("objeto inicial activado");

                
            }
        }
        
    }


}


[System.Serializable]
public class Elemento
{
    
    public ElementoEscenario elemento;
    public string nombre;
    public int cantidadSpawn;
    public float duracionRecorrido;
    public Transform posInicial;
    public Transform posFinal;
    
    public GameObject objetoInical;
    public float rateSpawn = 3.0f;
    [SerializeField]
    public float sigSpawn;
    public Queue<GameObject> objetos;


    public void Set()
    {
        if(!elemento)
        return;

        nombre = elemento.nombre;
        cantidadSpawn = elemento.cantidadSpawn;
        duracionRecorrido = elemento.duracionRecorrido;
    }

}
