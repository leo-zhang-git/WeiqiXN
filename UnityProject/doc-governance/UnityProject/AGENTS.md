# WeiqiXN UnityProject

## Project Positioning

**项目定位**

- WeiqiXN UnityProject 是一个基于 Unity 的围棋游戏项目。
- 当前项目以本地对局为基础能力：棋盘选择、棋盘生成、回合流转、鼠标落子、提子规则、自杀禁手、简单防重复局面、UI、资源加载和存档链路已经具备基础形态。
- 项目的后续产品方向是联机对局；当前代码库尚未实现网络层、房间、匹配、同步协议或断线恢复。

## Governance Profile

**治理画像**

- Governance Profile ID: stage-driven
- Governance Profile Name: 按阶段推进
- Governance Profile Summary: 阶段内连续推进，阶段结束后统一收口。

## Document Routing

**文档路由**

- 当前系统事实以 [SPECIFICATION.md](SPECIFICATION.md) 为准。
- 架构边界、设计理由和长期约束以 [ARCHITECTURE.md](ARCHITECTURE.md) 为准。
- 阶段计划、当前目标和范围控制以 [ROADMAP.md](ROADMAP.md) 为准。
- 模块扫描说明放在 [modules/00-progress-check.md](modules/00-progress-check.md) 及同目录的模块文档中。
- 本文件只定义入口、执行约束、文档路由和维护规则，不承载详细系统事实。

## Execution Boundaries

**执行边界**

- `AGENTS.md`、`SPECIFICATION.md`、`ARCHITECTURE.md`、`ROADMAP.md` 是四个核心权威文档。
- `modules/` 目录是支撑说明层，可以解释模块细节，但不能覆盖四个核心文档中的权威结论。
- 生成物、临时状态、样例、日志、Unity 构建产物、`Library/`、`Temp/`、`Logs/`、IDE 元数据不属于文档权威层。
- 当前治理方式是按阶段推进：阶段内可以连续整理或实现，阶段结束后统一收口。
- 联机功能必须视为阶段性架构变更。新增网络框架、协议形态、服务器权威模型或同步模型时，必须同步更新 [ARCHITECTURE.md](ARCHITECTURE.md)、[ROADMAP.md](ROADMAP.md) 和 [modules/11-online-readiness.md](modules/11-online-readiness.md)。
- 在准备联机功能时，本地对局仍然是回归基线，除非某个阶段明确声明要替换本地流程。
- 修改棋规、落子、提子、回合、存档、场景切换或 UI 输入时，需要同时判断逻辑行为和可见流程是否受到影响。

## Document Update Matrix

**文档更新矩阵**

- 当前行为发生变化 -> 更新 [SPECIFICATION.md](SPECIFICATION.md)。
- 架构边界、职责划分或设计约束发生变化 -> 更新 [ARCHITECTURE.md](ARCHITECTURE.md)。
- 阶段目标、待办、范围或联机路线发生变化 -> 更新 [ROADMAP.md](ROADMAP.md)。
- 某个模块的入口文件、完成度、风险或维护说明发生变化 -> 更新 `modules/` 下对应模块文档。
- 文档路由、执行约束或治理方式发生变化 -> 更新本文件。

## Done Definition

**完成定义**

- 代码或文档修改已经落地。
- 必要检查已经完成；如无法执行，需要记录原因。
- 已判断对本地对局基线的影响。
- 已判断是否触发四件套文档或模块文档更新。
- 被触发的权威文档已经同步更新。
