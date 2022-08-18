using System;
using System.Collections;


using UnityEngine;
using UnityEngine.SceneManagement;


using Photon.Pun;
using Photon.Realtime;


namespace Com.MyCompany.MyGame
{
    public class GameManager : MonoBehaviourPunCallbacks
    {

        #region Public Fields

        [Tooltip("The prefab to use for representing the player")]
        public GameObject playerPref;
        public GameObject playerIconBackPref;

        bool[] IsPlayerInRoomNow = new bool[4];

        int SlotNumber;
        private int PlayerCountInThisRoom;

        void Start()
        {

            if (playerPref == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
            }
            else
            {
                if (PlayerManager.LocalPlayerInstance == null)
                {
                    
                    PlayerCountInThisRoom = PhotonNetwork.CurrentRoom.PlayerCount;
                    Debug.LogFormat("Your Player ID is {0}", PhotonNetwork.LocalPlayer.ActorNumber);
                    
                    Debug.LogFormat("Player count is - {0}", PlayerCountInThisRoom);
                    SlotNumber = PlayerCountInThisRoom;
                    IsPlayerInRoomNow[SlotNumber-1] = true;
                    
                    Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
                    // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                    /*
                   if (PlayerInRoomNow[0])
                    {
                        PhotonNetwork.Instantiate(this.playerPref.name, new Vector3(-1.8f, 0, 0f), Quaternion.identity, 0);
                    }
                   else if (PlayerInRoomNow[1])
                    {
                        PhotonNetwork.Instantiate(this.playerPref.name, new Vector3(-1.8f, 1.2f, 0f), Quaternion.identity, 0);
                    }
                    else if (PlayerInRoomNow[2])
                    {
                        PhotonNetwork.Instantiate(this.playerPref.name, new Vector3(-1.8f, 1.2f*2, 0f), Quaternion.identity, 0);
                    }
                    */


                    PhotonNetwork.Instantiate(this.playerPref.name, new Vector3(-1.8f, -1.2f*(PlayerCountInThisRoom-1), 0f), Quaternion.identity, 0);
                    //PhotonNetwork.Instantiate(this.playerIconBackPref.name, new Vector3(-1.8f, -1.2f * (PlayerCountInThisRoom - 1), 0f), Quaternion.identity, 0);
                }
                else
                {
                    Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
                }
            }
        }


        #endregion

        #region Photon Callbacks


        /// <summary>
        /// Called when the local player left the room. We need to load the launcher scene.
        /// </summary>
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);//”¡–¿“‹ ≈—À» Õ≈ ”¡–¿À ¬ ƒ–”√ŒÃ Ã≈—“≈(LeaveRoom)!!!!!!!!
        }


        
        public override void OnPlayerEnteredRoom(Player other)
        {
            Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting

            


        }


        public override void OnPlayerLeftRoom(Player other)
        {
            Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects

            /*
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom


                GameLoading();
            }
            */
        }


        #endregion


        #region Public Methods


        public void LeaveRoom()
        {
            //ÃÓÊÌÓ ‰Ó·‡‚ËÚ¸ ÔÓ‰Ú‚ÂÊ‰ÂÌËÂ ËÎË ÒÓı‡ÌÂÌËÂ.
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene(0);


        }

        public void StartGame()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (PhotonNetwork.CurrentRoom.PlayerCount > 0)//»«Ã≈Õ»“‹ Õ¿ 1!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                {
                    PhotonNetwork.LoadLevel("PlayingGame");
                    //PhotonNetwork.LocalPlayer
                    Debug.LogFormat("{0}", PhotonNetwork.LocalPlayer);
                    PhotonNetwork.CurrentRoom.IsOpen = false;
                    
                }
            }
            
        }

        #endregion

        #region Private Methods

        //Õ≈ –¿¡Œ“¿≈“
        void GameLoading()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
            }
            Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.LocalPlayer);
            //ƒÂ·‡„ ‚˚¯Â Û‰‡ÎËÚ¸/ÔÂÂÔËÒ‡Ú¸.
            PhotonNetwork.LoadLevel("DrawingRoom1");
        }


        #endregion
    }
}