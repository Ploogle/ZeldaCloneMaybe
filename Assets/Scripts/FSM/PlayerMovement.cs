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
        FSM = new PlayerFSM(gameObject);

        PlayerFSM.MoveState move = new PlayerFSM.MoveState();
        
        PlayerFSM.AttackState attack = new PlayerFSM.AttackState();

        FSM.AddState(move);
        FSM.AddState(attack);
    }

    void Update()
    {
        FSM.Update();
    }
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

    //public class SubStates
    //{
    //    public const int IDLE = 0;
    //    public const int WALK = 1;
    //    public const int RUN = 2;
    //    public const int JUMP = 3;
    //}

    public class MoveState : FSMState
    {
        Transform myTransform;
        Rigidbody myRigidbody;
        float MOVEMENT_SPEED = 2.5f;
        float RUNNING_SPEED = 5f;
        float ROTATION_SPEED = 10f;

        bool isRunning = false;

        float inputTimer = 0f;
        float idleTimer = 0f;
        float DOUBLE_TAP_TIME = .1f;

        public MoveState()
        {
            stateID = PlayerStates.MOVE;
        }

        public override void OnEnter(DataPackageBase data)
        {
            if(myTransform == null)
                myTransform = FSM.Actor.transform;

            if(myRigidbody == null)
                myRigidbody = FSM.Actor.GetComponent<Rigidbody>();
        }

        public override void OnLeave()
        {

        }

        protected override void OnUpdate()
        {
            Quaternion facingTarget;
            Quaternion currentRotation;
            Vector3 moveDirection;

            // Relative to world X and Z axes.
            moveDirection = Vector3.forward * Input.GetAxisRaw("Vertical") +
                            Vector3.right * Input.GetAxisRaw("Horizontal");

            if (moveDirection.sqrMagnitude > 0)
            {
                //Log.Make("inputTimer: " + inputTimer);

                if(idleTimer > 0 && inputTimer > 0)
                {
                    isRunning = true;

                    Log.Make("running");
                }

                inputTimer += Time.deltaTime;
                idleTimer = 0;
            }
            else
            {
                idleTimer += Time.deltaTime;
                //Log.Make("idleTimer: " + idleTimer);
                isRunning = false;

                Log.Make("not running");

                if(idleTimer > DOUBLE_TAP_TIME)
                    inputTimer = 0;
            }

            currentRotation = myTransform.rotation;
            myTransform.LookAt(myTransform.position + moveDirection);
            facingTarget = myTransform.rotation;
            myTransform.rotation = Quaternion.Lerp(currentRotation, facingTarget, ROTATION_SPEED * Time.deltaTime);
            
            if (moveDirection != Vector3.zero)
                myRigidbody.MovePosition(myTransform.position + moveDirection * (isRunning ? RUNNING_SPEED : MOVEMENT_SPEED ) * Time.deltaTime);

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

    //public class JumpState : FSMState
    //{
    //    public override void OnEnter(DataPackageBase data)
    //    {

    //    }

    //    protected override void OnUpdate()
    //    {

    //    }
    //}

    //public class AttackState : FSMState
    public class AttackState : FSMSnapbackState
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

        public override void OnLeave()
        {

        }

        protected override void OnUpdate()
        {
            Log.Make("OnUpdate");
            counter += Time.deltaTime;

            if(counter < .1f)
            {
                myRigidbody.MovePosition(myRigidbody.transform.position + myRigidbody.transform.forward * (AttackType == AttackInput.LIGHT ? .1f : .25f));
            }

            //if (counter > (AttackType == AttackInput.LIGHT ? .2f : .4f))
            //{
            //    ChangeState(PlayerStates.MOVE, null);
            //}
        }

        public override bool IsDone()
        {
            Log.Make("Check...");
            return (counter > (AttackType == AttackInput.LIGHT ? .2f : .4f));
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


