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
    public Transform[] CheckPoints;
    private int indice;
    private int direccionPatrulla = 1;
    private int ultimoCheckpointAvanzado = -1;
    public float distanciaCheckpoints;
    private float distanciaCheckpoints2;
    

    public override void Awake()
    {
        base.Awake();
        agente = GetComponent<NavMeshAgent>();

        agente.autoTraverseOffMeshLink = true;
        agente.autoRepath = true;
        agente.updateRotation = true;
        agente.updateUpAxis = true;

        distanciaCheckpoints2 = Mathf.Max(distanciaCheckpoints * distanciaCheckpoints, 0.25f);
        if (CheckPoints != null && CheckPoints.Length > 0)
        {
            indice = 0;
            agente.SetDestination(CheckPoints[indice].position);
        }
    }

    private void Update()
    {
        animaciones.SetFloat("distancia", distancia);
    }

    public override void EstadoIdle()
    {
        if (estado != Estados.idle)
        {
            return;
        }

        if (CheckPoints == null || CheckPoints.Length == 0)
        {
            return;
        }

        if (target != null && distancia < distanciaSeguir)
        {
            CambiarEstado(Estados.seguir);
            return;
        }

        animaciones.SetFloat("velocidad", 1f);

        NavMeshPathStatus estadoRuta = agente.pathStatus;
        

        Vector3 destinoActual = CheckPoints[indice].position;
        float distanciaAlObjetivo = Vector3.Distance(transform.position, destinoActual);

        bool llegoAlCheckpoint = !agente.pathPending && agente.remainingDistance <= Mathf.Max(distanciaCheckpoints, 0.5f)
            && agente.velocity.sqrMagnitude <= 0.01f;

        if (llegoAlCheckpoint && ultimoCheckpointAvanzado != indice)
        {
            ultimoCheckpointAvanzado = indice;
            indice = (indice + 1) % CheckPoints.Length;
            agente.SetDestination(CheckPoints[indice].position);
            return;
        }

        if (distanciaAlObjetivo <= Mathf.Max(distanciaCheckpoints, 0.5f) && ultimoCheckpointAvanzado != indice)
        {
            ultimoCheckpointAvanzado = indice;
            indice = (indice + 1) % CheckPoints.Length;
        }

        agente.SetDestination(CheckPoints[indice].position);
    }

    
    public override void EstadoSeguir()
    {
        if (estado != Estados.seguir)
        {
            return;
        }

        if (target == null)
        {
            CambiarEstado(Estados.idle);
            return;
        }

        if (distancia <= distanciaAtacar)
        {
            CambiarEstado(Estados.atacar);
            return;
        }

        if (distancia > distanciaEscapar)
        {
            CambiarEstado(Estados.idle);
            return;
        }

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
        if (estado != Estados.atacar)
        {
            return;
        }

        if (target == null)
        {
            CambiarEstado(Estados.idle);
            return;
        }

        if (distancia > distanciaAtacar + 0.4f)
        {
            CambiarEstado(Estados.seguir);
            SolicitarFinCombo();
            return;
        }

        animaciones.SetFloat("velocidad", 0f);
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
