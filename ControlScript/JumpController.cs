using UnityEngine;
using System.Collections;



    class JumpController :ThirdPersonController
    {

        public override void UpdateSmoothedMovementDirection()
        {

            if (InputManager.instance.GetButton("Fire1"))
            {
                characterState = CharacterState.Running;
                moveDirection = Vector3.forward;
                moveSpeed = 1;
            }
            else
            {
                characterState = CharacterState.Idle;
                moveDirection = Vector3.zero;
                moveSpeed = 0;
            }
        }
        void Update()
        {
            if (!isControllable)
            {
                // kill all inputs if not controllable.
                Input.ResetInputAxes();
            }

            if (InputManager.instance.GetButtonDown("Fire2"))
            {
                lastJumpButtonTime = Time.time;


            }
            if (PlayerMainGui.IsMouseAV)
            {
               

                pawn.UpdateRotation(-InputManager.instance.GetMouseAxis("Mouse Y"), InputManager.instance.GetMouseAxis("Mouse X"));


            }
        }
        protected override void ApplyDoubleJumping()
        {

            if (!pawn.isGrounded)
            {
                switch (pawn.GetState())
                {
                    case CharacterState.DoubleJump:
                        if ((InputManager.instance.GetButton("Fire2")))
                        {
                            characterState = CharacterState.DoubleJump;

                        }
                        else
                        {
                            characterState = CharacterState.Jumping;
                        }
                        break;

                    case CharacterState.Jumping:
                    case CharacterState.Sprinting:
                        if (lastDoubleTime < Time.time - 1.0f)
                        {
                            if ((InputManager.instance.GetButton("Fire2")) )
                            {
                                characterState = CharacterState.DoubleJump;
                                lastDoubleTime = Time.time;
                            }
                            else
                            {
                                characterState = CharacterState.Jumping;
                            }
                        }
                        break;
                    default:
                        break;


                }

            }
        }
    }
