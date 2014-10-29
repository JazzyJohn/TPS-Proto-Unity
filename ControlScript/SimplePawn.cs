using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class SimplePawn : Pawn {


    protected bool stopSprint;

    public void FixedUpdate()
    {

        if (!isActive)
        {
            return;
        }
        if (!foxView.isMine)
        {
            return;
        }
        if (!_rb.isKinematic)
        {

            _rb.AddForce(new Vector3(0, -gravity * rigidbody.mass, 0) + pushingForce);
        }
        if (!canMove)
        {
            return;
        }
        if (isDead)
        {
            return;
        }
        if (myTransform.position.y < GameRule.instance.DeathY)
        {
            KillIt(null);
        }
        Vector3 velocity = _rb.velocity;
        /* if(nextMovement.y==0){
             nextMovement.y = velocity.y;
         }*/
        // nextMovement = nextMovement;// -Vector3.up * gravity + pushingForce / rigidbody.mass;
        Vector3 velocityChange;
        if (isAi)
        {
            if (nextState == CharacterState.Jumping)
            {
                velocityChange = (nextMovement - velocity);
            }
            else
            {
                velocityChange = Vector3.ClampMagnitude((nextMovement - velocity), aiVelocityChangeMax);
            }
        }
        else
        {
            velocityChange = (nextMovement - velocity);
        }

        switch (characterState)
        {
            case CharacterState.Idle:
            case CharacterState.Running:
            case CharacterState.Walking:
                if (isGrounded)
                {
                    jetPackEnable = false;
                    stopSprint = false;
                    if (_rb.isKinematic) _rb.isKinematic = false;

                    //Debug.Log (this+ " " +velocityChange);
                    rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);

                    if (nextState == CharacterState.Sprinting)
                    {
                        StopFire();
                        StartSprint();

                    }
                    else
                    {
                        characterState = nextState;
                    }
                    if (nextState == CharacterState.Jumping)
                    {
                        Jump();

                    }
                }
                else
                {
                    characterState = CharacterState.Jumping;
                }

                break;
            case CharacterState.Sprinting:

                if (_rb.isKinematic) _rb.isKinematic = false;
                nextMovement = myTransform.forward;
                if (nextState != CharacterState.Jumping)
                {
                    velocityChange = nextMovement.normalized * groundSprintSpeed - velocity;
                }
                //Debug.Log (velocityChange);
                if (!jetPackEnable || stopSprint)
                {
                    if (isGrounded)
                    {
                        // Debug.Log("STOPSPRINT");
                        characterState = CharacterState.Running;
                    }
                    else
                    {
                        characterState = CharacterState.Jumping;
                    }
                }
                else
                {
                    characterState = nextState;
                    if (characterState != CharacterState.Sprinting)
                    {
                        StopSprint();
                    }
                    if (isGrounded)
                    {
                        rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);

                    }
                    else
                    {
                        characterState = CharacterState.Jumping;
                    }
                    
                }



              
                ToggleAim(false);



                break;
            case CharacterState.Jumping:
                if (characterState != CharacterState.DoubleJump)
                {
                    animator.FreeFall();
                }
                animator.ApllyJump(true);
                            
               
                if (isGrounded)
                {
                    JumpEnd(nextState);
                }
                break;
           
            
            default:
                characterState = nextState;
                break;

        }
       
        isGrounded = false;

    }
    public override void StartFire()
    {
        if (CurWeapon != null)
        {
            CurWeapon.StartFire();
        }
        stopSprint = true;
    }
    public override bool CanSprint()
    {
        return base.CanSprint() && !CurWeapon.IsShooting();
    }
	
}