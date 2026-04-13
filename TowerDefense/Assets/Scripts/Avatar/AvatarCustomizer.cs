using UnityEngine;

public class AvatarCustomizer : MonoBehaviour
{
    [System.Serializable]
    public class ClassSet
    {
        public AvatarClass avatarClass;
        public RuntimeAnimatorController[] colorVariants;
    }

    [Header("Target")]
    [SerializeField] private SpriteRenderer bodyRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer accessoryRenderer;

    [Header("Sets")]
    [SerializeField] private ClassSet[] classSets;
    [SerializeField] private Color[] primaryPalette;

    [Header("Mask")]
    [SerializeField] private Texture2D accessoryMask;

    private AvatarProfile _profile;
    private MaterialPropertyBlock _mpb;

    public Color[] PrimaryPalette => primaryPalette;

    void Awake()
    {
        _mpb = new MaterialPropertyBlock();
        if (bodyRenderer == null) bodyRenderer = GetComponentInChildren<SpriteRenderer>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
    }

    public void Apply(AvatarProfile profile)
    {
        _profile = profile;
        if (profile == null) return;

        RuntimeAnimatorController controller = ResolveController(profile.avatarClass, profile.primaryColorIndex);
        if (animator != null && controller != null)
            animator.runtimeAnimatorController = controller;

        Vector3 baseScale = transform.localScale;
        float s = Mathf.Clamp(profile.scale, 0.6f, 1.4f);
        transform.localScale = new Vector3(
            Mathf.Abs(baseScale.x) * s * (profile.flipHorizontal ? -1f : 1f),
            Mathf.Abs(baseScale.y) * s,
            baseScale.z);

        ApplyTint(profile.secondaryTint);
    }

    private RuntimeAnimatorController ResolveController(AvatarClass avatarClass, int colorIndex)
    {
        if (classSets == null) return null;
        foreach (ClassSet set in classSets)
        {
            if (set == null || set.avatarClass != avatarClass) continue;
            if (set.colorVariants == null || set.colorVariants.Length == 0) return null;
            int idx = Mathf.Clamp(colorIndex, 0, set.colorVariants.Length - 1);
            return set.colorVariants[idx];
        }
        return null;
    }

    private void ApplyTint(Color tint)
    {
        if (accessoryRenderer == null) return;
        accessoryRenderer.GetPropertyBlock(_mpb);
        _mpb.SetColor("_Color", tint);
        if (accessoryMask != null) _mpb.SetTexture("_MaskTex", accessoryMask);
        accessoryRenderer.SetPropertyBlock(_mpb);
    }
}
