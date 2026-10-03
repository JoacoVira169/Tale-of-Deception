using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VidaOrco : MonoBehaviour
{
    public float vidaOrco = 75f;
    [SerializeField] public float vidaActualOrco;
    
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
        gameObject.SendMessage(
            "GolpeOrco",
            damage,
            SendMessageOptions.DontRequireReceiver
        );
        if (vidaActualOrco == 0)
        {
            Debug.Log("Muerto!!! ->" + gameObject.name);
            //eventoMorir?.Invoke();
        }
    }
}
