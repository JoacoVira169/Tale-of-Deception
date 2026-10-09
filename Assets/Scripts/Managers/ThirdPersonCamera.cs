using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour 
{        
    [SerializeField] GameObject player;
    [SerializeField][Range(0.5f, 2f)]
    float mouseSense = 1; 
    [SerializeField][Range(-20, -10)]
    int lookUp = -15;
    [SerializeField][Range(15, 25)]
    int lookDown = 20;
    private Vida vidaJugador;

    private void Start() 
    {
        Cursor.lockState = CursorLockMode.Locked; 
        if (player != null)
        {
            vidaJugador = player.GetComponent<Vida>();
            if (vidaJugador != null)
            {
                vidaJugador.eventoMorir.AddListener(DetenerRotacion);
            }
        }
    }

    private void OnDestroy()
    {
        if (vidaJugador != null)
        {
            vidaJugador.eventoMorir.RemoveListener(DetenerRotacion);
        }
    }

    private void DetenerRotacion()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        enabled = false;
    }

    void Update()
    {     
        float rotateX = Input.GetAxis("Mouse X") * mouseSense;
        float rotateY = Input.GetAxis("Mouse Y") * mouseSense;

        Vector3 rotCamera = transform.rotation.eulerAngles;
        Vector3 rotPlayer = player.transform.rotation.eulerAngles;

        rotCamera.x = (rotCamera.x > 180) ? rotCamera.x - 360 : rotCamera.x;
        rotCamera.x = Mathf.Clamp(rotCamera.x, lookUp, lookDown);
        rotCamera.x -= rotateY;               
        
        rotCamera.z = 0;
        rotPlayer.y += rotateX;

        transform.rotation = Quaternion.Euler(rotCamera);
        player.transform.rotation = Quaternion.Euler(rotPlayer);     
    }
}