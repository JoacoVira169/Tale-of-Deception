using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]

public class Orco : Enemy
{
    private NavMeshAgent agente;
    private Coroutine comboAtaque;
    private bool terminarCombo;
    public Animator animaciones;
    public float daño = 3;

    void Awake()
    {
        base.Awake();
        agente = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        animaciones.SetFloat("distancia", distancia);
    }

    public override void EstadoIdle()
    {
        base.EstadoIdle();

        if (estado != Estados.idle)
        {
            return;
        }

        animaciones.SetFloat("velocidad", 0f);
        agente.isStopped = true;
        agente.SetDestination(transform.position);
    }

    public override void EstadoSeguir()
    {
        base.EstadoSeguir();

        if (estado != Estados.seguir)
        {
            return;
        }

        agente.isStopped = false;
        agente.SetDestination(target.position);

        if (comboAtaque != null)
        {
            animaciones.SetFloat("velocidad", 0f);
            return;
        }

        animaciones.SetFloat("velocidad", 1f);
    }

    public override void EstadoAtacar()
    {
        base.EstadoAtacar();

        if (distancia > distanciaAtacar)
        {
            CambiarEstado(Estados.seguir);
            SolicitarFinCombo();
            return;
        }

        animaciones.SetFloat("velocidad", 0f);
        agente.isStopped = true;
        agente.SetDestination(transform.position);
        transform.LookAt(target, Vector3.up);

        if (comboAtaque == null)
        {
            comboAtaque = StartCoroutine(EjecutarCombo());
        }
    }

    public override void EstadoMuerto()
    {
        base.EstadoMuerto();
        CancelarCombo();
        animaciones.SetBool("vivo", false);
        agente.enabled = false;
    }

    private IEnumerator EjecutarCombo()
    {
        terminarCombo = false;

        while (vivo && !terminarCombo && estado == Estados.atacar && distancia <= distanciaAtacar)
        {
            yield return EjecutarAtaque("at1");
            if (!PuedeContinuarCombo()) break;

            yield return EjecutarAtaque("at2");
            if (!PuedeContinuarCombo()) break;

            yield return EjecutarAtaque("at3");
        }

        LimpiarAtaques();
        comboAtaque = null;
        terminarCombo = false;
    }

    private IEnumerator EjecutarAtaque(string nombreAtaque)
    {
        animaciones.SetBool(nombreAtaque, true);

        bool dañoAplicado = false;
        float tiempoLimite = 2f;

        while (tiempoLimite > 0f)
        {
            AnimatorStateInfo estadoAnimacion = animaciones.GetCurrentAnimatorStateInfo(0);

            if (estadoAnimacion.IsName(nombreAtaque))
            {
                float tiempo = estadoAnimacion.normalizedTime;

                if (!dañoAplicado && tiempo >= 0.35f && tiempo <= 0.65f)
                {
                    if (Player.singelton != null && Player.singelton.vida != null)
                    {
                        Player.singelton.vida.CausarDaño(daño);
                    }
                    dañoAplicado = true;
                }

                if (tiempo >= 0.7f)
                {
                    break;
                }
            }

            tiempoLimite -= Time.deltaTime;
            yield return null;
        }

        animaciones.SetBool(nombreAtaque, false);
    }

    private void SolicitarFinCombo()
    {
        terminarCombo = true;
    }

    private bool PuedeContinuarCombo()
    {
        return vivo && estado == Estados.atacar && distancia <= distanciaAtacar;
    }

    private void CancelarCombo()
    {
        terminarCombo = true;

        if (comboAtaque != null)
        {
            StopCoroutine(comboAtaque);
            comboAtaque = null;
        }

        LimpiarAtaques();
    }

    private void LimpiarAtaques()
    {
        animaciones.SetBool("at1", false);
        animaciones.SetBool("at2", false);
        animaciones.SetBool("at3", false);
    }

   
}
