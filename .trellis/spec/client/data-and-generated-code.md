# 配置与生成代码规范

## 当前配置体系

本项目使用 `ClientExcel/` 和自有 Unity Editor DataTableGenerator。

```text
ClientExcel/Game/*.xlsx
ClientExcel/GameHotfix/*.xlsx
        ↓ Unity Editor: GameApp/DataTable/Generate/*
Client/Assets/Scripts/GameApp/Generate/TableCode/DR*.cs
Client/Assets/Scripts/GameApp/Hotfix/Code/Runtime/Generate/TableEnum/E*.cs
Client/Assets/Res/Generate/TableData/**/*.bytes
Client/Assets/Res/Generate/TableData/Localization/*.xml
```

生成入口定义在：

- `Client/Assets/Scripts/GameApp/Editor/DataTableGenerator/DataTableGeneratorAll.cs`
- `Client/Assets/Scripts/GameApp/Editor/DataTableGenerator/DataTableGeneratorMenu.cs`
- `Client/Assets/Scripts/GameApp/Editor/DataTableGenerator/DataTableEnumGenerator.cs`
- `Client/Assets/Scripts/GameApp/Editor/DataTableGenerator/DataTableLocalizationGenerator.cs`

## 源文件和产物

| 类型 | 应修改的源 | 生成产物 |
| --- | --- | --- |
| UI/Entity/Scene/Sound 等常驻配置 | `ClientExcel/Game/*.xlsx` | `DR*.cs`、`*.bytes` |
| Guide 等热更配置 | `ClientExcel/GameHotfix/*.xlsx` | 热更表数据、`DRGuide` 等 |
| 运行时 ID 枚举 | Excel 表中的 ID/名称 | `Client/Assets/Scripts/GameApp/Hotfix/Code/Runtime/Generate/TableEnum/E*.cs` |
| 多语言 | `ClientExcel/GameHotfix/$Localization.xlsx` | `Client/Assets/Res/Generate/TableData/Localization/*.xml` |
| UI/Entity 控件绑定 | Prefab 节点与 CodeBind 标记 | `*.Bind.cs` |
| Input System | Input Actions 资产 | `Generate/InputCode/*Actions.cs` |

生成文件通常带有“自动生成，请勿直接修改”文件头。看到该标记时，必须追溯到 Excel、Prefab、Input Actions 或生成模板修改。

## 表格变更流程

1. 确认数据属于常驻 `Game` 还是热更 `GameHotfix`。
2. 修改对应 Excel；资源引用通过 `Asset.xlsx` 建立，不在业务代码写死路径。
3. 在 Unity 执行 `GameApp/DataTable/Generate/Gen All By Bin`。
4. 检查生成差异是否只包含预期表、枚举、二进制和本地化文件。
5. 编译 Unity，确认 `DR*` 字段与消费代码一致。
6. 在 Play Mode 验证数据实际能被 `GameEntry.DataTable` 读取。

`Gen All By Txt` 是另一种已有模式；除非目标环境明确要求文本表，否则保持当前 Bin 流程。

## UI/Entity 配置关系

UI：

- `UIForm.xlsx` 保存 AssetId、GroupId、多实例和覆盖暂停等配置。
- `UIFormGroup.xlsx` 定义分组。
- `Asset.xlsx` 提供最终资源路径。
- 运行时由 `UIExtension.TryGetTableData` 串联 `DRUIForm`、`DRUIFormGroup`、`DRAsset`。

Entity：

- `Entity.xlsx` 保存实体与 Asset/Group 的映射。
- `EntityGroup.xlsx` 定义分组。
- `Asset.xlsx` 提供预制体路径。
- 运行时由 `EntityExtension.TryGetTableData` 串联 `DREntity`、`DREntityGroup`、`DRAsset`。

不要在 Excel、枚举和业务代码各维护一套互不关联的 ID。

## 本地化生成与加载

本地化源文件为 `ClientExcel/GameHotfix/$Localization.xlsx`。生成器会同步维护：

- `Client/Assets/Res/Generate/TableData/Localization/<Language>.xml`
- `Client/Assets/Scripts/GameApp/Hotfix/Code/Runtime/Generate/TableConst/LocalizationKey.cs`

运行时由 `LocalizationExtension.LoadLanguageAsync` 清空旧字典、恢复随包内置字典，再按 `AssetPathUtility.GetDictionaryAsset(language, false)` 读取 XML。业务优先使用生成的 `HotConstant.LocalizationKey`，不要手工维护另一份热更 Key 常量。

语言表改动后必须同时检查 XML 与 `LocalizationKey.cs` 差异，并在启动预加载或语言界面流程中验证实际读取。详细生命周期见 `resource-and-localization-guidelines.md`。

## 禁止直接编辑

- `Client/Assets/Scripts/GameApp/Generate/TableCode/DR*.cs`
- `Client/Assets/Scripts/GameApp/Hotfix/Code/Runtime/Generate/TableEnum/E*.cs`
- `Client/Assets/Scripts/GameApp/Hotfix/Code/Runtime/Generate/InputCode/*Actions.cs`
- 任意 `*.Bind.cs`
- `Client/Assets/Res/Generate/TableData/**`

若生成器本身有缺陷，修改 `Client/Assets/Scripts/GameApp/Editor/DataTableGenerator/` 或对应 CodeBind/Input 配置，再重新生成并检查所有受影响产物。
