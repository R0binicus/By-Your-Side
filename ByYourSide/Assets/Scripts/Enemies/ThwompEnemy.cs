using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThwompEnemy : BaseEnemy
{
    //[SerializeField]
    //private GameObject projectilePrefab; // the projectile that this enemy fires at the player
    //Rigidbody rb;
    [SerializeField]
    private Transform entity; // the reference to this enemy
    [SerializeField]
    private float shootDistance = 12.0f;
    [SerializeField]
    private float slowDistance = 10.0f;
    //[SerializeField] private string soundName;
	//private AudioSource abilitySound;
    //[SerializeField] private string deathName;
	//private AudioSource deathSound;
    [SerializeField] private string projectileTarget;


    [Header("Lightning Stats")]
    [SerializeField] private BasicProjectile proj;
    [SerializeField] private float projectileLifeTime;
    [SerializeField] private float projectileDamage;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float projectileKnockback;
    
    [SerializeField] private float lightningCD;
    [SerializeField] private int projectileNum;
    [SerializeField] private int projectileNumChange;

    [Header("Wind Gust Stats")]
    [SerializeField] private EnemyKnockBackProjectile windProj;
    [SerializeField] private float windLifeTime;
    [SerializeField] private float windDamage;
    [SerializeField] private float knockbackAmt;
    [SerializeField] private float windSpeed;
    [SerializeField] private float wingGustCD;


    private bool attackRotate = false;
    public bool canAttack2 = true;

    private void Awake()
	{
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        moveSpeed = agent.speed;

        lastSeenPosition = transform.position;

        oldMoveSpeed = moveSpeed;
        canMove = true;
        //abilitySound = GameObject.Find(soundName).GetComponent<AudioSource>();
        //deathSound = GameObject.Find(deathName).GetComponent<AudioSource>();
	}

	protected override void Update()
	{
        //Update the player's position as they move
        GetPlayerLocation();
        //Determine whether the enemy can see the player
        CanSeePlayer();
        // if the enemy can see the player, shoot at them
        if (playerInLOS && directionToPlayer.magnitude <= shootDistance)
        {
            //moveSpeed = oldMoveSpeed/10;
            if (canAttack)
            {
                StartCoroutine(Attack());
            }
            if (canAttack2)
            {
                StartCoroutine(Attack2());
            }
        }
        // if the enemy can't see the player, move towards them
        else
        {
            moveSpeed = oldMoveSpeed;
        }
        if (canMove)
        {
            Move();
        }

        if (directionToPlayer.magnitude < slowDistance) //If player is inside slow distance
        {
            agent.speed = oldMoveSpeed/10; //Slow speed by 90%
        }
        else {agent.speed = oldMoveSpeed;} //Reset speed
    }

    // Wind Gust
    public override IEnumerator Attack()
	{
        canAttack = false;
        WingGustAttack(projectileNum);

        if (attackRotate)
        {
            attackRotate = false;
            projectileNum -= projectileNumChange;
        }
        else
        {
            attackRotate = true;
            projectileNum += projectileNumChange;
        }
        
        // wait amount of seconds before firing again
        yield return new WaitForSeconds(wingGustCD);
        canAttack = true;
    }

    // Lightning
    public IEnumerator Attack2()
	{
        canAttack = false;
        LightningAttack();
        
        // wait amount of seconds before firing again
        yield return new WaitForSeconds(lightningCD);
        canAttack = true;
    }

    public void WingGustAttack(int localNum)
    {
        float radius = 5f;
        float angleStep = 360f / localNum;
        float angle = 0f;

        for (int i = 0; i <= localNum; i++) 
        {

            float directionX = Mathf.Sin ((angle * Mathf.PI) / 180) * radius;
            float directionZ = Mathf.Cos ((angle * Mathf.PI) / 180) * radius;

            Vector3 projectileVector = new Vector3 (directionX, 0, directionZ);
            Vector3 projectileMoveDirection = (projectileVector).normalized * projectileSpeed;

            var projectile = Instantiate(windProj, new Vector3(this.rb.position.x, this.rb.position.y, this.rb.position.z), Quaternion.Euler(0, angle, 0));
            
            projectile.GetComponent<Rigidbody>().velocity = new Vector3 (projectileMoveDirection.x, 0, projectileMoveDirection.z);
            projectile.lifeTime = windLifeTime;
            projectile.speed = windSpeed;
            projectile.target = projectileTarget;
            projectile.damage = windLifeTime;
            projectile.knockBack = knockbackAmt;

            //projectile.lifeTime = windLifeTime;
            //projectile.damage = windDamage;
            //projectile.speed = windSpeed;
            //projectile.target = projectileTarget
            //projectile.knockBack = knockbackAmt;
            //projectile.knockBackDir = heading;

            angle += angleStep;
        }
    }

    public void LightningAttack()
	{

        float angleStep = 180f / projectileNum;
        float angle = 0f;
        float spreadRadius = 2f;
        //angle = (Vector3.SignedAngle(this.rb.position, transform.forward, directionToPlayer)) + (180f/2);
        //Debug.Log(angle);



        for (int i = 0; i <= projectileNum; i++) 
        {

            float dirX = directionToPlayer.x + Mathf.Sin ((angle * Mathf.PI) / 180) * spreadRadius;
            float dirZ = directionToPlayer.z + Mathf.Cos ((angle * Mathf.PI) / 180) * spreadRadius;
            

            Vector3 projectileVector = new Vector3 (dirX, 0, dirZ);
            //Vector3 projectileMoveDirection = (projectileVector - directionToPlayer).normalized * projectileSpeed;
            Vector3 projectileMoveDirection = (projectileVector).normalized * projectileSpeed;

            var projectile = Instantiate(proj, new Vector3(this.rb.position.x, this.rb.position.y, this.rb.position.z), Quaternion.identity);
            
            projectile.GetComponent<Rigidbody>().velocity = new Vector3 (projectileMoveDirection.x, 0, projectileMoveDirection.z);
            projectile.lifeTime = projectileLifeTime;
            projectile.damage = projectileDamage;
            projectile.speed = projectileSpeed;
            projectile.knockback = projectileKnockback;
            projectile.target = projectileTarget;
        
            angle += angleStep;
        }
	}

    public void DeathSound()
    {
        //deathSound.Play();
    }
}
