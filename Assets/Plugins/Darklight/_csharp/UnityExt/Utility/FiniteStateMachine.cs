using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.UnityExt.Utility
{
    public interface IState
    {
        /// <summary>
        /// Called when the state is entered.
        /// </summary>
        void Enter();

        /// <summary>
        /// Called when the state is exited.
        /// </summary>
        void Exit();

        /// <summary>
        /// Called when the state is executed.
        /// </summary>
        /// <remarks>
        /// This method should be called in the game's update loop.
        /// </remarks>
        void Execute();
    }

    [System.Serializable]
    public abstract class FiniteState<TState> : IState where TState : Enum
    {
        public TState stateType {get; private set;}
        public FiniteState(TState stateType)
        {
            this.stateType = stateType;
        }

        public abstract void Enter();
        public abstract void Exit();
        public abstract void Execute();
    }

    public abstract class FiniteStateMachine<TState> where TState : Enum
    {
        protected TState initialState;
        protected Dictionary<TState, FiniteState<TState>> possibleStates;
        protected FiniteState<TState> currentFiniteState;
        protected object[] args;

        public delegate void OnStateChange(TState state);
        public event OnStateChange OnStateChanged;

        public TState CurrentState { get { return currentFiniteState.stateType; } }

        public FiniteStateMachine() { }

        public FiniteStateMachine(Dictionary<TState, FiniteState<TState>> possibleStates, TState initialState, object[] args)
        {
            this.initialState = initialState;
            this.possibleStates = possibleStates;
            if (possibleStates.ContainsKey(initialState))
            {
                this.currentFiniteState = possibleStates[initialState];
            }
            this.args = args;
        }

        public void AddState(FiniteState<TState> finiteState)
        {
            if (possibleStates == null) { possibleStates = new Dictionary<TState, FiniteState<TState>>(); }
            possibleStates.Add(finiteState.stateType, finiteState);
        }

        public virtual void Step()
        {
            if (currentFiniteState != null) { currentFiniteState.Execute(); }
            else { GoToState(initialState); }
        }

        public virtual bool GoToState(TState state)
        {
            // Exit from the previous state
            if (currentFiniteState != null && currentFiniteState.stateType.Equals(state)) { return false; }
            if (currentFiniteState != null) { currentFiniteState.Exit(); }

            // Check if the state exists
            if (possibleStates.ContainsKey(state))
            {
                // Enter the new state
                currentFiniteState = possibleStates[state];
                currentFiniteState.Enter();
            }
            else
            {
                Debug.LogError($"State {state} not found in possible states.");
                return false;
            }

            // Invoke the OnStateChanged event
            OnStateChanged?.Invoke(state);
            return true;
        }
    }
}