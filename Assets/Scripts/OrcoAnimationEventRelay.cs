using UnityEngine;

public class OrcoAnimationEventRelay : MonoBehaviour
{
    private AxeHitbox hacha;

    private void Awake()
    {
        hacha = transform.root.GetComponentInChildren<AxeHitbox>(true);
        if (hacha == null)
        {
            Debug.LogError("No se encontro AxeHitbox en la jerarquia del orco.", this);
        }
    }

    public void AbrirVentanaDeDaño()
    {
        if (hacha != null)
        {
            hacha.AbrirVentanaDeDaño();
        }
    }

    public void CerrarVentanaDeDaño()
    {
        if (hacha != null)
        {
            hacha.CerrarVentanaDeDaño();
        }
    }
}