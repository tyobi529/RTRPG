using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// MonoBehaviourではなくMonoBehaviourPunCallbacksを継承して、Photonのコールバックを受け取れるようにする
public class LoginController : MonoBehaviourPunCallbacks
{
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



    }




    //時間差で生成する用
    IEnumerator GameControllerGenerate()
    {

        PhotonNetwork.Instantiate("Player2", new Vector3(0, 0, 0), Quaternion.identity);
        yield return new WaitForSeconds(2);

        PhotonNetwork.Instantiate("GameController", new Vector3(0, 0, 0), Quaternion.identity);
    }
}