using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GodUnityPlugin
{
    public class FadingObject : MonoBehaviour
    {
        public UnityEvent onStartFadeOut;
        public UnityEvent onFinishFadeOut;
        public UnityEvent onStartFadeIn;
        public UnityEvent onFinishFadeIn;

        public Renderer[] renderers;

        public float fadeOutSpeed = 0.5f;
        public float fadeInSpeed = 0.35f;

        public float fadeInSmoothness = 100;
        public float fadeOutSmoothness = 100;

        public bool autoFadeAllMesh = true;

        private Material[] defaultMats;

        private void Awake()
        {
            if (autoFadeAllMesh)
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

        public IEnumerator FadeOut()
        {
            yield return StartCoroutine(FadeOut(fadeOutSpeed, fadeInSmoothness));
        }

        public IEnumerator FadeOut(float glowSpeed,float smoothness)
        {
            yield return StartCoroutine(StartFadeOut(glowSpeed, fadeInSmoothness));
        }

        public IEnumerator FadeInAndOut()
        {
            yield return StartCoroutine(FadeInAndOut(fadeOutSpeed, fadeInSmoothness, fadeInSpeed, fadeOutSmoothness));
        }

        public IEnumerator FadeInAndOut(float glowSpeed,float glowSmoothness, float fadeSpeed, float fadeSmoothness)
        {
            if (defaultMats == null)
            {
                defaultMats = new Material[renderers.Length];
                for (int i = 0; i < renderers.Length; i++)
                    defaultMats[i] = renderers[i].material;
            }

            yield return StartCoroutine(FadeOut(glowSpeed, glowSmoothness));
            yield return StartCoroutine(FadeIn(fadeSpeed, fadeSmoothness, defaultMats));
        }

        private IEnumerator FadeIn(float fadeSpeed, float smoothness, Material[] defaultMats)
        {
            if (onStartFadeIn != null)
                onStartFadeIn.Invoke();

            float delay = fadeSpeed / smoothness;

            Color[] defaultColors = new Color[defaultMats.Length];

            for (int i = 0; i < defaultMats.Length; i++)
                defaultColors[i] = defaultMats[i].GetColor("_BaseColor"); ;

            if (fadeSpeed == 0.0f)
            {
                for (int j = 0; j < defaultColors.Length; j++)
                {
                    Color targetColor = defaultColors[j];

                    renderers[j].material.SetColor("_BaseColor", targetColor);
                }

                yield break;
            }

            for (int i = 0; i < fadeOutSmoothness; i++)
            {
                float t = i / fadeOutSmoothness;

                for (int j = 0; j < defaultColors.Length; j++)
                {
                    Material fadeMat = renderers[j].material;

                    Color color = fadeMat.GetColor("_BaseColor");

                    Color nextColor = Color.Lerp(color, defaultColors[j], t);

                    fadeMat.SetColor("_BaseColor", nextColor);
                }

                yield return new WaitForSeconds(delay);
            }

            for (int i = 0; i < renderers.Length; i++)
                renderers[i].material = defaultMats[i];

            if (onFinishFadeIn != null)
                onFinishFadeIn.Invoke();

            defaultMats = null;
        }

        private IEnumerator StartFadeOut(float glowSpeed, float smoothness)
        {
            if (onStartFadeOut != null)
                onStartFadeOut.Invoke();

            Material[] fadeMats = new Material[renderers.Length];

            for (int i = 0; i < renderers.Length; i++)
            {
                Material fadeMat = new Material(renderers[i].material);

                if (fadeMat.GetFloat("_Surface") != 1.0f)
                    fadeMat.SetFloat("_Surface", 1.0f);

                fadeMats[i] = fadeMat;
                renderers[i].material = fadeMat;
            }

            float delay = glowSpeed / fadeInSmoothness;

            Color[] defaultColors = new Color[fadeMats.Length];

            for (int i = 0; i < fadeMats.Length; i++)
                defaultColors[i] = fadeMats[i].GetColor("_BaseColor");

            if (glowSpeed == 0.0f)
            {
                for (int j = 0; j < fadeMats.Length; j++)
                {
                    Color targetColor = defaultColors[j];
                    targetColor.a = 0.0f;

                    fadeMats[j].SetColor("_BaseColor", targetColor);
                }

                yield break;
            }

            for (int i = 0; i < fadeInSmoothness; i++)
            {
                float t = i / fadeInSmoothness;

                for (int j = 0; j < fadeMats.Length; j++)
                {
                    Color targetColor = defaultColors[j];
                    targetColor.a = 0.0f;

                    Color nextColor = Color.Lerp(defaultColors[j], targetColor, t);
                    fadeMats[j].SetColor("_BaseColor", nextColor);
                }

                yield return new WaitForSeconds(delay);
            }

            if (onFinishFadeOut != null)
                onFinishFadeOut.Invoke();
        }
    }
}