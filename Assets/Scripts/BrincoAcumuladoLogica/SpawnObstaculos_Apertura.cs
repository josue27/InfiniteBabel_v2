using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Brinco;

public class SpawnObstaculos_Apertura : MonoBehaviour
{
   
   
    public float intervaloTiempo = 3.0f;
    public GameObject obstaculos_prefab;
    public float maxY=6.0f, minY=1.0f;
    [SerializeField] private float alturaPrevia;
    // Start is called before the first frame update
    void Start()
    {
       

        Eventos_Dispatcher.inicioJuego += EmpezoJuego;
        Eventos_Dispatcher.jugadorPerdio += FinJuego;
    }
    void Spawnear()
    {
        Vector3 nuevasPos = new Vector3(this.transform.position.x, RandomPosY(), this.transform.position.z);
        GameObject ob = Instantiate(obstaculos_prefab, nuevasPos, Quaternion.identity) as GameObject;
    }


    void EmpezoJuego()
    {
        InvokeRepeating("Spawnear", intervaloTiempo, intervaloTiempo);
    }

    void FinJuego()
    {
        CancelInvoke();
    }

    float RandomPosY()
    {
        float rand = Random.Range(minY, maxY);
        while(rand == alturaPrevia)
        {
            rand = Random.Range(minY, maxY);
        }
        alturaPrevia = rand;
        return rand;
    }
    private void OnDestroy()
    {

        Eventos_Dispatcher.inicioJuego -= EmpezoJuego;
        Eventos_Dispatcher.jugadorPerdio -= FinJuego;
    }
}
