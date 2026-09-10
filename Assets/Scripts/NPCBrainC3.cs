using UnityEngine;
using UnityEngine.AI;

public class NPCBrainC3 : MonoBehaviour
{
    public enum NPCState
    {
        Patrol,
        Chase,
        Search
    }

    [Header("References")]
    [SerializeField]
    private NPCSensor sensor;

    [SerializeField]
    private NavMeshAgent agent;

    [Header("Patrol Settings")]
    [SerializeField]
    private Transform[] patrolPoints;

    [SerializeField]
    private float waypointTolerance = 0.7f;

    [SerializeField]
    private float patrolSpeed = 2f;

    // Challenge 1
    [SerializeField]
    private float waypointWaitTime = 2f;

    private float patrolTimer = 0f;
    private bool isWaiting = false;
    // 

    // Challenge 2
    [Header("Search Rotation Settings")]
    [SerializeField] private float lookAngle = 45f;
    [SerializeField] private float rotationSpeed = 3f;

    private enum SearchSubState { MovingToPoint, LookLeft, LookRight, Wait }
    private SearchSubState searchSubState;
    private Quaternion targetRotation;
    private Quaternion originalRotation;
    private float waitTimer;
    //

    // Challenge 3
    [Header("Alert & Confusion Indicators")]
    [SerializeField] private GameObject chaseIndicator;
    [SerializeField] private GameObject searchIndicator;
    //

    [Header("Chase Settings")]
    [SerializeField]
    private float chaseSpeed = 4f;

    [Header("Search Settings")]
    [SerializeField]
    private float searchDuration = 4f;

    [SerializeField]
    private float searchTolerance = 0.8f;

    [Header("Debug")]
    [SerializeField]
    private NPCState currentState;

    private NPCState previousState;

    private int patrolIndex = 0;

    // =============================
    // MEMORY
    // =============================

    private Vector3 lastKnownPosition;

    private bool hasLastKnownPosition;

    private float searchTimer;

    // Challenge 3
    private void UpdateIndicators()
    {
        if (chaseIndicator != null)
        {
            chaseIndicator.SetActive(currentState == NPCState.Chase);
        }

        if (searchIndicator != null)
        {
            searchIndicator.SetActive(currentState == NPCState.Search);
        }
    }
    //
    private void Start()
    {
        currentState = NPCState.Patrol;
        previousState = currentState;

        UpdateIndicators(); // Challenge 3
        GoToCurrentPatrolPoint();
    }

    private void Update()
    {
        UpdateMemory();

        MakeDecision();

        ExecuteCurrentState();
    }

    // ======================================
    // MEMORY
    // ======================================

    private void UpdateMemory()
    {
        if (sensor.CanSeePlayer)
        {
            lastKnownPosition =
                sensor.Player.position;

            hasLastKnownPosition = true;
        }
    }

    // ======================================
    // DECISION
    // ======================================

    private void MakeDecision()
    {
        // PRIORITAS 1
        // PLAYER TERLIHAT
        if (sensor.CanSeePlayer)
        {
            ChangeState(
                NPCState.Chase
            );

            return;
        }

        // PRIORITAS 2
        // PLAYER BARU HILANG
        if (currentState ==
                NPCState.Chase &&
            hasLastKnownPosition)
        {
            searchTimer =
                searchDuration;

            // Challenge 2
            searchSubState = SearchSubState.MovingToPoint;
            //

            ChangeState(
                NPCState.Search
            );

            return;
        }

        // PRIORITAS 3
        // SEARCH SELESAI
        if (currentState ==
                NPCState.Search &&
            searchTimer <= 0f)
        {
            hasLastKnownPosition =
                false;

            ChangeState(
                NPCState.Patrol
            );
        }
    }

    // ======================================
    // ACTION
    // ======================================

    private void ExecuteCurrentState()
    {
        switch (currentState)
        {
            case NPCState.Patrol:

                Patrol();
                break;

            case NPCState.Chase:

                Chase();
                break;

            case NPCState.Search:

                Search();
                break;
        }
    }

    // ======================================
    // PATROL
    // ======================================

    private void Patrol()
    {
        agent.speed = patrolSpeed;

        if (patrolPoints == null ||
            patrolPoints.Length == 0)
        {
            return;
        }

        // Challenge 1
        if (isWaiting)
        {
            patrolTimer -= Time.deltaTime;

            if (patrolTimer <= 0f)
            {
                isWaiting = false;
                patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
                GoToCurrentPatrolPoint();
            }
            return;
        }

        if (!agent.pathPending &&
            agent.remainingDistance <=
            waypointTolerance)
        {

            isWaiting = true;
            patrolTimer = waypointWaitTime;
            agent.ResetPath();
        }
        //
    }

    private void GoToCurrentPatrolPoint()
    {
        if (patrolPoints == null ||
            patrolPoints.Length == 0)
        {
            return;
        }

        agent.SetDestination(
            patrolPoints[
                patrolIndex
            ].position
        );
    }

    // ======================================
    // CHASE
    // ======================================

    private void Chase()
    {
        agent.speed =
            chaseSpeed;

        if (sensor.Player == null)
            return;

        agent.SetDestination(
            sensor.Player.position
        );
    }

    // ======================================
    // SEARCH
    // ======================================

    private void Search()
    {
        // Challenge 2
        switch (searchSubState)
        {
            case SearchSubState.MovingToPoint:
                agent.speed = patrolSpeed;
                agent.SetDestination(lastKnownPosition);

                if (!agent.pathPending && agent.remainingDistance <= searchTolerance)
                {
                    agent.ResetPath();
                    originalRotation = transform.rotation;
                    targetRotation = originalRotation * Quaternion.Euler(0f, -lookAngle, 0f);
                    searchSubState = SearchSubState.LookLeft;
                }
                break;

            case SearchSubState.LookLeft:
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                if (Quaternion.Angle(transform.rotation, targetRotation) < 2f)
                {
                    targetRotation = originalRotation * Quaternion.Euler(0f, lookAngle, 0f);
                    searchSubState = SearchSubState.LookRight;
                }
                break;

            case SearchSubState.LookRight:
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                if (Quaternion.Angle(transform.rotation, targetRotation) < 2f)
                {
                    targetRotation = originalRotation;
                    waitTimer = 1.5f;
                    searchSubState = SearchSubState.Wait;
                }
                break;

            case SearchSubState.Wait:
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                waitTimer -= Time.deltaTime;

                if (waitTimer <= 0f)
                {
                    searchTimer = 0f;
                }
                break;
        }
        //
    }

    // ======================================
    // STATE TRANSITION
    // ======================================

    private void ChangeState(
        NPCState newState
    )
    {
        if (currentState ==
            newState)
        {
            return;
        }

        previousState =
            currentState;

        currentState =
            newState;

        Debug.Log(
            gameObject.name +
            ": " +
            previousState +
            " -> " +
            currentState
        );

        UpdateIndicators(); // Challenge 3

        if (currentState ==
            NPCState.Patrol)
        {
            GoToCurrentPatrolPoint();
        }
    }

    // ======================================
    // DEBUG GIZMOS
    // ======================================

    private void OnDrawGizmosSelected()
    {
        switch (currentState)
        {
            case NPCState.Patrol:

                Gizmos.color =
                    Color.green;
                break;

            case NPCState.Chase:

                Gizmos.color =
                    Color.red;
                break;

            case NPCState.Search:

                Gizmos.color =
                    Color.blue;
                break;
        }

        Gizmos.DrawWireSphere(
            transform.position,
            0.8f
        );

        if (hasLastKnownPosition)
        {
            Gizmos.color =
                Color.magenta;

            Gizmos.DrawSphere(
                lastKnownPosition,
                0.3f
            );

            Gizmos.DrawLine(
                transform.position,
                lastKnownPosition
            );
        }
    }
}