using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController2D controller;
    public float runSpeed = 40f;
    float horizontalMove = 0f;

    public float cooldownTime = 2f;
    private float nextDashTime = 0f;
    
    bool jump = false;
    bool crouch = false;
    bool dash = false;
    

    // Update is called once per frame
    void Update()
    {
        //Horizontal Movement Detection
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
       
        //Detect Jump
        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
        }
        //Detect Crouch
        if (Input.GetButtonDown("Crouch"))
        {
            crouch = true;
        }
        else if (Input.GetButtonUp("Crouch"))
        {
            crouch = false;
        }
        //Dash Ability
        if (Time.time > nextDashTime) //Dash Ability cooldown 
        {
            if (Input.GetButtonDown("Dash"))
            {
                nextDashTime = Time.time + cooldownTime;
                dash = true;
            }
        }
    }

    void FixedUpdate()
    {
        //Move Character
        controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump, dash);
        jump = false;
        dash = false;
    }
}
