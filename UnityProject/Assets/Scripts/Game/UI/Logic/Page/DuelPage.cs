using UnityEngine;
using XNClient.ChessBoard;

public class DuelPage : UIPageWithBinder<DuelPageUI>
{
    public override string pageName => UIPage.GetPageName<DuelPage>();
    public GameObject aimChessPreview;
    public RectCoordinates aimCoords = new RectCoordinates(-1, -1);
    private PlayerFlag aimChessPreviewPlayerFlag;

    protected override void OnLoaded()
    {
        base.OnLoaded();

        RegisterSystemEvent<OnDuelStateChanged>(OnDuelStateChanged);

        binder.btn_save.onClick.AddListener(OnClickBtnSave);
        binder.btn_exit.onClick.AddListener(OnClickBtnExit);
    }

    protected override void OnOpen()
    {
        base.OnOpen();

        RefreshDebugPanel();
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        RefreshDebugPanel();

        aimCoords.SetValue(-1, -1);
        var mainScene = Global.Instance.sceneManager.mainScene;
        var compDuel = mainScene.GetComponent<SceneComponentDuel>();
        if (compDuel != null && compDuel.duelFSM.curState.stateName == DuelStateDefine.STATE_TURN_INPUT) {
            RefreshAimChessPreview(mainScene, compDuel);
        } else {
            SetAimChessPreviewActive(false);
        }

        // TODO input manager
        if (UnityEngine.Input.GetKeyDown(KeyCode.Mouse0)) {
            OnMouse0Down();
        }
    }

    protected override void OnClose()
    {
        base.OnClose();
        if (aimChessPreview != null) {
            GameObject.DestroyImmediate(aimChessPreview);
            aimChessPreview = null;
        }
    }

    private void RefreshAimChessPreview(SceneBase mainScene, SceneComponentDuel compDuel)
    {
        Player curPlayer = mainScene.GetEntity<Player>(compDuel.curTurnPlayerGuid.value);
        if (curPlayer == null) {
            SetAimChessPreviewActive(false);
            return;
        }

        Ray mouseRay = Global.Instance.uiManager.uiCamera.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(mouseRay.origin, mouseRay.direction, out var hitInfo, 500)) {
            SetAimChessPreviewActive(false);
            return;
        }

        var compChessBoard = mainScene.GetComponent<SceneComponentChessBoard>();
        if (compChessBoard == null) {
            SetAimChessPreviewActive(false);
            return;
        }

        Transform gridTransform = compChessBoard.chessBoardGrid.transform;
        Vector3 localHitPoint = gridTransform.InverseTransformPoint(hitInfo.point);
        float cellSideLength = ChessBoardConfig.rectCellSideLength;

        int nearestCellX = Mathf.RoundToInt(localHitPoint.x / cellSideLength - 0.5f);
        int nearestCellZ = Mathf.RoundToInt(localHitPoint.z / cellSideLength - 0.5f);

        int maxCellIndex = Mathf.Max(compChessBoard.chessBoardGrid.gridSize - 1, 0);
        nearestCellX = Mathf.Clamp(nearestCellX, 0, maxCellIndex);
        nearestCellZ = Mathf.Clamp(nearestCellZ, 0, maxCellIndex);

        RectCoordinates nearestCoords = new RectCoordinates(nearestCellX, nearestCellZ);
        int posIndex = compChessBoard.GetPosIndexByCoords(nearestCoords);
        if (posIndex < 0 || compChessBoard.chessInfoDict.ContainsKey(posIndex.ToString())) {
            SetAimChessPreviewActive(false);
            return;
        }

        EnsureAimChessPreview((PlayerFlag)curPlayer.playerFlag.value);
        if (aimChessPreview == null) {
            return;
        }

        Vector3 nearestCellCenterLocalPos = new Vector3(
            (nearestCellX + 0.5f) * cellSideLength,
            0f,
            (nearestCellZ + 0.5f) * cellSideLength
        );
        aimChessPreview.transform.position = gridTransform.TransformPoint(nearestCellCenterLocalPos);
        aimCoords.SetValue(nearestCoords.x, nearestCoords.z);
        SetAimChessPreviewActive(true);
    }

    private void EnsureAimChessPreview(PlayerFlag playerFlag)
    {
        if (aimChessPreview != null && aimChessPreviewPlayerFlag == playerFlag) {
            return;
        }

        if (aimChessPreview != null) {
            GameObject.DestroyImmediate(aimChessPreview);
            aimChessPreview = null;
        }

        string gamePrefabTypeId = DuelUtils.GetGamePrefabTypeIdWithPlayerFlag(playerFlag);
        var gamePrefabCfg = GamePrefabDataType.GetConfigData(gamePrefabTypeId);
        if (gamePrefabCfg == null) {
            return;
        }

        aimChessPreview = Global.Instance.resourceManager.LoadGamePrefab(gamePrefabCfg.resPath);
        if (aimChessPreview == null) {
            return;
        }

        aimChessPreviewPlayerFlag = playerFlag;
        SetAimChessPreviewActive(false);
        foreach (var collider in aimChessPreview.GetComponentsInChildren<Collider>()) {
            collider.enabled = false;
        }
    }

    private void SetAimChessPreviewActive(bool isActive)
    {
        if (aimChessPreview != null) {
            aimChessPreview.SetActive(isActive);
        }
    }

    public void OnDuelStateChanged(OnDuelStateChanged evt)
    {
        RefreshDebugPanel();
    }

    public void RefreshDebugPanel()
    {
        var mainScene = Global.Instance.sceneManager.mainScene;
        var compDuel = mainScene.GetComponent<SceneComponentDuel>();
        if (compDuel != null && compDuel.duelFSM.isActivated) {
            binder.txt_cur_state.text = compDuel.duelFSM.curState.stateName;
            switch (compDuel.duelFSM.curState.stateName) {
                case DuelStateDefine.STATE_TURN_INPUT:
                    Player player = mainScene.GetEntity<Player>(compDuel.curTurnPlayerGuid.value);
                    if (player != null) {
                        binder.txt_cur_player.text = player.guid;
                        var compDuelInfo = player.GetComponent<ComponentDuelInfo>();
                        if (compDuelInfo != null) {
                            binder.txt_turn_time.text = compDuelInfo.turnLeftTimes.value.ToString();
                        }
                    }
                    break;
            }
        }
    }

    public void OnMouse0Down()
    {
        var mainScene = Global.Instance.sceneManager.mainScene;
        var compDuel = mainScene.GetComponent<SceneComponentDuel>();
        if (compDuel != null && compDuel.duelFSM.curState.stateName == DuelStateDefine.STATE_TURN_INPUT) {
            EmitSystemEvent(new OnAddChessToBoard(aimCoords.Clone()));
        }
    }

    public void OnClickBtnSave()
    {
        EmitSystemEvent(new OnSaveDuelScene());
    }

    public void OnClickBtnExit()
    {
        Global.Instance.sceneManager.EnterMainScene(SceneConfig.MAIN_MENU_SCENE_TYPE_ID, SceneCreateParams.Default);
    }
}
