using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using Com.LuisPedroFonseca.ProCamera2D;

public class GetAudioData : MonoBehaviour {

    public Image _audioRange;

    AudioSource _audioSource;
    public AudioMixerGroup _audioMixerMic;

    bool isReady;

    public float rmsVal;
    public float dbVal;
    public float pitchVal;

    private const int QSamples = 1024;
    private const float RefValue = 0.1f;
    private const float Threshold = 0.02f;

    float[] _samples;
    private float[] _spectrum;
    private float _fSample;

    public float _rotationSpeed;

    private float _resetShotsFired;
    private bool shotsFired;

    public GameObject _shot;

    public int _hitPoints;

    public Image[] _shotMarkers;

    public GameObject camera;

    public static int playerKills;

    public Text killCounterText;

    // Use this for initialization
    void Start () {

        GetAudioData.playerKills = 0;

        camera.GetComponent<ProCamera2DTransitionsFX>().TransitionEnter();

        _hitPoints = 3;

        _samples = new float[QSamples];
        _spectrum = new float[QSamples];
        _fSample = AudioSettings.outputSampleRate;

        _audioRange.fillAmount = 0;

        if (Microphone.devices.Length > 0) {
            _audioSource = GetComponent<AudioSource>();
            _audioSource.outputAudioMixerGroup = _audioMixerMic;
            _audioSource.clip = Microphone.Start(null, true, 10, 44100);
            _audioSource.loop = true;
            while (!(Microphone.GetPosition(null) > 0)) {
                _audioSource.Play();
            }

            isReady = true;
        }

    }

    void Update() {

            if (isReady) {

            killCounterText.text = playerKills + " of 10";

            if(playerKills >= 10) {
                isReady = false;
                // play win audio
                // and confetti!
                camera.GetComponent<ProCamera2DTransitionsFX>().TransitionExit();
                Invoke("GameWon", 2f);
            }

            AnalyzeSound();

            _audioRange.fillAmount = Mathf.Min(pitchVal / 1024, 1f);

            if(pitchVal > 250f && pitchVal < 650f) {
                this.transform.Rotate(Vector3.forward, _rotationSpeed * -Time.deltaTime);
            }

            if(pitchVal > 730f || Input.GetKey(KeyCode.Space)) {
                if(!shotsFired) {
                    shotsFired = true;
                    //GameObject clone = (Instantiate(_shot, transform.position + 1.0f * transform.right, transform.rotation));
                    GameObject clone = (Instantiate(_shot, transform.position, transform.rotation));
                    clone.GetComponent<Rigidbody2D>().velocity = (clone.transform.right * 5);
                }
            }

            if(shotsFired) {
                _resetShotsFired += Time.deltaTime;
                if (_resetShotsFired > 2.0f) {
                    shotsFired = false;
                    _resetShotsFired = 0;
                }
            }
        }
    }

    void AnalyzeSound() {
        _audioSource.GetOutputData(_samples, 0); // fill array with samples
        int i;
        float sum = 0;
        for (i = 0; i < QSamples; i++) {
            sum += _samples[i] * _samples[i]; // sum squared samples
        }
        rmsVal = Mathf.Sqrt(sum / QSamples); // rms = square root of average
        dbVal = 20 * Mathf.Log10(rmsVal / RefValue); // calculate dB
        if (dbVal < -160) dbVal = -160; // clamp it to -160dB min
                                        // get sound spectrum
        _audioSource.GetSpectrumData(_spectrum, 0, FFTWindow.BlackmanHarris);
        float maxV = 0;
        var maxN = 0;
        for (i = 0; i < QSamples; i++) { // find max 
            if (!(_spectrum[i] > maxV) || !(_spectrum[i] > Threshold))
                continue;

            maxV = _spectrum[i];
            maxN = i; // maxN is the index of max
        }
        float freqN = maxN; // pass the index to a float variable
        if (maxN > 0 && maxN < QSamples - 1) { // interpolate index using neighbours
            var dL = _spectrum[maxN - 1] / _spectrum[maxN];
            var dR = _spectrum[maxN + 1] / _spectrum[maxN];
            freqN += 0.5f * (dR * dR - dL * dL);
        }
        pitchVal = freqN * (_fSample / 2) / QSamples; // convert index to frequency
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if(collision.gameObject.name.StartsWith("Armada")) {
            if (_hitPoints > 0) {
                camera.GetComponent<ProCamera2DShake>().Shake(0);
                _shotMarkers[3 - _hitPoints].color = Color.red;
                _hitPoints -= 1;

                if (_hitPoints <= 0) {
                    for (int i = 0; i < _shotMarkers.Length; i++) {
                        _shotMarkers[i].enabled = false;
                    }
                    camera.GetComponent<ProCamera2DTransitionsFX>().TransitionExit();
                    Invoke("GameOver", 2f);
                }
            }
        }
    }

    private void GameOver() {
        UnityEngine.SceneManagement.SceneManager.LoadScene("SceneGameOver");
    }

    private void GameWon() {
        UnityEngine.SceneManagement.SceneManager.LoadScene("SceneGameWon");
    }
}
