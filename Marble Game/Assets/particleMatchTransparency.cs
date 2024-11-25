using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class particleMatchTransparency : MonoBehaviour
{
    [SerializeField] private Image target;
    [SerializeField] private ParticleSystem partSys;
    [SerializeField] private ParticleSystem.MainModule partMain;
    

    private void Start()
    {
        partMain = partSys.main;
    }

    private void Update()
    {
        var startColor = partMain.startColor;
        
        if (startColor.mode == ParticleSystemGradientMode.Color)
        {
            var currentColor = startColor.color;
            currentColor = new Color(currentColor.r, currentColor.g, currentColor.b, target.color.a);
            
            startColor = new ParticleSystem.MinMaxGradient(currentColor);
            partMain.startColor = startColor;
        }
        
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[partSys.particleCount];
        int count = partSys.GetParticles(particles);

        for (int i = 0; i < count; i++)
        {
            var particleColor = particles[i].startColor;
            particles[i].startColor = new Color(particleColor.r, particleColor.g, particleColor.b, target.color.a);
        }
        
        partSys.SetParticles(particles, count);
    }
}
