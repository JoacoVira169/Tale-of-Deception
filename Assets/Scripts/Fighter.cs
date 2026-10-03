using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : MonoBehaviour
{
    private Animator anim;
    private Player player;
    private int noOfClicks;
    private int processedStateHash;
    private bool comboWindowProcessed;
    private bool impactoProcesado;
    private readonly HashSet<int> enemigosGolpeados = new HashSet<int>();
    private const float comboTransitionTime = 0.7f;
    [SerializeField, Range(0f, 1f)] private float momentoImpacto = 0.35f;
    [SerializeField, Range(0f, 180f)] private float anguloAtaque = 100f;
    [SerializeField] private float alcanceFallback = 1.5f;
    
    void Start()
    {
        anim = GetComponent<Animator>();
        if (anim == null)
        {
            anim = GetComponentInChildren<Animator>();
        }

        player = GetComponentInParent<Player>();
        if (player == null)
        {
            player = GetComponent<Player>();
        }

        if (anim == null || player == null)
        {
            Debug.LogError("Fighter requiere un Animator y un Player en su jerarquía.", this);
            enabled = false;
            return;
        }

    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnClick();
        }

        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
        if (state.fullPathHash != processedStateHash)
        {
            processedStateHash = state.fullPathHash;
            comboWindowProcessed = false;
            impactoProcesado = false;
            enemigosGolpeados.Clear();
        }

        bool isAttackState = state.IsName("hit1") || state.IsName("hit2") || state.IsName("hit3");
        if (isAttackState && !impactoProcesado && state.normalizedTime >= momentoImpacto)
        {
            impactoProcesado = true;
            AplicarDañoPorDistanciaYOrientacion();
        }

        if (!isAttackState || comboWindowProcessed || state.normalizedTime < comboTransitionTime)
        {
            return;
        }

        comboWindowProcessed = true;
        if (state.IsName("hit1"))
        {
            if (noOfClicks >= 2)
            {
                anim.SetBool("hit1", false);
                anim.SetBool("hit2", true);
            }
            else
            {
                anim.SetBool("hit1", false);
                noOfClicks = 0;
            }
        }
        else if (state.IsName("hit2"))
        {
            if (noOfClicks >= 3)
            {
                anim.SetBool("hit2", false);
                anim.SetBool("hit3", true);
            }
            else
            {
                anim.SetBool("hit2", false);
                noOfClicks = 0;
            }
        }
        else if (state.IsName("hit3"))
        {
            anim.SetBool("hit3", false);
            noOfClicks = 0;
        }
    }

    private void AplicarDañoPorDistanciaYOrientacion()
    {
        Enemy[] enemigos = FindObjectsOfType<Enemy>();
        Vector3 forward = player.transform.forward;
        forward.y = 0f;
        forward.Normalize();
        float dotMinimo = Mathf.Cos(anguloAtaque * 0.5f * Mathf.Deg2Rad);

        foreach (Enemy enemigo in enemigos)
        {
            if (enemigo == null || !enemigo.vivo || enemigo.estado == Estados.muerto)
            {
                continue;
            }

            Vector3 haciaEnemigo = enemigo.transform.position - player.transform.position;
            haciaEnemigo.y = 0f;
            float distancia = haciaEnemigo.magnitude;
            float alcance = enemigo.distanciaAtacar > 0f ? enemigo.distanciaAtacar : alcanceFallback;
            if (distancia > alcance || distancia <= 0.001f)
            {
                continue;
            }

            if (Vector3.Dot(forward, haciaEnemigo / distancia) < dotMinimo)
            {
                continue;
            }

            VidaOrco vidaOrco = enemigo.vidaOrco;
            if (vidaOrco == null)
            {
                vidaOrco = enemigo.GetComponentInChildren<VidaOrco>();
            }

            if (vidaOrco == null || vidaOrco.vidaActualOrco <= 0f)
            {
                continue;
            }

            if (enemigosGolpeados.Add(vidaOrco.GetInstanceID()))
            {
                vidaOrco.CausarDañoO(player.damage);
            }
        }
    }

    void OnClick()
    {
        if (noOfClicks == 0)
        {
            noOfClicks = 1;
            anim.SetBool("hit1", true);
            return;
        }

        if (noOfClicks < 3)
        {
            noOfClicks++;
        }
    }
}
