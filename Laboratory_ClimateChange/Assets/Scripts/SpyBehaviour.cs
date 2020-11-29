using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpyBehaviour : MonoBehaviour
{
    private const double V = 0.5;
    public float moveSpeed = 5f;
    public float rotateSpeed = 80f;
    public float rot = 0f;
    public float gravity = 8;
    public float jumpVelocity = 5f;
    public float groundDistance = 0.3f;
    public LayerMask whatIsGround;
    Vector3 moveDir = Vector3.zero;
    CharacterController controller;
    Animator anim;
    private Rigidbody _rb;
    private float fbInput;
    private float lrInput;
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        fbInput = Input.GetAxis("Vertical")*moveSpeed;
        lrInput = Input.GetAxis("Horizontal")*rotateSpeed;

        Move();
        
        
    }

    void Move(){
        if (controller.isGrounded){
            if(Input.GetKey(KeyCode.W) || (Input.GetKey(KeyCode.UpArrow))){
                anim.SetInteger("condition", 1);
                moveDir = new Vector3(0, 0, 1);
                moveDir *= moveSpeed;
                moveDir = transform.TransformDirection(moveDir);
            }
            else if(Input.GetKeyUp(KeyCode.W) || (Input.GetKeyUp(KeyCode.UpArrow))){
                anim.SetInteger("condition", 0);
                moveDir = new Vector3(0,0,0);
            }
            else if(Input.GetKey(KeyCode.R)){
                anim.SetInteger("condition", 2);
                moveDir = new Vector3(0, 0, 1);
                moveDir *= moveSpeed*4;
                moveDir = transform.TransformDirection(moveDir);
            }
            else if(Input.GetKeyUp(KeyCode.R)){
                anim.SetInteger("condition", 0);
                moveDir = new Vector3(0,0,0);
            }
            else if(Input.GetKey(KeyCode.S) || (Input.GetKey(KeyCode.DownArrow))){
                anim.SetInteger("condition", -1);
                moveDir = new Vector3(0, 0, -1);
                moveDir *= moveSpeed;
                moveDir = transform.TransformDirection(moveDir);
            }
            else if(Input.GetKeyUp(KeyCode.S) || (Input.GetKeyUp(KeyCode.DownArrow))){
                anim.SetInteger("condition", 0);
                moveDir = new Vector3(0,0,0);
            }
            else if (Input.GetButtonDown ("Jump")) {
            anim.SetTrigger ("Jump");
            _rb.AddForce (Vector3.up * jumpVelocity,  ForceMode.Impulse);
            /*if (Physics.Raycast (transform.position + (Vector3.up * 0.1f), Vector3.down, 
                                groundDistance, whatIsGround)) {
            anim.SetBool ("grounded", true);
            anim.applyRootMotion = true;} */
            }

    
        }
        rot += Input.GetAxis("Horizontal")*rotateSpeed*Time.deltaTime;
        transform.eulerAngles = new Vector3(0, rot, 0);

        moveDir.y -= gravity * Time.deltaTime;
        controller.Move(moveDir*Time.deltaTime);
    }

    
    
}
