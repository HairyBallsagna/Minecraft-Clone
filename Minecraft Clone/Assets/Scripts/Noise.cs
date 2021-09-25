using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public static float OctavePerlin(float x, float z, NoiseData data)
    {
        x *= data.noiseScale;
        z *= data.noiseScale;
        x += data.noiseScale;
        z += data.noiseScale;

        float total = 0f;
        float frequency = 1f;
        float amplitude = 1f;
        float amplitudeSum = 0f; // used for normalizing result from 0.0 to 1.0 range

        for (int i = 0; i < data.octaves; i++)
        {
            total += Mathf.PerlinNoise((data.offset.x + data.worldOffset.x + x) * frequency,
                (data.offset.y + data.worldOffset.y + z) * frequency) * amplitude;

            amplitudeSum += amplitude;

            amplitude *= data.persistance;
            frequency *= 2;
        }

        return total / amplitudeSum;
    }

    public static float RemapValue(float value, float initialMin, float initialMax, float outputMin, float outputMax)
    {
        return outputMin + (value - initialMin) * (outputMax - outputMin) / (initialMax - initialMin);
    }
    
    public static float RemapValue01(float value, float outputMin, float outputMax)
    {
        return outputMin + (value - 0) * (outputMax - outputMin) / (1 - 0);
    }
    
    public static float RemapValue01Int(float value, float outputMin, float outputMax)
    {
        return (int) RemapValue01(value, outputMin, outputMax);
    }

    public static float Redistribution(float noise, NoiseData data)
    {
        return Mathf.Pow(noise * data.redistributionModifier, data.exponent);
    }
}
