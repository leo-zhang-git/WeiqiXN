# 编辑器工具模块

## 主要文件

- `Assets/Scripts/Editor/UI/UICodeGenerator.cs`
- `Assets/Scripts/Editor/UI/CSCodeGenerator.cs`
- `Assets/Scripts/Editor/Inspector/UIBinderEditor_Inspector.cs`
- `Assets/Scripts/Editor/Inspector/UIBinderBase_Inspector.cs`
- `Assets/Scripts/Editor/Build/AssetBundleGenerator.cs`
- `Assets/Scripts/Editor/Build/BuildConfig.cs`
- `Assets/Scripts/Editor/TMPSprite/SpriteAtlasToTMPSpriteTool.cs`
- `Assets/Scripts/Editor/Inspector/TextureArrayWizard.cs`

## 职责

编辑器工具模块负责提升内容生产效率，包括 UI Binder 代码生成、UI 逻辑脚手架、AssetBundle 生成、TMP sprite 转换和材质/贴图辅助工具。

## 当前进度

- `UICodeGenerator` 会根据 `UIBinderEditor` 节点导出 Binder 脚本。
- 如果 UI 逻辑文件不存在，生成器会创建基础 `UIPageWithBinder<T>` 或 `UIWidgetWithBinder<T>` 逻辑类。
- 生成脚本会自动刷新 AssetDatabase。
- 项目已有多份生成的 `*PageUI.cs` 和页面逻辑类。
- AssetBundle、TMP sprite、TextureArray 等编辑器工具文件已经存在。

## 设计观察

UI 工具链已经承担了重复代码生成工作，这对后续快速增加联机页面有帮助。现有自动生成 Binder 的方式要求 prefab 上的绑定信息和生成脚本保持同步。

## 风险和缺口

- 自动生成文件可能覆盖手写 Binder 修改，Binder 文件应视为生成产物。
- UI 逻辑文件只在不存在时生成，后续逻辑需要手工维护。
- 编辑器工具没有在文档中形成使用流程，新成员容易误改生成文件。

## 后续建议

- 在 UI 模块补一份“新增页面流程”。
- 明确 `*UI.cs` Binder 文件为生成文件，业务逻辑写在 `Logic/Page` 或 `Logic/Widget`。
- 联机页面新增前先跑一遍 UI 生成流程，避免手工维护绑定字段。
