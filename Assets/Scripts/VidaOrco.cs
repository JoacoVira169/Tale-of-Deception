using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VidaOrco : MonoBehaviour
{
    public float vidaOrco = 75f;
    [SerializeField] public float vidaActualOrco;
    public UnityEvent eventoMorir = new UnityEvent();
    
    void Start()
    {
        vidaActualOrco = vidaOrco;
    }
    public void CausarDañoO(float damage)
    {
        if (vidaActualOrco <= 0 || damage <= 0)
        {
            return;
        }

        vidaActualOrco = Mathf.Max(0, vidaActualOrco - damage);
        Enemy enemigo = GetComponentInParent<Enemy>();
        if (enemigo != null)
        {
            enemigo.RecibirDaño(damage);
        }

        if (vidaActualOrco == 0)
        {
            Debug.Log("Muerto!!! ->" + gameObject.name);
            eventoMorir.Invoke();
        }
    }
}
