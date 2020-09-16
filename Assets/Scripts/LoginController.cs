using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// MonoBehaviourではなくMonoBehaviourPunCallbacksを継承して、Photonのコールバックを受け取れるようにする
public class LoginController : MonoBehaviourPunCallbacks
{
    GameObject Hero;

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

        FieldGenerator fieldGenerator = GameObject.Find("FieldGenerator").GetComponent<FieldGenerator>();

        if (PhotonNetwork.LocalPlayer.ActorNumber == 1)
        {
            Vector2 v = Camera.main.ScreenToWorldPoint(new Vector2(750f / 2f, fieldGenerator.footer + 750f / fieldGenerator.yoko / 2f));


            PhotonNetwork.Instantiate("GameController", v, Quaternion.identity);




        }
        else if (PhotonNetwork.LocalPlayer.ActorNumber == 2)
        {
            //Vector2 v = Camera.main.ScreenToWorldPoint(new Vector2(750f / 2f, fieldGenerator.footer + 750f / 2f));
            Vector2 v = Camera.main.ScreenToWorldPoint(new Vector2(750f / 2f, fieldGenerator.footer + 750f + 750f / fieldGenerator.yoko / 2f));

            //PhotonNetwork.Instantiate("Maou", v, Quaternion.identity);




        }


    }
}