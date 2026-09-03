using UnityEngine;

public class Player : MonoBehaviour
{
     Rigidbody rb;
     [SerializeField] float movementSpeed = 5f;
     float currentSpeed;
     Vector3 direction;
     [SerializeField] float shiftSpeed = 10f;
     [SerializeField] float jumpForce = 7f;
     bool isGrounded = true;
     [SerializeField] Animator anim;



     void Start()
     {
          rb = GetComponent<Rigidbody>();
          currentSpeed = movementSpeed;
          anim = GetComponent<Animator>();
     }

     void Update()
     {
          float moveHorizontal = Input.GetAxis("Horizontal");
          float moveVertical = Input.GetAxis("Vertical");

          direction = new Vector3(moveHorizontal, 0.0f, moveVertical);
          direction = transform.TransformDirection(direction);
          if (direction.x != 0 || direction.z != 0)
          {
               if (Input.GetKey(KeyCode.LeftShift))
               {
                    currentSpeed = shiftSpeed;
                    anim.SetBool("Run", true);
                    anim.SetBool("Walk", false);
               }
               else if (!Input.GetKey(KeyCode.LeftShift))
               {
                    currentSpeed = movementSpeed;
                    anim.SetBool("Run", false);
                    anim.SetBool("Walk", true);
               }
          }
          else if (direction.x == 0 && direction.z == 0)
          {
               anim.SetBool("Walk", false);
          }

          if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
          {
               rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
               isGrounded = false;
               anim.SetBool("Jump", true);
          }

          
          


     }

     void FixedUpdate()
     {
          rb.MovePosition(transform.position + direction * currentSpeed * Time.deltaTime);

     }

     void OnCollisionEnter(Collision collision)
     {
          isGrounded = true;
          anim.SetBool("Jump", false);
     }
}
