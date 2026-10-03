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
    public float daño = 20;
    private string ataqueInterrumpido;
    private static readonly int At1Hash = Animator.StringToHash("At1");
    private static readonly int At2Hash = Animator.StringToHash("At2");
    private static readonly int At3Hash = Animator.StringToHash("At3");
    

    public override void Awake()
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

        float tiempoLimite = 2f;

        while (tiempoLimite > 0f)
        {
            AnimatorStateInfo estadoAnimacion = animaciones.GetCurrentAnimatorStateInfo(0);
            string nombreEstado = char.ToUpperInvariant(nombreAtaque[0]) + nombreAtaque.Substring(1);

            if (estadoAnimacion.IsName("Base Layer." + nombreEstado))
            {
                float tiempo = estadoAnimacion.normalizedTime;

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
    
    public void GolpeOrco(float damage)
    {
        if (animaciones == null || animaciones.GetBool("orcdamage"))
        {
            return;
        }

        ataqueInterrumpido = ObtenerAtaqueActual();
        animaciones.SetBool("at1", false);
        animaciones.SetBool("at2", false);
        animaciones.SetBool("at3", false);
        animaciones.SetBool("orcdamage", true);
    }

    public void TerminarAnimacionDaño()
    {
        if (animaciones == null)
        {
            return;
        }

        animaciones.SetBool("at1", ataqueInterrumpido == "at1");
        animaciones.SetBool("at2", ataqueInterrumpido == "at2");
        animaciones.SetBool("at3", ataqueInterrumpido == "at3");
        animaciones.SetBool("orcdamage", false);
        ataqueInterrumpido = null;
    }

    private string ObtenerAtaqueActual()
    {
        if (animaciones.IsInTransition(0))
        {
            string ataqueSiguiente = ObtenerAtaque(animaciones.GetNextAnimatorStateInfo(0));
            if (ataqueSiguiente != null)
            {
                return ataqueSiguiente;
            }
        }

        return ObtenerAtaque(animaciones.GetCurrentAnimatorStateInfo(0));
    }

    private string ObtenerAtaque(AnimatorStateInfo estadoAnimacion)
    {
        if (estadoAnimacion.shortNameHash == At1Hash) return "at1";
        if (estadoAnimacion.shortNameHash == At2Hash) return "at2";
        if (estadoAnimacion.shortNameHash == At3Hash) return "at3";
        return null;
    }
}
