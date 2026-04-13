using UnityEngine;

public enum AvatarClass { Archer, Lancer, Warrior, Monk }

[System.Serializable]
public class AvatarProfile
{
    public AvatarClass avatarClass = AvatarClass.Archer;
    public int primaryColorIndex = 0;
    public Color secondaryTint = Color.white;
    public float scale = 1f;
    public bool flipHorizontal = false;

    public static AvatarProfile LoadForPlayer(int playerIndex)
    {
        string p = Prefix(playerIndex);
        AvatarProfile profile = new AvatarProfile();
        profile.avatarClass = (AvatarClass)PlayerPrefs.GetInt(p + "class", 0);
        profile.primaryColorIndex = PlayerPrefs.GetInt(p + "color", 0);
        profile.secondaryTint = new Color(
            PlayerPrefs.GetFloat(p + "tintR", 1f),
            PlayerPrefs.GetFloat(p + "tintG", 1f),
            PlayerPrefs.GetFloat(p + "tintB", 1f),
            1f);
        profile.scale = PlayerPrefs.GetFloat(p + "scale", 1f);
        profile.flipHorizontal = PlayerPrefs.GetInt(p + "flip", 0) == 1;
        return profile;
    }

    public void SaveForPlayer(int playerIndex)
    {
        string p = Prefix(playerIndex);
        PlayerPrefs.SetInt(p + "class", (int)avatarClass);
        PlayerPrefs.SetInt(p + "color", primaryColorIndex);
        PlayerPrefs.SetFloat(p + "tintR", secondaryTint.r);
        PlayerPrefs.SetFloat(p + "tintG", secondaryTint.g);
        PlayerPrefs.SetFloat(p + "tintB", secondaryTint.b);
        PlayerPrefs.SetFloat(p + "scale", scale);
        PlayerPrefs.SetInt(p + "flip", flipHorizontal ? 1 : 0);
        PlayerPrefs.Save();
    }

    private static string Prefix(int playerIndex) => $"avatar_p{playerIndex}_";
}
