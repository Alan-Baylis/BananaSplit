using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    public AudioClip Swoosh = null;

    private List<AudioSource> _sources = null;
    private int _currentSource = 0;

    private void Start()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        _sources = new List<AudioSource>();

        for (int i = 0; i < 5; i++)
        {
            var go = new GameObject("Audio Source", new[] { typeof(AudioSource) });
            go.transform.parent = transform;
            _sources.Add(go.GetComponent<AudioSource>());
        }
    }

    public void Play(AudioClip clip)
    {
        var source = _sources[_currentSource];
        source.clip = clip;
        source.Play();
        _currentSource++;
        if (_currentSource == _sources.Count)
            _currentSource = 0;
    }
}
