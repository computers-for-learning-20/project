using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarbleBehavior : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotateSpeed = 180f;
    public float jumpVelocity = 5f;
    public float distanceToGround = 0.5f;
    public LayerMask groundLayer;
    public GameObject blast;
    public float blastSpeed = 50f;
    public GameBehavior gameManager;

    public ParticleSystem ObstacleCollision;

    private float fbInput;
    private float lrInput;
    
    private Rigidbody _rb;
    private SphereCollider _col;
    
    void Start()
    {
        //You'll need to add a rigidbody to the marble first
        _rb = GetComponent<Rigidbody>();
        _col = GetComponent<SphereCollider>();

        gameManager = GameObject.Find("GameManager")
            .GetComponent<GameBehavior>();

    }

    // Update is called once per frame
    void Update()
    {
        // Put code is for movement using the Sprite's native variables here
        fbInput = Input.GetAxis("Vertical")*moveSpeed;
        lrInput = Input.GetAxis("Horizontal")*rotateSpeed;
    }
    
    void FixedUpdate()
    {
        
        Vector3 rotation = Vector3.up * lrInput;
        Quaternion angleRot = Quaternion.Euler(rotation *
            Time.fixedDeltaTime);

        _rb.MovePosition(this.transform.position +
            this.transform.forward * fbInput * Time.fixedDeltaTime);

        _rb.MoveRotation(_rb.rotation * angleRot);

        //Check if Marble is near the ground and if space bar
        if(IsGrounded() && Input.GetKeyDown(KeyCode.Space)){
            _rb.AddForce(Vector3.up * jumpVelocity, ForceMode.Impulse);
        }

        //Check a mouse click to project blasts
        if (Input.GetMouseButtonDown(0)){
            GameObject newBlast = Instantiate(blast, 
                            this.transform.position + new Vector3(1, 0, 0),
                            this.transform.rotation) as GameObject;
            Rigidbody blastRB = newBlast.GetComponent<Rigidbody>();

            blastRB.velocity = this.transform.forward * blastSpeed; 
        }
    }

    private bool IsGrounded(){
        Vector3 capsuleBottom = new Vector3(_col.bounds.center.x, _col.bounds.min.y,
                                            _col.bounds.center.z);
        bool grounded = Physics.CheckCapsule(_col.bounds.center, 
                        capsuleBottom, distanceToGround, groundLayer,
                        QueryTriggerInteraction.Ignore);
        return grounded;
    }

    void OnCollisionEnter(Collision collision){
        
        //Decrease Marble's health if it collides with any obstacle
        if(collision.gameObject.name == "obstacleBody")
        {
               ObstacleCollision.Play();
               gameManager.HealthMarble -= 10;
               Debug.Log("Ouch! Marble's Health decreased!");
        }

        //Decrease Marble's health if it collides with enemy
        if (collision.gameObject.name == "EnemyBody")
        {
            if(gameManager.HealthMarble <= 20)
            {
                gameManager.HealthMarble = 0;
                Debug.Log("Ouch! You've lost!");
            }
            else
            {
                gameManager.HealthMarble -= 20;
                Debug.Log("Ouch! Marble's Health decreased!");
            }
        }
    }
    
}
