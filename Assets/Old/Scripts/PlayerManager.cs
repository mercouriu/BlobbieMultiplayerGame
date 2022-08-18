using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;


namespace Com.MyCompany.MyGame
{
    /// <summary>
    /// Player managerCheckAllPlayerLoadedLevel
    /// </summary>
    public class PlayerManager : MonoBehaviourPunCallbacks
    {


        #region Public Fields

        [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
        public static GameObject LocalPlayerInstance;

        [Tooltip("The current Score of our player")]
        public float Score = 0f;

        [Tooltip("The Player's UI GameObject Prefab")]
        [SerializeField]
        public GameObject PlayerUiPrefab;

        int levelNum;


        public Vector3 fromPosition = Vector3.zero;
        public Vector3 toPosition = Vector3.one;
        public float speed = 1;

        private float progress;

        bool[] IsPlayerInRoomNow = {false, false, false, false, false};

        int SlotNumber;
        private int PlayerCountInThisRoom;

        #endregion



        #region MonoBehaviour CallBacks

        void Awake()
        {
            // #Important
            // used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
            if (photonView.IsMine)
            {
                PlayerManager.LocalPlayerInstance = this.gameObject;
            }
            // #Critical
            // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
            DontDestroyOnLoad(this.gameObject);
        }

        void Start()
        {
            if (PlayerUiPrefab != null)
            {
                GameObject _uiGo = Instantiate(PlayerUiPrefab);
                _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
            }
            else
            {
                Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.", this);
            }

            if (PhotonNetwork.IsMasterClient)
            {
                IsPlayerInRoomNow[0] = true;
            }



            #if UNITY_5_4_OR_NEWER
            // Unity 5.4 has a new scene management. register a method to call CalledOnLevelWasLoaded.
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
            #endif
        }

        void CalledOnLevelWasLoaded(int level)
        {
            Scene m_Scene;
            string sceneName;
            m_Scene = SceneManager.GetActiveScene();
            sceneName = m_Scene.name;
            Debug.LogFormat("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAa");

            if (sceneName == "PlayingGame")
            {
                GameObject _uiGo = Instantiate(this.PlayerUiPrefab);
                _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
                gameObject.GetComponentInChildren<SpriteRenderer>().enabled = false;
                transform.Find("PlayerSlot").gameObject.SetActive(false);
                Debug.LogFormat("BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB");
                //transform.position += Vector3.
            }



        }

        #endregion



        #region Private Metods

        #if UNITY_5_4_OR_NEWER
        void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadingMode)
        {
            this.CalledOnLevelWasLoaded(scene.buildIndex);
        }
        #endif


        public override void OnPlayerLeftRoom(Player other)
        {

            //if (levelNum == 1)
            //{
            
                if (PhotonNetwork.IsMasterClient)
                {
                    Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName);
                    
                //transform.position = Vector3.Lerp(fromPosition, toPosition, progress);

            }

            //photonView.RPC()

            //}
            //this.transform.position.y < other.GetComponent<Transform>()transform.position.y
            if (true)
            {
                //transform.Translate(Vector3.up * 1.2f);
            }
            
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            
            Debug.LogFormat("OnPlayerEnteredRoom() {0}", newPlayer.NickName);

            if (PhotonNetwork.IsMasterClient)
            {
                if (IsPlayerInRoomNow[0] == false)
                {
                    IsPlayerInRoomNow[0] = true;
                    
                }
                else if (IsPlayerInRoomNow[1] == false)
                {
                    IsPlayerInRoomNow[1] = true;
                }
                else if (IsPlayerInRoomNow[2] == false)
                {
                    IsPlayerInRoomNow[2] = true;
                }
                else if (IsPlayerInRoomNow[3] == false)
                {
                    IsPlayerInRoomNow[3] = true;
                }
                else if (IsPlayerInRoomNow[4] == false)
                {
                    IsPlayerInRoomNow[4] = true;
                }
            }

        }

        #endregion

    }
}