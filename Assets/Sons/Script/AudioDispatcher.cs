using System;
using UnityEngine;

public enum AudioType
{
    None,
    Throw,
    Grab,
    Ranger,
    Busted,
    Boss,
    Surpris


}

[Serializable]
public struct AudioInfos
{
    public AudioType audioType;
    public AudioClip audioClip;
}

[CreateAssetMenu(fileName = "AudioEventDispatcher", menuName = "Scriptable Objects/AudioEventDispatcher")]
public class AudioDispatcher : ScriptableObject
{
    [SerializeField] private AudioInfos[] _audioClips;


    public event Action<AudioClip> OnAudioEvent;


    public void PlayAudio(AudioType audioType)
    {
        for (int i = 0; i < _audioClips.Length; i++)
        {
            if (_audioClips[i].audioType == audioType)
            {
                OnAudioEvent?.Invoke(_audioClips[i].audioClip);
                return;
            }
        }

    }

}
