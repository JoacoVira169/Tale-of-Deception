using UnityEngine;

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
                    isRunning = true;
                    currentSpeed = shiftSpeed;
                    anim.SetBool("Run", true);
                    anim.SetBool("Walk", false);
               }
               else if (!Input.GetKey(KeyCode.LeftShift))
               {
                    isRunning = false;
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
          
          if (Input.GetKey(KeyCode.LeftControl) && isGrounded)
          {
               if (!isCrouching) // Evitamos llamar a SetBool cada frame innecesariamente
               {
                    isCrouching = true;
                    anim.SetBool("Crouch", true);
               }
          }
          else
          {
               if (isCrouching)
               {
                    isCrouching = false;
                    anim.SetBool("Crouch", false);
               }
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
