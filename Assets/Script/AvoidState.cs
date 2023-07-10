using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using System.Threading.Tasks;
using static UnityEngine.GraphicsBuffer;

public class AvoidState : StateMachineBehaviour
{
    PetManager petManager;
    NavMeshAgent petAgent;
    Transform player;
    float avoidanceDistance = 3f;
    Animator petAnimation;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        petManager = animator.GetComponent<PetManager>();
        petAgent = animator.GetComponent<NavMeshAgent>();
        petAnimation = animator.GetComponent<Animator>();
        player = petManager.player;
        petManager.timerToAvoid = 5;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        petManager.timerToAvoid -= Time.deltaTime;
        Vector3 targetDirection = player.position - petAgent.transform.position;
        float distanceToTarget = targetDirection.magnitude;

        // Check if the target is within the avoidance distance
        if (distanceToTarget < avoidanceDistance)
        {
            // Calculate the desired velocity to avoid the target
            Vector3 desiredVelocity = -targetDirection.normalized * petAgent.speed;

            // Set the avoidance velocity for the NavMesh agent
            petAgent.velocity = desiredVelocity;
        }
        if (petManager.timerToAvoid <= 0)
        {
            petAnimation.SetBool("Patrol", true);
            petAnimation.SetBool("Avoid", false);

        }



        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    
        //}

        // OnStateMove is called right after Animator.OnAnimatorMove()
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that processes and affects root motion
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK()
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that sets up animation IK (inverse kinematics)
        //}
    }
}
