﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GodUnityPlugin
{
    public abstract class StatusEffect
    {
        public abstract void OnStartEffect();

        public abstract void OnUpdateEffect();

        public abstract void OnFinishEffect();
    }
}