using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moneda_Control : MonoBehaviour
{

    public Animator animMoneda;
   
   void OnTriggerEnter(Collider other)
   {
       if(other.CompareTag("Player"))
       {
          
           StartCoroutine(SecuenciaTomar());
           Eventos_Dispatcher.eventos.MonedaTomada_Call();
           
       }
   }
   IEnumerator SecuenciaTomar()
   {
       animMoneda.SetTrigger("tomar");
       yield return new WaitForSeconds(0.2f);
       this.gameObject.SetActive(false);
   }
}
