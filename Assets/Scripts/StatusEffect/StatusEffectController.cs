using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GodUnityPlugin
{
    public class StatusEffectController
    {
        protected List<StatusEffect> currentEffects = new List<StatusEffect>();

        int index;
        bool requireFinish;
        public StatusEffectController()
        {
        }

        public void ManualUpdate()
        {
            index = 0;
            while (index < currentEffects.Count)
            {
                StatusEffect effect = currentEffects[index];

                effect.OnUpdateEffect(out requireFinish);

                if (requireFinish)
                    FinishEffect(effect);
                else
                    index++;
            }
        }
        public void StartEffect(StatusEffect statusEffect)
        {
            if (currentEffects.Contains(statusEffect))
                return;

            currentEffects.Add(statusEffect);
            statusEffect.OnStartEffect(); 
        }
        public void FinishEffect(StatusEffect statusEffect)
        {
            if (!currentEffects.Contains(statusEffect))
                return;

            statusEffect.OnFinishEffect();

            ClearEffect(statusEffect);
        }

        public void FinishAllEffects()
        {
            for (int i = 0; i < currentEffects.Count; i++)
            {
                StatusEffect effect = currentEffects[i];
                effect.OnFinishEffect();
            }

            ClearAllEffects();
        }

        public void ClearAllEffects()
        {
            currentEffects.Clear();
        }
        private void ClearEffect(StatusEffect statusEffect)
        {
            if (currentEffects.Contains(statusEffect))
                currentEffects.Remove(statusEffect);
        }
    }
}