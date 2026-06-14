using Assets.WasapiAudio.Scripts.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class BarSpectrum : AudioVisualizationEffect
    {
        private GameObject[] _spectrumBars;
        private Vector3[] _originalPositions;
        private Vector3 _originalScale;
        private float _updateInterval = 1f/30f;
        private float _lastUpdate;
        private Color violet = new Color(0.9333334f, 0.509804f, 0.9333334f, 1f);
        private Color yellow = new Color(0.9254f, 0.792156f, 0.3411764f, 1f);
        private Color heartred = new Color(0.654902f, 0.3137255f, 0.3137255f, 1f);
        private Color disgustGreen = new Color(0.45882f, 0.701961f, 0.32156f, 1f);
        private Color sadBlue = new Color(0.5f, 0.5725f, 0.6705883f, 1f);
        private Color sansBlue = new Color(0.313725f, 0.48627f, 0.97647f, 1f);
        private Color nightmareRed = new Color(0.5f, 0f, 0f, 1f);

        public GameObject Prefab;
        public float AudioScale;
        public float Power;
        public Slider angrySlider, disgustedSlider, happySlider, neutralSlider, sadSlider, surprisedSlider,
            hypnoticSlider, heartSlider, rainbowSlider, nightmareSlider, gearsSlider, sansSlider, mischievousSlider;

        public void Start()
        {
            _spectrumBars = new GameObject[SpectrumSize];
            _originalPositions = new Vector3[SpectrumSize];
            _originalScale = Prefab.transform.localScale;
            var width = Prefab.transform.localScale.x;
            for (var i = 0; i < SpectrumSize; i++)
            {
                var spectrumBar = GameObject.Instantiate(Prefab);
                spectrumBar.transform.parent = transform;
                spectrumBar.transform.localPosition = new Vector3(width * i, 0, 0);
                spectrumBar.transform.localRotation = Quaternion.identity;
                _spectrumBars[i] = spectrumBar;
                _originalPositions[i] = spectrumBar.transform.localPosition;
            }
        }

        public void Update()
        {
            if (Time.time - _lastUpdate < _updateInterval) return;
            _lastUpdate = Time.time;
            var spectrumData = GetSpectrumData();
            Color finalColor = Color.black;
            float totalAlpha = 0f;
            finalColor += yellow * angrySlider.value;
            finalColor += disgustGreen * disgustedSlider.value;
            finalColor += yellow * happySlider.value;
            finalColor += Color.white * neutralSlider.value;
            finalColor += sadBlue * sadSlider.value;
            finalColor += Color.white * surprisedSlider.value;
            finalColor += heartred * heartSlider.value;
            finalColor += nightmareRed * nightmareSlider.value;
            finalColor += Color.white * gearsSlider.value;
            finalColor += Color.white * mischievousSlider.value;
            finalColor += violet * rainbowSlider.value;
            finalColor += sansBlue * sansSlider.value;
            totalAlpha = 1f - hypnoticSlider.value;
            totalAlpha = Mathf.Clamp01(totalAlpha);
            finalColor.a = totalAlpha;
            finalColor.r = Mathf.Clamp01(finalColor.r);
            finalColor.g = Mathf.Clamp01(finalColor.g);
            finalColor.b = Mathf.Clamp01(finalColor.b);
            for (var i = 0; i < SpectrumSize; i++)
            {
                var audioScale = Mathf.Pow(spectrumData[i] * AudioScale, Power);
                var newScale = new Vector3(_originalScale.x, _originalScale.y + audioScale, _originalScale.z);
                _spectrumBars[i].transform.localScale = newScale;
                _spectrumBars[i].transform.localPosition = _originalPositions[i];
                var barRenderer = _spectrumBars[i].GetComponent<Renderer>();
                barRenderer.material.SetColor("_Color", finalColor);
            }
        }
    }
}
