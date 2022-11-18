using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISwitching : MonoBehaviour
{
    public Transform LoginUIScr;
    public GameObject MainLoginUI;

    public Transform SelectJoinUIScr;
    public GameObject MainSelectJoinUI;

    public Transform CreateRoomUIScr;
    public GameObject MainCreateRoomUI;

    public Transform EnterRoomUIScr;
    public GameObject MainEnterRoomUI;




    private void Start()
    {
        SelectJoinUIScr.localPosition = new Vector2(0, 2000);
        CreateRoomUIScr.localPosition = new Vector2(0, -2000);
        MainSelectJoinUI.SetActive(true);
        
    }


    private void OnEnable()
    {

    }

    public void SwitchFromLoginUI()
    {
        LoginUIScr.LeanMoveLocalY(-2000, 1f).setEaseOutExpo().delay = 0.1f;
        SelectJoinUIScr.LeanMoveLocalY(0, 1f).setEaseOutExpo().delay = 0.1f;
        MainCreateRoomUI.SetActive(true);
    }

    public void SwitchToRoomCreating()
    {
        MainCreateRoomUI.SetActive(true);
        CreateRoomUIScr.LeanMoveLocalY(200, 2f).setEaseOutExpo().delay = 0.1f;
        SelectJoinUIScr.LeanMoveLocalY(1700, 2f).setEaseOutExpo().delay = 0.1f;
    }

    public void SwitchForCancelCreating()
    {
        MainSelectJoinUI.SetActive(true);

        CreateRoomUIScr.LeanMoveLocalY(-2000, 1f).setEaseOutExpo().delay = 0.1f;
        SelectJoinUIScr.LeanMoveLocalY(0, 1f).setEaseOutExpo().delay = 0.1f;


    }

    public void SwitchForEnterRoom()
    {
        MainEnterRoomUI.SetActive(true);

        CreateRoomUIScr.LeanMoveLocalX(-2000, 1f).setEaseOutExpo().delay = 0.1f;
        SelectJoinUIScr.LeanMoveLocalY(2000, 1f).setEaseOutExpo().delay = 0.1f;
        MainEnterRoomUI.LeanMoveLocalX(0, 1f).setEaseOutExpo().delay = 0.1f;
    }

    public void SwitchForMenuFromExitingRoom()
    {
        MainCreateRoomUI.SetActive(true);
        MainEnterRoomUI.SetActive(false);

        SelectJoinUIScr.LeanMoveLocalY(0, 1f).setEaseOutExpo().delay = 0.1f;
        
    }

}
