using System;
using UnityEngine;

namespace _project.Scripts.Managers
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance;
        public Sound[] Sounds;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            foreach (var sound in Sounds)
            {
                sound.source = gameObject.AddComponent<AudioSource>();
                sound.source.clip = sound.clip;
                sound.source.volume = sound.volume;
                sound.source.loop = sound.loop;
            }
        }

        public void Play(string name)
        {
            Sound s = Array.Find(Sounds, sound => sound.name == name);
            if (s == null)
            {
                Debug.LogWarning("Sound: " + name + " not found!");
                return;
            }

            //  si le son est en loop (musique par ex.), utilise Play()
            if (s.loop)
            {
                if (!s.source.isPlaying)
                    s.source.Play();
            }
            else
            {
                //  si c’est un son ponctuel (clic, bruit de verre, etc.), utilise PlayOneShot()
                s.source.PlayOneShot(s.clip);
            }
        }

        public void Stop(string name)
        {
            Sound s = Array.Find(Sounds, sound => sound.name == name);
            if (s == null)
            {
                Debug.LogWarning("Sound: " + name + " not found!");
                return;
            }

            s.source.Stop();
        }
        public float GetLength(string name)
        {
            Sound s = Array.Find(Sounds, sound => sound.name == name);
            return s.clip.length;
        }
    }
}