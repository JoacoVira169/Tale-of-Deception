using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody), typeof(Vida))]
public class Player : MonoBehaviour
{
     Rigidbody rb;
     [SerializeField] float movementSpeed = 8f;
     float currentSpeed;
     Vector3 direction;
     [SerializeField] float shiftSpeed = 20f;
     [SerializeField] float jumpForce = 9f;
     bool isGrounded = true;
     bool isCrouching = false;
     public bool isRunning = false;
     [SerializeField] Animator anim;
     public static Player singelton;
     public Vida vida;
     [SerializeField] public float damage = 15f;
     bool isStunned;
     public int noOfClicks;
     PlayerAudio playerAudio;
     private float temporizadorPasos = 0f;
     public float tiempoEntrePasosCaminando = 0.5f;
     public float tiempoEntrePasosCorriendo = 0.3f;



     public void Awake()
     {
          if (singelton == null)
          {
               singelton = this;
          }
          else if (singelton != this)
          {
               enabled = false;
               Destroy(gameObject);
               return;
          }
     }



     void Start()
     {
          rb = GetComponent<Rigidbody>();
          playerAudio = GetComponent<PlayerAudio>();
          currentSpeed = movementSpeed;
          if (anim == null)
          {
               anim = GetComponentInChildren<Animator>();
          }
          if (vida == null)
          {
               vida = GetComponent<Vida>();
          }
          if (anim == null || rb == null || vida == null)
          {
               Debug.LogError("Player requires a Rigidbody, Animator, and Vida component.", this);
               enabled = false;
               return;
          }
     }


     void Update()
     {
          temporizadorPasos -= Time.deltaTime;

          if (Input.GetMouseButtonUp(1))
          {
               anim.SetBool("Block", false);
          }

          if (Input.GetMouseButtonDown(1))
          {
               isBlocking();
          }

          AnimatorStateInfo attackState = anim.GetCurrentAnimatorStateInfo(0);
          bool isAttacking = anim.GetBool("hit1") || anim.GetBool("hit2") || anim.GetBool("hit3") ||
                             attackState.IsName("hit1") || attackState.IsName("hit2") || attackState.IsName("hit3");
          bool actionLocksMovement = isStunned || isAttacking || anim.GetBool("Block") || anim.GetBool("Damage");

          if (actionLocksMovement)
          {
               direction = Vector3.zero;
               anim.SetBool("Walk", false);
               anim.SetBool("Run", false);
               isRunning = false;
               temporizadorPasos = 0f;
               if (playerAudio != null)
               {
                    playerAudio.DetenerPasos();
               }
               return;
          }

          float moveHorizontal = Input.GetAxis("Horizontal");
          float moveVertical = Input.GetAxis("Vertical");

          direction = new Vector3(moveHorizontal, 0.0f, moveVertical);
          direction = transform.TransformDirection(direction);

          if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
          {
               rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
               isGrounded = false;
               anim.SetBool("Jump", true);
               if (playerAudio != null)
               {
                    playerAudio.ReproducirSalto();
               }
          }

          if (Input.GetKey(KeyCode.LeftControl) && isGrounded)
          {
               if (!isCrouching)
               {
                    isCrouching = true;
                    anim.SetBool("Crouch", true);
               }
          }
          else if (isCrouching)
          {
               isCrouching = false;
               anim.SetBool("Crouch", false);
          }

          bool hasMovement = direction.sqrMagnitude > 0.01f;
          if (hasMovement && isGrounded)
          {
               bool isSprinting = Input.GetKey(KeyCode.LeftShift) && !isCrouching;
               isRunning = isSprinting;

               if (isSprinting)
               {
                    currentSpeed = shiftSpeed;
                    anim.SetBool("Run", true);
                    anim.SetBool("Walk", false);
               }
               else
               {
                    currentSpeed = movementSpeed;
                    anim.SetBool("Run", false);
                    anim.SetBool("Walk", true);
               }

               if (temporizadorPasos <= 0f && playerAudio != null)
               {
                    if (isCrouching)
                    {
                         playerAudio.ReproducirPasoAgachado();
                         temporizadorPasos = tiempoEntrePasosCaminando;
                    }
                    else if (isSprinting)
                    {
                         playerAudio.ReproducirPasoCorriendo();
                         temporizadorPasos = tiempoEntrePasosCorriendo;
                    }
                    else
                    {
                         playerAudio.ReproducirPasoCaminando();
                         temporizadorPasos = tiempoEntrePasosCaminando;
                    }
               }
          }
          else
          {
               isRunning = false;
               anim.SetBool("Walk", false);
               anim.SetBool("Run", false);
               temporizadorPasos = 0f;
               if (playerAudio != null)
               {
                    playerAudio.DetenerPasos();
               }
          }
     }

     void FixedUpdate()
     {
          rb.MovePosition(transform.position + direction * currentSpeed * Time.deltaTime);

     }

     void OnCollisionEnter(Collision collision)
     {
          foreach (ContactPoint contact in collision.contacts)
          {
               if (contact.normal.y > 0.5f)
               {
                    if (!isGrounded && playerAudio != null)
                    {
                         playerAudio.ReproducirAterrizaje();
                    }

                    isGrounded = true;
                    anim.SetBool("Jump", false);
                    break;
               }
          }
     }

     public void GolpeAnimacion(float cuanto)
     {
          Debug.Log("GolpeAnimacion");

          if (anim != null)
          {
               if (playerAudio != null)
               {
                    playerAudio.ReproducirDaño();
               }

               anim.SetBool("Damage", true);
               StartCoroutine(EsperarYTerminarDaño());
          }
     }
     private IEnumerator EsperarYTerminarDaño()
     {
          yield return new WaitForSeconds(1.5f);
          TerminarAnimacionDaño();
     }


     public void TerminarAnimacionDaño()
     {
          anim.SetBool("Damage", false);
          isStunned = false;
     }

     public void isBlocking()
     {
          if (anim.GetBool("Walk") == true)
          {
               anim.SetBool("Walk", false);
               anim.SetBool("Block", true);
          }
          else if (anim.GetBool("Run") == true)
          {
               anim.SetBool("Run", false);
               anim.SetBool("Block", true);
          }

          else if (anim.GetBool("Damage") == true)
          {
               StartCoroutine(EsperarYTerminarDaño());
          }
          else if (anim.GetBool("hit1") == true || anim.GetBool("hit2") == true || anim.GetBool("hit3") == true)
          {
               noOfClicks = 0;
               Fighter fighter = GetComponentInChildren<Fighter>();
               if (fighter != null)
               {
                    fighter.noOfClicks = 0;
               }
               anim.SetBool("hit1", false);
               anim.SetBool("hit2", false);
               anim.SetBool("hit3", false);
          }

          anim.SetBool("Block", true);


     }

}
