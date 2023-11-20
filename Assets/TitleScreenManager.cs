using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SaveManager;

public class TitleScreenManager : SaveManager
{
    public List<LocationAnimationPair> pairs;
    public bool isPlayingAnimation = false;
    public int currentAnimationIdx = 0;

    public GameObject player;
    private Animator _animator;
    private void Start()
    {
        print(Resources.Load<TextAsset>("saves/TitleSave"));
        GameData saveData = SaveSystem.LoadGameDataFromString(Resources.Load<TextAsset>("saves/TitleSave").text);
        gridBuildingSystem.processInput = false;
        LoadGameSave(saveData);
        _animator = player.GetComponent<Animator>();
    }
    void Update()
    {
        if (!isPlayingAnimation)
        {
            TeleportAndPlayAnimation(pairs[currentAnimationIdx]);
        }

    }

    void TeleportAndPlayAnimation(LocationAnimationPair pair)
    {
        // Find the location GameObject by name or tag
        GameObject location = GameObject.Find(pair.locationName); // or GameObject.FindWithTag(pair.locationTag);

        // Teleport and animate
        player.transform.position = location.transform.position;
        player.transform.rotation = location.transform.rotation;
        _animator.Play(pair.animationName);
        isPlayingAnimation = true;
        StartCoroutine(HandleAnimationDuration(pair.duration));
    }
    IEnumerator HandleAnimationDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        isPlayingAnimation = false;
        currentAnimationIdx++;
        if (currentAnimationIdx == pairs.Count)
        {
            currentAnimationIdx = 0;
        }
    }
}
