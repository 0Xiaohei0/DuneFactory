using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu()]
public class DialogueSO : ScriptableObject
{
    [TextArea(15, 20)]
    public string sentence;
    public AudioClip audioClip;
}
