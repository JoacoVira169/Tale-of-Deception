using UnityEngine;

public class OrcoAnimationEventRelay : MonoBehaviour
{
    private AxeHitbox hacha;
    private Orco orco;

    private void Awake()
    {
        Transform raiz = transform.root;
        hacha = raiz.GetComponentInChildren<AxeHitbox>(true);
        orco = raiz.GetComponentInChildren<Orco>(true);
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

    public void TerminarAnimacionDaño()
    {
        if (orco != null)
        {
            orco.TerminarAnimacionDaño();
        }
    }
}