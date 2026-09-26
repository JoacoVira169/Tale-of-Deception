using System.Collections.Generic;
using UnityEngine;

public class AxeHitbox : MonoBehaviour
{
    private Collider hitbox;
    private Orco orco;
    private readonly HashSet<int> jugadoresGolpeados = new HashSet<int>();
    private bool ventanaActiva;

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

    public void AbrirVentanaDeDaño()
    {
        jugadoresGolpeados.Clear();
        ventanaActiva = true;
        hitbox.enabled = true;
    }

    public void CerrarVentanaDeDaño()
    {
        ventanaActiva = false;
        if (hitbox != null)
        {
            hitbox.enabled = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!ventanaActiva)
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

        float daño = orco != null ? orco.daño : 3f;
        vida.CausarDaño(daño);
    }

    private void OnDisable()
    {
        CerrarVentanaDeDaño();
    }
}