using UnityEngine;

public sealed class SwordHitboxTriggerRelay : MonoBehaviour
{
    private SwordHitbox owner;

    public void SetOwner(SwordHitbox hitboxOwner)
    {
        owner = hitboxOwner;
    }

    private void OnTriggerEnter(Collider other)
    {
        owner?.ProcesarTriggerEnter(other);
    }

    private void OnTriggerStay(Collider other)
    {
        owner?.ProcesarTriggerStay(other);
    }
}