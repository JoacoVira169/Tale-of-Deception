using System.Collections.Generic;
using UnityEngine;

public class AxeHitbox : MonoBehaviour
{
    private Collider hitbox;
    private Orco orco;
    private readonly HashSet<int> jugadoresGolpeados = new HashSet<int>();
    private string ataqueActivo;

    private void Awake()
    {
        hitbox = GetComponent<Collider>();
        orco = GetComponentInParent<Orco>();

        if (hitbox == null || !hitbox.isTrigger)
        {
            Debug.LogError("AxeHitbox requiere un collider configurado como trigger.", this);
            enabled = false;
            return;
        }

        hitbox.enabled = false;
    }

    private void Update()
    {
        ActualizarAtaqueActivo();
    }

    public void AbrirVentanaDeDaño()
    {
        ActualizarAtaqueActivo();
    }

    public void CerrarVentanaDeDaño()
    {
        ActualizarAtaqueActivo();
    }

    private void OnTriggerEnter(Collider other)
    {
        IntentarCausarDaño(other);
    }

    private void OnTriggerStay(Collider other)
    {
        IntentarCausarDaño(other);
    }

    private void IntentarCausarDaño(Collider other)
    {
        if (ataqueActivo == null)
        {
            return;
        }

        Player player = other.GetComponentInParent<Player>();
        if (player == null || !player.CompareTag("Player"))
        {
            return;
        }

        Vida vida = player.vida != null ? player.vida : player.GetComponent<Vida>();
        if (vida == null || !jugadoresGolpeados.Add(vida.GetInstanceID()))
        {
            return;
        }

        float daño = orco != null ? orco.daño : 10f;
        vida.CausarDaño(daño);
    }

    private void ActualizarAtaqueActivo()
    {
        string nuevoAtaque = ObtenerAtaqueActivo();
        if (nuevoAtaque != ataqueActivo)
        {
            jugadoresGolpeados.Clear();
            ataqueActivo = nuevoAtaque;
        }

        bool activarHitbox = ataqueActivo != null;
        if (hitbox != null && hitbox.enabled != activarHitbox)
        {
            hitbox.enabled = activarHitbox;
        }
    }

    private string ObtenerAtaqueActivo()
    {
        if (orco == null || orco.animaciones == null)
        {
            return null;
        }

        Animator animador = orco.animaciones;
        if (animador.GetBool("at1")) return "at1";
        if (animador.GetBool("at2")) return "at2";
        if (animador.GetBool("at3")) return "at3";
        return null;
    }

    private void OnDisable()
    {
        ataqueActivo = null;
        jugadoresGolpeados.Clear();
        if (hitbox != null)
        {
            hitbox.enabled = false;
        }
    }
}