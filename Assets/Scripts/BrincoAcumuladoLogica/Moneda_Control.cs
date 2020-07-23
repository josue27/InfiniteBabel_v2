using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moneda_Control : MonoBehaviour
{

    public Animator animMoneda;
   /// <summary>
   /// OnTriggerEnter is called when the Collider other enters the trigger.
   /// </summary>
   /// <param name="other">The other Collider involved in this collision.</param>
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
