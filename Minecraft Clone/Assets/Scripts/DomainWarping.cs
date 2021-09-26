using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DomainWarping : MonoBehaviour
{
    public NoiseData noiseDomainX;
    public NoiseData noiseDomainY;
    public int amplitudeX = 20;
    public int amplitudeY = 20;

    public float GenerateDomainNoise(int x, int z, NoiseData defaultNoiseData)
    {
        Vector2 domainOffset = GenerateDomainOffset(x, z);
        return Noise.OctavePerlin(x + domainOffset.x, z + domainOffset.y, defaultNoiseData);
    }

    public Vector2 GenerateDomainOffset(int x, int z)
    {
        float noiseX = Noise.OctavePerlin(x, z, noiseDomainX) * amplitudeX;
        float noiseY = Noise.OctavePerlin(x, z, noiseDomainY) * amplitudeY;
        return new Vector2(noiseX, noiseY);
    }

    public Vector2Int GenerateDomainOffsetInt(int x, int z)
    {
        return Vector2Int.RoundToInt(GenerateDomainOffset(x, z));
    }
}
