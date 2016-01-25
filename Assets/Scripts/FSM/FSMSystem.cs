
#define DEBUG_LOG

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// TODO: Rewrite this whole file.

/// <summary>
/// FSMSystem class represents the Finite State Machine class.
///  It has a List with the States the NPC has and methods to add,
///  delete a state, and to change the current state the Machine is on.
/// </summary>
public abstract class FSMSystem
{
    //private List<FSMState> states;
    private Dictionary<int, FSMState> states;

    // The only way one can change the state of the FSM is by performing a transition
    // Don't change the CurrentState directly
    private int currentStateID;
    public int CurrentStateID { get { return currentStateID; } }

    private FSMState currentState;
    public FSMState CurrentState { get { return currentState; } }

    private FSMState previousState;
    public FSMState PreviousState { get { return previousState; } }

    public GameObject Actor;

    //public enum States { None = 0 }
    //public enum SubStates { None = 0}
    
    public FSMSystem(GameObject actor)
    {
        Actor = actor;
        
        states = new Dictionary<int, FSMState>();
    }

    /// <summary>
    /// This method places new states inside the FSM,
    /// or prints an ERROR message if the state was already inside the List.
    /// First state added is also the initial state.
    /// </summary>
    public void AddState(FSMState state)
    {
        // Check for Null reference before deleting
        if (state == null)
        {
            Debug.LogError("FSM ERROR: Null reference is not allowed");
        }

        state.SetFSM(this);
        states.Add(state.ID, state);
        
        Log.Make("Added state " + state.ToString() + " ( " + state.ID + ")");


        // First State inserted is also the Initial state,
        //   the state the machine is in when the simulation begins
        if (states.Count == 1)
        {
            ChangeState(state.ID);
        }
    }

    /// <summary>
    /// This method delete a state from the FSM List if it exists, 
    ///   or prints an ERROR message if the state was not on the List.
    /// </summary>
    public void DeleteState(int state)
    {
        if (states.ContainsKey(state))
            states.Remove(state);
    }

    /// <summary>
    /// This method tries to change the state the FSM is in based on
    /// the current state and the transition passed. If current state
    ///  doesn't have a target state for the transition passed, 
    /// an ERROR message is printed.
    /// </summary>
   // public void ChangeState(int state, object[] parameters)
   public void ChangeState(int state, FSMState.DataPackageBase data)
    {
        if(!states.ContainsKey(state))
        {
            Debug.LogError("FSM ERROR: No definition for " + state.ToString());
            return;
        }

        if (currentState != null)
        {
            currentState.OnLeave();
        }

        previousState = currentState;
        
        currentState = states[state];
        
        currentState.OnEnter(data);
    }

    public void ChangeState(int state)
    {
        ChangeState(state, null);
    }

    public virtual void Update()
    {
        CurrentState.Update();
    }

    /// <summary>
    /// TODO: Write summary
    /// </summary>
    public abstract class FSMState
    { 
        // Reference to the system we're contained within.
        protected FSMSystem FSM;

        //protected Dictionary<Edge, StateID> map = new Dictionary<Edge, StateID>();
        protected int stateID;
        //protected SubStateID subStateID;

        // Used to Augment a state based on what it's transitioned from.
        //protected int previousStateID;
        //protected SubStates augmentSubStateID;

        public int ID { get { return stateID; } }
       // public virtual SubStates GetSubState() { return SubStates.None; }

        // If set (during update), ProcessStateChange will change state.
        protected int? DestinationState = null;
        //object[] DestinationStateParameters = null;
        protected DataPackageBase DestinationStateDataPackage = null;


        // Called in FSMSystem.AddState
        public void SetFSM(FSMSystem fsm)
        {
            FSM = fsm;
        }
        
        //public virtual void OnEnter(object[] parameters) { }

        public virtual void OnEnter(DataPackageBase data) { }

        public void OnEnter()
        {
            OnEnter(null);
        }

        /// <summary>
        /// This method is used to make anything necessary, as reseting variables
        /// before the FSMSystem changes to another one. It is called automatically
        /// by the FSMSystem before changing to a new state.
        /// </summary>
        public virtual void OnLeave() { }

        /// <summary>
        /// Queues up a state to be transitioned to after the current Update.
        /// </summary>
        /// <param name="state"></param>
       // public void ChangeState(int state, params object[] parameters)
        public void ChangeState(int state, DataPackageBase data)
        {
            Log.Make("Queuing change from " + stateID + " to " + state);

            DestinationState = state;
            // DestinationStateParameters = parameters;
            DestinationStateDataPackage = data;
        }

        /// <summary>
        /// State logic for the update loop. Implemented by the inheriting state.
        /// </summary>
        protected abstract void OnUpdate();

        /// <summary>
        /// Process any state changes that were made during the Update function.
        /// </summary>
        protected virtual void ProcessStateChange()
        {
            if (DestinationState != null)
            {
                try
                {
                    FSM.ChangeState((int)DestinationState, DestinationStateDataPackage);
                }
                catch(Exception ex)
                {
                    Debug.LogError("Error in FSM.ChangeState: " + ex.ToString());
                }

                DestinationState = null;
                DestinationStateDataPackage = null;
            }
        }

        /// <summary>
        /// Encapsulates the OnUpdate() and ProcessStateChange() calls.
        /// </summary>
        public void Update()
        {
            OnUpdate();
            
            ProcessStateChange();
        }

        public abstract class DataPackageBase { }
    }

    public abstract class FSMSnapbackState : FSMState
    {
        public abstract bool IsDone();
        
        protected override void ProcessStateChange()
        {
            if (IsDone())
            {
                FSM.ChangeState(FSM.previousState.ID);
            }
        }
    }

}