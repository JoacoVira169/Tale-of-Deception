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
    public float distancia;
    public bool vivo = true;

    public virtual void Awake()
    {
        StartCoroutine(CalcularDistancia());
    }

    private void Start()
    {
        if (autoseleccionarTarget && Player.singelton != null)
        {
            target = Player.singelton.transform;
        }

        if (target == null)
        {
            Debug.LogWarning("Enemy has no target assigned.", this);
        }
    }

    private void LateUpdate()
    {
        CheckEstado();
    }

    private void CheckEstado()
    {
        if (target == null && estado != Estados.muerto)
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
        if (distancia < distanciaSeguir)
        {
            CambiarEstado(Estados.seguir);
        }
    }
    public virtual void EstadoSeguir()
    {
        if (distancia < distanciaAtacar)
        {
            CambiarEstado(Estados.atacar);
        }
        else if (distancia > distanciaEscapar)
        {
            CambiarEstado(Estados.idle);
        }
    }
    public virtual void EstadoAtacar()
    {
        if (distancia > distanciaAtacar + 0.4f)
        {
            CambiarEstado(Estados.seguir);
        }
    }
    public virtual void EstadoMuerto()
    {

    }

    IEnumerator CalcularDistancia()
    {
        while (vivo)
        {
            if(target != null)
            {
                distancia = Vector3.Distance(transform.position, target.position);
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