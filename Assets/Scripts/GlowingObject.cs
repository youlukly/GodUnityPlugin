using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GodUnityPlugin
{
    public class GlowingObject : MonoBehaviour
    {
        public UnityEvent onStartGlow;
        public UnityEvent onFinishGlow;
        public UnityEvent onStartFade;
        public UnityEvent onFinishFade;

        public Color glowColor;

        public Renderer[] renderers;

        public float glowSpeed = 0.5f;
        public float fadeSpeed = 0.35f;

        public float maxIntensity = 6.0f;
        public float glowSmoothness = 100;
        public float fadeSmoothness = 100;

        public bool autoGlowAllMesh = true;

        private Material[] defaultMats;

        private void Awake()
        {
            if (autoGlowAllMesh)
            {
                Renderer[] renderers = GetComponents<Renderer>();
                Renderer[] childRenderers = GetComponentsInChildren<Renderer>();

                renderers = new Renderer[renderers.Length + childRenderers.Length];

                renderers.CopyTo(renderers, 0);
                childRenderers.CopyTo(renderers, renderers.Length);
            }
        }

        private void OnDisable()
        {
            defaultMats = null;
        }

        public IEnumerator Glow()
        {
            yield return StartCoroutine(Glow(glowSpeed, maxIntensity, glowSmoothness, glowColor));
        }

        public IEnumerator Glow(float glowSpeed, float maxIntensity, float smoothness, Color glowColor)
        {
            yield return StartCoroutine(StartGlow(glowSpeed, maxIntensity, glowSmoothness, glowColor));
        }

        public IEnumerator GlowAndFade(Color color)
        {
            yield return StartCoroutine(GlowAndFade(glowSpeed, maxIntensity, glowSmoothness, fadeSpeed, fadeSmoothness, color));
        }

        public IEnumerator GlowAndFade(float glowSpeed, float maxIntensity, float glowSmoothness, float fadeSpeed, float fadeSmoothness, Color color)
        {
            if (defaultMats == null)
            {
                defaultMats = new Material[renderers.Length];
                for (int i = 0; i < renderers.Length; i++)
                    defaultMats[i] = renderers[i].material;
            }

            yield return StartCoroutine(Glow(glowSpeed, maxIntensity, glowSmoothness, color));
            yield return StartCoroutine(Fade(fadeSpeed, fadeSmoothness, defaultMats));
        }

        private IEnumerator Fade(float fadeSpeed, float smoothness, Material[] defaultMats)
        {
            if (onStartFade != null)
                onStartFade.Invoke();

            float delay = fadeSpeed / smoothness;

            Color[] defaultColors = new Color[defaultMats.Length];

            for (int i = 0; i < defaultMats.Length; i++)
                defaultColors[i] = defaultMats[i].GetColor("_EmissionColor");

            if (fadeSpeed == 0.0f)
            {
                for (int j = 0; j < defaultColors.Length; j++)
                {
                    Color targetColor = defaultColors[j];

                    renderers[j].material.SetColor("_EmissionColor", targetColor);
                }

                yield break;
            }

            for (int i = 0; i < fadeSmoothness; i++)
            {
                float t = i / fadeSmoothness;

                for (int j = 0; j < defaultColors.Length; j++)
                {
                    Material glowMat = renderers[j].material;

                    Color color = glowMat.GetColor("_EmissionColor");

                    Color nextColor = Color.Lerp(color, defaultColors[j], t);

                    glowMat.SetColor("_EmissionColor", nextColor);
                }

                yield return new WaitForSeconds(delay);
            }

            for (int i = 0; i < renderers.Length; i++)
                renderers[i].material = defaultMats[i];

            if (onFinishFade != null)
                onFinishFade.Invoke();

            defaultMats = null;
        }

        private IEnumerator StartGlow(float glowSpeed, float maxIntensity, float smoothness, Color glowColor)
        {
            if (onStartGlow != null)
                onStartGlow.Invoke();

            Material[] glowMats = new Material[renderers.Length];

            for (int i = 0; i < renderers.Length; i++)
            {
                Material glowMat = new Material(renderers[i].material);
                glowMats[i] = glowMat;
                renderers[i].material = glowMat;
            }

            float delay = glowSpeed / glowSmoothness;

            Color[] defaultColors = new Color[glowMats.Length];

            for (int i = 0; i < glowMats.Length; i++)
                defaultColors[i] = glowMats[i].GetColor("_EmissionColor");

            if (glowSpeed == 0.0f)
            {
                for (int j = 0; j < glowMats.Length; j++)
                {
                    Color targetColor = glowColor * maxIntensity;

                    glowMats[j].SetColor("_EmissionColor", targetColor);
                }

                yield break;
            }

            for (int i = 0; i < glowSmoothness; i++)
            {
                float t = i / glowSmoothness;

                for (int j = 0; j < glowMats.Length; j++)
                {
                    Color targetColor = glowColor * maxIntensity;

                    Color nextColor = Color.Lerp(defaultColors[j], targetColor, t);

                    glowMats[j].SetColor("_EmissionColor", nextColor);
                }

                yield return new WaitForSeconds(delay);
            }

            if (onFinishGlow != null)
                onFinishGlow.Invoke();
        }
    }
}