using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Vida : MonoBehaviour
{
    private Animator anim;
    public float vidaInicial;
    public float vidaActual;
    public UnityEvent eventoMorir = new UnityEvent();

    void Start()
    {
       anim = GetComponentInChildren<Animator>();
       vidaActual = vidaInicial; 
    }

    public void CausarDaño(float cuanto)
    {
        if (vidaActual <= 0 || cuanto <= 0)
        {
            return;
        }

        if (anim != null && anim.GetBool("Block"))
        {
            return;
        }

        vidaActual = Mathf.Max(0, vidaActual - cuanto);
        gameObject.SendMessage(
            "GolpeAnimacion",
            cuanto,
            SendMessageOptions.DontRequireReceiver
        );
        if (vidaActual == 0)
        {
            Debug.Log("Muerto!!! ->" + gameObject.name);
            eventoMorir?.Invoke();
        }
    }

}    
