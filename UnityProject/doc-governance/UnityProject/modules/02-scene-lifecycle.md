# 场景与生命周期模块

## 主要文件

- `Assets/Scripts/GlobalModule/SceneManager/SceneManager.cs`
- `Assets/Scripts/Game/Scene/SceneBase.cs`
- `Assets/Scripts/Game/Scene/MainMenuScene.cs`
- `Assets/Scripts/Game/Scene/DuelScene.cs`
- `Assets/Scripts/Game/Scene/SceneConfig.cs`
- `Assets/Scripts/Game/Scene/SceneCreateParams.cs`
- `Assets/Scripts/Game/Scene/SceneFixedRef/DuelSceneFixedRef.cs`
- `Assets/Config/DataJson/scene/scene.json`

## 职责

场景模块负责根据配置创建项目层的 `SceneBase` 子类，再异步加载 Unity 场景。项目层场景负责组合组件、系统、实体、UI 和保存恢复逻辑。

## 当前进度

- `SceneManager.EnterMainScene` 可根据 scene type id 进入主菜单或对局。
- `SceneBase` 支持异步加载 Unity 场景、设置 active scene、发出 `OnActiveSceneChanged`、关闭 LoadingPage。
- `SceneBase` 同时承担实体容器、场景组件容器、系统容器、事件接收者、定时器挂载者和资源加载绑定者。
- `MainMenuScene` 加载后打开 `MainMenuPage`。
- `DuelScene` 创建棋盘组件和对局组件，绑定 `DuelSceneFixedRef`，添加对局相关系统，打开 `DuelPage`。
- 进入带 `saveFilePath` 的场景时，`SceneBase.RestoreSceneData` 会先读取保存数据，再加载 Unity 场景。

## 设计观察

项目层场景和 Unity 场景分离是合理的。Unity 场景负责可视对象和固定引用，项目层场景负责逻辑生命周期。这个边界对后续联机有价值，因为网络同步通常要驱动项目层状态，而不是直接驱动 Unity 场景对象。

## 风险和缺口

- `SceneBase` 职责较多，既是保存对象，又是实体容器、系统容器、事件接收者、定时器挂载者和资源绑定者。
- 加载进度目前是 `TODO update load progress`。
- 目前只支持一个 `mainScene`，没有叠加场景、战斗外后台场景或联机房间场景概念。
- 保存恢复发生在 Unity 场景加载前，固定引用绑定发生在加载后，相关组件必须能承受这个顺序。

## 后续建议

- 为场景进入、退出、恢复存档写一份手动验证清单。
- 如果联机要引入房间/大厅，先判断它是普通 UI 页面还是新的 scene type。
- 保持“网络状态不直接等于 Unity 场景状态”的边界，避免重连时难以恢复。
