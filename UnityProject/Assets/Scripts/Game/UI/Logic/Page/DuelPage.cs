using UnityEngine;
using XNClient.ChessBoard;

public class DuelPage : UIPageWithBinder<DuelPageUI>
{
    public override string pageName => UIPage.GetPageName<DuelPage>();
    public GameObject aimVFX;
    public RectCoordinates aimCoords = new RectCoordinates(-1, -1);
    private const string AIM_VFX_GAME_PREFAB_TYPEID = "FX_LootDrop_Blue";

    protected override void OnLoaded()
    {
        base.OnLoaded();

        RegisterSystemEvent<OnDuelStateChanged>(OnDuelStateChanged);

        binder.btn_save.onClick.AddListener(OnClickBtnSave);
        binder.btn_exit.onClick.AddListener(OnClickBtnExit);

        if (aimVFX == null) {
            var gamePrefabCfg = GamePrefabDataType.GetConfigData(AIM_VFX_GAME_PREFAB_TYPEID);
            if (gamePrefabCfg != null) {
                aimVFX = Global.Instance.resourceManager.LoadGamePrefab(gamePrefabCfg.resPath);
            }
        }
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
        if (compDuel != null && aimVFX != null) {
            if (compDuel.duelFSM.curState.stateName == DuelStateDefine.STATE_TURN_INPUT) {
                Ray mouseRay = Global.Instance.uiManager.uiCamera.ScreenPointToRay(Input.mousePosition);
                // UI射线把落子vfx放到指定位置
                if (Physics.Raycast(mouseRay.origin, mouseRay.direction, out var hitInfo, 500)) {
                    aimVFX.SetActive(true);
                    var compChessBoard = mainScene.GetComponent<SceneComponentChessBoard>();
                    if (compChessBoard != null) {
                        Transform gridTransform = compChessBoard.chessBoardGrid.transform;
                        Vector3 localHitPoint = gridTransform.InverseTransformPoint(hitInfo.point);
                        float cellSideLength = ChessBoardConfig.rectCellSideLength;

                        int nearestCellX = Mathf.RoundToInt(localHitPoint.x / cellSideLength - 0.5f);
                        int nearestCellZ = Mathf.RoundToInt(localHitPoint.z / cellSideLength - 0.5f);

                        int maxCellIndex = Mathf.Max(compChessBoard.chessBoardGrid.gridSize - 1, 0);
                        nearestCellX = Mathf.Clamp(nearestCellX, 0, maxCellIndex);
                        nearestCellZ = Mathf.Clamp(nearestCellZ, 0, maxCellIndex);

                        Vector3 nearestCellCenterLocalPos = new Vector3(
                            (nearestCellX + 0.5f) * cellSideLength,
                            0f,
                            (nearestCellZ + 0.5f) * cellSideLength
                        );
                        aimVFX.transform.position = gridTransform.TransformPoint(nearestCellCenterLocalPos);
                        aimCoords.SetValue(nearestCellX, nearestCellZ);
                    }
                } else {
                    aimVFX.SetActive(false);
                }
            }
        }

        // TODO input manager
        if (UnityEngine.Input.GetKeyDown(KeyCode.Mouse0)) {
            OnMouse0Down();
        }
    }

    protected override void OnClose()
    {
        base.OnClose();
        if (aimVFX != null) {
            GameObject.DestroyImmediate(aimVFX);
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
