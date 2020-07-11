using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementoEscenario_Control : MonoBehaviour
{

    public void Mover(Vector3 aPosicion,float duracion){
        LeanTween.move(this.gameObject,aPosicion,duracion).setOnComplete(()=>
        {
                Debug.Log(this.gameObject.name+" termino recorrido" );
                Reiniciar();

         });
    }
    public void Reiniciar()
    {
        this.gameObject.SetActive(false);
    }
}
