using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Вражеский бот преследующий Defender
/// </summary>
public class RunBot : MonoBehaviour
{
    // State of Bot's AI.
    public enum BotState
    {
        GoToAttackPoint,
        AttackAndFollowTarget,
        Attack,
        MoveAwayFromTarget,
    }

    public float movementSpeed = 5f;
    public float targetStoppingDistance = 8f;
    public float targetStoppingDistanceRandomRange = 1.5f;
    public float stopAttackDistance = 14f;
    public float minDistanceToTarget = 5f;

    [SerializeField] private Animator animator;
    [SerializeField] private Weapon weapon;
    [SerializeField] private BotState state;
    private Transform attackPoint;
    private bool isMoving;
    private float randomizedTargetStoppingDistance;
    private Vector3 randomizedRelativeTargetTargetPosition;

    private void Start()
    {
        ChangeState(BotState.GoToAttackPoint);

    }

    private void Update()
    {
        Think();
        Animate();
    }

    public void SetAttackPoint(Transform newAttackPoint)
    {
        attackPoint = newAttackPoint;
    }

    /// <summary>
    /// Kills the bot.
    /// </summary>
    public void Die()
    {
        animator.SetTrigger("Death");
        Destroy(this,0.5f);
        Destroy(this.gameObject, 1.5f);
    }

    /// <summary>
    /// AI of the bot. (Some kind of Strategy programming pattern).
    /// </summary>
    private void Think()
    {
        switch(state)
        {
            case BotState.GoToAttackPoint:
            if(attackPoint != null)
            GoToAttackPoint();
            break;

            case BotState.AttackAndFollowTarget:
            if(FireCover.Instance.mainDefender != null)
            AttackAndFollowTarget();
            break;

            case BotState.Attack:
            if(FireCover.Instance.mainDefender != null)
            Attack();
            break;

            case BotState.MoveAwayFromTarget:
            MoveAwayFromTarget();
            break;

            default:
            break;
        }
    }

    /// <summary>
    /// Changes the state of Bot's AI.
    /// </summary>
    private void ChangeState(BotState newState)
    {
        state = newState;

        RandomizeStoppingDistance();

        switch(newState)
        {
            case BotState.GoToAttackPoint:
            isMoving = true;
            animator.SetTrigger("Started Attack");
            break;

            case BotState.AttackAndFollowTarget:
            animator.SetTrigger("Started Attack");
            isMoving = true;
            break;

            case BotState.Attack:
            animator.SetTrigger("Stopped Attack");
            isMoving = false;
            break;

            case BotState.MoveAwayFromTarget:
            animator.SetTrigger("Stopped Attack");
            isMoving = true;
            break;

            default:
            break;
        }
    }

    private void GoToAttackPoint()
    {
        float dist = Vector3.Distance(transform.position, attackPoint.position);

        if(dist < 0.05f)
        {
            ChangeState(BotState.AttackAndFollowTarget);

            return;
        }

        MoveTowards(attackPoint.position);
    }

    private void AttackAndFollowTarget()
    {
        Enemy defender = FireCover.Instance.mainDefender;
        float dist = GetDistanceToDefender();
        Vector3 targetPosition = defender.transform.TransformPoint(randomizedRelativeTargetTargetPosition);
        targetPosition.y = transform.position.y;

        if(dist >= minDistanceToTarget && dist <= randomizedTargetStoppingDistance)
        {
            ChangeState(BotState.Attack);
            return;
        }

        if(dist < minDistanceToTarget)
        {   
            ChangeState(BotState.MoveAwayFromTarget);
            return;
        }

        if(Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            ChangeState(BotState.Attack);
            return;
        }
        
        MoveTowards(targetPosition);
    }

    private void MoveAwayFromTarget()
    {
        Enemy defender = FireCover.Instance.mainDefender;
        float dist = GetDistanceToDefender();

        if(dist < randomizedTargetStoppingDistance)
        {
            Vector3 dirFromDefender = (transform.position - defender.transform.position).normalized;
            Vector3 targetPosition = defender.transform.position + dirFromDefender * 999f;
            MoveTowards(targetPosition);
        }
        else
        {
            ChangeState(BotState.Attack);
            return;
        }
    }

    private void Attack()
    {
        Enemy defender = FireCover.Instance.mainDefender;
        float dist = GetDistanceToDefender();

        if(dist > stopAttackDistance)
        {
            ChangeState(BotState.AttackAndFollowTarget);
            return;
        }
        else if(dist < minDistanceToTarget)
        {
            ChangeState(BotState.MoveAwayFromTarget);
            return;
        }

        LookAt(defender.transform.position);
        weapon.SetTargetPos(defender.transform);
        weapon.Shot();
    }

    /// <summary>
    /// Moves bot towards a target.
    /// </summary>
    private void MoveTowards(Vector3 targetPosition)
    {
        Vector3 newPosition = Vector3.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);
        newPosition.y = transform.position.y;
        transform.position = newPosition;

        LookAt(targetPosition);
    }

    /// <summary>
    /// Rotates bot so it looks at target.
    /// </summary>
    private void LookAt(Vector3 target)
    {
        transform.LookAt(target);

        // Make sure that unit is not rotation on X and Z axes.
        transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);
    }

    private void Animate()
    {
        float speed = isMoving ? 1f : 0f;
        animator.SetFloat("Speed", speed);
        //bool shooting = state == BotState.Attack || state == BotState.AttackAndFollowTarget;
        //animator.SetBool("IsShooting", shooting);
    }

    private void RandomizeStoppingDistance()
    {
        float newValue = targetStoppingDistance + Random.Range(
        -randomizedTargetStoppingDistance / 2f, randomizedTargetStoppingDistance / 2f);

        newValue = Mathf.Clamp(newValue, minDistanceToTarget, float.MaxValue);

        randomizedTargetStoppingDistance = newValue;

        Transform defender = FireCover.Instance.mainDefender.transform;

        Vector2 random = Random.insideUnitCircle.normalized;
        Vector3 randomPosition = defender.position;
        randomPosition += new Vector3(random.x * randomizedTargetStoppingDistance * 0.9f, 0, random.y * randomizedTargetStoppingDistance * 0.9f);

        randomizedRelativeTargetTargetPosition = defender.InverseTransformPoint(randomPosition);
    }

    private float GetDistanceToDefender()
    {
        Vector3 defenderPosition = FireCover.Instance.mainDefender.transform.position;
        defenderPosition.y = transform.position.y;
        return Vector3.Distance(defenderPosition, transform.position);
    }
}
