using System;
using System.Collections.Generic;
using UnityEngine;

public class AbstractStateMachine<TState> where TState : AbstractState
{
    private Dictionary<Type, TState> m_states;

    private List<TState> m_defaultPipelineStates;
    private Queue<TState> m_queue;

    private TState m_currentState;
    private TState m_initialState;
    private int m_currentStateIndex;
    public TState CurrentState => m_currentState;

    public AbstractStateMachine(TState initialState)
    {
        m_states = new();
        m_defaultPipelineStates = new();
        m_queue = new();
        m_initialState = initialState;
    }

    public void Start()
    {
        m_currentState = m_initialState;
        m_currentStateIndex = 0;
    }

    public void Update()
    {
        m_currentState?.Update();
    }

    public void Stop()
    {
        m_currentState?.Stop();
        m_currentState = null;
    }

    public void NextState()
    {
        if (m_queue.TryDequeue(out TState state))
        {
            StartState(state);
            return;
        }

        if (m_currentStateIndex >= m_defaultPipelineStates.Count)
        {
            m_currentStateIndex = 0;
        }

        StartState(m_defaultPipelineStates[m_currentStateIndex]);
        m_currentStateIndex++;
    }

    public void SetNextState<T>() where T : TState
    {
        if(TryGetState(out T state))
        {
            m_queue.Enqueue(state);
        }
    }

    public void SetCurrentState<T>() where T : TState
    {
        if (TryGetState(out T state))
        {
            StartState(state);
        }
    }

    public void AddState<T>(T state, bool registerToDefaultPipeline = false) where T : TState
    {
        if (TryGetState(out T added))
        {
            Debug.LogError($"{typeof(T)} is already added");
            return;
        }

        m_states.Add(typeof(T), state);

        if (registerToDefaultPipeline)
        {
            m_defaultPipelineStates.Add(state);
        }
    }

    private bool TryGetState<T>(out T state) where T : TState
    {
        Type type = typeof(T);
        state = null;

        if (m_states.TryGetValue(type, out TState added))
        {
            state = (T)added;
            return true;
        }

        return false;
    }


    public void RemoveState<T>() where T : TState
    {
        if (!TryGetState(out T state))
        {
            Debug.LogError($"{typeof(T)} not found");
            return;
        }

        m_states.Remove(typeof(T));

        if (m_defaultPipelineStates.Contains(state))
        {
            m_defaultPipelineStates.Remove(state);
        }
    }

    private void StartState(TState state)
    {
        m_currentState?.Stop();
        m_currentState = state;
        m_currentState.Start();
    }
}

public abstract class AbstractState
{
    public abstract bool Finished { get; }
    public abstract void Start();
    public abstract void Update();
    public abstract void Stop();
}

