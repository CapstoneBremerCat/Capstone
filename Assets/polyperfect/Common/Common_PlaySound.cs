using UnityEngine;
using Game;

namespace Polyperfect.Common
{
    public class Common_PlaySound : MonoBehaviour
    {
        [SerializeField]
        private AudioClip animalSound;
        [SerializeField]
        private AudioClip walking;
        [SerializeField]
        private AudioClip eating;
        [SerializeField]
        private AudioClip running;
        [SerializeField]
        private AudioClip attacking;
        [SerializeField]
        private AudioClip death;
        [SerializeField]
        private AudioClip sleeping;

        void AnimalSound()
        {
            if (animalSound)
            {
                SoundManager.PlaySound(animalSound, transform.position);
            }
        }

        void Walking()
        {
            if (walking)
            {
                SoundManager.PlaySound(walking, transform.position);
            }
        }

        void Eating()
        {
            if (eating)
            {
                SoundManager.PlaySound(eating, transform.position);
            }
        }

        void Running()
        {
            if (running)
            {
                SoundManager.PlaySound(running, transform.position);
            }
        }

        void Attacking()
        {
            if (attacking)
            {
                SoundManager.PlaySound(attacking, transform.position);
            }
        }

        void Death()
        {
            if (death)
            {
                SoundManager.PlaySound(death, transform.position);
            }
        }

        void Sleeping()
        {
            if (sleeping)
            {
                SoundManager.PlaySound(sleeping, transform.position);
            }
        }
    }
}