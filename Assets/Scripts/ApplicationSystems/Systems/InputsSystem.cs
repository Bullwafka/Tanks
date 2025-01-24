using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputsSystem : MonoBehaviour, IApplicationSystem
{
    [SerializeField] private InputActionAsset m_inputActionAsset;
    private Dictionary<string, InputAction> m_inputActions = new();

    public void Initialize()
    {
        m_inputActionAsset.Enable();
        InputActionMap map = m_inputActionAsset.actionMaps[0];

        foreach (InputAction action in map.actions)
        {
            m_inputActions.Add(action.name, action);
        }
    }

    public InputAction GetInputAction(string name)
    {
        if(m_inputActions.TryGetValue(name, out InputAction action))
            return action;

        throw new System.Exception($"Input action with name {name} not found");
    }

    public void Shutdown() {}
}
