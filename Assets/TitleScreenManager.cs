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
    ParticleSystem ps;
    private void Start()
    {
        print(Resources.Load<TextAsset>("saves/TitleSave"));
        GameData saveData = SaveSystem.LoadGameDataFromString(Resources.Load<TextAsset>("saves/TitleSave").text);
        gridBuildingSystem.processInput = false;
        LoadGameSave(saveData);
        _animator = player.GetComponent<Animator>();
        ps = player.GetComponentInChildren<ParticleSystem>();
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
        if (pair.animationName == "Standing Using Touchscreen Tablet")
        {
            player.transform.Find("Tablet").gameObject.SetActive(true);
        }
        isPlayingAnimation = true;
        StartCoroutine(HandleAnimationDuration(pair.duration));
    }
    IEnumerator HandleAnimationDuration(float duration)
    {
        yield return new WaitForSeconds(duration - 0.5f);
        ps.Play();
        yield return new WaitForSeconds(0.5f);
        isPlayingAnimation = false;
        currentAnimationIdx++;
        if (currentAnimationIdx == pairs.Count)
        {
            currentAnimationIdx = 0;
        }
        player.transform.Find("Tablet")?.gameObject.SetActive(false);
    }
}
