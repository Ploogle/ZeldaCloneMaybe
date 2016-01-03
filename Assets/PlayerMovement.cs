using UnityEngine;
using System.Collections;
using System;

public class PlayerMovement : MonoBehaviour {

    Rigidbody myRigidbody;
    float movementSpeed = .1f;
    float rotationSpeed = .3f;//.5f;

    FSMSystem FSM;
    
	void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();

        SetupFSM();
	}

    void SetupFSM()
    {
        //FollowPathState follow = new FollowPathState(path);
        //follow.AddTransition(Transition.SawPlayer, StateID.ChasingPlayer);

        //ChasePlayerState chase = new ChasePlayerState();
        //chase.AddTransition(Transition.LostPlayer, StateID.FollowingPath);

        PlayerFSM.MoveState move = new PlayerFSM.MoveState();

        PlayerFSM.JumpState jump = new PlayerFSM.JumpState();

        PlayerFSM.AttackState attack = new PlayerFSM.AttackState();

        FSM = new PlayerFSM(gameObject);
        //fsm.AddState(follow);
        //fsm.AddState(chase);
    }

	void Update()
    {
        Quaternion facingTarget;
        Quaternion currentRotation;
        Vector3 moveDirection;

        // Relative to world X and Z axes.
        moveDirection = Vector3.forward * Input.GetAxisRaw("Vertical") +
                        Vector3.right * Input.GetAxisRaw("Horizontal");

        currentRotation = transform.rotation;
        transform.LookAt(transform.position + moveDirection);
        facingTarget = transform.rotation;
        transform.rotation = Quaternion.Lerp(currentRotation, facingTarget, rotationSpeed);

        //if (moveDirection != Vector3.zero)
        //    myRigidbody.MovePosition(transform.position + transform.forward * movementSpeed);

        if (moveDirection != Vector3.zero)
            myRigidbody.MovePosition(transform.position + moveDirection * movementSpeed);
    }
}

public class PlayerFSM : FSMSystem
{
    public PlayerFSM(GameObject actor) : base(actor) { }

    public new enum States
    {
        Move,
        Attack,
        Jump
    }

    public class MoveState : FSMState
    {
        Transform myTransform;
        Rigidbody myRigidbody;
        float movementSpeed = .1f;
        float rotationSpeed = .3f;//.5f;

        public override void OnEnter()
        {
            base.OnEnter();

            myTransform = FSM.Actor.transform;

            myRigidbody = FSM.Actor.GetComponent<Rigidbody>();
        }

        public override void CheckEdges(GameObject actor)
        {
            // throw new NotImplementedException();
        }

        public override void Act(GameObject actor)
        {
            Quaternion facingTarget;
            Quaternion currentRotation;
            Vector3 moveDirection;

            // Relative to world X and Z axes.
            moveDirection = Vector3.forward * Input.GetAxisRaw("Vertical") +
                            Vector3.right * Input.GetAxisRaw("Horizontal");

            currentRotation = myTransform.rotation;
            myTransform.LookAt(myTransform.position + moveDirection);
            facingTarget = myTransform.rotation;
            myTransform.rotation = Quaternion.Lerp(currentRotation, facingTarget, rotationSpeed);

            //if (moveDirection != Vector3.zero)
            //    myRigidbody.MovePosition(transform.position + transform.forward * movementSpeed);

            if (moveDirection != Vector3.zero)
                myRigidbody.MovePosition(myTransform.position + moveDirection * movementSpeed);
        }
    }

    public class JumpState : FSMState
    {
        public override void CheckEdges(GameObject actor)
        {
            //throw new NotImplementedException();
        }

        public override void Act(GameObject actor)
        {
            //throw new NotImplementedException();
        }
    }

    public class AttackState : FSMSnapbackState
    {
        public override bool ShouldLeave()
        {
            return false;
        }

        public override void Act(GameObject actor)
        {
            //throw new NotImplementedException();
        }
    }

    //// Potential future state
    //public class PlayerDamagedState : FSMSnapbackState
    //{
    //    public override bool ShouldLeave()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override void Act(GameObject actor)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}


