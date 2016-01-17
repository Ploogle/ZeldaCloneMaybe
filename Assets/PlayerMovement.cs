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
        FSM = new PlayerFSM(gameObject);

        PlayerFSM.MoveState move = new PlayerFSM.MoveState();

        //PlayerFSM.JumpState jump = new PlayerFSM.JumpState();

        PlayerFSM.AttackState attack = new PlayerFSM.AttackState();

        FSM.AddState(move);
        FSM.AddState(attack);
    }

    void Update()
    {
        FSM.Update();
    }

	//void Update()
 //   {
 //       Quaternion facingTarget;
 //       Quaternion currentRotation;
 //       Vector3 moveDirection;

 //       // Relative to world X and Z axes.
 //       moveDirection = Vector3.forward * Input.GetAxisRaw("Vertical") +
 //                       Vector3.right * Input.GetAxisRaw("Horizontal");

 //       currentRotation = transform.rotation;
 //       transform.LookAt(transform.position + moveDirection);
 //       facingTarget = transform.rotation;
 //       transform.rotation = Quaternion.Lerp(currentRotation, facingTarget, rotationSpeed);

 //       //if (moveDirection != Vector3.zero)
 //       //    myRigidbody.MovePosition(transform.position + transform.forward * movementSpeed);

 //       if (moveDirection != Vector3.zero)
 //           myRigidbody.MovePosition(transform.position + moveDirection * movementSpeed);
 //   }
}

public class PlayerFSM : FSMSystem
{
    public PlayerFSM(GameObject actor) : base(actor) { }

    public class PlayerStates
    {
        public const int NONE = 0;
        public const int MOVE = 1;
        public const int ATTACK = 2;
    }

    public class AttackInput
    {
        public const int LIGHT = 0;
        public const int HEAVY = 1;
    }

    public new enum SubStates
    {
        Idle,
        Walk,
        Run,
        Jump
    }

    public class MoveState : FSMState
    {
        Transform myTransform;
        Rigidbody myRigidbody;
        float movementSpeed = 2.5f;
        float rotationSpeed = 10f;//.5f;

        public MoveState()
        {
            stateID = PlayerStates.MOVE;
        }

        public override void OnEnter(DataPackageBase data)
        {
            //base.OnEnter();

            if(myTransform == null)
                myTransform = FSM.Actor.transform;

            if(myRigidbody == null)
                myRigidbody = FSM.Actor.GetComponent<Rigidbody>();
        }

        //public override void CheckEdges(GameObject actor)
        //{
        //    // throw new NotImplementedException();
        //}

        protected override void OnUpdate()
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
            myTransform.rotation = Quaternion.Lerp(currentRotation, facingTarget, rotationSpeed * Time.deltaTime);

            //if (moveDirection != Vector3.zero)
            //    myRigidbody.MovePosition(transform.position + transform.forward * movementSpeed * Time.deltaTime);

            if (moveDirection != Vector3.zero)
                myRigidbody.MovePosition(myTransform.position + moveDirection * movementSpeed * Time.deltaTime);

            if (Input.GetButtonDown("Light Attack"))
            {
                Log.Make("Light Attack");
                
                ChangeState(PlayerStates.ATTACK, new AttackState.DataPackage(inputType: AttackInput.LIGHT));
            }
            else if (Input.GetButtonDown("Heavy Attack"))
            {
                Log.Make("Heavy Attack");

                ChangeState(PlayerStates.ATTACK, new AttackState.DataPackage(inputType: AttackInput.HEAVY));
            }

        }

        
    }

    public class JumpState : FSMState
    {
        public override void OnEnter(DataPackageBase data)
        {
           // base.OnEnter();
        }

        protected override void OnUpdate()
        {

        }
    }

    public class AttackState : FSMState
    {
        float counter = 0;

        Rigidbody myRigidbody;

        int AttackType = 0;

        public AttackState()
        {
            stateID = PlayerStates.ATTACK;
        }

        public override void OnEnter(DataPackageBase data)
        {
            DataPackage package = data as DataPackage;

            counter = 0;

            if (myRigidbody == null)
                myRigidbody = FSM.Actor.GetComponent<Rigidbody>();

            AttackType = package.InputType;
        }

        protected override void OnUpdate()
        {
            counter += Time.deltaTime;

            if(counter < .1f)
            {
                myRigidbody.MovePosition(myRigidbody.transform.position + myRigidbody.transform.forward * (AttackType == AttackInput.LIGHT ? .1f : .25f));
            }

            if (counter > (AttackType == AttackInput.LIGHT ? .2f : .4f))
            {
                ChangeState(PlayerStates.MOVE, null);
            }
        }

        public class DataPackage : DataPackageBase
        {
            public int InputType;

            public DataPackage(int inputType = AttackInput.LIGHT)
            {
                InputType = inputType;
            }
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


