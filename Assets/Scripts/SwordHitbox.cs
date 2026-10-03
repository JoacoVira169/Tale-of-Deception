using System.Collections.Generic;
using UnityEngine;

public class SwordHitbox : MonoBehaviour
{
    private Collider hitbox;
    private Animator animador;
    private Player jugador;
    private readonly HashSet<int> enemigosGolpeados = new HashSet<int>();
    private readonly HashSet<int> collidersSinVida = new HashSet<int>();
    private string ataqueActivo;
    private bool controlarVentanaPorEventos;

    private void Awake()
    {
        hitbox = GetComponent<Collider>();
        animador = GetComponentInParent<Animator>();
        jugador = GetComponentInParent<Player>();

        if (hitbox == null)
        {
            Debug.LogError("SwordHitbox requiere un Collider.", this);
            enabled = false;
            return;
        }

        if (animador == null)
        {
            Debug.LogError("SwordHitbox necesita un Animator en el padre del arma.", this);
            enabled = false;
            return;
        }

        if (jugador == null)
        {
            Debug.LogError("SwordHitbox necesita un Player en el padre del arma.", this);
            enabled = false;
            return;
        }

        hitbox.isTrigger = true;
        hitbox.enabled = false;
    }

    private void Update()
    {
        ActualizarAtaqueActivo();
    }

    public void AbrirVentanaDeDañoPlayer()
    {
        controlarVentanaPorEventos = true;
        ataqueActivo = "evento";
        enemigosGolpeados.Clear();
        if (hitbox != null)
        {
            hitbox.enabled = true;
        }
    }

    public void CerrarVentanaDeDañoPlayer()
    {
        controlarVentanaPorEventos = true;
        ataqueActivo = null;
        enemigosGolpeados.Clear();
        if (hitbox != null)
        {
            hitbox.enabled = false;
        }
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
        if (string.IsNullOrEmpty(ataqueActivo) || other == null)
        {
            return;
        }

        if (other.transform.root == transform.root)
        {
            return;
        }

        VidaOrco vidaOrco = other.GetComponentInParent<VidaOrco>();
        if (vidaOrco == null)
        {
            vidaOrco = other.transform.root.GetComponentInChildren<VidaOrco>();
        }

        if (vidaOrco == null)
        {
            int colliderId = other.GetInstanceID();
            if (collidersSinVida.Add(colliderId))
            {
                Debug.LogWarning(
                    $"El collider '{other.name}' no encuentra VidaOrco en sus padres ni en la raíz del enemigo.",
                    other
                );
            }
            return;
        }

        int id = vidaOrco.GetInstanceID();
        if (!enemigosGolpeados.Add(id))
        {
            return;
        }

        vidaOrco.CausarDañoO(jugador.damage);
    }

    private void ActualizarAtaqueActivo()
    {
        if (controlarVentanaPorEventos)
        {
            return;
        }

        string nuevoAtaque = null;
        if (animador.GetBool("hit1")) nuevoAtaque = "hit1";
        if (animador.GetBool("hit2")) nuevoAtaque = "hit2";
        if (animador.GetBool("hit3")) nuevoAtaque = "hit3";

        if (nuevoAtaque != ataqueActivo)
        {
            ataqueActivo = nuevoAtaque;
            enemigosGolpeados.Clear();
            collidersSinVida.Clear();
        }

        bool activarHitbox = ataqueActivo != null;
        if (hitbox != null)
        {
            hitbox.enabled = activarHitbox;
        }
    }

    private void OnDisable()
    {
        ataqueActivo = null;
        controlarVentanaPorEventos = false;
        enemigosGolpeados.Clear();
        collidersSinVida.Clear();
        if (hitbox != null)
        {
            hitbox.enabled = false;
        }
    }
}
