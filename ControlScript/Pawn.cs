using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using nstuff.juggerfall.extension.models;
using Sfs2X.Entities.Data;




public enum CharacterState
{
    Idle = 0,
    Walking = 1,
    Running = 2,
    Sprinting = 3,

    Jumping = 4,
    WallRunning = 5,
    PullingUp = 6,
    DoubleJump = 7,
    DeActivate = 8,
    Activate = 9,
    Dead = 10,
}
public enum WallState
{
    WallL,
    WallR,
    WallF
}

public class singleDPS
{
    public BaseDamage damage;
    public GameObject killer;
    public float lastTime = 1.0f;
    public float showInterval = 1.0f;
	//how long fire damage without signal from particleCollision
	public float damageWithouSignalDelay= 1.0f;
    public bool noOnwer = false;

}
public class DamagerEntry
{
    public float forgetTime;
    public float amount = 0f;
    public Pawn pawn;
    public Vector3 lastHitDirection;
    public DamagerEntry(Pawn initPawn)
    {
        pawn = initPawn;
    }

}

public class Pawn : DamagebleObject
{



    public List<singleDPS> activeDPS = new List<singleDPS>();

    public const int SYNC_MULTUPLIER = 5;

    public const float ASSIT_FORGET_TIME = 5.0f;

    private LayerMask groundLayers = 1;
    private LayerMask wallRunLayers = 1;
    private LayerMask climbLayers = 1 << 9; // Layer 9
    public LayerMask seenlist = 1;
    public bool isActive = false;

    //If this spawn pre defained by game designer we don't want to start in on start but on AIDirector start so set this to false;

    public bool isSpawned = true;
	
	public bool isSpawnImortality = false;
	
	public float spawnImortalityDuration= 3.0f;
    //Weapon that in hand
    public BaseWeapon CurWeapon;

    public Transform weaponSlot;

    //Nautaral weapons like hand or claws

    public WeaponOfExtremities naturalWeapon;

    public List<HTHHitter> AttackType = new List<HTHHitter>();

    public Transform myTransform;

    protected Vector3 correctPlayerPos = Vector3.zero; //We lerp towards this

    protected Quaternion correctPlayerRot = Quaternion.identity; //We lerp towards this

    protected float weaponOffset;

    public Vector3 weaponRotatorOffset;

    public bool isDead = false;

    public string publicName;

    protected Vector3 aimRotation;

    public Vector3 aiAimRotation;

    public static float aimRange = 1000.0f;

    //rotation for moment when rotation of camera and pawn can be different e.t.c wall run	
    protected Vector3 forwardRotation;

    public AnimationManager animator;

    private CharacterState _characterState;

    protected CharacterState characterState
    {

        get
        {
            return _characterState;
        }


        set
        {

            _characterState = value;

        }

    }

    private WallState wallState;

    protected CharacterState nextState;

    private float jetPackCharge;

    protected bool jetPackEnable = false;

    public float jetPackFlyFuelConsumption = 1.0f;

    public float jetPackForwardWallRunFuelConsumption = 0.5f;

    public float jetPackWallRunFuelConsumption = 0.5f;

    public float jetPackDefaultFuelConsumption = 0.5f;

    protected Vector3 nextMovement;

    public PlayerCamera cameraController;

    public AIBase mainAi;

    public bool isAi;

    public Pawn enemy;

    protected Rigidbody _rb;

    protected Vector3 pushingForce;

    private const float FORCE_MULIPLIER = 10.0f;

    protected float distToGround;

    public float stepHeight;

    public bool isCheckSteps = false;

    public LayerMask floorLayer;

    protected float size;

    protected CapsuleCollider capsule;

    public bool canWallRun;

    protected bool _canWallRun;

    protected bool canMove = true;

    public bool canPullUp;

    public bool canJump;

    public bool canBeKnockOut;

    private bool _knockOut;

    public float KnockOutTimeOut = 3.0f;

    public float wallRunSpeed;

    public float aiVelocityChangeMax = 1.0f;

    public float flyForwardSpeed;

    public float groundSprintSpeed;

    public float groundRunSpeed;

    public float flySpeed;

    public float groundWalkSpeed;

    public float jumpHeight;

    public float climbSpeed;

    public float climbCheckRadius = 0.1f;

    public float climbCheckDistance = 0.5f;

    public float heightOffsetToEdge = 2.0f;

    public float PullUpStartTimer = 0.0f;

    public float PullUpTime = 2.0f;

    public Transform curLookTarget = null;

    public Player player = null;

    public int team;

    private List<Pawn> seenPawns = new List<Pawn>();

    public float seenDistance;

    protected Collider myCollider;

    private bool _isGrounded;

    public float distanceToGround; // Проверка дистанции до земли (+)

    public bool isGrounded
    {

        get
        {
            return _isGrounded;
        }


        set
        {
            if (_isGrounded != value && value)
            {
                SendMessage("DidLand", SendMessageOptions.DontRequireReceiver);

            }

            _isGrounded = value;

            RaycastHit hitGround; // Луч (+)

            switch (characterState) // Если прыжок проверить растояние (+)
            {
                case CharacterState.Jumping:
                case CharacterState.DoubleJump:
                    if (Physics.Raycast(transform.position, -Vector3.up, out hitGround))
                    {
                        distanceToGround = hitGround.distance;
                        if (distanceToGround < 0.35 + GetComponent<CapsuleCollider>().height / 2)
                        {
                            animator.animator.SetBool("DistanceJump", false);
                        }
                        else
                        {
                            animator.animator.SetBool("DistanceJump", true);
                        }
                    }
                    else
                    {
                        animator.animator.SetBool("DistanceJump", true);
                    }
                    break;
            }
        }


    }
    protected float lastTimeOnWall;

    private float lastJumpTime;

    private Vector3 floorNormal;

    public Vector3 centerOffset;

    public Vector3 headOffset;

    public float headOddsetFloat = 0.5f;

    public static float gravity = 20.0f;

    public InventoryManager ivnMan;

    public bool isAiming = false;
	
	public bool isFps= false;

    public float aimModCoef = -10.0f;

    public bool isLookingAt = true;

    public bool initialReplication = true;

    public class BasePawnStatistic
    {
        //Shoot Counter
        public int shootCnt = 0;

    };

    protected CharacteristicManager charMan;
    public SkillManager skillManager;
    public BasePawnStatistic statistic = new BasePawnStatistic();
    //effects

	public PawnEffectController effectController;
	


    //звуки

    private AudioSource aSource;
    public AudioClip stepSound;
    public AudioClip jumpSound;
    public AudioClip spawnSound;
    public AudioClip[] painSoundsArray;//массив звуков воплей при попадании
    private float lastPainSound = -10.0f;
    protected soundControl sControl;//глобальный обьект контроллера звука

    protected bool isSpawn = false;//флаг респавна


    //FOR killcamera size offset; Like robot always big;

    public bool bigTarget = false;

    //Visual Components

    public PawnOnGuiComponent guiComponent;

    public string tauntAnimation = "";

    public float maxStandRotate = 60.0f;

    //AssistSection


    protected List<DamagerEntry> damagers = new List<DamagerEntry>();

    public List<Pawn> attackers = new List<Pawn>();

    protected Vector3 lastHitDirection = Vector3.zero;

    //Serialization

    PawnModel serPawn = new PawnModel();

    protected Vector3 replicatedVelocity;

    //DYNAMIC OCLUSION SECTION
    public float lastNetUpdate = 0;

    private bool isLocalVisible = true;

    public static float MAXRENDERDISTANCE = 250000.0f;

    //Some special event handler(like damage on boss that we mast send to server)

    public SpecialEventsHandler eventHandler;
    public float timeSpawned;

    protected void Awake()
    {
        myTransform = transform;
        ivnMan = GetComponent<InventoryManager>();
        _rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
        myCollider = collider;
        foxView = GetComponent<FoxView>();
        animator = transform.GetComponentInChildren<AnimationManager>();
        PlayerManager.instance.addPawn(this);
        aSource = GetComponent<AudioSource>();
        if (!isSpawned)
        {
            correctPlayerPos = transform.position;
        }
        effectController = GetComponentInChildren<PawnEffectController>();
        charMan = GetComponent<CharacteristicManager>();
        charMan.Init();
        skillManager = GetComponent<SkillManager>();
        sControl = new soundControl(aSource);//создаем обьект контроллера звука
        weaponRenderer = GetComponentInChildren<Renderer>();
        if (guiComponent != null)
        {
            guiComponent.baseTitle = publicName;
            guiComponent.SetPawn(this);
        }
        desiredRotation = Quaternion.LookRotation(myTransform.forward);
        xAngle = desiredRotation.eulerAngles.x;
        yAngle = desiredRotation.eulerAngles.y;
        Invoke("SpawnImmortalityEnd", spawnImortalityDuration);
        timeSpawned = Time.time;
    }
	public void SpawnImmortalityEnd(){
		isSpawnImortality= false;
	}

    // Use this for initialization
    protected void Start()
    {
        if (isSpawned || !foxView.isMine)
        {
            StartPawn();
        }

    }

    public virtual void StartPawn()
    {
        //Debug.Log("Start PAwn");

        _canWallRun = canWallRun;
        //проигрываем звук респавна
        sControl.playClip(spawnSound);
        isActive = true;
        if (effectController!=null&&effectController.IsSpawn())
        {
        
            isSpawn = true;//отключаем движения и повреждения
        }

        if (!foxView.isMine)
        {

            Destroy(GetComponent<ThirdPersonController>());
            Destroy(GetComponent<PlayerCamera>());

            GetComponent<Rigidbody>().isKinematic = true;
            //ivnMan.Init ();
        }
        else
        {
           
            cameraController = GetComponent<PlayerCamera>();
            isAi = cameraController == null;
            SetCurWeaponFov();
        }
        naturalWeapon = GetComponent<WeaponOfExtremities>();
        mainAi = GetComponent<AIBase>();

        isAi = mainAi != null;
        if (isAi)
        {
            if (foxView.isMine)
            {
                mainAi.StartAI();


            }
        }
        GetSize();

        correctPlayerPos = transform.position;

        ivnMan.Init();
        centerOffset = capsule.bounds.center - myTransform.position;
        headOffset = centerOffset;
        headOffset.y = capsule.bounds.max.y - myTransform.position.y- headOddsetFloat;

        distToGround = capsule.height / 2 - capsule.center.y;

        health = GetMaxHealth();
        SpeedInit();
        if (canJump)
        {
            jetPackCharge = charMan.GetFloatChar(CharacteristicList.JETPACKCHARGE);
        }
        else
        {
            jetPackCharge = 0;
        }

        if (isAi)
        {
            ivnMan.Init();
            AfterSpawnAction();
        }
        //Debug.Log (distToGround);

    }
    //INIT PLAYER PAWN TO TAKE CHARACTERISTICK
    public void Init()
    {
        charMan.AddList(player.GetCharacteristick());
        if (foxView.isMine && guiComponent!=null)
        {
            Destroy(guiComponent.gameObject);
        }

    }
	public void AddExternalCharacteristic(List<CharacteristicToAdd> effects){
		charMan.AddList(effects);		
	}
    public int GetMaxHealth()
    {
        return (int)(((float)charMan.GetIntChar(CharacteristicList.MAXHEALTH)) * GetPercentValue(CharacteristicList.MAXHEALTH_PERCENT));

    }
    public void SpeedInit()
    {
        Debug.Log("speed mod" + GetPercentValue(CharacteristicList.SPEED));
        wallRunSpeed = wallRunSpeed * GetPercentValue(CharacteristicList.SPEED);

        groundSprintSpeed = groundSprintSpeed * GetPercentValue(CharacteristicList.SPEED);

        flyForwardSpeed = flyForwardSpeed * GetPercentValue(CharacteristicList.SPEED);

        groundRunSpeed = groundRunSpeed * GetPercentValue(CharacteristicList.SPEED);

        flySpeed = flySpeed * GetPercentValue(CharacteristicList.SPEED);

        groundWalkSpeed = groundWalkSpeed * GetPercentValue(CharacteristicList.SPEED);

        jumpHeight = jumpHeight * GetPercentValue(CharacteristicList.JUMPHEIGHT);
    }
    public float GetSize()
    {
        if (size == 0)
        {
            size = Mathf.Sqrt(capsule.height * capsule.height + capsule.radius * capsule.radius);
        }
        return size;
    }
    public Collider GetCollider()
    {
        return myCollider;
    }
    public Vector3 GetVelocity()
    {
        if (foxView.isMine)
        {
            return _rb.velocity;
        }
        else
        {
            return replicatedVelocity;
        }
    }
    public virtual void AfterSpawnAction()
    {
        ivnMan.GenerateWeaponStart();

    }
    public virtual void ChangeDefaultWeapon(int myId)
    {
        WeaponIndex idPersonal = Choice._Personal[myId],
        idMain = Choice._Main[myId],
        idExtra = Choice._Extra[myId],
        idTaunt = Choice._Taunt[myId];
        ivnMan.Init();
        if (!idPersonal.IsSameIndex(WeaponIndex.Zero))
        {
            ivnMan.SetSlot(ItemManager.instance.GetWeaponprefabByID(idPersonal));
        }
        if (!idMain.IsSameIndex(WeaponIndex.Zero))
        {
            //Debug.Log(ItemManager.instance.weaponPrefabsListbyId[idMain]);
            ivnMan.SetSlot(ItemManager.instance.GetWeaponprefabByID(idMain));
        }
        if (!idExtra.IsSameIndex(WeaponIndex.Zero))
        {
            ivnMan.SetSlot(ItemManager.instance.GetWeaponprefabByID(idExtra));
        }
        ivnMan.GenerateWeaponStart();
        if (!idTaunt.IsSameIndex(WeaponIndex.Zero))
        {
            tauntAnimation = ItemManager.instance.animsIndexTable[idTaunt.prefabId].animationId;
        }
    }
    public override void Damage(BaseDamage damage, GameObject killer)
    {
        if (isSpawn || killer == null || !isActive)
        {//если только респавнились, то повреждений не получаем
            return;
        }


        float allPrecent = GetValue(CharacteristicList.DAMAGE_REDUCE_ALL);
        if (damage.weapon)
        {
            allPrecent += GetValue(CharacteristicList.DAMAGE_REDUCE_GUN);
        }
        if (damage.splash)
        {
            allPrecent += GetValue(CharacteristicList.DAMAGE_REDUCE_SPLASH);
        }
        damage.Damage = damage.Damage * (((float)allPrecent) / 100f + 1f);
        bool isVs = (damage.isVsArmor && charMan.GetBoolChar(CharacteristicList.ARMOR)) || (!damage.isVsArmor && !charMan.GetBoolChar(CharacteristicList.ARMOR));
        if (!isVs)
        {
            damage.Damage *= 0.5f;
        }

        //вопли при попадании
        //выбираются случайно из массива. Звучат не прерываясь при следующем вызове
        if (lastPainSound + 1.0f < Time.time && painSoundsArray.Length > 0)
        {
            lastPainSound = Time.time;
            sControl.playClipsRandom(painSoundsArray);

        }


        Pawn killerPawn = killer.GetComponent<Pawn>();
        if (killerPawn != null && killerPawn.team != 0 && killerPawn.team == team && !PlayerManager.instance.frendlyFire && killerPawn != this)
        {

            return;
        }
        if (killerPawn != null)
        {
            Player killerPlayer = killerPawn.player;
            if (killerPlayer != null && killerPawn != this)
            {
                //Debug.Log ("DAMAGE" +damage.sendMessage);
                killerPlayer.DamagePawn(damage);
            }
        }
       
        if (foxView.isMine)
        {

            float forcePush = charMan.GetFloatChar(CharacteristicList.STANDFORCE);
            ///Debug.Log(forcePush +" "+damage.pushForce);
            forcePush = damage.pushForce - forcePush;
            //Debug.Log(forcePush);
            if (forcePush > 0)
            {
                damage.pushDirection.y = 0;
                AddPushForce(forcePush * damage.pushDirection * FORCE_MULIPLIER);


            }
            if (killerPawn != null)
            {


                DamagerEntry entry = damagers.Find(delegate(DamagerEntry searchentry) { return searchentry.pawn == killerPawn; });

                if (entry == null)
                {
                    entry = new DamagerEntry(killerPawn);
                    damagers.Add(entry);
                }
                entry.forgetTime = Time.time + ASSIT_FORGET_TIME;
                entry.amount += damage.Damage;
                entry.lastHitDirection = damage.pushDirection;
            }
            if (isAi)
            {
                mainAi.WasHitBy(killer, damage.Damage);

            }
            if (canBeKnockOut)
            {
                if (damage.knockOut)
                {
                    StartCoroutine(KnockOut());
                }

            }
            if (eventHandler != null)
            {
                eventHandler.Damage(killer, damage.Damage);
            }
        }
        else
        {
            lastHitDirection = damage.pushDirection;
           
        }
        if (killerPawn == null && foxView.isMine)
        {
			if (damage.sendMessage)
			{
				AddEffect(damage.hitPosition,damage.pushDirection ,damage.type);
			}
			if(CanBeDamaged()){
				base.Damage(damage, killer);
			}
        }
        if (killerPawn != null && killerPawn.foxView.isMine)
        {
            if (foxView.isMine)
            {
   			    if (damage.sendMessage)
				{
					AddEffect(damage.hitPosition,damage.pushDirection ,damage.type);
				}
				if(CanBeDamaged()){
					base.Damage(damage, killer);
				}
            }
            else
            {
                foxView.LowerHPRequest(damage, killerPawn.foxView.viewID);
            }
          
        }
       
        //Debug.Log ("DAMAGE");
       
    }
    //For network purpose 
    public void LowerHealth(BaseDamageModel damageModel, GameObject killer)
    {
        BaseDamage damage = damageModel.GetDamage();
        Pawn killerPawn = killer.GetComponent<Pawn>();
        if (killerPawn != null)
        {


            DamagerEntry entry = damagers.Find(delegate(DamagerEntry searchentry) { return searchentry.pawn == killerPawn; });

            if (entry == null)
            {
                entry = new DamagerEntry(killerPawn);
                damagers.Add(entry);
            }
            entry.forgetTime = Time.time + ASSIT_FORGET_TIME;
            entry.amount += damage.Damage;
            entry.lastHitDirection = damage.pushDirection;
        }
        if (isAi)
        {
            mainAi.WasHitBy(killer, damage.Damage);

        }
        if (canBeKnockOut)
        {
            if (damage.knockOut)
            {
                StartCoroutine(KnockOut());
            }

        }
        if (eventHandler != null)
        {
            eventHandler.Damage(killer, damage.Damage);
        }
        //Debug.Log ("DAMAGE");
		if (damage.sendMessage)
		{
			AddEffect(damage.hitPosition,damage.pushDirection ,damage.type);
		}
		if(CanBeDamaged()){
			base.Damage(damage, killer);
		}
    }
	public virtual bool CanBeDamaged(){
		return 	!isSpawnImortality;
	}

    public void Heal(float damage, GameObject Healler)
    {
        if (isDead)
        {
            return;
        }
        int maxHealth = GetMaxHealth();
        health += damage;
        if (maxHealth < health)
        {
            health = maxHealth;
        }

    }

    public void OverHeal(float hp, float maxModifier)
    {
        if (isDead)
        {
            return;
        }
        int maxHealth = (int)(((float)GetMaxHealth()) * maxModifier);
        health += hp;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }

    public bool IsMaxHP()
    {
        int maxHealth = GetMaxHealth();
        return maxHealth <= health;
    }
    public void SetTeam(int team)
    {
        this.team = team;
    }
    public override void KillIt(GameObject killer)
    {
        if (isDead)
        {
            return;
        }
        try
        {


            isDead = true;

            //StartCoroutine (CoroutineRequestKillMe ());
            Pawn killerPawn = null;
            Player killerPlayer = null;
            int killerID = -1;
            if (killer != null)
            {
                killerPawn = killer.GetComponent<Pawn>();

                if (killerPawn != null)
                {
                    killerPlayer = killerPawn.player;
                    if (killerPlayer != null)
                    {
                        killerID = killerPlayer.playerView.GetId();
                        if (foxView.isMine && killerPawn.foxView.isMine)
                        {
                            RobotPawn robot = this as RobotPawn;
                            if (robot != null)
                            {
                                killerPlayer.JuggerKill(this, player, myTransform.position, killInfo);
                            }
                            else
                            {
                                killerPlayer.PawnKill(this, player, myTransform.position, killInfo);
                            }
                        }
                    }
                }

            }

            foxView.PawnDiedByKill(killerID);

            if (player != null)
            {
                if (player.GetRobot() == this)
                {
                    player.RobotDead(killerPlayer);
                }
                else
                {
                    player.PawnDead(killerPlayer, killerPawn, killInfo);
                }
                GA.API.Design.NewEvent("Game:PawnDead:Player:" + killInfo.weaponId);
                GA.API.Design.NewEvent("Game:Lifetime:Player", Time.time - timeSpawned);
            }
            else
            {
                GA.API.Design.NewEvent("Game:PawnDead:AI:" + killInfo.weaponId);
                GA.API.Design.NewEvent("Game:Lifetime:AI", Time.time - timeSpawned);
            }

        }
        catch (Exception e)
        {
            Debug.Log("ErrorExeption in Pawn KillIt" + e);
        }
        finally
        {
            PawnKill();
        }


    }
    /// <summary>
    /// Sort and return last damager that hit pawn
    /// </summary>
    protected DamagerEntry RetrunLastDamager()
    {
        damagers.Sort(delegate(DamagerEntry x, DamagerEntry y)
        {
            return x.forgetTime.CompareTo(y.forgetTime) * -1;
        });
        if (damagers.Count > 0)
        {
            return damagers[0];
        }
        else
        {
            return null;
        }

    }
    public void PawnKill()
    {
        if (isAi && mainAi != null)
        {
            mainAi.Death();
        }

        ActualKillMe();
    }

    protected override void ActualKillMe()
    {
        AITargetManager.DeadPawn(this);
        StartCoroutine(AfterAnimKill());
        StopFire();
        if (!foxView.isMine && player!=null)
        {
            player.Score.Death++;
        }

        isDead = true;
        health = 0;
        characterState = CharacterState.Dead;
        DamagerEntry last = RetrunLastDamager();
        StopKick();
        //Debug.Log(last);
        if (last == null)
        {
            if (animator != null)
            {
                animator.StartDeath(AnimDirection.Front);
            }
        }
        else
        {
            float angle = Vector3.Dot(last.lastHitDirection, myTransform.forward);
            // If last hit direction equals negative forward it's hit in face

            if (angle <= 0.0f)
            {
                animator.StartDeath(AnimDirection.Front);
            }
            else
            {
                animator.StartDeath(AnimDirection.Back);
            }
        }
        if (cameraController != null)
        {
            cameraController.enabled = false;
            GetComponent<ThirdPersonController>().enabled = false;
        }

    
       
    }

    public IEnumerator AfterAnimKill()
    {
        yield return new WaitForSeconds(8f);
        Destroy(gameObject);
    }
    //EFFECCT SECTION
    void AddEffect(Vector3 position,Vector3 direction,DamageType type)
    {
        if (player != null)
        {

            player.ShowDamageIndicator(-direction);
        }
		if(isFps){
			return;
		}
        if (effectController != null)
        {
            effectController.DamageEffect(type,position,direction);
        }
		
    }


    //END OF EFFECT SECTIOn

    // Update is called once per frame

    protected void UpdateSeenList()
    {
        List<Pawn> allPawn = PlayerManager.instance.FindAllPawn();
        seenPawns.Clear();

        for (int i = 0; i < allPawn.Count; i++)
        {
            if (allPawn[i] == null)
            {
                continue;
            }
            if (allPawn[i] == this)
            {
                continue;
            }
            Vector3 distance = (allPawn[i].myTransform.position - myTransform.position);

            if (distance.sqrMagnitude < seenDistance)
            {
                RaycastHit hitInfo;
                Vector3 normalDist = distance.normalized;
                Vector3 startpoint = myTransform.position + normalDist * Mathf.Max(capsule.radius, capsule.height);
                if (isAi)
                {
                    ///Debug.DrawRay(startpoint, normalDist * distance.magnitude);
                }
                float distanceMag = distance.magnitude;
                if (allPawn[i].team != team && Physics.Raycast(startpoint, normalDist, out hitInfo, distanceMag, seenlist))
                {


                    if (allPawn[i].myCollider != hitInfo.collider)
                    {

                        if (!isAi && foxView.isMine && player != null)
                        {
                            allPawn[i].LocalPlayerSeeMe(distanceMag,team, false);
                        }
                        continue;
                    }
                }

                if (!isAi && foxView.isMine && player != null)
                {
                    allPawn[i].LocalPlayerSeeMe(distanceMag,team, true);

                }
                //   Debug.Log(this + "  " + allPawn[i]);
                seenPawns.Add(allPawn[i]);
            }
            else
            {
                if (!isAi&&foxView.isMine && player != null)
                {
                    allPawn[i].LocalPlayerSeeMe(seenDistance,team, false);
                }
            }
        }
        //If we already do something slow)) lets clean up damagers lists
        damagers.RemoveAll(delegate(DamagerEntry v)
        {
            return v.forgetTime < Time.time;
        });

    }
    protected virtual void LocalPlayerSeeMe(float distance,int team, bool state)
    {
        if (guiComponent != null)
        {
            guiComponent.LocalPlayerSeeMe(distance, team, state);
        }
    }
    protected virtual void UpdateAnimator()
    {
        float strafe = 0;
        //Debug.Log (strafe);	
        float speed = 0;
        if (isDead)
        {
            return;
        }

        //Debug.Log (speed);
        if (animator != null && animator.gameObject.activeSelf)
        {
            if (foxView.isMine)
            {


                strafe = CalculateStarfe();
                //Debug.Log(characterState);
                speed = CalculateSpeed();
                //Debug.Log(speed);	
                switch (characterState)
                {
                    case CharacterState.Jumping:
                        animator.ApllyJump(true);
                        break;
                    case CharacterState.DoubleJump:
                        animator.ApllyJump(true);
                        animator.DoubleJump();
                        break;
                    case CharacterState.Idle:
                        if (isGrounded)
                        {
                            animator.ApllyJump(false);
                        }
                        //проигрываем звук шагов

                        animator.ApllyMotion(0.0f, speed, strafe);
                        break;
                    case CharacterState.Running:
                        animator.ApllyJump(false);
                        sControl.playFullClip(stepSound);
                        animator.ApllyMotion(2.0f, speed, strafe);
                        break;
                    case CharacterState.Sprinting:
                        animator.Sprint();
                        sControl.playFullClip(stepSound);
                        animator.ApllyMotion(2.0f, speed, strafe);
                        break;
                    case CharacterState.Walking:
                        animator.ApllyJump(false);
                        sControl.playFullClip(stepSound);
                        animator.ApllyMotion(1.0f, speed, strafe);
                        break;
                    case CharacterState.WallRunning:
                        sControl.playFullClip(stepSound);
                        //Debug.Log ("INSWITCH");
                        switch (wallState)
                        {
                            case WallState.WallF:
                                animator.WallAnimation(false, false, true);
                                break;
                            case WallState.WallR:
                                animator.WallAnimation(false, true, false);
                                break;
                            case WallState.WallL:
                                animator.WallAnimation(true, false, false);
                                break;
                        }

                        break;
                }
                if (isLookingAt)
                {
                    Vector3 laimRotation;
                    if (isAi)
                    {
                        laimRotation = aimRotation; 
                    }
                    else
                    {
                        laimRotation = myTransform.position + desiredRotation * Vector3.forward * aimRange;
                    }
                 
                    /*if(animator.isWeaponAimable()){
                        Quaternion diference = Quaternion.FromToRotation(CurWeapon.muzzlePoint.forward,myTransform.forward);

                        Vector3 direction= laimRotation-myTransform.position;
				
                        laimRotation =(diference *direction.normalized)*direction.magnitude +myTransform.position;
                    }*/

                    animator.SetLookAtPosition(laimRotation);
                    //animator.animator.SetLookAtWeight (1, 0.5f, 0.7f, 0.0f, 0.5f);

                }
                //
            }
            else
            {
                strafe = CalculateRepStarfe();
                //Debug.Log (strafe);	
                speed = CalculateRepSpeed();

                switch (nextState)
                {
                    case CharacterState.Idle:
                        animator.ApllyJump(false);
                        animator.ApllyMotion(0.0f, speed, strafe);
                        break;
                    case CharacterState.Running:
                        animator.ApllyJump(false);
                        sControl.playFullClip(stepSound);
                        animator.ApllyMotion(2.0f, speed, strafe);
                        break;
                    case CharacterState.Sprinting:
                        if (characterState == CharacterState.Jumping)
                        {
                            animator.ApllyJump(false);
                        }
                        animator.Sprint();
                        sControl.playFullClip(stepSound);
                        animator.ApllyMotion(2.0f, speed, strafe);
                        break;
                    case CharacterState.Walking:
                        animator.ApllyJump(false);
                        sControl.playFullClip(stepSound);
                        animator.ApllyMotion(1.0f, speed, strafe);
                        break;
                    case CharacterState.WallRunning:
                        sControl.playFullClip(stepSound);
                        switch (wallState)
                        {
                            case WallState.WallF:
                                animator.WallAnimation(false, false, true);
                                break;
                            case WallState.WallR:
                                animator.WallAnimation(false, true, false);
                                break;
                            case WallState.WallL:
                                animator.WallAnimation(true, false, false);
                                break;
                        }

                        break;
                    case CharacterState.Jumping:
                        animator.ApllyJump(true);

                        if (characterState == CharacterState.WallRunning)
                        {
                            animator.WallAnimation(false, false, false);
                            animator.FreeFall();

                        }
                        break;
                    case CharacterState.DoubleJump:
                        animator.ApllyJump(true);
                        animator.DoubleJump();
                        if (characterState == CharacterState.WallRunning)
                        {
                            animator.WallAnimation(false, false, false);
                        }
                        break;
                    case CharacterState.PullingUp:
                        if (characterState != CharacterState.PullingUp)
                        {
                            animator.WallAnimation(false, false, false);
                            animator.FreeFall();
                            StartCoroutine("PullUpEnd", PullUpTime);
                            animator.StartPullingUp();
                        }
                        break;
                    case CharacterState.Dead:
                       
                        if (characterState != CharacterState.Dead)
                        {
                            float angle = Vector3.Dot(lastHitDirection, myTransform.forward);
                            // If last hit direction equals negative forward it's hit in face
                            if (angle <= 0)
                            {
                                animator.StartDeath(AnimDirection.Front);
                            }
                            else
                            {
                                animator.StartDeath(AnimDirection.Back);
                            }

                        }
                        return;
                }
                characterState = nextState;
                if (isLookingAt)
                {
                    Vector3 laimRotation = aimRotation;
                    /*if(animator.isWeaponAimable()){
                        Quaternion diference = Quaternion.FromToRotation(CurWeapon.muzzlePoint.forward,myTransform.forward);

                        Vector3 direction= laimRotation-myTransform.position;
				
                        laimRotation =(diference *direction.normalized)*direction.magnitude +myTransform.position;
                    }*/

                    animator.SetLookAtPosition(laimRotation);
                    //animator.animator.SetLookAtWeight (1, 0.5f, 0.7f, 0.0f, 0.5f);

                }
            }
         
        }

    }
    protected void LateUpdate()
    {


    }

    public void ProgressEdit()
    {
        if (guiComponent!=null)
        {
            guiComponent.UpdateGui();
        }
    }



    protected void Update()
    {

        //Debug.Log (photonView.isSceneView);

        ProgressEdit();
        if (!isActive && !foxView.isMine)
        {
            //replicate position to get rid off teleportation after bot is dead			
            ReplicatePosition();
            return;
        }

        if (isSpawn)
        {//если респавн

            if (effectController != null && effectController.IsSpawn())
            {//если все партиклы кончились
                isSpawn = false;//то освобождаем все движения и повреждения
            }
        }


        if (foxView.isMine)
        {

            UpdateSeenList();
            if (jetPackEnable == false)
            {
                float maxCharge = charMan.GetFloatChar(CharacteristicList.JETPACKCHARGE);
                if (canJump && jetPackCharge < maxCharge)
                {

                    jetPackCharge += Time.deltaTime;

                    if (jetPackCharge > maxCharge)
                    {
                        jetPackCharge = maxCharge;
                    }

                }
            }
            else
            {
                if (jetPackCharge > 0.0f)
                {
                    switch (characterState)
                    {
                        case CharacterState.DoubleJump:
                            jetPackCharge -= Time.deltaTime * jetPackFlyFuelConsumption;

                            break;
                        case CharacterState.WallRunning:
                            if (wallState == WallState.WallF)
                            {
                                jetPackCharge -= Time.deltaTime * jetPackForwardWallRunFuelConsumption;
                            }
                            else
                            {
                                jetPackCharge -= Time.deltaTime * jetPackWallRunFuelConsumption;
                            }
                            break;
                        default:
                            jetPackCharge -= Time.deltaTime * jetPackDefaultFuelConsumption;
                            break;


                    }

                }
                else
                {
                    jetPackCharge = 0.0f;
                    jetPackEnable = false;
                }

            }

            if (canMove && !isDead && shouldRotate())
            {

                //if(aimRotation.sqrMagnitude==0){
                getAimRotation();
                /*}else{
                    aimRotation = Vector3.Lerp(aimRotation,getAimRotation(CurWeapon.weaponRange), Time.deltaTime*10);
                }*/
                Vector3 eurler;
                if (isAi)
                {
                    eurler = Quaternion.LookRotation((aimRotation - myTransform.position).normalized).eulerAngles;
                }
                else
                {
                    eurler = desiredRotation.eulerAngles;
                }
               

            
                eurler.z = 0;
                eurler.x = 0;
                if (characterState == CharacterState.WallRunning || characterState == CharacterState.PullingUp)
                {
                   
                }
                else
                {
                    if (isLookingAt && characterState == CharacterState.Idle || characterState == CharacterState.DoubleJump)
                    {

                        if ((Math.Abs(eurler.y - myTransform.rotation.eulerAngles.y) > maxStandRotate))
                        {
                            myTransform.rotation = Quaternion.Lerp(myTransform.rotation, Quaternion.Euler(eurler), Time.deltaTime);

                        }
                    }
                    else
                    {
                        myTransform.rotation = Quaternion.Euler(eurler);
                    }


                }
                //animator.animator.SetLookAtPosition (aimRotation);
                //animator.animator.SetLookAtWeight (1, 0.5f, 0.7f, 0.0f, 0.5f);
                //CurWeapon.curTransform.rotation =  Quaternion.LookRotation(aimRotation-CurWeapon.curTransform.position);
                /*Quaternion diff = Quaternion.identity;
                Vector3 target = (aimRotation-CurWeapon.transform.position).normalized;
                if(!CurWeapon.IsReloading()){
                    diff= Quaternion.FromToRotation(CurWeapon.transform.forward,target);
                }
*/


            }

            SendNetUpdate();
            if (isAi)
            {
                CheckVisibilite();
            }
        }
        else
        {
            ReplicatePosition();

        }
        //		Debug.Log (characterState);

        if (!Application.isPlaying && Application.isEditor)
        {
            foreach (BodyHurt bh in this.GetComponentsInChildren<BodyHurt>()) { bh.TargetHarm = this; }
        }
        //		Debug.Log (characterState);
        UpdateAnimator();
        DpsCheck();


    }

    public virtual bool shouldRotate()
    {
        return true;
    }
    public void SendNetUpdate()
    {
        /*   updateTimer += Time.deltaTime;
           if (updateTimer > updateDelay&&!isDead)
           {
               updateTimer = 0.0f;
              // foxView.PawnUpdate(GetSerilizedData());
           }*/
    }
    public bool NeedUpdate()
    {
        return foxView.isMine && !isDead;
    }
    public void DpsCheck()
    {


        if (activeDPS.Count > 0)
        {
            //Debug.Log("dps" + this + activeDPS.Count);
            for (int i = 0; i < activeDPS.Count; i++)
            {
                singleDPS key = activeDPS[i];
                key.lastTime += Time.deltaTime;
				
                //Debug.Log(key.lastTime);
                if (key.noOnwer)
                {

                    if (key.lastTime > key.showInterval)
                    {

                        activeDPS.RemoveAt(i);
                        i--;
                        continue;
                    }
                    //Debug.Log( " NO OWNER EXIT" );
                    continue;
                }
				if (key.lastTime > key.showInterval)
                {
                BaseDamage ldamage = new BaseDamage(key.damage);
                ldamage.hitPosition = myTransform.position + UnityEngine.Random.onUnitSphere;
                //ldamage.isContinius = true;
                //Debug.Log(ldamage.Damage + " " + Time.deltaTime);
                ldamage.Damage *= key.showInterval;
                ldamage.sendMessage = false;

                // Debug.Log(ldamage.Damage);

                Damage(ldamage, key.killer);
            
                    Pawn killerPawn = key.killer.GetComponent<Pawn>();
                    if (killerPawn != null && killerPawn.team != 0 && killerPawn.team == team && !PlayerManager.instance.frendlyFire && killerPawn != this)
                    {

                        continue;
                    }
                    if (killerPawn != null)
                    {
                        Player killerPlayer = killerPawn.player;
                        if (killerPlayer != null && killerPawn != this)
                        {
                            //Debug.Log ("DAMAGE" +damage.sendMessage);
                            killerPlayer.DamagePawn((ldamage.Damage / Time.deltaTime * key.showInterval).ToString("0"), ldamage.hitPosition);
                        }
                    }
                    key.lastTime = 0;
                }

            }
        }
    }
    //Net replication of position 
    public void ReplicatePosition()
    {
        if (Time.time - lastNetUpdate > 2.0f)
        {
            //StopLocalVisibilite();
        }
        if (initialReplication)
        {
            myTransform.position = correctPlayerPos;
            myTransform.rotation = correctPlayerRot;
            initialReplication = false;
        }
        myTransform.position = Vector3.Lerp(myTransform.position, correctPlayerPos, Time.deltaTime * SYNC_MULTUPLIER);
        myTransform.rotation = Quaternion.Lerp(myTransform.rotation, correctPlayerRot, Time.deltaTime * SYNC_MULTUPLIER);


    }

    public void CheckVisibilite()
    {
        Pawn pawn = PlayerManager.instance.LocalPlayer.GetActivePawn();
        if (pawn != null && (pawn.myTransform.position - myTransform.position).sqrMagnitude > MAXRENDERDISTANCE)
        {
            StopLocalVisibilite();
        }
        else
        {
            RestartLocalVisibilite();
        }

    }
  
    //Weapon Section
    public virtual void StartFire()
    {
        if (CurWeapon != null)
        {
            CurWeapon.StartFire();
        }
		isSpawnImortality= false;
    }
    public virtual void StopFire()
    {
        if (CurWeapon != null)
        {
            CurWeapon.StopFire();
        }
    }
    public virtual void StartPumping()
    {
        if (CurWeapon != null)
        {
            CurWeapon.StartPumping();
        }
    }
    public virtual void StopPumping()
    {
        if (CurWeapon != null)
        {
            CurWeapon.StopPumping();
        }
    }
    //Setting new weapon if not null try to attach it
    public void setWeapon(BaseWeapon newWeapon)
    {
        if (CurWeapon != null)
        {
            ToggleAim(false);
        }
        CurWeapon = newWeapon;	
        //Debug.Log (newWeapon);
        if (CurWeapon != null)
        {
            CurWeapon.AttachWeapon(weaponSlot, Vector3.zero, Quaternion.Euler(weaponRotatorOffset), this);
            if (animator != null)
            {
                animator.SetWeaponType(CurWeapon.animType);
            }
            weaponOffset = CurWeapon.MuzzleOffset();
           
            SetCurWeaponFov();

            weaponRenderer = CurWeapon.GetComponentInChildren<Renderer>();
            if (invisible) MakeWeaponInvisible();
        }
    }
    public void SetCurWeaponFov()
    {
        if (cameraController != null)
        {
//            Debug.Log(CurWeapon.aimFOV);
            cameraController.SetAimFOV(CurWeapon.aimFOV);
        }
    }
    public void ChangeWeapon(int weaponIndex)
    {
        ivnMan.ChangeWeapon(weaponIndex);
    }

    public bool isAimimngAtEnemy()
    {
        if (enemy == null)
        {
            return false;
        }
        else
        {
            Vector3 currentDirection = aimRotation - myTransform.position;
            Vector3 desireDirection = enemy.myTransform.position - myTransform.position;
            return (Vector3.Dot(currentDirection.normalized, desireDirection.normalized) > 0.9f);
        }
    }
    public void SetAiRotation(Vector3 Target)
    {
        if (Target.sqrMagnitude <0.3f)
        {
            Target = myTransform.forward;
        }
        aiAimRotation = myTransform.position +Target;
    }
    public Vector3 getAimpointForWeapon(float speed)
    {
        
        if (foxView.isMine)
        {
            if (isAi)
            {
                return enemy.myTransform.position + (enemy.myTransform.position - myTransform.position).magnitude / speed * enemy.GetVelocity();
            }
            else
            {
                return aimRotation;
            }
        }
        else
        {
            return aimRotation;
        }

    }

    public Vector3 getAimpointForWeapon()
    {

        return aimRotation;
    }
    public Vector3 getAimpointForCamera()
    {

        return myTransform.position + headOffset + desiredRotation * Vector3.forward * aimRange;
    }
    Quaternion desiredRotation;
    float xAngle = 0;
    float yAngle = 0;
    public static float fromOldRotationMod =5.0f;
    public virtual void UpdateRotation(float xDeltaAngle,float yDeltaAngle)
    {

        xAngle += xDeltaAngle * fromOldRotationMod;
        xAngle = Mathf.Clamp(xAngle, cameraController.MinYAngle, cameraController.MaxYAngle);
        yAngle += yDeltaAngle * fromOldRotationMod;
        if (yAngle > 360)
        {
            yAngle -= 360;
        }
        if (yAngle < -360)
        {
            yAngle+=360;
        }

        desiredRotation = Quaternion.Euler(xAngle, yAngle,0.0f);

    }

    public Quaternion GetDesireRotation()
    {
        return desiredRotation;
    }
    public virtual void getAimRotation()
    {

        if (foxView.isMine)
        {
            if (isAi)
            {
                if (enemy == null)
                {
                    if (aiAimRotation.sqrMagnitude > 0)
                    {
                        aimRotation = aiAimRotation;
                    }
                    else
                    {
                        aimRotation = myTransform.position + myTransform.forward * 10;
                    }
                    curLookTarget = null;
                }
                else
                {
                    aimRotation = Vector3.Lerp(aimRotation, enemy.myTransform.position, Time.deltaTime * 10);
                    curLookTarget = enemy.myTransform;
                }

            }
            else
            {
               
                if (cameraController.enabled == false)
                {
                    aimRotation = myTransform.position + myTransform.forward * 50;

                }
                Camera maincam = Camera.main;
                Ray centerRay = maincam.ViewportPointToRay(new Vector3(.5f, 0.5f, 1f));

                //Ray centerRay = new Ray(myTransform.position + headOffset, desiredRotation * Vector3.forward);
                Debug.DrawRay(myTransform.position + headOffset,desiredRotation * Vector3.forward);
                Vector3 targetpoint = Vector3.zero;
                bool wasHit = false;
                float magnitude = aimRange;
                float range = aimRange;
				Transform localTarget=null;
                foreach (RaycastHit hitInfo in Physics.RaycastAll(centerRay, aimRange, ThirdPersonCamera.cameraLayer))
                {
                    if (hitInfo.collider == myCollider || hitInfo.transform.IsChildOf(myTransform) || hitInfo.collider.isTrigger)
                    {
                        continue;
                    }

                    //
                    //Debug.DrawRay(centerRay.origin,centerRay.direction);


                    if (hitInfo.distance < magnitude)
                    {
                        magnitude = hitInfo.distance;
                    }
                    else
                    {
                        continue;
                    }
                    wasHit = true;
                    targetpoint = hitInfo.point;
                    localTarget = hitInfo.transform;
					
					
                    //Debug.Log (curLookTarget);



                }

                if (!wasHit)
                {
                   // Debug.Log("NO HIT");
                    SwitchLookTarget(curLookTarget,null);
                    animator.WeaponDown(false);
                    targetpoint = maincam.transform.forward * aimRange + maincam.ViewportToWorldPoint(new Vector3(.5f, 0.5f, 1f));

//                    targetpoint = desiredRotation * Vector3.forward * aimRange;
                }
                else
                {
					SwitchLookTarget(curLookTarget,localTarget);
                    //Debug.Log(range.ToString()+(cameraController.normalOffset.magnitude+5));
                    if (CurWeapon != null && IsBadSpot(targetpoint))
                    {
                        //targetpoint =maincam.transform.forward*aimRange +maincam.ViewportToWorldPoint(new Vector3(.5f, 0.5f, 1f));
                        animator.WeaponDown(true);
                    }
                    else
                    {
                        animator.WeaponDown(false);
                    }
                }
                aimRotation = targetpoint;

            }


        }
    }
	
	public void SwitchLookTarget(Transform oldTarget,Transform newTarget){
		if(oldTarget==newTarget){
			return;
		}
		curLookTarget= newTarget;
		Pawn tPawn ;
		if(oldTarget!=null){
            tPawn  = oldTarget.root.GetComponent<Pawn>();
			if(tPawn!=null){
				tPawn.HideName();
			}
		}
		if(newTarget!=null){
			tPawn = newTarget.root.GetComponent<Pawn>();
			if(tPawn!=null){
				tPawn.ShowName();
				if(player!=null&&tPawn!=null){
				
					if(tPawn.team==team){
						player.CrosshairType(CrosshairColor.ALLY);
					}else{
						player.CrosshairType(CrosshairColor.ENEMY);
					}
				
				}
			}else{
				if(player!=null){
					player.CrosshairType(CrosshairColor.NEUTRAL);
				}
			}
		}else{
			if(player!=null){
				player.CrosshairType(CrosshairColor.NEUTRAL);
			}
		}
		
	}
	void HideName(){
		guiComponent.HideName();
	}
	void ShowName(){
		guiComponent.ShowName();
	}
    public bool IsBadSpot(Vector3 spot)
    {
        Vector3 charDirection = (spot - myTransform.position).normalized,
        weaponDirection = (spot - (weaponSlot.position + myTransform.forward * weaponOffset)).normalized;
        return Vector3.Dot(charDirection, weaponDirection) < 0;
    }
    void OnDrawGizmos()
    {
        //Gizmos.color = Color.yellow;
        //Gizmos.DrawSphere(aimRotation, 1);
    }
    public Vector3 getCachedAimRotation()
    {
        return aimRotation;

    }

    public float AimingCoef()
    {
        if (isAiming)
        {
            return aimModCoef;
        }
        return 0.0f;
    }


    public virtual void ToggleAim(bool value)
    {
        if (value && (characterState == CharacterState.WallRunning || characterState == CharacterState.Sprinting))
        {
            return;
        }
        if (CurWeapon!=null)
        {
            if (CurWeapon.ToggleAim(value) && value)
            {
                return;
            }
        }
        isAiming = value;
        animator.ToggleAim(value);
        if (cameraController != null)
        {
            if (CurWeapon != null)
            {
                cameraController.ToggleAim(value, CurWeapon.isAimingFPS);
            }
            else
            {
                cameraController.ToggleAim(value,false);
            }
            
        }
		isFps= false;
        if (foxView.isMine && CurWeapon != null&&CurWeapon.isAimingFPS)
        {
            PlayerMainGui.instance.ToggleFpsAim(value);
			if(value){
				isFps= true;
				HideRenders();
			}else{
				ShowRenders();
			}
		}
    }
    public int GetAmmoInBag()
    {
        return ivnMan.GetAmmo(CurWeapon.ammoType);

    }

    public void AddAmmo(float p)
    {
        ivnMan.AddAmmoAll(p);
    }
    public bool IsShooting()
    {
        if (CurWeapon == null)
        {
            return false;
        }
        return CurWeapon.IsShooting();
    }
    public float RecoilMod()
    {
        if (CurWeapon != null)
        {
            return CurWeapon.camRecoilMod;
        }
        return 0;
    }
    public void HasShoot()
    {
        if (cameraController != null)
        {
            cameraController.AddShake(RecoilMod());
        }
    }
    public void Reload()
    {
        if (CurWeapon != null)
        {
            CurWeapon.ReloadStart();

        }

    }
    //For weapon that have shoot animation like  bug tail
    public void StartShootAnimation(string animName)
    {
        animator.StartShootAniamtion(animName);
        if (foxView.isMine)
        {
            foxView.StartShoot(animName);
            //photonView.RPC("RPCStartShootAnimation",PhotonTargets.Others, animName);
        }
    }



    public void StopShootAnimation(string animName)
    {
        animator.StopShootAniamtion(animName);
        if (foxView.isMine)
        {
            foxView.StopShoot(animName);
        }
    }


    //We must tell our gun it's time to spit some projectiles cause of animation 
    public void WeaponShoot()
    {
        AnimationRelatedWeapon myWeapon = CurWeapon as AnimationRelatedWeapon;
        if (myWeapon != null)
        {
            myWeapon.WeaponShoot();
        }
    }
    public void shootEffect()
    {
        if (animator != null)
        {
            animator.ShootAnim();
        }
    }

    //Natural weapon

    public void Kick(int i)
    {


        //		Debug.Log ("ATtack");
        //animator.SetSome("Any",true);
        //((DogAnimationManager) animator).AnyDo();
        if (naturalWeapon.StartKick(AttackType[i]) && foxView.isMine)
        {
            foxView.StartKick(i);
        }
    }

    public void RandomKick()
    {
        if (AttackType.Count == 0)
        {
            return;
        }
        int i = (int)(UnityEngine.Random.value * AttackType.Count);



        //animator.SetSome("Any",true);
        //((DogAnimationManager) animator).AnyDo();
        if (naturalWeapon.StartKick(AttackType[i]) && foxView.isMine)
        {
            foxView.StartKick(i);
        }

    }

    public void StopKick()
    {
        if (naturalWeapon != null)
        {
            naturalWeapon.StopKick();
        }
        if (foxView.isMine)
        {
            foxView.StopKick();
        }
    }
    public void KickFinish()
    {
        if (mainAi != null)
        {
            mainAi.KickFinish();
        }

    }


    public float OptimalDistance(bool isMelee)
    {
        if (CurWeapon != null && !isMelee)
        {
            return CurWeapon.weaponRange / 2;
        }
        if (naturalWeapon != null)
        {
            return naturalWeapon.WeaponDistance;
        }
        return 0.0f;

    }

    public void UseSkill(int i)
    {
        if (skillManager != null)
        {
            skillManager.ActivateSkill(i);
        }
    }
    public void UnUseSkill(int i)
    {
        if (skillManager != null)
        {
            skillManager.DeActivateSkill(i);
        }
    }
    public SkillBehaviour GetMainSkill()
    {
        if (skillManager != null)
        {
            return skillManager.GetSkill(0);
        }
        return null;
    }
    //END WEAPON SECTION
    void OnCollisionEnter(Collision collision)
    {
        //Debug.Log ("COLLISION ENTER PAWN " + this + collision);
    }
    void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other);

        if (other.tag == "damageArea")
        {

            WeaponDamager muzzlePoint = other.GetComponent<WeaponDamager>();

            if (muzzlePoint != null)
            {
                muzzlePoint.gun.GetComponent<ContiniusGun>().fireDamage(this);
            }
            DamageArea area = other.GetComponent<DamageArea>();
            if (area != null)
            {
                area.fireDamage(this);
            }

        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "damageArea")
        {
            WeaponDamager muzzlePoint = other.GetComponent<WeaponDamager>();
            singleDPS newDPS = null;
            if (muzzlePoint != null)
            {
                newDPS = muzzlePoint.gun.GetComponent<ContiniusGun>().getId();
            }
            DamageArea area = other.GetComponent<DamageArea>();
            if (area != null)
            {
                newDPS = area.getId();
            }
            if (newDPS != null)
            {
                foreach (singleDPS key in activeDPS)
                {
                    if (newDPS.killer == key.killer)
                    {
                        //Debug.Log("exit");
                        key.noOnwer = true;
                        break;
                    }
                }
            }
        }

    }

	public override  void clearDps(GameObject killer){
		
        foreach (singleDPS key in activeDPS)
        {
            if (killer == key.killer)
            {
                key.noOnwer = true;
                return;
            }
        }
		
	}
    public override void addDPS(BaseDamage damage, GameObject killer, float fireInterval = 1.0f)
    {
        foreach (singleDPS key in activeDPS)
        {
            if (killer == key.killer)
            {
                key.noOnwer = false;
                return;
            }
        }
        singleDPS newDPS = new singleDPS();
        newDPS.damage = damage;
        newDPS.killer = killer;
        newDPS.showInterval = fireInterval;
        newDPS.lastTime = fireInterval;
        activeDPS.Add(newDPS);
    }

    //TODO: MOVE THAT to PAwn and turn on replication of aiming
    //TODO REPLICATION





    //Movement section
    public CharacterState GetState()
    {
        return characterState;
    }
    protected float CalculateStarfe()
    {
        return Vector3.Dot(myTransform.right, _rb.velocity.normalized);


    }

    public Vector3 JumpVector()
    {
        return Vector3.up * CalculateJumpVerticalSpeed(jumpHeight);
    }

    protected float CalculateSpeed()
    {
        float result = Vector3.Project(_rb.velocity, myTransform.forward).magnitude;
        //Debug.Log (result);
        if (result < groundWalkSpeed * 0.5f)
        {
            return 0.0f;
        }
        if (result > groundWalkSpeed * 0.5f && result < groundWalkSpeed)
        {
            return 1.0f * Mathf.Sign(Vector3.Dot(_rb.velocity.normalized, myTransform.forward));
        }
        if (result > groundWalkSpeed && result < groundRunSpeed)
        {
            return 2.0f * Mathf.Sign(Vector3.Dot(_rb.velocity.normalized, myTransform.forward));
        }
        if (result > groundRunSpeed)
        {
            return 2.0f * Mathf.Sign(Vector3.Dot(_rb.velocity.normalized, myTransform.forward));
        }
        return 0.0f;
    }
    protected float CalculateRepStarfe()
    {
        Vector3 velocity = replicatedVelocity;
        return Vector3.Dot(myTransform.right, velocity.normalized);


    }
    protected float CalculateRepSpeed()
    {
        Vector3 velocity = correctPlayerPos - myTransform.position;
        velocity = velocity / (Time.deltaTime * SYNC_MULTUPLIER);
        float result = Vector3.Project(velocity, myTransform.forward).magnitude;
        if (result < groundWalkSpeed * 0.5f)
        {
            return 0.0f;
        }
        if (result > groundWalkSpeed * 0.5f && result < groundWalkSpeed)
        {
            return 1.0f * Mathf.Sign(Vector3.Dot(velocity.normalized, myTransform.forward));
        }
        if (result > groundWalkSpeed && result < groundRunSpeed)
        {
            return 2.0f * Mathf.Sign(Vector3.Dot(velocity.normalized, myTransform.forward));
        }
        if (result > groundRunSpeed)
        {
            return 2.0f * Mathf.Sign(Vector3.Dot(velocity, myTransform.forward));
        }
        return 0.0f;
    }
    public void Movement(Vector3 movement, CharacterState state)
    {
        //Debug.Log (state);
        //Debug.Log (state);
        if (isSpawn)
        {//если только респавнились, то не шевелимся
            return;
        }



        nextState = state;

        if (nextState != CharacterState.Jumping && nextState != CharacterState.DoubleJump)
        {
            movement = (movement - Vector3.Project(movement, floorNormal)).normalized * movement.magnitude;
            //Debug.DrawRay (myTransform.position, movement.normalized);
            //Debug.DrawRay (myTransform.position, floorNormal);
            nextMovement = movement;
        }
        else
        {
            nextMovement = movement;
        }

    }
    public bool IsSprinting()
    {
        return characterState == CharacterState.Sprinting;

    }
    public virtual bool CanSprint()
    {
        return jetPackCharge >= 1.0f || characterState == CharacterState.Sprinting;
    }
    bool WallRun(Vector3 movement, CharacterState state)
    {
        if ((!canWallRun || !_canWallRun) && foxView.isMine) return false;

        //if (isGrounded) return false;
        if (lastTimeOnWall + 0.5f > Time.time)
        {
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

            return false;
        }
        if (characterState != CharacterState.DoubleJump && characterState != CharacterState.Sprinting && characterState != CharacterState.WallRunning && characterState != CharacterState.Jumping)
        {
            return false;
        }
        //Debug.Log (characterState);
        RaycastHit leftH, rightH, frontH;


        bool leftW = Physics.Raycast(myTransform.position,
                                      (-1 * myTransform.right).normalized, out leftH, capsule.radius + 0.4f, wallRunLayers);
        bool rightW = Physics.Raycast(myTransform.position,
                                       (myTransform.right).normalized, out rightH, capsule.radius + 0.4f, wallRunLayers);
        bool frontW = Physics.Raycast(myTransform.position,
                                       myTransform.forward, out frontH, capsule.radius + 0.2f, wallRunLayers);

        Debug.DrawLine(myTransform.position,
                        myTransform.position +  (- myTransform.right).normalized * (capsule.radius + 0.2f));

        Debug.DrawLine(myTransform.position,
                        myTransform.position + ( myTransform.right).normalized * (capsule.radius + 0.2f));

        //Debug.DrawLine (myTransform.position,
        // myTransform.forward);




        Vector3 tangVect = Vector3.zero, normal = Vector3.zero;

        if (!animator.animator.IsInTransition(0) && !_rb.isKinematic)
        {
            if (leftW)
            {

                normal = leftH.normal;
                tangVect = Vector3.Cross(leftH.normal, Vector3.up);
                //tangVect = Vector3.Project(movement,tangVect).normalized;
                _rb.velocity = tangVect * wallRunSpeed + myTransform.up * wallRunSpeed / 3;
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
                    _rb.velocity = myTransform.up * movement.y + WallJumpDirection(leftH.normal) * movement.y;
                    StartCoroutine(WallJump(1f)); // Exclude if not needed
                }
            }

            else if (rightW)
            {
                normal = rightH.normal;
                tangVect = -Vector3.Cross(rightH.normal, Vector3.up);
                //tangVect = Vector3.Project(movement,tangVect).normalized;
                _rb.velocity = tangVect * wallRunSpeed + myTransform.up * wallRunSpeed / 3;
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
                    _rb.velocity = myTransform.up * movement.y + WallJumpDirection(rightH.normal) * movement.y;
                    StartCoroutine(WallJump(1f)); // Exclude if not needed
                }
            }

            else if (frontW)
            {
                _rb.velocity = myTransform.up * wallRunSpeed / 1.5f;
                normal = frontH.normal;
                tangVect = frontH.normal * -1;
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
                    _rb.velocity = (myTransform.up  + WallJumpDirection(myTransform.forward * -1)).normalized * movement.y;
                    StartCoroutine(WallJump(1f)); // Exclude if not needed
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

            }
            float angle = Mathf.Abs(Vector3.Dot(normal, Vector3.up));

            if (angle > 0.5f)
            {
                characterState = CharacterState.Jumping;
                lastTimeOnWall = Time.time;
                jetPackEnable = false;
                return false;
            }

            forwardRotation = tangVect;
            if (forwardRotation.sqrMagnitude > 0)
            {
                myTransform.rotation = Quaternion.LookRotation(forwardRotation);
            }
            //Debug.Log(forwardRotation);
           // Debug.DrawRay(myTransform.position,forwardRotation,Color.green);
            //animator.WallAnimation(leftW,rightW,frontW);


            return leftW || rightW || frontW;
        }
        return false;

    }
    // Wall run cool-down
    void WallRunCoolDown()
    {
        _canWallRun = true;
        if (player != null)
        {
            EventHolder.instance.FireEvent(typeof(LocalPlayerListener), "EventStartWallRun", player, myTransform.position);

        }

    }
    // Wall run cool-down
    IEnumerator WallJump(float sec)
    {
        Jump();
        SendMessage("WallJumpMessage", SendMessageOptions.DontRequireReceiver);
        //Debug.Log ("WALLJUMP");
        _canWallRun = false;
        jetPackEnable = false;
        characterState = CharacterState.Jumping;
        yield return new WaitForSeconds(sec);
        _canWallRun = true;
    }
    public Vector3 WallJumpDirection(Vector3 wallNormal)
    {
        Vector3 direction = aimRotation - myTransform.position;
        if (Vector3.Dot(wallNormal, direction) < 0)
        {
            return wallNormal;
        }
        else
        {
            if (direction.y < 0)
            {
                direction.y = 0;
            }
            return direction.normalized;
        }
    }


    //TODO ADD STEP CHECK WITH RAYS
    void OnCollisionStay(Collision collisionInfo)
    {
        if (lastJumpTime + 0.1f > Time.time)
        {
            return;
        }
        if (characterState == CharacterState.WallRunning)
        {
            return;
        }


        foreach (ContactPoint contact in collisionInfo.contacts)
        {
            /*if(contact.otherCollider.CompareTag("decoration")){
                continue;
            }*/
            Vector3 Direction = contact.point - myTransform.position - ((CapsuleCollider)myCollider).center;
            //Debug.Log (this.ToString()+collisionInfo.collider+Vector3.Dot(Direction.normalized ,Vector3.down) );
            float minAngle = 0.75f;
            if (((CapsuleCollider)myCollider).direction == 2)
            {
                minAngle = 0.2f;
            }
            else
            {
                CapsuleCollider capsule = ((CapsuleCollider)myCollider);
                minAngle = Mathf.Atan(capsule.radius / capsule.height * 2)  ;
            }
           // Debug.Log(Mathf.Cos(minAngle));
          
            /*
             *   float normalRadius;
             *   
            if (((CapsuleCollider)myCollider).direction == 2)
            {
                minAngle = 0.2f;
                normalRadius =((CapsuleCollider)myCollider).radius;
            }else{
                 normalRadius =((CapsuleCollider)myCollider).height/2;
            }
            float radiusPoint = Mathf.Sin(Vector3.Angle(Direction.normalized, Vector3.down)) * Direction.magnitude;
            Debug.Log(radiusPoint);
             *
             * */
            //Debug.Log(Vector3.Dot(Direction.normalized, Vector3.down));
            if (Vector3.Dot(Direction.normalized, Vector3.down) > Mathf.Cos(minAngle))
            {
                isGrounded = true;

                floorNormal = contact.normal;
            }

           // Debug.DrawLine(myTransform.position + ((CapsuleCollider)myCollider).center, contact.point, Color.white);

        }



    }


    public void StartSprint()
    {

        if (jetPackCharge >= 1.0f)
        {
            StartJetPack();
            characterState = CharacterState.Sprinting;
        }

    }
    public void StopSprint()
    {
        jetPackEnable = false;
    }
    public void StartJetPack()
    {
        jetPackEnable = true;

    }

    public float CalculateJumpVerticalSpeed(float targetJumpHeight)
    {
        // From the jump height and gravity we deduce the upwards speed 
        // for the character to reach at the apex.
        return Mathf.Sqrt(2 * targetJumpHeight * gravity);
    }

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
       // Debug.Log(characterState +" " +isGrounded);
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
                    if (_rb.isKinematic) _rb.isKinematic = false;

                    //Debug.Log (this+ " " +velocityChange);
                    rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);

                    if (nextState == CharacterState.Sprinting)
                    {

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
                if (!jetPackEnable)
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

                        if (nextState == CharacterState.DoubleJump)
                        {
                            DoubleJump();

                        }
                    }
                    else
                    {
                        ForcedDoubleJump();
                    }
                }



                if (WallRun(nextMovement, nextState))
                {
                    SendMessage("WallLand", SendMessageOptions.DontRequireReceiver);
                }
                ToggleAim(false);



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

                if (WallRun(nextMovement, nextState))
                {
                    SendMessage("WallLand", SendMessageOptions.DontRequireReceiver);
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

                nextMovement = myTransform.forward;

                velocityChange = nextMovement.normalized * flyForwardSpeed + Vector3.up * flySpeed - velocity;
                animator.ApllyJump(true);
                animator.WallAnimation(false, false, false);
                rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
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
                    if (characterState != CharacterState.DoubleJump)
                    {
                        animator.FreeFall();
                    }
                }

                PullUp();
                if (characterState != CharacterState.WallRunning)
                {
                    if (player != null)
                    {
                        EventHolder.instance.FireEvent(typeof(LocalPlayerListener), "EventEndWallRun", player, myTransform.position);

                    }

                }
                ToggleAim(false);
                break;
            case CharacterState.PullingUp:
                PullUp();
                break;
            default:
                characterState = nextState;
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
        isGrounded = false;

    }

    public void JumpEnd(CharacterState nextState)
    {
        if (nextState == CharacterState.Jumping)
        {
            characterState = CharacterState.Idle;
        }
        else
        {
            characterState = nextState;
        }
    }

    public bool IsGrounded()
    {

        return isGrounded;
    }
    public void Jump()
    {
        if (animator != null)
        {
            animator.ApllyJump(true);
            //звук прыжка
            sControl.playClip(jumpSound);
        }
        AchievementManager.instance.UnEvnetAchive(AchievementManager.PARAM_JUMP, 1.0f);
        lastJumpTime = Time.time;
        //photonView.RPC("JumpChange",PhotonTargets.OthersBuffered,true);
    }
    public float GetJetPackCharges()
    {
        return jetPackCharge;

    }
    public float GetMaxJetPack()
    {
        return charMan.GetFloatChar(CharacteristicList.JETPACKCHARGE);
    }

    public void DidLand()
    {

        //Debug.Log ("LAND");
        lastTimeOnWall = -10.0f;
        //photonView.RPC("JumpChange",PhotonTargets.OthersBuffered,false);
    }

    bool PullUpCheck()
    {
        if (!canPullUp)
        {
            return false;
        }
        if (characterState == CharacterState.PullingUp)
        {
            return true;
        }
        RaycastHit frontH;
        bool upCol = Physics.Raycast(myTransform.position,
                                               Vector3.up, out frontH, capsule.height, wallRunLayers);
        if (upCol)
        {
            return false;
        }
        /*Debug.DrawRay (myTransform.position ,
                       myTransform.forward ,Color.black,2f );
        Debug.DrawRay (myTransform.position+ myTransform.up ,
                       myTransform.forward ,Color.blue,2f );
        Debug.DrawLine(myTransform.position + myTransform.up,
                      myTransform.position + myTransform.up + myTransform.forward * capsule.radius * 5, Color.blue,2f   );*/
        bool frontW = Physics.Raycast(myTransform.position,
                                       myTransform.forward, out frontH, capsule.radius * 2, wallRunLayers);
        bool middleAir = Physics.Raycast(myTransform.position + myTransform.up / 2,
                                          myTransform.forward, out frontH, capsule.radius + 0.2f, wallRunLayers);
        if (frontW || middleAir)
        {
            bool frontAir = Physics.Raycast(myTransform.position + myTransform.up,
                                       myTransform.forward, out frontH, capsule.radius * 5, wallRunLayers);
            //forwardRotation= frontH.normal*-1;


            animator.SetLong(!middleAir);
            //Debug.Log("frontAir" + frontH.transform +" is" +frontAir);
            return !frontAir;

        }

        return false;
        //Deprecated system of collider pullup system
        /*Vector3 p1 = myTransform.position - (myTransform.up * -heightOffsetToEdge) + myTransform.forward;
        Vector3 p2 = myTransform.position - (myTransform.up * -heightOffsetToEdge);
        //Debug.DrawLine (p1, p2);
        RaycastHit hit;
        //Debug.DrawLine (p1-myTransform.up*climbCheckDistance, p2-myTransform.up*climbCheckDistance);
        // Hit nothing and not at edge -> Out
        return Physics.CapsuleCast (p1, p2, climbCheckRadius, -myTransform.up, out hit, climbCheckDistance, climbLayers);*/

    }
    // Wall run cool-down
    IEnumerator PullUpEnd(float sec)
    {

        yield return new WaitForSeconds(sec);
        _rb.isKinematic = false;
        animator.FinishPullingUp();
        characterState = CharacterState.Idle;
        isGrounded = true;

        SendMessage("DidLand", SendMessageOptions.DontRequireReceiver);

    }

    void PullUp()
    {

        if (!PullUpCheck())
        {
            return;
        }
        //Debug.Log (characterState);
        if (characterState != CharacterState.PullingUp)
        {
            jetPackEnable = false;
            characterState = CharacterState.PullingUp;
            animator.WallAnimation(false, false, false);
            _rb.isKinematic = true;
            StartCoroutine("PullUpEnd", PullUpTime);
            animator.StartPullingUp();
            PullUpStartTimer = 0.0f;
        }
        PullUpStartTimer += Time.deltaTime;
        float nT = PullUpStartTimer / PullUpTime;

        if (nT <= 1.0f)
        {
            if (nT <= 0.4f)
            { // Step up
                myTransform.Translate(Vector3.up * Time.deltaTime * climbSpeed);
            }
            else
            { // Step forward
                if (nT <= 0.6f)
                    myTransform.Translate(Vector3.forward * Time.deltaTime * climbSpeed);
                else if (nT >= 0.6f && _rb.isKinematic) // fall early
                    _rb.isKinematic = false;
                if (!_rb.isKinematic)
                    _rb.velocity = new Vector3(0, _rb.velocity.y, 0);
            }
        }


    }
    void StopDoubleJump()
    {
        jetPackEnable = false;
    }
    void DoubleJump()
    {
        if (jetPackCharge >= 1.0f)
        {
            Vector3 velocity = _rb.velocity;
            Vector3 velocityChange = (nextMovement - velocity);
            StartJetPack();
            animator.DoubleJump();
            rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
            if (player != null)
            {
                EventHolder.instance.FireEvent(typeof(LocalPlayerListener), "EventPawnDoubleJump", player);

            }
            characterState = CharacterState.DoubleJump;
        }


    }
    /// <summary>
    /// Double jump for forced situation like fallin of a cliff
    /// </summary>
    void ForcedDoubleJump()
    {
        StartJetPack();
        animator.DoubleJump();
        characterState = CharacterState.DoubleJump;
    }
    public void StopMachine()
    {
        characterState = CharacterState.Idle;
        nextMovement = Vector3.zero;
    }
    public void StopMovement()
    {
        characterState = CharacterState.Idle;
        nextMovement = Vector3.zero;
        canMove = false;
        _rb.velocity = Vector3.zero;
    }
    public void StartMovement()
    {
        if (!_knockOut)
        {
            canMove = true;
        }
    }
    void AddPushForce(Vector3 force)
    {
        pushingForce += force;
        StartCoroutine(RemoveForce(force));
    }
    public IEnumerator RemoveForce(Vector3 force)
    {
        yield return new WaitForSeconds(0.1f);
        pushingForce -= force;

    }

    //end Movement Section




    public void Activate()
    {
        if (cameraController != null)
        {
            _rb.isKinematic = false;
            _rb.detectCollisions = true;
            cameraController.enabled = true;
            cameraController.Reset();
            GetComponent<ThirdPersonController>().enabled = true;

        }
        isActive = true;
        for (int i = 0; i < myTransform.childCount; i++)
        {
            myTransform.GetChild(i).gameObject.SetActive(true);
        }
        foxView.Activate();
    }

    public void RemoteActivate()
    {
        //Debug.Log ("RPCActivate");
        if (cameraController != null)
        {
            cameraController.enabled = true;

            GetComponent<ThirdPersonController>().enabled = true;

        }
        isActive = true;
        for (int i = 0; i < myTransform.childCount; i++)
        {
            myTransform.GetChild(i).gameObject.SetActive(true);
        }
    }
    public void DeActivate()
    {
        if (cameraController != null)
        {
            _rb.isKinematic = true;

            _rb.detectCollisions = false;
            cameraController.enabled = false;

            GetComponent<ThirdPersonController>().enabled = false;
        }
        isActive = false;
        for (int i = 0; i < myTransform.childCount; i++)
        {
            myTransform.GetChild(i).gameObject.SetActive(false);
        }
        foxView.DeActivate();

    }

    public void RemoteDeActivate()
    {
        //Debug.Log ("RPCDeActivate");
        if (cameraController != null)
        {
            cameraController.enabled = false;

            GetComponent<ThirdPersonController>().enabled = false;
        }
        isActive = false;
        for (int i = 0; i < myTransform.childCount; i++)
        {
            myTransform.GetChild(i).gameObject.SetActive(false);
        }

    }


    //EndNetworkSection

    //Base Seenn Hear work

    public List<Pawn> getAllSeenPawn()
    {
        return seenPawns;
    }

    //end seen hear work


    //VISUAL EFFECT SECTION 

    public void PlayTaunt()
    {
        if (tauntAnimation == "")
        {
            return;
        }
        canMove = false;
        animator.PlayTaunt(tauntAnimation);

        foxView.Taunt(tauntAnimation);
    }
    public void StopTaunt()
    {
        canMove = true;

    }
    public void RemotePlayTaunt(string taunt)
    {
        animator.PlayTaunt(taunt);
    }
    public void StartKnockOut()
    {
        if (canBeKnockOut)
        {
            StartCoroutine(KnockOut());
        }
    }


    public IEnumerator KnockOut()
    {
        // Debug.Log("KnockOut");
        if (!_knockOut)
        {
            _knockOut = true;
            canMove = false;
            animator.KnockOut();
            StopKick();
            if (foxView.isMine)
            {
                foxView.KnockOut();
            }

            yield return new WaitForSeconds(KnockOutTimeOut);
            if (!isDead)
            {
                animator.StandUp();
            }
        }
    }
    public void StandUpFinish()
    {
        _knockOut = false;
        canMove = true;
    }

		
	public void HideRenders(){
		Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers) {
            renderer.enabled = false;
        }
	
	}
	public void ShowRenders(){
		Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers) {
            renderer.enabled = true;
        }
	
	}
    //END OF VISULA EFFECT


    //BUFF SECTION 
    public void AddBuff(int characteristic, object value)
    {
        switch ((CharacteristicList)characteristic)
        {
            case CharacteristicList.MAXHEALTH:
                health += (int)value;
                break;
        }


        charMan.AddEffect(characteristic, value);

    }
    public void RemoveBuff(int characteristic, object value)
    {
        switch ((CharacteristicList)characteristic)
        {
            case CharacteristicList.MAXHEALTH:
                health -= (int)value;
                if (health <= 0)
                {
                    health = 1;
                }
                break;
        }
        charMan.RemoveEffect(characteristic, value);

    }
    public int GetValue(CharacteristicList characteristic)
    {
        return charMan.GetIntChar(characteristic);
    }

    public float GetPercentValue(CharacteristicList characteristic)
    {
        return 1f + ((float)charMan.GetIntChar(characteristic)) / 100f;
    }
    //END OF BUFF SECTION


    //NETWORK SECTION
    void OnMasterClientSwitched()
    {
        Debug.Log("Master On PAwn");
        if (isAi)
        {
            mainAi.StartAI();

            RestartLocalVisibilite();
        }
        rigidbody.isKinematic = false;
    }

    public void RemoteSetAI(int group, int homeindex)
    {
        mainAi = GetComponent<AIBase>();
        mainAi.RemoteInit(group, homeindex);
        isActive = true;
    }

    public void PlayCustomAnimRemote(string name)
    {
        animator.SetSome(name);
    }

    public PawnModel GetSerilizedData()
    {
        serPawn.id = foxView.viewID;
        serPawn.wallState = (int)wallState;
        serPawn.team = team;
        serPawn.characterState = (int)characterState;
        serPawn.active = isActive;
        serPawn.isAiming = isAiming;
        serPawn.position.WriteVector(transform.position);
        serPawn.aimRotation.WriteVector(aimRotation);
        serPawn.rotation.WriteQuat(transform.rotation);
        serPawn.health = health;
        serPawn.velocity.WriteVector(_rb.velocity);
        return serPawn;
    }



    public virtual void NetUpdate(PawnModel pawn)
    {
        nextState = (CharacterState)pawn.characterState;
        wallState = (WallState)pawn.wallState;
        Vector3 oldPos = correctPlayerPos;

        correctPlayerPos = pawn.position.MakeVector(correctPlayerPos);
        correctPlayerRot = pawn.rotation.MakeQuaternion(correctPlayerRot);
     
        aimRotation = pawn.aimRotation.MakeVector(aimRotation);
        ToggleAim(pawn.isAiming);
        team = pawn.team;
        health = pawn.health;
        replicatedVelocity = correctPlayerPos - oldPos;
        
        float oldTime = lastNetUpdate;
        lastNetUpdate = Time.time;
        replicatedVelocity = pawn.velocity.GetVector();
        correctPlayerPos = correctPlayerPos+pawn.velocity.GetVector() * (oldTime - lastNetUpdate);
        RestartLocalVisibilite();
        if (pawn.active != isActive)
        {
            if (pawn.active)
            {
                Activate();
            }
            else
            {
                DeActivate();
            }
        }
    }
	public void InfoAboutDeath(ISFSObject data){
		
		data.PutInt("weaponId",killInfo.weaponId);
        data.PutBool("headShot", killInfo.isHeadShoot);
	}


    //For machine we turn off render but leave logic and movement 
    public void RestartLocalVisibilite()
    {
        if (!isLocalVisible)
        {
            for (int i = 0; i < myTransform.childCount; i++)
            {
                myTransform.GetChild(i).gameObject.SetActive(true);
            }
            isLocalVisible = true;
        }
    }
    //For machine we turn on render 
    public void StopLocalVisibilite()
    {
        isLocalVisible = false;
        for (int i = 0; i < myTransform.childCount; i++)
        {
            myTransform.GetChild(i).gameObject.SetActive(false);
        }
    }



    //Invsibilite Section;
    public void SetInvisible(bool p)
    {
        foreach (Renderer rend in GetComponentsInChildren<Renderer>())
        {
            rend.enabled = p;
        }
    }
    private Renderer mainRenderer;

    private Renderer weaponRenderer;

    private bool invisible = false;
    public void SetMaterial()
    {
        foreach (Material mat in mainRenderer.materials)
        {
            Color trans = mat.color;
            trans.a = 0.1f;
            mat.color = trans;
        }
        invisible = true;
        if (weaponRenderer != null)
        {
            foreach (Material mat in weaponRenderer.materials)
            {
                Color trans = mat.color;
                trans.a = 0.1f;
                mat.color = trans;
            }
        }
    }

    public void SetNormalMaterial()
    {
        foreach (Material mat in mainRenderer.materials)
        {
            Color trans = mat.color;
            trans.a = 0.1f;
            mat.color = trans;
        }
        invisible = false;
        MakeWeaponInvisible();
    }
    void MakeWeaponInvisible()
    {
        if (weaponRenderer != null)
        {
            foreach (Material mat in weaponRenderer.materials)
            {
                Color trans = mat.color;
                trans.a = 1f;
                mat.color = trans;
            }
        }
    }
    public void gameEnded()
    {
        StopFire();
       
        Destroy(gameObject);
       
    }
}
