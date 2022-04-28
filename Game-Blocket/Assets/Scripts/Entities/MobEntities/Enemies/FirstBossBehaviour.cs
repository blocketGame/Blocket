using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstBossBehaviour : MonoBehaviour
{
    #region public Variables
    [Header("[Boss Attributes]")]
    [Range(0, 50)] [SerializeField]
    float attackDamage = 25f;
    [Range(0,50)] [SerializeField]
    float siteRadius = 20f;
    [Range(0, 20)] [SerializeField]
    float stopMoveIfInRadius = 0.5f;
    [Range(0, 20)] [SerializeField]
    float attackRadius = 10f;
    [Range(0, 20)] [SerializeField]
    float moveAwayIfPlayerInRadius = 0f;
    [Range(0, 12)] [SerializeField]
    float movementSpeed = 4f;
    [Range(0, 12)] [SerializeField]
    float jumpForce = 4f;
    [Range(3, 30)] [SerializeField]
    float projectileSpeed = 12f;
    [Range(0.01f, 4)] [SerializeField]
    float timeBeetweenEachNormalAttack = 2f;
    [Range(10, 120)] [SerializeField]
    float timeBeetweenEachStompAttack = 12f;
    [Range(10, 120)] [SerializeField]
    float timeBeetweenEachTurretAttack = 20f;
    [SerializeField] Sprite projectileSprite;
    [SerializeField] Sprite shockwaveSprite;
    [SerializeField] Sprite turretSprite;
    
    #endregion

    #region privat Variables
    GameObject player;
    bool isInPlayerRange;
    bool isBurning;
    bool isJumpAllowed;
    double normalAttackCooldownLeft;
    bool normalAttackIsAllowed;
    double stompAttackCooldownLeft;
    bool stompAttackIsAllowed;
    double turretAttackCooldownLeft;
    bool turretAttackIsAllowed;
    bool isInStompingAction;
    bool isSpawningTurretOnNextCollision;
    byte side = 0;
    float localScaleX;
    #endregion

    void Start() {
        player = GameObject.FindGameObjectWithTag("Player");
        isInPlayerRange = false;
        isBurning = false;
        normalAttackCooldownLeft = 0;
        normalAttackIsAllowed = true;
        stompAttackCooldownLeft = 0;
        stompAttackIsAllowed = true;
        turretAttackCooldownLeft = 0;
        turretAttackIsAllowed = true;
        isInStompingAction = false;
        isSpawningTurretOnNextCollision = false;
        localScaleX = transform.localScale.x;

        addTurretAttackCooldown(timeBeetweenEachTurretAttack);
        addStompAttackCooldown(timeBeetweenEachStompAttack);
        addNormalAttackCooldown(timeBeetweenEachNormalAttack);
    }

    void Update()
    {

        if(normalAttackCooldownLeft < 0)
        {
            normalAttackIsAllowed = true;
            normalAttackCooldownLeft = 0;
        } else normalAttackCooldownLeft -= Time.deltaTime;


        if (stompAttackCooldownLeft < 0)
        {
            stompAttackIsAllowed = true;
            stompAttackCooldownLeft = 0;
        } else stompAttackCooldownLeft -= Time.deltaTime;

        if (turretAttackCooldownLeft < 0)
        {
            turretAttackIsAllowed = true;
            turretAttackCooldownLeft = 0;
        }
        else turretAttackCooldownLeft -= Time.deltaTime;

    }

    void FixedUpdate() {
        float distanceBetweenPlayerAndMe = Vector2.Distance(transform.position, player.transform.position);
        if ((player.transform.position.x - transform.position.x) < 0) {
            transform.localScale = new Vector3(localScaleX, transform.localScale.y, transform.localScale.z);
        } else transform.localScale = new Vector3(localScaleX * -1, transform.localScale.y, transform.localScale.z);

        if (distanceBetweenPlayerAndMe < attackRadius && !isBurning)
        {
            NormalAttack();
            StompAttack();
            TurretAttack();
        }

        if (distanceBetweenPlayerAndMe < moveAwayIfPlayerInRadius) {
            gameObject.transform.position = Vector2.MoveTowards(transform.position, new Vector2((player.transform.position.x) * -1000, transform.position.y), movementSpeed * Time.deltaTime);
            return;
        }
        
        if (distanceBetweenPlayerAndMe < stopMoveIfInRadius) {
            isInPlayerRange = true;
            return;
        }

        if(distanceBetweenPlayerAndMe < siteRadius) {
            isInPlayerRange=true;
            if(GetComponent<Rigidbody2D>().velocity == Vector2.zero) {
                Jump();
            }

            gameObject.transform.position = Vector2.MoveTowards(transform.position, new Vector2(player.transform.position.x, transform.position.y), movementSpeed * Time.deltaTime); ;
        } else 
            isInPlayerRange = false;
    }

    void Jump() { 
        GetComponent<Rigidbody2D>().AddForce(new Vector2 (0, jumpForce));
    }

    void NormalAttack() {
        if (normalAttackIsAllowed) {
        GameObject projectile = new GameObject();

        projectile.transform.position = transform.position;
        projectile.AddComponent<Rigidbody2D>();
        projectile.AddComponent<SpriteRenderer>();
        projectile.AddComponent<CircleCollider2D>();
        projectile.AddComponent<BulletHandler>();

        projectile.GetComponent<Rigidbody2D>().gravityScale = 0;
        projectile.GetComponent<CircleCollider2D>().isTrigger = true;
        projectile.GetComponent<SpriteRenderer>().sprite = projectileSprite;
        projectile.name = "Projectile";
        //projectile.transform.SetParent(transform, true);

            Vector2 diff = player.transform.position - transform.position;
        if (Mathf.Abs(diff.x) >= Mathf.Abs(diff.y) && Mathf.Abs(diff.x)>1)
        {
            diff = diff / Mathf.Abs(diff.x);
        } else if(Mathf.Abs(diff.x) < Mathf.Abs(diff.y) && Mathf.Abs(diff.y) > 1)
            diff = diff / Mathf.Abs(diff.y);

        projectile.GetComponent<Rigidbody2D>().velocity = (diff*projectileSpeed);

        addNormalAttackCooldown(timeBeetweenEachNormalAttack);
             }
    }

    void StompAttack()
    {
        if (stompAttackIsAllowed)
        {
            GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 650f));
            isInStompingAction = true;

            addStompAttackCooldown(timeBeetweenEachStompAttack);
        }
    }

    void TurretAttack()
    {
        if(turretAttackIsAllowed) { 
        isSpawningTurretOnNextCollision = true;

        addTurretAttackCooldown(timeBeetweenEachTurretAttack);
        }
    }

    void addNormalAttackCooldown(float seconds)
    {
        normalAttackIsAllowed = false;
        normalAttackCooldownLeft += seconds;
    }

    void addStompAttackCooldown(float seconds)
    {
        stompAttackIsAllowed = false;
        stompAttackCooldownLeft += seconds;
    }

    void addTurretAttackCooldown(float seconds)
    {
        turretAttackIsAllowed = false;
        turretAttackCooldownLeft += seconds;
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, siteRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, stopMoveIfInRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius); 
            Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, moveAwayIfPlayerInRadius);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //deal dmg to player
        }

        if (collision.gameObject.tag == "Weapon")
        {
            //deal dmg to self
        }

        if (isInStompingAction)
        {
            isInStompingAction=false;
            //spawn shockwave at this point

            GameObject shockwave = new GameObject();
            shockwave.AddComponent<Rigidbody2D>();
            shockwave.AddComponent<SpriteRenderer>();
            shockwave.AddComponent<CircleCollider2D>();
            shockwave.AddComponent<ShockwaveBehaviour>();

            shockwave.GetComponent<Rigidbody2D>().gravityScale = 0;
            shockwave.GetComponent<CircleCollider2D>().isTrigger = true;
            shockwave.GetComponent<SpriteRenderer>().sprite = shockwaveSprite;
            shockwave.transform.position = collision.GetContact(0).point;
            shockwave.name = "Shockwave";
            shockwave.GetComponent<SpriteRenderer>().color = Color.blue;
            //shockwave.transform.SetParent(transform, true);
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (isSpawningTurretOnNextCollision)
        {
            isSpawningTurretOnNextCollision = false;

            GameObject turret = new GameObject();
            turret.AddComponent<Rigidbody2D>();
            turret.AddComponent<SpriteRenderer>();
            turret.AddComponent<CircleCollider2D>();
            turret.AddComponent<TurretBehaviour>();

            turret.GetComponent<TurretBehaviour>().projectileSprite = projectileSprite;
            turret.GetComponent<Rigidbody2D>().gravityScale = 0;
            turret.GetComponent<CircleCollider2D>().isTrigger = true;
            turret.GetComponent<SpriteRenderer>().sprite = turretSprite;
            turret.transform.position = collision.GetContact(0).point;
            turret.name = "Turret";
            turret.GetComponent<SpriteRenderer>().color = Color.green;
            //turret.transform.SetParent(transform, true);

        }
    }

    // probably spawn turrets? that shoots, u can attack them and detroy them or they despawn after 30s
}