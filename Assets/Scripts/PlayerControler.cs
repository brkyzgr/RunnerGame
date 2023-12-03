using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerControler : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 direction;
    public float forwardSpeed;
    public float maxSpeed;
    private int desiredLane = 1; // 0:sol , 1:orta , 2:sağ
    public float laneDistance = 4; // 2 Lane arasındaki uzaklık
    public float jumpForce; 
    public float Gravity = -20;
    public bool isGrounded;
    
    public Animator animator;
    private bool isSliding = false;
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!PlayerManager.isGameStarted)
            return;

        
        // Hız Yükseltme
        if(forwardSpeed < maxSpeed)
            forwardSpeed += 0.1f * Time.deltaTime;
            
        animator.SetBool("isGameStarted", true);
        direction.z = forwardSpeed;

        
        
        if(controller.isGrounded)
        {
            
            if(SwipeManager.swipeUp)
            {
                Jump();
            }
        }
        else
        {
            direction.y += Gravity * Time.deltaTime;
        }
        

        if(SwipeManager.swipeDown && !isSliding)
        {
            StartCoroutine(Slide());
        }

        // Hangi şeritte olmamız gerektiğine dair girdileri toplayın
        if(SwipeManager.swipeRight)
        {
            desiredLane++;
            if(desiredLane == 3)
                desiredLane = 2;
        }
        if(SwipeManager.swipeLeft)
        {
            desiredLane--;
            if(desiredLane == -1)
                desiredLane = 0;
        }
        // Gelecekte nerede olmamız gerektiğini hesaplayın

        Vector3 targetPosition = transform.position.z * transform.forward + transform.position.y * transform.up;

        if(desiredLane == 0)
        {
            targetPosition += Vector3.left * laneDistance;
        }else if(desiredLane == 2)
        {
            targetPosition += Vector3.right * laneDistance;
        }

        transform.position = targetPosition;
        controller.center = controller.center;
    }

    private void FixedUpdate()
    {
        if(!PlayerManager.isGameStarted)
            return;
        controller.Move(direction*Time.fixedDeltaTime);
    }

    private void Jump()
    {
        direction.y = jumpForce; 
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(hit.transform.tag == "Obstacle")
        {
            PlayerManager.gameOver = true;
        }
    }
    private IEnumerator Slide()
    {
        isSliding = true;
        animator.SetBool("isSliding",true);
        controller.center = new Vector3(0,-0.5f,0);
        controller.height = 1;
        yield return new WaitForSeconds(1.3f);
        
        controller.center = new Vector3(0,-0.5f,0);
        controller.height = 2;
        animator.SetBool("isSliding",false);
        isSliding = false;
    }
}
