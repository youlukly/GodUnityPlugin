using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGUPState
{
    string id { get; }

    void OnEnter();
    void OnExit();
    void OnUpdate();
    void OnLateUpdate();
    void OnFixedUpdate();

    bool IsTransition(out string ID);
}
