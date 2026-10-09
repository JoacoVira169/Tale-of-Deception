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
       eventoMorir.AddListener(Muerte);
       eventoMorir.AddListener(DetenerMovimiento);
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
            eventoMorir?.Invoke();
        }
    }

    public void DetenerMovimiento()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        Rigidbody2D rb2D = GetComponent<Rigidbody2D>();
        if (rb2D != null)
        {
            rb2D.velocity = Vector2.zero;
            rb2D.angularVelocity = 0f;
        }
    }

    public void Muerte()
    {
        if (anim == null) return;

        anim.SetBool("Walk", false);
        anim.SetBool("Run", false);
        anim.SetBool("Jump", false);
        anim.SetBool("Crouch", false);
        anim.SetBool("Block", false);
        anim.SetBool("Damage", false);
        anim.SetBool("hit1", false);
        anim.SetBool("hit2", false);
        anim.SetBool("hit3", false);

        anim.ResetTrigger("Dash");
        anim.SetBool("Die", true);

        Player controlador = GetComponent<Player>();
        if (controlador != null)
        {
            controlador.enabled = false;
        }
    }

}
