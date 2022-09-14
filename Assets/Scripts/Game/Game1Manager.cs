// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsteroidsGameManager.cs" company="Exit Games GmbH">
//   Part of: Asteroid demo
// </copyright>
// <summary>
//  Game Manager for the Asteroid Demo
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------
using System.Linq;
using System;
using System.IO;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Photon.Pun.Demo.Asteroids
{
    public class Game1Manager : MonoBehaviourPunCallbacks
    {
        public static Game1Manager Instance = null;

        [SerializeField] public GameObject Stage1;
        [SerializeField] public GameObject Stage2;
        [SerializeField] public GameObject Stage3;
        [SerializeField] public GameObject Transparent;
        [SerializeField] public GameObject PaperBackground;
        [SerializeField] public GameObject PaperBackgroundTop;
        [SerializeField] public GameObject BottomPanel;
        [SerializeField] public GameObject ReadyButton;

        [SerializeField] public RawImage TestDrawingImage;
        [SerializeField] public Texture2D DrawingImage;
        [SerializeField] public Text reseivedText;

        public Texture2D receivedTexture;//хуета
        public Text InfoText;
        public Text PlayerInputText;
        public Text Stage3GuessInputText;
        public Button PlayerReadyButton;
        public Image PlayerReadyImage;

        private string ShuffledPlayersToSend;
        private int subEventNumber = 0;
        //private int CountOfPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
        private int ownerId;
        private int TimerCount;
        private bool isPlayerReady;
        private int GameStage;
        private Hashtable _extraProperties = new Hashtable();
        private Hashtable _shareProperties = new Hashtable();
        private bool first = true;

        List<string> ShuffledPlayers = new List<string>() { "10", "2" };
        public int ShuffledElement = 0;

        private int playerLocalStageNumber;

        private PhotonView PhotonView;

        #region UNITY

        public void Awake()
        {
            Instance = this;
        }

        public override void OnEnable()
        {
            base.OnEnable();

            CountdownTimer.OnCountdownTimerHasExpired += OnCountdownTimerIsExpired;
        }



        public void Start() //Даём игрокам узнать что мы загрузились в игру
        {
            PhotonView = GetComponent<PhotonView>();

            PlayerReadyButton.gameObject.SetActive(false);
            ownerId = PhotonNetwork.LocalPlayer.ActorNumber;
            Debug.LogFormat("Your Player ID is {0}", PhotonNetwork.LocalPlayer.ActorNumber);
            Debug.LogFormat("Your Owner ID is {0}", ownerId);

            Hashtable props = new Hashtable
            {
                {PropsGame.PLAYER_LOADED_LEVEL, true}, { PropsGame.All_PLAYERS_READY, false }
            };


            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
            Debug.LogFormat("Player loaded level state is {0}", PropsGame.PLAYER_LOADED_LEVEL);
            //

            if (PhotonNetwork.LocalPlayer.ActorNumber != ownerId)
            {
                PlayerReadyButton.gameObject.SetActive(false);
                
            }
            else //Взаимодействуем только со своей кнопкой готовности
            {
                PlayerReadyImage.enabled = false;//Выключаем галочу-маркер что мы готовы

                //начальные настройки готовности. сначала выключаем готовность
                Hashtable initialProps = new Hashtable() { { PropsGame.PLAYER_READY_NEXT_ROUND, isPlayerReady }, };
                PhotonNetwork.LocalPlayer.SetCustomProperties(initialProps);
                //PhotonNetwork.LocalPlayer.SetScore(0);

                PlayerReadyButton.onClick.AddListener(() =>
                {
                    isPlayerReady = !isPlayerReady;
                    SetPlayerReady(isPlayerReady);
                    Debug.LogFormat("Player ready?: {0}", isPlayerReady);
                    Hashtable props = new Hashtable() { { PropsGame.PLAYER_READY_NEXT_ROUND, isPlayerReady } };
                    PhotonNetwork.LocalPlayer.SetCustomProperties(props);

                    if (PhotonNetwork.IsMasterClient)
                    {
                        //FindObjectOfType<LobbyMainPanel>().LocalPlayerPropertiesUpdated();
                    }
                });
            }
        }


        public override void OnDisable()
        {
            base.OnDisable();

            CountdownTimer.OnCountdownTimerHasExpired -= OnCountdownTimerIsExpired;
        }

        public void Initialize(int playerId, string playerName)
        {
            ownerId = playerId;
            //PlayerNameText.text = playerName;
        }

        #endregion

        #region COROUTINES     

        private IEnumerator EndOfGame(string winner, int score)
        {
            float timer = 5.0f;

            while (timer > 0.0f)
            {
                InfoText.text = string.Format("Player {0} won with {1} points.\n\n\nReturning to login screen in {2} seconds.", winner, score, timer.ToString("n2"));

                yield return new WaitForEndOfFrame();

                timer -= Time.deltaTime;
            }

            PhotonNetwork.LeaveRoom();
        }

        #endregion

        #region PUN CALLBACKS

        public override void OnDisconnected(DisconnectCause cause)
        {
            
            UnityEngine.SceneManagement.SceneManager.LoadScene("MenuScene");
        }

        public override void OnLeftRoom()
        {
            PhotonNetwork.Disconnect();
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
            {
                //StartCoroutine(SpawnAsteroid());
            }
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            //CheckEndOfGame();
        }


        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (changedProps.ContainsKey(PropsGame.All_PLAYERS_READY))//Изменяем локальную готовность самого игрока, если мастер скажет, что все готовы
            {
                object playerReady;

                if (PhotonNetwork.LocalPlayer.ActorNumber == ownerId)
                {
                    if (targetPlayer.CustomProperties.TryGetValue(PropsGame.All_PLAYERS_READY, out playerReady))
                    {
                        if ((bool)playerReady)
                        {
                            isPlayerReady = false;
                            SetPlayerReady(isPlayerReady);
                            Debug.LogFormat("Игрок: {0}", PhotonNetwork.LocalPlayer);

                            Hashtable props = new Hashtable() { { PropsGame.PLAYER_READY_NEXT_ROUND, false }};
                            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
                        }
                    }
                }

                if (PhotonNetwork.IsMasterClient)
                {
                    Hashtable props = new Hashtable() { { PropsGame.All_PLAYERS_READY, false } };
                    PhotonNetwork.LocalPlayer.SetCustomProperties(props);
                }

                

                
            }
            



            /*
            if (changedProps.ContainsKey(AsteroidsGame.PLAYER_LIVES))
            {
                CheckEndOfGame();
                return;
            }*/

                if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }
            
            // if there was no countdown yet, the master client (this one) waits until everyone loaded the level and sets a timer start
            int startTimestamp;
            bool startTimeIsSet = CountdownTimer.TryGetStartTime(out startTimestamp);

            if (changedProps.ContainsKey(PropsGame.PLAYER_LOADED_LEVEL))
            {
                if (CheckAllPlayerLoadedLevel())
                {
                    if (!startTimeIsSet)
                    {
                        CountdownTimer.SetStartTime(5f);  //Таймер начала игры
                    }
                }
                else
                {
                    // not all players loaded yet. wait:
                    Debug.Log("setting text waiting for players! ",this.InfoText);
                    InfoText.text = "Waiting for other players...";
                }
            }


            //Если поменялся номер игровой стадии
            if (changedProps.ContainsKey("StageNumber")) //Обновление и запуск всех этапов игры
            {

                if (TimerCount > 3)
                {
                    TimerCount = 3;

                }

                /*
                int StageNumber = (int)targetPlayer.CustomProperties["StageNumber"];
                int Chapter = (int)targetPlayer.CustomProperties["Chapter"];
                */
                int StageNumber = TimerCount;
                int Chapter = (int)PhotonNetwork.LocalPlayer.CustomProperties["Chapter"];
                Debug.LogFormat("ЧАПТЕР: {0}", Chapter);

                
                switch (Chapter) //Устанавливаем главы игры, что включают в себя раунды
                {
                    case 1: //Глава 1

                        switch (StageNumber) //Этапы
                        {
                            case 0: break;//0

                            case 1:

                                CountdownTimer.SetStartTime(30f);
                                break;

                            case 2:
                                CountdownTimer.SetStartTime(100f);
                                ShufflePlayers();//Перемешиваем список игроков перед началом их взаимодействия
                                
                                

                                break;


                            case 3:
                                if (subEventNumber < PhotonNetwork.CurrentRoom.PlayerCount)
                                {
                                    ShuffledPlayersToSend = ShuffledPlayers[ShuffledElement];
                                    Debug.LogFormat("Элементы: {0}, {1}", ShuffledElement, ShuffledPlayers[ShuffledElement]);
                                    if (subEventNumber < PhotonNetwork.CurrentRoom.PlayerCount-1)
                                    { 
                                        ShuffledElement++;
                                    }
                                    subEventNumber++;


                                }

                                
                                CountdownTimer.SetStartTime(50f);

                                /*
                                if(ShuffledElement < PhotonNetwork.CurrentRoom.PlayerCount)
                                {
                                    goto case 3;
                                }
                                */
                                break;


                            default: // если не входит в case
                                Debug.LogFormat("Стадия {0}, но она не входит в case StageNumber", TimerCount);
                                break;
                        }

                        photonView.RPC("SendStageInfo", RpcTarget.All, StageNumber, ShuffledPlayersToSend); //Передаём клиентам информацию что сейчас за стадия



                        break;//Chapter ended
                }

                



            }

            //Если поменялась готовность игрока
            if (changedProps.ContainsKey(PropsGame.PLAYER_READY_NEXT_ROUND))
            {
                if (CheckPlayersReadyNextRound())
                {
                    TimerCount++;

                    

                    _extraProperties["StageNumber"] = TimerCount;
                    PhotonNetwork.LocalPlayer.SetCustomProperties(_extraProperties);
                    Debug.LogFormat("StageNumber {0}", _extraProperties["StageNumber"]);


                    Hashtable props = new Hashtable() { { PropsGame.PLAYER_READY_NEXT_ROUND, false }, { PropsGame.All_PLAYERS_READY, true } };
                    PhotonNetwork.LocalPlayer.SetCustomProperties(props);

                }
                else
                {
                    // not all players loaded yet. wait:
                    //Debug.Log("Players Check - Not ready! ", this.InfoText);
                    //InfoText.text = "Waiting for next round...";
                }
            }
            
            if (changedProps.ContainsKey("InputData"))
            {


                Debug.LogFormat("UPDATEDDDDDDDDDDDDDDDDDDDDDDD", _shareProperties["InputData"]);
            }

        }

        #endregion

        //Функция перемешивает игроков по ID в списке
        private void ShufflePlayers()
        {
            ShuffledPlayers.Clear(); //очистить список

            foreach (Player player in PhotonNetwork.PlayerList)
            {

                ShuffledPlayers.Add(player.ActorNumber.ToString()); //добавляем id игроков в список
            
            }


            

            System.Random rand = new System.Random();
            
            ShuffledPlayers = ShuffledPlayers.OrderBy(_ => rand.Next()).ToList(); //перемешиваем

            //Просто выводим (дебажим) список
            string result = "List contents: ";
            foreach (var item in ShuffledPlayers)
            {
                result += item.ToString() + ", ";
            }
            Debug.Log(result);


        }


        // Вызывается после конца стартового таймера
        private void StartGame()
        {
            Debug.Log("StartGame!");


            if (PhotonNetwork.IsMasterClient)
            {
                //StartCoroutine(GameTimer1());
                //StartCoroutine(SpawnAsteroid());
                _extraProperties["StageNumber"] = 0;
                _extraProperties["Chapter"] = 1;
                PhotonNetwork.LocalPlayer.SetCustomProperties(_extraProperties);
            }

            PlayerReadyButton.gameObject.SetActive(true);
        }


        //Функция проверки готовности игроков
        private bool CheckAllPlayerLoadedLevel() 
        {
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                object playerLoadedLevel;

                if (p.CustomProperties.TryGetValue(PropsGame.PLAYER_LOADED_LEVEL, out playerLoadedLevel))
                {
                    if ((bool) playerLoadedLevel)
                    {
                        continue;
                    }
                }

                return false;
            }

            return true;
        }

        private bool CheckPlayersReadyNextRound() //Тут мы чекаем готовы ли игроки скипнуть время и перейти к следующему этапу игры
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return false;
            }

            foreach (Player p in PhotonNetwork.PlayerList)
            {
                object isPlayerReady;
                if (p.CustomProperties.TryGetValue(PropsGame.PLAYER_READY_NEXT_ROUND, out isPlayerReady))
                {
                    if (!(bool) isPlayerReady)
                    {
                        return false;
                    }
                }
                else
                {
                    Debug.LogFormat("PLAYERS DOESNT READY(SKIP). ID {0}", ownerId);
                    return false;
                }
            }

            return true;
        }


        private void OnCountdownTimerIsExpired()
        {
            
            if (first)
            {
                StartGame();
                first = false;
            }

            TimerCount++;
            

            _extraProperties["StageNumber"] = TimerCount;
            PhotonNetwork.LocalPlayer.SetCustomProperties(_extraProperties);
            Debug.LogFormat("StageNumber {0}", _extraProperties["StageNumber"]);

        }

        public void LocalPlayerPropertiesUpdated()
        {
            //StartGameButton.gameObject.SetActive(CheckPlayersReady());
            //StartCoroutine(GameTimer2());
        }

        public void SetPlayerReady(bool playerReady)
        {
            PlayerReadyButton.GetComponentInChildren<Text>().text = playerReady ? "" : "Done?";
            PlayerReadyImage.enabled = playerReady;
            PlayerReadyButton.image.enabled = !playerReady;
        }


        [PunRPC]
        public void ShareInputWithPlayers()//test(no)
        {
            _shareProperties["InputData"] = PlayerInputText.text;
            PhotonNetwork.LocalPlayer.SetCustomProperties(_shareProperties);
            Debug.LogFormat("Share is: {0}", _shareProperties["InputData"]);
            Debug.LogError(gameObject);
        }

        [PunRPC]
        void ChatMessage(string fileName, byte[] content)
        {
            // e.g.
            File.WriteAllBytes(Path.Combine(Application.persistentDataPath, fileName), content);
        }

        public void SendFile(string fileName, byte[] content)
        {
            photonView.RPC(nameof(ChatMessage), RpcTarget.Others, fileName, content);
        }


        [PunRPC]//test
        private void SendImage(Player player, byte[] receivedByte)
        {
            SendFile("Transparent", DrawingImage.EncodeToPNG());
            receivedTexture = null;
            receivedTexture = new Texture2D(1, 1);
            receivedTexture.LoadImage(receivedByte);

        }
                                    //none
        //Сервер говорит чья очередь по списку показывать контент
        //Отправляется функция всем игрокам, а тот игрок, чьё имя совпадает со списком записывает в CustomProperties свой контент
        //Далее все игроки считывают контент этого игрока
        [PunRPC]
        private void SendPlayerTurnToShowContent(string TargetActorID)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber.ToString() == TargetActorID)
            {
                //photonView.RPC("PlayerShowContent", RpcTarget.All, PlayerInputText.text);
                        //ShareInputWithPlayers();
            }


            foreach (Player p in PhotonNetwork.PlayerList)
            {
                


                
                if (p.ActorNumber.ToString() == TargetActorID)
                {
                    
                }
            }
        }






        [PunRPC]
        private void SendStageInfo(int number, string targetPlayerActorNumber)
        {
            // the photonView.RPC() call is the same as without the info parameter.
            // the info.Sender is the player who called the RPC.
            //Debug.LogFormat("Info: {0} {1} {2}", info.Sender, info.photonView, info.SentServerTime);

            switch (number)
            {
                case 1:
                    //Ввод текста
                    Stage1.gameObject.SetActive(true);
                    Transparent.gameObject.SetActive(false);
                    PaperBackground.gameObject.SetActive(false);
                    BottomPanel.gameObject.SetActive(false);
                    ReadyButton.gameObject.SetActive(true);

                    break;

                case 2:
                    //Рисование рисунка с прошлыми словом
                    Stage1.gameObject.SetActive(false);
                    Stage2.gameObject.SetActive(true);
                    Transparent.gameObject.SetActive(true);
                    PaperBackground.gameObject.SetActive(true);
                    BottomPanel.gameObject.SetActive(true);
                    break;
                case 3:

                    Stage2.gameObject.SetActive(false);
                    Transparent.gameObject.SetActive(false);
                    PaperBackground.gameObject.SetActive(false); PaperBackgroundTop.gameObject.SetActive(true);
                    BottomPanel.gameObject.SetActive(false);

                    if (PhotonNetwork.LocalPlayer.ActorNumber != Int32.Parse(targetPlayerActorNumber))
                    {
                        
                        Stage3.gameObject.SetActive(true);
                        
                    }
                    

                    foreach (Player player in PhotonNetwork.PlayerList)
                    {
                        
                        //ShuffledPlayers.Add(player.ActorNumber.ToString()); //добавляем id игроков в список

                        if (player.ActorNumber == Int32.Parse(targetPlayerActorNumber))
                        {
                            reseivedText.text = player.CustomProperties["InputData"].ToString();
                            Debug.LogFormat("Share (received) is: {0}", reseivedText.text);
                        }
                    }

                    
                    //Показ по очереди
                    break;
                case 4:
                    Stage3.gameObject.SetActive(false);

                    //Показ по очереди
                    break;

                default:

                    break;
            }
        }



    }
}