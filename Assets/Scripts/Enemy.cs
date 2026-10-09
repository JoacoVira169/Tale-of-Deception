using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;



#if UNITY_EDITOR
using UnityEditor;
#endif

public class Enemy : MonoBehaviour
{
    public Estados estado;
    public float distanciaSeguir;
    public float distanciaAtacar;
    public float distanciaEscapar;

    public bool autoseleccionarTarget = true;
    public Transform target;
    public float distancia = Mathf.Infinity;
    public bool vivo = true;
    public VidaOrco vidaOrco; 
    private Vida vidaObjetivo;
    private Animator animacion;
    public OrcoAudio audioOrco;

    public virtual void Awake()
    {
        estado = Estados.idle;
        distancia = Mathf.Infinity;
        animacion = GetComponentInChildren<Animator>();
        audioOrco = GetComponent<OrcoAudio>();
        if (vidaOrco == null)
        {
            vidaOrco = GetComponentInChildren<VidaOrco>();
        }
        if (vidaOrco != null)
        {
            vidaOrco.eventoMorir.AddListener(Morir);
        }
        StartCoroutine(CalcularDistancia());
    }

    private void Start()
    {
        if (autoseleccionarTarget && Player.singelton != null)
        {
            target = Player.singelton.transform;
        }

        if (target != null)
        {
            vidaObjetivo = target.GetComponentInParent<Vida>();
            if (vidaObjetivo != null)
            {
                vidaObjetivo.eventoMorir.AddListener(AlMorirObjetivo);
            }
        }

        if (target == null)
        {
            Debug.LogWarning("Enemy has no target assigned.", this);
        }
    }

    private void OnDestroy()
    {
        if (vidaOrco != null)
        {
            vidaOrco.eventoMorir.RemoveListener(Morir);
        }
        if (vidaObjetivo != null)
        {
            vidaObjetivo.eventoMorir.RemoveListener(AlMorirObjetivo);
        }
    }

    public virtual void Morir()
    {
        if (estado == Estados.muerto)
        {
            return;
        }

        CambiarEstado(Estados.muerto);
    }

    protected virtual void AlMorirObjetivo()
    {
        target = null;
        distancia = Mathf.Infinity;
        if (estado != Estados.muerto)
        {
            CambiarEstado(Estados.idle);
            animacion.SetBool("Idle", false);
            animacion.SetTrigger("Win");
            if (audioOrco != null)
            {
                audioOrco.IniciarGruñido();
            }
        }
    }

    private void LateUpdate()
    {
        CheckEstado();
    }

    private void CheckEstado()
    {
        if (estado == Estados.muerto)
        {
            EstadoMuerto();
            return;
        }

        if (target == null && estado != Estados.muerto)
        {
            return;
        }

        if (float.IsInfinity(distancia))
        {
            return;
        }

        switch (estado)
        {
            case Estados.idle:
                EstadoIdle();
                break;
            case Estados.seguir:
                transform.LookAt(target, Vector3.up);
                EstadoSeguir();
                break;
            case Estados.atacar:
                EstadoAtacar();
                break;
            case Estados.muerto:
                EstadoMuerto();
                break;
            default:
                break;
        }
    }

    public void CambiarEstado(Estados e)
    {
        switch(e)
        {
            case Estados.idle:
                break;
            case Estados.seguir:
                break;
            case Estados.atacar:
                break;
            case Estados.muerto:
                vivo = false;
                break;
            default:
                break;
        }
        estado = e;
    }

    public virtual void EstadoIdle()
    {
    }
    public virtual void EstadoSeguir()
    {
    }
    public virtual void EstadoAtacar()
    {
    }
    public virtual void RecibirDaño(float damage)
    {
    }
    public virtual void EstadoMuerto()
    {

    }

    IEnumerator CalcularDistancia()
    {
        while (vivo)
        {
            if (target != null)
            {
                distancia = Vector3.Distance(transform.position, target.position);
            }
            else
            {
                distancia = Mathf.Infinity;
            }

            yield return new WaitForSeconds(0.3f);
        }
    }
    

#if UNITY_EDITOR

    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.red;
        Handles.DrawWireDisc(transform.position, Vector3.up, distanciaAtacar);
        Handles.color = Color.yellow;
        Handles.DrawWireDisc(transform.position, Vector3.up, distanciaSeguir);
        Handles.color = Color.green;
        Handles.DrawWireDisc(transform.position, Vector3.up, distanciaEscapar);
    }

    public void OnDrawGizmos()
    {
        int icono = (int)estado;
        icono++;
        Gizmos.DrawIcon(transform.position + Vector3.up*8.2f, "0" + icono + ".png");
    }
#endif
}

public enum Estados
{
    idle = 0,
    seguir = 1,
    atacar = 2,
    muerto = 3
}