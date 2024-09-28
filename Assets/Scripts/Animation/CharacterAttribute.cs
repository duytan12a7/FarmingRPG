[System.Serializable]
public struct CharacterAttribute
{
    public CharacterPartAnimator partAnimator;
    public PartVariantColour variantColour;
    public PartVariantType variantType;

    public CharacterAttribute(CharacterPartAnimator partAnimator, PartVariantColour variantColour, PartVariantType variantType)
    {
        this.partAnimator = partAnimator;
        this.variantColour = variantColour;
        this.variantType = variantType;

    }

}
