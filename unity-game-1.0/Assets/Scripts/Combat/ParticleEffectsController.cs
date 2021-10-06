using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEffectsController : MonoBehaviour
{
    public List<ParticleSystem> particles;

    public void PlayParticleEffect(int id)
    {
        particles[id].Play();
    }
}
