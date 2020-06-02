using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamaraControl : MonoBehaviour
{
    public Transform objetoSeguir;
    public float velocidadSeguimiento;
    public Vector3 offset;
    public Vector3 posFinal;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        posFinal = objetoSeguir.position - offset;
        posFinal.y = this.transform.position.y;
        this.transform.position = Vector3.Lerp(this.transform.position, posFinal, Time.deltaTime * velocidadSeguimiento);
    }
}
