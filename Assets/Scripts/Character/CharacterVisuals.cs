using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;


public class CharacterVisuals : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer leftPuff;

    [SerializeField]
    SpriteRenderer rightPuff;

    [SerializeField]
    List<Sprite> landingSprites;

    float animationRate = 1.0f/60.0f;
    float landingTimer;
    int currentLandingSprite;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        leftPuff.sprite = null;
        rightPuff.sprite = null;
        landingTimer = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayLanding()
    {
        landingTimer = 0.0f;
        currentLandingSprite = 1;
        StartCoroutine(LandingAnimation());
    }

    IEnumerator LandingAnimation()
    {
        while (currentLandingSprite < landingSprites.Count)
        {
            landingTimer += TimeUtil.deltaTime * 0.5f;
            if (landingTimer > animationRate)
            {
                currentLandingSprite++;
                if(currentLandingSprite < landingSprites.Count)
                {
                    leftPuff.sprite = landingSprites[currentLandingSprite];
                    rightPuff.sprite = landingSprites[currentLandingSprite];
                    landingTimer = 0.0f;
                }
            }
            yield return null;
        }

        currentLandingSprite = 0;
        leftPuff.sprite = landingSprites[currentLandingSprite];
        rightPuff.sprite= landingSprites[currentLandingSprite];
    }
}
