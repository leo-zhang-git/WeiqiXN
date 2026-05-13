# 实体、组件与系统模块

## 主要文件

- `Assets/Scripts/Game/Entity/EntityBase.cs`
- `Assets/Scripts/Game/Entity/EntityWithGO.cs`
- `Assets/Scripts/Game/Entity/EntityUtils.cs`
- `Assets/Scripts/Game/Entity/Player.cs`
- `Assets/Scripts/Game/Entity/Chess.cs`
- `Assets/Scripts/Game/Component/*`
- `Assets/Scripts/Game/System/SystemBase.cs`
- `Assets/Scripts/Game/Scene/SceneBase.cs`

## 职责

该模块负责项目层的运行时对象模型。`SceneBase` 持有实体、组件和系统；实体承载可保存数据和实体组件；系统处理场景行为。

## 当前进度

- `EntityBase` 支持 guid、实体类型、组件字典、销毁、定时器挂载。
- `EntityWithGO` 把项目层实体和 Unity `GameObject` 连接起来。
- `Player` 表示本地对局玩家，使用 `PlayerFlag` 区分双方。
- `Chess` 表示棋子实体，记录归属玩家和棋盘坐标。
- `SceneBase` 支持按 guid 获取实体、按类型维护实体集合、发出实体创建/销毁事件。
- `SystemBase` 提供系统名和场景引用，是场景行为的基础类。

## 设计观察

当前结构接近轻量 ECS：实体存状态，组件扩展状态，系统处理行为。它比直接在 Unity 物体上写业务逻辑更适合做联机，因为同步对象可以优先面向实体和领域状态。

## 风险和缺口

- 实体 guid 生成策略需要在联机前确认，不能让不同客户端各自生成冲突或不一致的权威 guid。
- `EntityWithGO` 销毁会销毁 Unity 对象，联机回滚或重放时要避免频繁直接操作表现对象。
- 系统执行顺序由 `DuelScene.AddSystem` 调用顺序决定，应作为稳定约束记录。

## 后续建议

- 联机前明确实体 id 来源：服务器分配、确定性生成，或本地临时 id 加服务器映射。
- 把“领域状态”和“表现对象”进一步区分，避免网络同步直接依赖 GameObject。
- 为系统顺序增加文档和必要注释，尤其是保存、棋盘、对局系统之间的顺序。
