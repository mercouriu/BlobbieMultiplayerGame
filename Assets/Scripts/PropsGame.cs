using UnityEngine;

namespace Photon.Pun.Demo.Asteroids
{
    public class PropsGame
    {
        //public const float ASTEROIDS_MIN_SPAWN_TIME = 5.0f;
        //public const float ASTEROIDS_MAX_SPAWN_TIME = 10.0f;

        //public const float PLAYER_RESPAWN_TIME = 4.0f;

        //public const int PLAYER_MAX_LIVES = 3;

        //public const string PLAYER_LIVES = "PlayerLives";
        public const string PLAYER_READY = "IsPlayerReady";
        public const string PLAYER_LOADED_LEVEL = "PlayerLoadedLevel";

        public const string PLAYER_READY_NEXT_ROUND = "IsPlayerReadyNext";
        public const string All_PLAYERS_READY = "IsAllPlayersReadyNext";

        public static Color GetColor(int colorChoice)
        {
            switch (colorChoice)
            {
                case 0: return new Color32(187, 62, 76, 255);
                case 1: return new Color32(187, 62, 76, 255);
                case 2: return new Color32(187, 62, 76, 255);
                case 3: return Color.yellow;
                case 4: return Color.cyan;
                case 5: return Color.grey;
                case 6: return Color.magenta;
                case 7: return Color.white;
            }

            return Color.black;
        }
    }
}

