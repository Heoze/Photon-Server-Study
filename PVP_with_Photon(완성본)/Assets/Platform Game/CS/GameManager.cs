using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// 에셋에서 Photon Pun2 Free 임포트
using Photon.Pun;
using Photon.Realtime;

// 해시테이블(= HashMap(in java))을 쓸 때, 작성해주어야 함.. (포톤과의 충돌때문에)
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameManager : MonoBehaviourPunCallbacks
{
    public ChattingManager CM;
	public GameObject[] scene;

	public InputField NickNameInput;

	GameObject LocalPlayer;

	public Image[] healthView = new Image[3];
	public Image[] scoreView = new Image[3];
	

    public Text result;

	void Awake() {
        PhotonNetwork.ConnectUsingSettings();

        if(PhotonNetwork.InRoom) Roaded();
    }

    public void GameStart() {
    	scene[1].SetActive(false);
        scene[3].SetActive(false);

    	scene[2].SetActive(true);

    	// 닉네임 설정
    	PhotonNetwork.LocalPlayer.NickName = NickNameInput.text;
    	// Player 소환
    	LocalPlayer = PhotonNetwork.Instantiate("Player", new Vector3(0, 0, 0), Quaternion.identity);

        CM.GameStart();
    }

    public void Roaded() {
    	scene[0].SetActive(false);
    	scene[2].SetActive(false);
        scene[3].SetActive(false);

    	scene[1].SetActive(true);
    }

    public void GameEnding(string winner) {
        scene[2].SetActive(false);
        scene[3].SetActive(true);

        result.text = "WINNER IS " + winner;
    }

    public void Restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Roaded();
    }

    public override void OnConnectedToMaster()=> PhotonNetwork.JoinLobby();
    public override void OnJoinedLobby() {
    	RoomOptions ros = new RoomOptions();
        ros.MaxPlayers = 20;
        PhotonNetwork.JoinOrCreateRoom("ROOM", ros, null);
    }
    public override void OnJoinedRoom()=> Roaded();
}
