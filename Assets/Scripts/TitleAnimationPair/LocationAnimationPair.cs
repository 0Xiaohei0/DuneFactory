using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class LocationAnimationPair : ScriptableObject
{
    public string locationName; // or public string locationTag;
    public string animationName;
    public float duration;
}
