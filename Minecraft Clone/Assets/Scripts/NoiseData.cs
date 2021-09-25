using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Noise Data", menuName = "Data/Noise Data")]
public class NoiseData : ScriptableObject
{
    public float noiseScale;
    [Range(1, 10)]
    public int octaves;
    public Vector2Int offset;
    public Vector2Int worldOffset;
    [Range(0, 2)]
    public float persistance;
    public float redistributionModifier;
    [Range(1, 20)]
    public float exponent;
}
