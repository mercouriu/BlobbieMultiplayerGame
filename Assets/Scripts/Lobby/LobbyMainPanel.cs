using ExitGames.Client.Photon;
using Photon.Realtime;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Hashtable = ExitGames.Client.Photon.Hashtable;


namespace Photon.Pun.Demo.Asteroids
{
    public class LobbyMainPanel : MonoBehaviourPunCallbacks
    {
        [Header("Login Panel")]
        public GameObject LoginPanel;

        public InputField PlayerNameInput;
        public GameObject ErrorName;

        [Header("Selection Panel")]
        public GameObject SelectionPanel;

        [Header("Create Room Panel")]
        public GameObject CreateRoomPanel;

        public InputField RoomNameInputField;
        public InputField MaxPlayersInputField;
        public GameObject ErrorCreateRoomFields;

        [Header("Join Room Panel")]
        public GameObject JoinRandomRoomPanel;
        public InputField joinInput;
        public GameObject JoinRoomError;

        [Header("Room List Panel")]
        public GameObject RoomListPanel;

        public GameObject RoomListContent;
        public GameObject RoomListEntryPrefab;

        [Header("Inside Room Panel")]
        public GameObject InsideRoomPanel;

        public Button StartGameButton;
        public GameObject PlayerListEntryPrefab;

        private Dictionary<string, RoomInfo> cachedRoomList;
        private Dictionary<string, GameObject> roomListEntries;
        private Dictionary<int, GameObject> playerListEntries;

        private bool IsAbleToUseButtons = true;
        public AudioSource PageTurnWaw;

        public Text GamemodeName;
        List<string> Gamemodelist = new ModeSwitching().GamemodeList;       //Импорт списка "простых" названий режимов игры

        public Transform LoginUIScr;
        public Transform SelectJoinUIScr;
        public Transform CreateRoomUIScr;

        #region UNITY

        public void Start()
        {
            SelectJoinUIScr.localPosition = new Vector2(0, 2000);
            CreateRoomUIScr.localPosition = new Vector2(0, -2000);
            
            LoginPanel.SetActive(true);
        }

        public void Awake()
        {
            PhotonNetwork.AutomaticallySyncScene = true;

            cachedRoomList = new Dictionary<string, RoomInfo>();
            roomListEntries = new Dictionary<string, GameObject>();
            
            //PlayerNameInput.text = "Player " + Random.Range(1000, 10000);
        }

        #endregion
        
        IEnumerator UITimeDelay()
        {
            yield return new WaitForSeconds(2f);
            IsAbleToUseButtons = true;
        }

        IEnumerator UITimeDelayShort()
        {
            yield return new WaitForSeconds(0.9f);
            IsAbleToUseButtons = true;
        }

        #region PUN CALLBACKS

        public override void OnConnectedToMaster()
        {
            this.SetActivePanel(SelectionPanel.name);
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            ClearRoomListView();

            UpdateCachedRoomList(roomList);
            //UpdateRoomListView();
        }

        public override void OnJoinedLobby()
        {
            // whenever this joins a new lobby, clear any previous room lists
            cachedRoomList.Clear();
            ClearRoomListView();
        }

        // note: when a client joins / creates a room, OnLeftLobby does not get called, even if the client was in a lobby before
        public override void OnLeftLobby()
        {
            cachedRoomList.Clear();
            ClearRoomListView();
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            SetActivePanel(SelectionPanel.name);
            
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            //SetActivePanel(SelectionPanel.name);
            JoinRoomError.SetActive(true);
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            string roomName = "Room " + Random.Range(1000, 10000);

            RoomOptions options = new RoomOptions {MaxPlayers = 8};

            PhotonNetwork.CreateRoom(roomName, options, null);
        }

        public override void OnJoinedRoom()
        {
            InsideRoomPanel.SetActive(true);
            CreateRoomUIScr.LeanMoveLocalX(-2000, 0.9f).setEaseOutExpo();
            SelectJoinUIScr.LeanMoveLocalY(2000, 0.9f).setEaseOutExpo();
            InsideRoomPanel.LeanMoveLocalX(0, 0.9f).setEaseOutExpo();
            PageTurnWaw.Play();

            //SetActivePanel(InsideRoomPanel.name);

            if (playerListEntries == null)
            {
                playerListEntries = new Dictionary<int, GameObject>();
            }

            foreach (Player p in PhotonNetwork.PlayerList)
            {
                GameObject entry = Instantiate(PlayerListEntryPrefab);
                entry.transform.SetParent(InsideRoomPanel.transform);
                entry.transform.localScale = Vector3.one;
                entry.GetComponent<PlayerListEntry>().Initialize(p.ActorNumber, p.NickName);

                object isPlayerReady;
                if (p.CustomProperties.TryGetValue(PropsGame.PLAYER_READY, out isPlayerReady))
                {
                    entry.GetComponent<PlayerListEntry>().SetPlayerReady((bool) isPlayerReady);
                }

                playerListEntries.Add(p.ActorNumber, entry);
            }

            StartGameButton.gameObject.SetActive(CheckPlayersReady());

            Hashtable props = new Hashtable
            {
                {PropsGame.PLAYER_LOADED_LEVEL, false}
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }

        public override void OnLeftRoom()
        {
            SetActivePanel(SelectionPanel.name);

            foreach (GameObject entry in playerListEntries.Values)
            {
                Destroy(entry.gameObject);
            }

            playerListEntries.Clear();
            playerListEntries = null;
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            GameObject entry = Instantiate(PlayerListEntryPrefab);
            entry.transform.SetParent(InsideRoomPanel.transform);
            entry.transform.localScale = Vector3.one;
            entry.GetComponent<PlayerListEntry>().Initialize(newPlayer.ActorNumber, newPlayer.NickName);

            playerListEntries.Add(newPlayer.ActorNumber, entry);

            StartGameButton.gameObject.SetActive(CheckPlayersReady());
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Destroy(playerListEntries[otherPlayer.ActorNumber].gameObject);
            playerListEntries.Remove(otherPlayer.ActorNumber);

            StartGameButton.gameObject.SetActive(CheckPlayersReady());
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
            {
                StartGameButton.gameObject.SetActive(CheckPlayersReady());
            }
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (playerListEntries == null)
            {
                playerListEntries = new Dictionary<int, GameObject>();
            }

            GameObject entry;
            if (playerListEntries.TryGetValue(targetPlayer.ActorNumber, out entry))
            {
                object isPlayerReady;
                if (changedProps.TryGetValue(PropsGame.PLAYER_READY, out isPlayerReady))
                {
                    entry.GetComponent<PlayerListEntry>().SetPlayerReady((bool) isPlayerReady);
                }
            }

            StartGameButton.gameObject.SetActive(CheckPlayersReady());
        }

        #endregion

        #region UI CALLBACKS

        public void OnBackButtonClicked()                                                       //При нажатии на кнопку Cancel
        {
            if (IsAbleToUseButtons)
            {
                if (PhotonNetwork.InLobby)
                {
                    PhotonNetwork.LeaveLobby();
                }

                ErrorCreateRoomFields.SetActive(false);

                //SetActivePanel(SelectionPanel.name);
                SelectionPanel.SetActive(true);
                CreateRoomUIScr.LeanMoveLocalY(-2000, 0.9f).setEaseOutExpo();
                SelectJoinUIScr.LeanMoveLocalY(0, 0.9f).setEaseOutExpo();
                PageTurnWaw.Play();

                IsAbleToUseButtons = false;
                StartCoroutine(UITimeDelayShort());
                
                
            }
            
        }

        public void OnCreateRoomButtonClicked()
        {
            if (IsAbleToUseButtons)
            {
                string roomName = RoomNameInputField.text;
                roomName = (roomName.Equals(string.Empty)) ? "Room " + Random.Range(1000, 10000) : roomName;

                byte maxPlayers;
                byte.TryParse(MaxPlayersInputField.text, out maxPlayers);
                maxPlayers = (byte)Mathf.Clamp(maxPlayers, 2, 8);

                RoomOptions options = new RoomOptions { MaxPlayers = maxPlayers, PlayerTtl = 10000 };
                if (RoomNameInputField.text == "")
                {
                    ErrorCreateRoomFields.SetActive(true);
                    return;
                }
                else if (maxPlayers > 8 || maxPlayers < 2 || MaxPlayersInputField.text == "")
                {
                    ErrorCreateRoomFields.SetActive(true);
                    return;
                }
                else
                {
                    PhotonNetwork.CreateRoom(roomName, options, null);
                    ErrorCreateRoomFields.SetActive(false);




                    InsideRoomPanel.SetActive(true);                                                //При нажатии на кнопку Create
                    CreateRoomUIScr.LeanMoveLocalX(-2000, 0.9f).setEaseOutExpo();
                    SelectJoinUIScr.LeanMoveLocalY(2000, 0.9f).setEaseOutExpo();
                    InsideRoomPanel.LeanMoveLocalX(0, 0.9f).setEaseOutExpo();


                }

                IsAbleToUseButtons = false;
                StartCoroutine(UITimeDelayShort());
            }



            


            
        }

        public void OnJoinRoomButtonClicked()                                                   //При нажатии на кнопку Join
        {
            //SetActivePanel(JoinRandomRoomPanel.name);

            JoinRoomError.SetActive(true);
            PhotonNetwork.JoinRoom(joinInput.text);
        }

        public void OnLeaveGameButtonClicked()
        {
            

            if (IsAbleToUseButtons)
            {
                PhotonNetwork.LeaveRoom();
                CreateRoomPanel.SetActive(true);
                InsideRoomPanel.SetActive(false);

                SelectJoinUIScr.LeanMoveLocalY(0, 0.9f).setEaseOutExpo();
                PageTurnWaw.Play();

                IsAbleToUseButtons = false;
                StartCoroutine(UITimeDelayShort());

            }

        }

        public void OnLoginButtonClicked()
        {
            string playerName = PlayerNameInput.text;

            if (IsAbleToUseButtons)
            {
                if (!playerName.Equals(""))
                {
                    PhotonNetwork.LocalPlayer.NickName = playerName;
                    PhotonNetwork.ConnectUsingSettings();
                    ErrorName.SetActive(false);

                    PageTurnWaw.Play();
                    LoginUIScr.LeanMoveLocalY(-2000, 0.9f).setEaseOutExpo();             //При нажатии на кнопку Login
                    SelectJoinUIScr.LeanMoveLocalY(0, 0.9f).setEaseOutExpo();
                    SelectionPanel.SetActive(true);

                }
                else
                {
                    Debug.LogError("Player Name is invalid.");
                    ErrorName.SetActive(true);
                }


                IsAbleToUseButtons = false;
                StartCoroutine(UITimeDelayShort());

            }


            
        }

        public void OnRoomListButtonClicked()
        {
            if (!PhotonNetwork.InLobby)
            {
                PhotonNetwork.JoinLobby();
            }

            SetActivePanel(RoomListPanel.name);
        }

        public void OnStartGameButtonClicked()
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            string Name = GamemodeName.text;


            if (Name == "<Random Game>")
            {
                Name = Gamemodelist[Random.Range(1, Gamemodelist.Count - 1)];
                Debug.LogFormat("Randomly picked gamemode: {0}", Name);
            }

            switch (Name)
                {
                    case "Guess the theme":
                        PhotonNetwork.LoadLevel("GameScene1");
                        break;
                    case "Animal analyzing":
                        PhotonNetwork.LoadLevel("GameScene1");
                        break;
                    case "Suspicious inspectors":
                        PhotonNetwork.LoadLevel("GameScene1");
                        break;
                    case "Cooking Time":
                        PhotonNetwork.LoadLevel("GameScene_CookingTime");
                        break;
                }


        }

        #endregion

        private bool CheckPlayersReady()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return false;
            }

            foreach (Player p in PhotonNetwork.PlayerList)
            {
                object isPlayerReady;
                if (p.CustomProperties.TryGetValue(PropsGame.PLAYER_READY, out isPlayerReady))
                {
                    if (!(bool) isPlayerReady)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }
        
        private void ClearRoomListView()
        {
            foreach (GameObject entry in roomListEntries.Values)
            {
                Destroy(entry.gameObject);
            }

            roomListEntries.Clear();
        }

        public void LocalPlayerPropertiesUpdated()
        {
            StartGameButton.gameObject.SetActive(CheckPlayersReady());
        }

        private void SetActivePanel(string activePanel)
        {
            LoginPanel.SetActive(activePanel.Equals(LoginPanel.name));
            //SelectionPanel.SetActive(activePanel.Equals(SelectionPanel.name));
            //CreateRoomPanel.SetActive(activePanel.Equals(\
            //
            //
            //Panel.name));
            //JoinRandomRoomPanel.SetActive(activePanel.Equals(JoinRandomRoomPanel.name));
            //RoomListPanel.SetActive(activePanel.Equals(RoomListPanel.name));    // UI should call OnRoomListButtonClicked() to activate this
            //InsideRoomPanel.SetActive(activePanel.Equals(InsideRoomPanel.name));
        }

        private void UpdateCachedRoomList(List<RoomInfo> roomList)
        {
            foreach (RoomInfo info in roomList)
            {
                // Remove room from cached room list if it got closed, became invisible or was marked as removed
                if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
                {
                    if (cachedRoomList.ContainsKey(info.Name))
                    {
                        cachedRoomList.Remove(info.Name);
                    }

                    continue;
                }

                // Update cached room info
                if (cachedRoomList.ContainsKey(info.Name))
                {
                    cachedRoomList[info.Name] = info;
                }
                // Add new room info to cache
                else
                {
                    cachedRoomList.Add(info.Name, info);
                }
            }
        }

        /*private void UpdateRoomListView()
        {
            foreach (RoomInfo info in cachedRoomList.Values)
            {
                GameObject entry = Instantiate(RoomListEntryPrefab);
                entry.transform.SetParent(RoomListContent.transform);
                entry.transform.localScale = Vector3.one;
                entry.GetComponent<RoomListEntry>().Initialize(info.Name, (byte)info.PlayerCount, info.MaxPlayers);

                roomListEntries.Add(info.Name, entry);
            }
        }*/


        #region UI
       
        public void SwitchToRoomCreating()
        {
            if (IsAbleToUseButtons)
            {
                CreateRoomUIScr.localPosition = new Vector2(0, -2000);

                CreateRoomPanel.SetActive(true);
                CreateRoomUIScr.LeanMoveLocalY(100, 0.9f).setEaseOutExpo();
                SelectJoinUIScr.LeanMoveLocalY(1650, 0.9f).setEaseOutExpo();

                JoinRoomError.SetActive(false);
                PageTurnWaw.Play();

                IsAbleToUseButtons = false;
                StartCoroutine(UITimeDelayShort());

            }
            
        }

        public void SwitchToCancelCreating()
        {
            if (IsAbleToUseButtons)
            {

                IsAbleToUseButtons = false;
                StartCoroutine(UITimeDelay());

            }


        }


        #endregion



    }
}