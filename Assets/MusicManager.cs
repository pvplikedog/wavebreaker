using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private List<AudioClip> audioClips;
    private AudioSource _audioSource;
    
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_audioSource.isPlaying)
        {
            _audioSource.clip = audioClips[Random.Range(0, audioClips.Count)];
            _audioSource.Play();
        }
    }
}
