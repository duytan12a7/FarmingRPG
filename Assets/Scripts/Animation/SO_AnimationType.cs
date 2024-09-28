using UnityEngine;

[CreateAssetMenu(fileName = "so_AnimationType", menuName = "Scriptable Objects/Animation/Animation Type")]
public class SO_AnimationType : ScriptableObject
{
    public AnimationClip animationClip;
    public AnimationName animationName;
    public CharacterPartAnimator partAnimator;
    public PartVariantColour variantColour;
    public PartVariantType variantType;
    
}
