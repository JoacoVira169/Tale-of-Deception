using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]

public class Orco : Enemy
{
    private NavMeshAgent agente;
    private Coroutine comboAtaque;
    private Coroutine rutinaDaño;
    private bool muerteProcesada;
    private bool terminarCombo;
    [SerializeField, Min(0f)] private float cooldownEntreCombos = 2f;
    private float cooldownComboRestante;
    private float temporizadorPasos;
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
        audioOrco = GetComponent<OrcoAudio>();
        if (animaciones == null)
        {
            animaciones = GetComponentInChildren<Animator>();
        }

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
        temporizadorPasos -= Time.deltaTime;
        cooldownComboRestante = Mathf.Max(0f, cooldownComboRestante - Time.deltaTime);
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
        ActualizarPasos(false);

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
            DetenerPasos();
            return;
        }

        animaciones.SetFloat("velocidad", 1f);
        ActualizarPasos(true);
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
        DetenerPasos();
        agente.SetDestination(transform.position);
        transform.LookAt(target, Vector3.up);

        if (comboAtaque == null && cooldownComboRestante <= 0f)
        {
            comboAtaque = StartCoroutine(EjecutarCombo());
        }
    }

    public override void EstadoMuerto()
    {
        base.EstadoMuerto();
        if (muerteProcesada)
        {
            return;
        }

        muerteProcesada = true;
        if (rutinaDaño != null)
        {
            StopCoroutine(rutinaDaño);
            rutinaDaño = null;
        }
        CancelarCombo();
        DetenerPasos();
        animaciones.SetBool("vivo", false);
        animaciones.SetBool("Dead", true);
        if (agente != null && agente.enabled && agente.isOnNavMesh)
        {
            agente.isStopped = true;
            agente.ResetPath();
            agente.velocity = Vector3.zero;
        }
    }

    
    private IEnumerator EjecutarCombo()
    {
        terminarCombo = false;
        bool comboCompletado = false;

        if (PuedeContinuarCombo())
        {
            yield return EjecutarAtaque("at1");
            if (PuedeContinuarCombo())
            {
                yield return EjecutarAtaque("at2");
                if (PuedeContinuarCombo())
                {
                    yield return EjecutarAtaque("at3");
                    comboCompletado = true;
                }
            }
        }

        LimpiarAtaques();
        if (comboCompletado)
        {
            cooldownComboRestante = cooldownEntreCombos;
        }

        comboAtaque = null;
        terminarCombo = false;
    }

    private IEnumerator EjecutarAtaque(string nombreAtaque)
    {
        animaciones.SetBool(nombreAtaque, true);
        if (audioOrco != null)
        {
            audioOrco.ReproducirGolpeHacha();
        }

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
        return vivo && !terminarCombo && estado == Estados.atacar && distancia <= distanciaAtacar;
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
    
    public override void RecibirDaño(float damage)
    {
        if (animaciones == null || estado == Estados.muerto || animaciones.GetBool("orcdamage"))
        {
            return;
        }

        if (audioOrco != null)
        {
            audioOrco.ReproducirDañoOrco();
        }

        ataqueInterrumpido = null;
        CancelarCombo();
        animaciones.SetBool("orcdamage", true);
        animaciones.CrossFade("Base Layer.Odamage", 0.05f, 0);
        rutinaDaño = StartCoroutine(EsperarFinDaño());
    }

    public void GolpeOrco(float damage)
    {
        RecibirDaño(damage);
    }

    private IEnumerator EsperarFinDaño()
    {
        float tiempoRestante = 3f;
        bool dañoIniciado = false;

        while (tiempoRestante > 0f)
        {
            AnimatorStateInfo estadoActual = animaciones.GetCurrentAnimatorStateInfo(0);
            bool enAnimacionDaño = estadoActual.IsName("Base Layer.Odamage") || estadoActual.IsName("Odamage");

            if (enAnimacionDaño)
            {
                dañoIniciado = true;
                if (estadoActual.normalizedTime >= 1f)
                {
                    break;
                }
            }
            else if (dañoIniciado)
            {
                break;
            }

            tiempoRestante -= Time.deltaTime;
            yield return null;
        }

        rutinaDaño = null;
        TerminarAnimacionDaño();
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
        if (ataqueInterrumpido == null && estado != Estados.muerto)
        {
            animaciones.CrossFade("Base Layer.Idle", 0.1f, 0);
        }
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

    private void ActualizarPasos(bool corriendo)
    {
        if (audioOrco == null)
        {
            return;
        }

        if (!agente.enabled || agente.velocity.sqrMagnitude <= 0.04f)
        {
            temporizadorPasos = 0f;
            audioOrco.DetenerPasos();
            return;
        }

        if (temporizadorPasos > 0f)
        {
            return;
        }

        if (corriendo)
        {
            audioOrco.ReproducirPasoCorriendoOrco();
            temporizadorPasos = 0.3f;
        }
        else
        {
            audioOrco.ReproducirPasoCaminando();
            temporizadorPasos = 0.5f;
        }
    }

    private void DetenerPasos()
    {
        temporizadorPasos = 0f;
        if (audioOrco != null)
        {
            audioOrco.DetenerPasos();
        }
    }

    public void ReproducirImpactoPlayer()
    {
        if (audioOrco != null)
        {
            audioOrco.ReproducirImpactoPlayer();
        }
    }
}
