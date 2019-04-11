using UnityEngine;

public class SavingAndLoading : MonoBehaviour
{
    public PlayerData playerData;

    static void SaveCharacter(PlayerData data, int characterSlot)
    {
        PlayerPrefs.SetString("playerName_CharacterSlot" + characterSlot, data.playerName);
        PlayerPrefs.SetFloat("time_CharacterSlot" + characterSlot, data.playerTime);
        PlayerPrefs.SetInt("position_CharacterSlot" + characterSlot, data.leaderboardPosition);
        PlayerPrefs.Save();
    }
    
    static PlayerData LoadCharacter(int characterSlot)
    {
        PlayerData loadedCharacter = new PlayerData();
        loadedCharacter.playerName = PlayerPrefs.GetString("playerName_CharacterSlot" + characterSlot);
        loadedCharacter.playerTime = PlayerPrefs.GetFloat("time_CharacterSlot" + characterSlot);
        loadedCharacter.leaderboardPosition = PlayerPrefs.GetInt("position_CharacterSlot" + characterSlot);

        return loadedCharacter;
    }
}
