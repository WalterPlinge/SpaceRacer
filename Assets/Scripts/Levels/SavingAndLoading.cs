using UnityEngine;

public class PlayerPrefsCharacterSaver : MonoBehaviour
{
    public PlayerData playerData;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
            SaveCharacter(playerData, 0);

        if (Input.GetKeyDown(KeyCode.L))
            playerData = LoadCharacter(0);
    }

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
