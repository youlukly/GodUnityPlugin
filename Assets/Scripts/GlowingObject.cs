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
            int matCount = 0;

            for (int i = 0; i < renderers.Length; i++)
            {
                foreach (var renderer in renderers)
                    matCount += renderer.materials.Length;
            }

            if (defaultMats == null)
            {
                defaultMats = new Material[matCount];
                for (int i = 0; i < renderers.Length; i++)
                    for (int j = 0; j < renderers[i].materials.Length; j++)
                        defaultMats[i + j] = renderers[i].materials[j];
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

            for (int i = 0; i < fadeSmoothness; i++)
            {
                float t = i / fadeSmoothness;

                for (int j = 0; j < renderers.Length; j++)
                    for (int k = 0; k < renderers[j].materials.Length; k++)
                    {
                        Material glowMat = renderers[j].materials[k];

                        Color color = glowMat.GetColor("_EmissionColor");

                        Color nextColor = Color.Lerp(color, defaultColors[j+k], t);

                        glowMat.SetColor("_EmissionColor", nextColor);
                    }

                yield return new WaitForSeconds(delay);
            }

            for (int i = 0; i < renderers.Length; i++)
                for (int j = 0; j < renderers[i].materials.Length; j++)
                    renderers[i].materials[j] = defaultMats[i + j];

            if (onFinishFade != null)
                onFinishFade.Invoke();

            defaultMats = null;
        }

        private IEnumerator StartGlow(float glowSpeed, float maxIntensity, float smoothness, Color glowColor)
        {
            if (onStartGlow != null)
                onStartGlow.Invoke();

            int matCount = 0;

            for (int i = 0; i < renderers.Length; i++)
            {
                foreach (var renderer in renderers)
                    matCount += renderer.materials.Length;
            }

            Material[] glowMats = new Material[matCount];

            for (int i = 0; i < renderers.Length; i++)
            {
                for (int j = 0; j < renderers[i].materials.Length; j++)
                {
                    Material mat = renderers[i].materials[j];
                    Material glowMat = new Material(mat);
                    glowMats[i+j] = glowMat;
                    renderers[i].materials[j] = glowMat;
                }
            }

            float delay = glowSpeed / glowSmoothness;

            Color[] defaultColors = new Color[glowMats.Length];

            for (int i = 0; i < glowMats.Length; i++)
                defaultColors[i] = glowMats[i].GetColor("_EmissionColor");

            for (int i = 0; i < glowSmoothness; i++)
            {
                float t = i / glowSmoothness;

                for (int j = 0; j < glowMats.Length; j++)
                {
                    Color targetColor = glowColor * maxIntensity;

                    Color nextColor = Color.Lerp(defaultColors[j], targetColor, t);

                    glowMats[j].SetColor("_EmissionColor", nextColor);

                    glowMats[j].color = Color.red;
                }

                yield return new WaitForSeconds(delay);
            }

            if (onFinishGlow != null)
                onFinishGlow.Invoke();
        }
    }
}