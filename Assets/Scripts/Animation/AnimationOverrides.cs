using System.Collections.Generic;
using UnityEngine;

public class AnimationOverrides : MonoBehaviour
{
    [SerializeField] private GameObject character = null;
    [SerializeField] private SO_AnimationType[] animationTypes = null;

    private Dictionary<AnimationClip, SO_AnimationType> animationTypeByClip;
    private Dictionary<string, SO_AnimationType> animationTypeByCompositeKey;

    private void Start()
    {
        animationTypeByClip = new Dictionary<AnimationClip, SO_AnimationType>();
        animationTypeByCompositeKey = new Dictionary<string, SO_AnimationType>();

        foreach (SO_AnimationType animationType in animationTypes)
        {
            animationTypeByClip.Add(animationType.animationClip, animationType);

            string compositeKey = GenerateCompositeKey(animationType);
            animationTypeByCompositeKey.Add(compositeKey, animationType);
        }
    }

    public void ApplyCharacterCustomisation(List<CharacterAttribute> characterAttributes)
    {
        Animator[] animators = character.GetComponentsInChildren<Animator>();

        foreach (CharacterAttribute attribute in characterAttributes)
        {
            Animator currentAnimator = FindAnimator(animators, attribute.partAnimator.ToString());
            if (currentAnimator == null) continue;

            AnimatorOverrideController overrideController = new(currentAnimator.runtimeAnimatorController);
            List<KeyValuePair<AnimationClip, AnimationClip>> overrideClips = new();

            foreach (AnimationClip originalClip in overrideController.animationClips)
            {
                if (animationTypeByClip.TryGetValue(originalClip, out SO_AnimationType animationType))
                {
                    string key = GenerateCharacterAttributeKey(attribute, animationType);

                    if (animationTypeByCompositeKey.TryGetValue(key, out SO_AnimationType swapAnimationType))
                    {
                        overrideClips.Add(new KeyValuePair<AnimationClip, AnimationClip>(originalClip, swapAnimationType.animationClip));
                    }
                }
            }

            overrideController.ApplyOverrides(overrideClips);
            currentAnimator.runtimeAnimatorController = overrideController;
        }
    }

    private Animator FindAnimator(Animator[] animators, string animatorName)
    {
        foreach (Animator animator in animators)
        {
            if (animator.name.ToLower() == animatorName)
            {
                return animator;
            }
        }
        return null;
    }

    private string GenerateCompositeKey(SO_AnimationType animationType)
    {
        return $"{animationType.partAnimator}{animationType.variantColour}{animationType.variantType}{animationType.animationName}";
    }

    private string GenerateCharacterAttributeKey(CharacterAttribute characterAttribute, SO_AnimationType animationType)
    {
        return $"{characterAttribute.partAnimator}{characterAttribute.variantColour}{characterAttribute.variantType}{animationType.animationName}";
    }
}
