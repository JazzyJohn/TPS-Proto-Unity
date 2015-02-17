using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


class JumpPawn :Pawn 
{
    Vector3 acceleration=Vector3.zero;

    Vector3 velocity=Vector3.zero;

    public float timeForJump =0.3f;

    private float timeGrounded  = 0.0f;

    public float timeForImpulseJump = 0.2f;

    private Vector3 wallRunVelocity;

    private Vector3 start;

    public void Awake()
    {
        base.Awake();
        start = myTransform.position;

    }
    public void MoveToStart()
    {
        myTransform.position = start;
        _rb.velocity = Vector3.zero;
        characterState = CharacterState.Idle;
        velocity = Vector3.zero;

    }

    public override void FixedUpdate()
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
            MoveToStart();
        }
       // Debug.Log(characterState +" " +isGrounded);
        Vector3 newvelocity;
        /* if(nextMovement.y==0){
             nextMovement.y = velocity.y;
         }*/
        // nextMovement = nextMovement;// -Vector3.up * gravity + pushingForce / rigidbody.mass;

      

        switch (characterState)
        {
            case CharacterState.Idle:
            case CharacterState.Running:
            case CharacterState.Walking:
                if (isGrounded)
                {
                    jetPackEnable = false;
                    if (_rb.isKinematic) _rb.isKinematic = false;

                    //Debug.Log (this+ " " +velocityChange);
                    //rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
                    if ((Time.time-timeGrounded) > timeForJump)
                    {
                        newvelocity =groundRunSpeed*myTransform.forward*nextMovement.normalized.magnitude;
                    }else{
                        newvelocity = JumpAssist.GetMeFlat(velocity).magnitude * myTransform.forward * nextMovement.normalized.magnitude;
                    }
                    characterState = nextState;
                  
                    if (nextState == CharacterState.Jumping)
                    {
                        newvelocity =  Jump();

                    }
                }
                else
                {
                    newvelocity = velocity;
                    characterState = CharacterState.Jumping;
                }

                break;
          
            case CharacterState.Jumping:
                if (characterState != CharacterState.DoubleJump)
                {
                    animator.FreeFall();
                }
                animator.ApllyJump(true);
                if (canWallRun)
                {
                    animator.WallAnimation(false, false, false);
                }
                newvelocity = velocity;

                if (WallRun(nextMovement, nextState))
                {
                    newvelocity = GetWallRunVelocity();
                    Land();
                    SendMessage("WallLand", SendMessageOptions.DontRequireReceiver);
                }
                else
                {
                    newvelocity += Vector3.down * gravity * Time.fixedDeltaTime;
                }
                PullUp();
                if (nextState == CharacterState.DoubleJump)
                {
                    DoubleJump();

                }
                if (isGrounded)
                {
                    JumpEnd(nextState);
                }
                break;
            case CharacterState.DoubleJump:



                newvelocity =Math.Max( JumpAssist.GetMeFlat(velocity).magnitude,flyForwardSpeed) * myTransform.forward + Vector3.up * flySpeed;
                animator.ApllyJump(true);
                animator.WallAnimation(false, false, false);
                //rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
                if (!jetPackEnable)
                {
                    if (isGrounded)
                    {

                        JumpEnd(nextState);
                    }
                    else
                    {
                        characterState = CharacterState.Jumping;
                    }
                }
                else
                {
                    characterState = nextState;
                    if (characterState != CharacterState.DoubleJump)
                    {
                        StopDoubleJump();
                    }
                    PullUp();
                    if (WallRun(nextMovement, nextState))
                    {
                        newvelocity = GetWallRunVelocity();
                        Land();
                        SendMessage("WallLand", SendMessageOptions.DontRequireReceiver);
                    }
                }





                break;
            case CharacterState.WallRunning:
                //Debug.Log(nextState);
                if (!WallRun(nextMovement, nextState))
                {

                    characterState = CharacterState.Jumping;
					jetPackEnable= false;
                    animator.ApllyJump(true);
                    animator.WallAnimation(false, false, false);
                    animator.FreeFall();
                  
                }
                PullUp();
                newvelocity = GetWallRunVelocity();

            
                if (characterState != CharacterState.WallRunning)
                {
                    if (player != null)
                    {
                        EventHolder.instance.FireEvent(typeof(LocalPlayerListener), "EventEndWallRun", player, myTransform.position);

                    }

                }
                
                break;
            case CharacterState.PullingUp:
                newvelocity =Vector3.zero;
                PullUp();
              
                break;
            default:
                characterState = nextState;
                newvelocity = velocity;
                break;

        }
        /*
        //Debug.Log (_rb.velocity.magnitude);
        if (isGrounded) {
            //Debug.Log ("Ground"+characterState);


            if(nextState==CharacterState.Sprinting&&characterState!=CharacterState.Sprinting){
                StartSprint();
            }
            characterState = nextState;
            if(nextState==CharacterState.Jumping){
                Jump ();

            }

        } else {
            //Debug.Log ("Air"+characterState);
            v = nextMovement.normalized.magnitude;
			
            switch(nextState)
            {
            case CharacterState.DoubleJump:
                if(characterState!=CharacterState.WallRunning
                   &&characterState!=CharacterState.PullingUp){


                }
                break;
            default:

                if(!WallRun (nextMovement,nextState)){
                    if(characterState==CharacterState.Idle
                       ||characterState==CharacterState.Walking 
                       ||characterState==CharacterState.Running 
                       ||characterState==CharacterState.Sprinting){
                        characterState=CharacterState.Jumping;
                    }
                    animator.ApllyJump(true);						
                    animator.WallAnimation(false,false,false);
                    if(characterState!=CharacterState.DoubleJump){
                        animator.FreeFall();
                    }
                    //Debug.Log ("My Name" +this +"  "+nextState+"  "+isGrounded);
                }else{
                    SendMessage ("WallLand", SendMessageOptions.DontRequireReceiver);
                }
                if(PullUpCheck()){

                    PullUp();
                }
                break;
            }
			
        }
        //Debug.Log(_rb.isKinematic);
        */
        isGrounded = CheckGrounded();
     
        velocity = newvelocity;
       // myTransform.position += velocity;
       Debug.Log(characterState +"  " +wallState);
        if (!_rb.isKinematic)
        {
            _rb.velocity = velocity;
        }
        velocity = _rb.velocity;
    }

    bool CheckGrounded()
    {
        if (lastJumpTime + 0.1f > Time.time)
        {
            return false;
        }
        Ray ray = new Ray(myTransform.position,Vector3.down);
        RaycastHit hitInfo;
        return Physics.SphereCast(ray, capsule.radius,out hitInfo, capsule.height / 2 , floorLayer);
    }
    protected override bool ShouldRotateTorso()
    {
        return false;
    }
    Vector3 GetWallRunVelocity()
    {
        return wallRunVelocity;
    }
    bool WallRun(Vector3 movement, CharacterState state)
    {
        if ((!canWallRun || !_canWallRun) && foxView.isMine) return false;

        //if (isGrounded) return false;
        if (lastTimeOnWall + 0.5f > Time.time)
        {
            wallState = WallState.WallNone;
            return false;
        }

        /*if (_rb.velocity.sqrMagnitude < 0.02f ) {
            if(characterState == CharacterState.WallRunning){
                    characterState = CharacterState.Jumping;
                    lastTimeOnWall = Time.time;
            }
            return false;
        }*/

        if ((_rb.velocity.sqrMagnitude < 0.02f || !jetPackEnable) && characterState == CharacterState.WallRunning)
        {
            characterState = CharacterState.Jumping;
            lastTimeOnWall = Time.time;
            wallState = WallState.WallNone;
            return false;
        }
        if (characterState != CharacterState.DoubleJump && characterState != CharacterState.Sprinting && characterState != CharacterState.WallRunning && characterState != CharacterState.Jumping)
        {
            wallState = WallState.WallNone;
            return false;
        }
        //Debug.Log (characterState);
        RaycastHit leftH, rightH, frontH;


        bool leftW = Physics.Raycast(myTransform.position,
                                      (-1 * myTransform.right).normalized, out leftH, capsule.radius + 0.4f, wallRunLayers);
        bool rightW = Physics.Raycast(myTransform.position,
                                       (myTransform.right).normalized, out rightH, capsule.radius + 0.4f, wallRunLayers);
        bool frontW;
        if(  wallState == WallState.WallF){
            frontW = Physics.Raycast(myTransform.position,
                                    -myTransform.up, out frontH, capsule.height/2 + 0.2f, wallRunLayers);
           /* Debug.DrawRay(myTransform.position,
            -myTransform.up,Color.white,10.0f);*/

        }else{
            frontW = Physics.Raycast(myTransform.position,
                                       myTransform.forward, out frontH, capsule.radius + 0.2f, wallRunLayers);
          /*  Debug.DrawRay (myTransform.position,
             myTransform.forward, Color.black,10.0f);*/

        }
    

   /* Debug.DrawLine(myTransform.position,
                        myTransform.position + (-myTransform.right).normalized * (capsule.radius + 0.2f));

        Debug.DrawLine(myTransform.position,
                        myTransform.position + (myTransform.right).normalized * (capsule.radius + 0.2f));*/

      
       

        Vector3 tangVect = Vector3.zero, normal = Vector3.zero;
       
        if (!animator.animator.IsInTransition(0) && !_rb.isKinematic)
        {

           
            if (leftW)
            {
               
                normal = leftH.normal;
                tangVect = Vector3.Cross(leftH.normal, Vector3.up);
                myTransform.rotation = Quaternion.LookRotation(tangVect);
                //tangVect = Vector3.Project(movement,tangVect).normalized;
                wallRunVelocity = tangVect * velocity.magnitude;
                if (!(characterState == CharacterState.WallRunning))
                {
                    StartJetPack();
                    wallState = WallState.WallL;
                    characterState = CharacterState.WallRunning;
                    state = characterState;
                    //animator.SetBool("WallRunL", true);
                    WallRunCoolDown();
                }

                if (state == CharacterState.Jumping || state == CharacterState.DoubleJump)
                {
                   // _rb.velocity = myTransform.up * movement.y + WallJumpDirection(leftH.normal) * movement.y;
                      StartCoroutine(FromWallJump( WallJumpDirection(leftH.normal) ));
                   
                }
            }

            else if (rightW)
            {
            
                normal = rightH.normal;
                tangVect = -Vector3.Cross(rightH.normal, Vector3.up);
                myTransform.rotation = Quaternion.LookRotation(tangVect);
                //tangVect = Vector3.Project(movement,tangVect).normalized;
                wallRunVelocity = tangVect * velocity.magnitude;
                if (!(characterState == CharacterState.WallRunning))
                {
                    StartJetPack();
                    wallState = WallState.WallR;
                    characterState = CharacterState.WallRunning;
                    state = characterState;
                    WallRunCoolDown();
                }

                if (state == CharacterState.Jumping || state == CharacterState.DoubleJump)
                {
                  //  _rb.velocity = myTransform.up * movement.y + WallJumpDirection(rightH.normal) * movement.y;
                         StartCoroutine(FromWallJump( WallJumpDirection(rightH.normal)));
                
                   
                }
            }

            else if (frontW)
            {
                
                normal = frontH.normal;

                Vector3 forwardonWall = Vector3.Cross( myTransform.right,normal);

             

                tangVect = frontH.normal * -1;

            

                myTransform.rotation = Quaternion.LookRotation(forwardonWall, normal);
             
                wallRunVelocity = myTransform.forward * velocity.magnitude; 
                if (!(characterState == CharacterState.WallRunning))
                {
                    StartJetPack();
                    wallState = WallState.WallF;
                    characterState = CharacterState.WallRunning;
                    state = characterState;
                    WallRunCoolDown();
                }

                if (state == CharacterState.Jumping || state == CharacterState.DoubleJump)
                {
                   /// _rb.velocity = (myTransform.up + WallJumpDirection(myTransform.forward * -1)).normalized * movement.y;
                     StartCoroutine(FromWallJump((myTransform.up + WallJumpDirection(myTransform.forward * -1)).normalized));
                    
                }
            }
            else
            {
                if (characterState == CharacterState.WallRunning)
                {
                 
                    characterState = CharacterState.Jumping;
                    lastTimeOnWall = Time.time;
                    jetPackEnable = false;
                    //Debug.Log("nOhIT");
                }
                wallState = WallState.WallNone;
            }
            float angle = Mathf.Abs(Vector3.Dot(normal, Vector3.up));

          
         
            //Debug.Log(forwardRotation);
            // Debug.DrawRay(myTransform.position,forwardRotation,Color.green);
            //animator.WallAnimation(leftW,rightW,frontW);


            return leftW || rightW || frontW;
        }
        wallState = WallState.WallNone;
        return false;

    }

    public float jumpPulseVertical =5.0f;

    public float AfterJumpWallRunDelay=1.0f;

    public float jumpPulseHorizontal = 5.0f;
    protected  Vector3 Jump( )
    {
        if (animator != null)
        {
            animator.ApllyJump(true);
            //звук прыжка
            sControl.playClip(jumpSound);
        }
        AchievementManager.instance.UnEvnetAchive(ParamLibrary.PARAM_JUMP, 1.0f);
        lastJumpTime = Time.time;
        //photonView.RPC("JumpChange",PhotonTargets.OthersBuffered,true);
        return _Jump((Vector3.up + myTransform.forward).normalized);
       
    }

    public Vector3 _Jump(Vector3 direction){
  

        if (Time.time - timeGrounded < timeForImpulseJump)
        {

            return JumpAssist.FlatAdd(direction, velocity, jumpPulseHorizontal) + Vector3.up * direction.y * jumpPulseVertical;
        }
        else
        {
            Vector3 resultSpeed = JumpAssist.FlatAdd(direction, velocity) + Vector3.up * direction.y * jumpPulseVertical;
            Vector3 simpelSpeed = Vector3.up * direction.y * jumpPulseVertical + JumpAssist.GetMeFlat(direction) * jumpPulseHorizontal;
            if (resultSpeed.sqrMagnitude < simpelSpeed.sqrMagnitude)
            {
                return simpelSpeed;
            }
            else
            {
                return resultSpeed;
            }
         
        }
       
    }
    protected IEnumerator FromWallJump(Vector3 direction)
    {
        wallRunVelocity = _Jump(direction);
        SendMessage("WallJumpMessage", SendMessageOptions.DontRequireReceiver);
        //Debug.Log ("WALLJUMP");
        _canWallRun = false;
        jetPackEnable = false;
        characterState = CharacterState.Jumping;
        yield return new WaitForSeconds(AfterJumpWallRunDelay);
        _canWallRun = true;
    }


   
    public virtual void JumpEnd(CharacterState nextState)
    {
        if (nextState == CharacterState.Jumping)
        {
            characterState = CharacterState.Idle;
        }
        else
        {
            characterState = nextState;
        }
        Land();
    }
    public override   void  Land(){
        timeGrounded = Time.time;
        velocity.y = 0;
    }
    public override void ChangeDefaultWeapon(int myId)
    {

    }
    public override Vector3 getAimpointForCamera()
    {

        return myTransform.position + Quaternion.Euler(myTransform.eulerAngles.x, 0, myTransform.eulerAngles.z) * headOffset + desiredRotation * Quaternion.Euler(myTransform.eulerAngles.x, 0, 0) * Vector3.forward * aimRange;
    }

    public override Quaternion GetDesireRotation()
    {
        return desiredRotation * Quaternion.Euler(myTransform.eulerAngles.x, 0, myTransform.eulerAngles.z);
    }
}

public static class  JumpAssist{

    public static  Vector3 GetMeFlat(Vector3 vect)
    {
        vect.y = 0;
        return vect;
    }
    public static Vector3 FlatAdd(Vector3 direction, Vector3 velocity, float addd = 0.0f)
    {
       return GetMeFlat(direction).normalized *(GetMeFlat(velocity).magnitude+addd);
    }
}