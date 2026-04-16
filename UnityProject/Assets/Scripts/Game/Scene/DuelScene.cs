using UnityEngine;

public class DuelScene : SceneBase
{
    public SceneComponentChessBoard compChessBoard;

    public DuelScene(SceneDataType configData) : base(configData)
    {
        compChessBoard = new SceneComponentChessBoard(this);
    }

    public override void OnSceneLoaded()
    {
        base.OnSceneLoaded();

        foreach (var rootObj in unityScene.GetRootGameObjects()) {
            DuelSceneFixedRef fixedRef = rootObj.GetComponent<DuelSceneFixedRef>();
            if (fixedRef != null) {
                compChessBoard.chessBoardGrid = fixedRef.chessBoardGrid;
                compChessBoard.duelVCam = fixedRef.duelVCam;
                break;
            }
        }

        // Test Code
        string[] settings = new string[]
        {
            "9x9", "13x13", "19x19"
        };
        compChessBoard.boardCfgId.value = settings[Random.Range(0, settings.Length)];

        AddSystem(new DuelSaveSystem(this));
        AddSystem(new ChessBoardSystem(this));

        Global.Instance.uiManager.ShowPage<DuelPage>();
    }
}
