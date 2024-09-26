using UnityEngine;

public class LevelManager : MonoBehaviour
{
    // Reference to the player
    private NewPlayer player;

    private void Start()
    {
        // Get reference to the player
        player = NewPlayer.Instance;
    }

    // Function to handle player stats update upon leveling up
    public void UpdatePlayerStats()
    {
        player.playerLevel++;
        player.currentXP -= player.xpToNextLevel; // Carry over extra XP
        player.xpToNextLevel *= 1.5f;      // Increase XP threshold for next level
        player.maxHealth += 10f;           // Increase max health
        player.health += 15f;              // Heal a bit upon leveling up
        player.ObjectSpeed *= 1.2f;

        // Additional logic based on the player's level
        if (player.playerLevel == 0)
        {
            // Level 0 logic
        }
        else if (player.playerLevel == 1)
        {
            // Level 1 logic
        }
    }
}
