using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStatusEffect
{
    string id { get; }

    void OnStartEffect();
    void OnUpdateEffect();
    void OnFinishEffect();
    bool IsRequireFinish();
}
