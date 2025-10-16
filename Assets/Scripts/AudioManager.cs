using System;
using System.Collections;
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

            if (Sounds == null) return;

            foreach (var sound in Sounds)
            {
                if (sound == null) continue;

                // create an AudioSource for each sound entry
                sound.source = gameObject.AddComponent<AudioSource>();
                sound.source.clip = sound.clip;
                sound.source.volume = sound.volume;
                sound.source.loop = sound.loop;
                // do not play automatically
                sound.source.playOnAwake = false;
            }
        }

        public void Play(string name)
        {
            if (Sounds == null) return;

            Sound s = Array.Find(Sounds, sound => sound != null && sound.name == name);
            if (s == null)
            {
                Debug.LogWarning("[AudioManager] Sound: " + name + " not found!");
                return;
            }

            if (s.clip == null)
            {
                Debug.LogWarning("[AudioManager] Sound clip is null for: " + name);
                return;
            }

            // loop means background music, use Play()
            if (s.loop)
            {
                if (!s.source.isPlaying)
                    s.source.Play();
            }
            else
            {
                // One-shot SFX
                s.source.PlayOneShot(s.clip);
            }
        }

        public void Stop(string name)
        {
            if (Sounds == null) return;

            Sound s = Array.Find(Sounds, sound => sound != null && sound.name == name);
            if (s == null)
            {
                Debug.LogWarning("[AudioManager] Sound: " + name + " not found!");
                return;
            }

            if (s.source != null)
                s.source.Stop();
        }

        /// <summary>
        /// Returns length of the clip (seconds) or 0 if not found.
        /// </summary>
        public float GetLength(string name)
        {
            if (Sounds == null) return 0f;
            Sound s = Array.Find(Sounds, sound => sound != null && sound.name == name);
            if (s == null || s.clip == null) return 0f;
            return s.clip.length;
        }

        /// <summary>
        /// Returns true if the named sound source is currently playing.
        /// For non-looped PlayOneShot calls this may return false depending on timing,
        /// but it's useful for looped/background sounds.
        /// </summary>
        public bool IsPlaying(string name)
        {
            if (Sounds == null) return false;
            Sound s = Array.Find(Sounds, sound => sound != null && sound.name == name);
            if (s == null || s.source == null) return false;
            return s.source.isPlaying;
        }

        /// <summary>
        /// Coroutine helper that waits the duration of the clip (or until the source stops if looped).
        /// Usage: yield return StartCoroutine(AudioManager.Instance.WaitForSoundEnd("name"));
        /// </summary>
        public IEnumerator WaitForSoundEnd(string name)
        {
            if (Sounds == null)
            {
                yield break;
            }

            Sound s = Array.Find(Sounds, sound => sound != null && sound.name == name);

            if (s == null)
            {
                Debug.LogWarning("[AudioManager] WaitForSoundEnd: sound not found -> " + name);
                yield break;
            }

            if (s.clip == null)
            {
                Debug.LogWarning("[AudioManager] WaitForSoundEnd: clip is null -> " + name);
                yield break;
            }

            // If the clip is looped background music: wait until the source stops playing.
            if (s.loop)
            {
                // If it's not playing yet, just return immediately
                if (s.source == null || !s.source.isPlaying)
                    yield break;

                // Wait while it's playing
                while (s.source != null && s.source.isPlaying)
                {
                    yield return null;
                }

                yield break;
            }

            // For one-shot SFX / voice lines we wait the clip's length.
            float len = s.clip.length;

            // Safety: if length is zero, bail out
            if (len <= 0f)
            {
                yield break;
            }

            yield return new WaitForSeconds(len);
        }
    }
}
