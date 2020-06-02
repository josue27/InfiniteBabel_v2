using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PisoSpawn : MonoBehaviour
{
    public static PisoSpawn _pisoSpawn;
    public GameObject piso_prefab;
    public Vector3 posInicial;
    // Start is called before the first frame update
    void Start()
    {
        _pisoSpawn = this;
        posInicial = FindObjectOfType<PisoControl>().transform.position;
    }

    public void SpawnearPiso()
    {
        Debug.Log("Spawneando piso");
        Vector3 sigPos = posInicial;
        sigPos.z +=  10.0f;
        GameObject sigPiso = Instantiate(piso_prefab, sigPos, Quaternion.identity);//as GameObject;
        sigPiso.SetActive(true);
        posInicial = sigPiso.transform.position;
        //Destroy(this.gameObject);
    }

}
