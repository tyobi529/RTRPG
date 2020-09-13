using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// MonoBehaviourではなくMonoBehaviourPunCallbacksを継承して、Photonのコールバックを受け取れるようにする
public class LoginController : MonoBehaviourPunCallbacks
{
    //public GameObject EnemyGenerator;
    public GameObject Enemy1;
    public GameObject Enemy2;
    public GameObject Enemy3;

    public GameObject HeroText;
    public GameObject HPText;
    public GameObject AttackText;
    public GameObject SpeedText;

    //勇者側は必殺ボタンを表示
    public GameObject SpecialText;
    public GameObject SpecialButton1;
    public GameObject SpecialButton2;
    public GameObject SpecialButton3;


    private void Start()
    {
        // PhotonServerSettingsに設定した内容を使ってマスターサーバーへ接続する
        PhotonNetwork.ConnectUsingSettings();
    }

    // マスターサーバーへの接続が成功した時に呼ばれるコールバック
    public override void OnConnectedToMaster()
    {
        // "room"という名前のルームに参加する（ルームが無ければ作成してから参加する）
        PhotonNetwork.JoinOrCreateRoom("room", new RoomOptions(), TypedLobby.Default);
    }

    // マッチングが成功した時に呼ばれるコールバック
    public override void OnJoinedRoom()
    {

        //MasterClientにGameControllerの生成
        if (PhotonNetwork.LocalPlayer.ActorNumber == 1)
        {
            PhotonNetwork.Instantiate("GameController", new Vector3(0, 0, 0), Quaternion.identity);


        }

        else
        {
            //EnemyGenerator.SetActive(true);
            Enemy1.SetActive(true);
            Enemy2.SetActive(true);
            Enemy3.SetActive(true);

            HeroText.SetActive(false);
            HPText.SetActive(false);
            AttackText.SetActive(false);
            SpeedText.SetActive(false);

            //SpecialText.SetActive(false);
            //SpecialButton1.SetActive(false);
            //SpecialButton2.SetActive(false);
            //SpecialButton3.SetActive(false);


        }



    }


}