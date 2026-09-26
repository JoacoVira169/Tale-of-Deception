using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Vida : MonoBehaviour
{
    public float vidaInicial;
    public float vidaActual;
    public UnityEvent eventoMorir = new UnityEvent();

    void Start()
    {
       vidaActual = vidaInicial; 
    }

    public void CausarDaño(float cuanto)
    {
        if (vidaActual <= 0 || cuanto <= 0)
        {
            return;
        }

        vidaActual = Mathf.Max(0, vidaActual - cuanto);
        if (vidaActual == 0)
        {
            Debug.Log("Muerto!!! ->" + gameObject.name);
            eventoMorir?.Invoke();
        }
    }

}    
