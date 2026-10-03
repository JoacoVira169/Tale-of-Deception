using System.Collections.Generic;
using UnityEngine;

public class SwordHitbox : MonoBehaviour
{
    [SerializeField] private Collider hitbox;
    private Animator animador;
    private Player jugador;
    private readonly HashSet<int> enemigosGolpeados = new HashSet<int>();
    private readonly HashSet<int> collidersSinVida = new HashSet<int>();
    private string ataqueActivo;
    private bool controlarVentanaPorEventos;
    private bool colliderEnHijo;

    private void Awake()
    {
        if (hitbox == null)
        {
            hitbox = GetComponent<Collider>();
        }
        if (hitbox == null)
        {
            hitbox = GetComponentInChildren<Collider>(true);
        }
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
        hitbox.enabled = true;

        colliderEnHijo = hitbox.gameObject != gameObject;
        if (colliderEnHijo)
        {
            SwordHitboxTriggerRelay relay = hitbox.GetComponent<SwordHitboxTriggerRelay>();
            if (relay == null)
            {
                relay = hitbox.gameObject.AddComponent<SwordHitboxTriggerRelay>();
            }
            relay.SetOwner(this);
        }
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
    }

    public void CerrarVentanaDeDañoPlayer()
    {
        controlarVentanaPorEventos = true;
        ataqueActivo = null;
        enemigosGolpeados.Clear();
    }

    private void OnTriggerEnter(Collider Enemy)
    {
        if (colliderEnHijo) return;
        ProcesarTriggerEnter(Enemy);
    }

    internal void ProcesarTriggerEnter(Collider Enemy)
    {
        Debug.Log($"SwordHitbox detectó '{Enemy.name}'. Ataque activo: {ataqueActivo ?? "ninguno"}.", Enemy);
        IntentarCausarDaño(Enemy);
    }

    private void OnTriggerStay(Collider Enemy)
    {
        if (colliderEnHijo) return;
        ProcesarTriggerStay(Enemy);
    }

    internal void ProcesarTriggerStay(Collider Enemy)
    {
        IntentarCausarDaño(Enemy);
    }

    private void IntentarCausarDaño(Collider Enemy)
    {
        if (string.IsNullOrEmpty(ataqueActivo) || Enemy == null)
        {
            return;
        }

        if (!TieneEtiquetaEnemy(Enemy.transform))
        {
            Debug.Log($"'{Enemy.name}' no tiene la etiqueta Enemy en sí mismo ni en sus padres.", Enemy);
            return;
        }

        if (Enemy.transform.IsChildOf(jugador.transform))
        {
            return;
        }

        VidaOrco vidaOrco = Enemy.GetComponentInParent<VidaOrco>();
        if (vidaOrco == null)
        {
            vidaOrco = Enemy.transform.root.GetComponentInChildren<VidaOrco>();
        }

        if (vidaOrco == null)
        {
            int colliderId = Enemy.GetInstanceID();
            if (collidersSinVida.Add(colliderId))
            {
                Debug.LogWarning(
                    $"El collider '{Enemy.name}' no encuentra VidaOrco en sus padres ni en la raíz del enemigo.",
                    Enemy
                );
            }
            return;
        }

        int id = vidaOrco.GetInstanceID();
        if (!enemigosGolpeados.Add(id))
        {
            return;
        }

        float vidaAntes = vidaOrco.vidaActualOrco;
        vidaOrco.CausarDañoO(jugador.damage);
        Debug.Log($"Golpe aplicado a '{vidaOrco.name}': {vidaAntes} -> {vidaOrco.vidaActualOrco}.", vidaOrco);
    }

    private bool TieneEtiquetaEnemy(Transform objetivo)
    {
        while (objetivo != null)
        {
            if (objetivo.CompareTag("Enemy"))
            {
                return true;
            }

            objetivo = objetivo.parent;
        }

        return false;
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
