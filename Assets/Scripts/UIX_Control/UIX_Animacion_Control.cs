using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIX_Animacion_Control : MonoBehaviour
{
    // Start is called before the first frame update
    public Animator animator;
    public string nombreTrigger;

    void Start()
    {
        
    }
    public void BotonPresionado()
    {
        if (nombreTrigger != null)
            animator.SetBool(nombreTrigger,true);
    }
    public void BotonSoltado()
    {
        if (nombreTrigger != null)
            animator.SetBool(nombreTrigger, false);
    }

    private void OnMouseDown()
    {
        
    }





}
