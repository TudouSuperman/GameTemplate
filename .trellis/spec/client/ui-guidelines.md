# UI 开发规范

本项目 UI 使用 GF UIComponent、项目级 `UGuiFormLogic` / `UGuiFormView`、CodeBind 和配置表。

## 一条完整 UI 链路

```text
UI Prefab
  + ClientExcel/Game/Asset.xlsx
  + ClientExcel/Game/UIForm.xlsx
  + ClientExcel/Game/UIFormGroup.xlsx
  + FormView / FormLogic
  + CodeBind 生成的 *.Bind.cs
  → EUIFormID
  → GameEntry.UI.OpenUIForm(...)
```

参考实现：

- 预制体：`Client/Assets/Res/Artwork/UI/UIForm/MainMenuForm.prefab`
- 逻辑：`Client/Assets/Scripts/GameApp/Hotfix/Code/Runtime/UI/MainMenu/MainMenuFormLogic.cs`
- 视图：`Client/Assets/Scripts/GameApp/Hotfix/Code/Runtime/UI/MainMenu/MainMenuFormView.cs`
- 绑定：`Client/Assets/Scripts/GameApp/Hotfix/Code/Runtime/UI/MainMenu/MainMenuFormView.Bind.cs`

## 创建或修改 UI

1. 在 `Client/Assets/Res/Artwork/UI/UIForm/` 创建或修改预制体。可使用 Unity 菜单 `GameObject/UI/UGuiForm` 创建项目模板。
2. 在 `ClientExcel/Game/Asset.xlsx` 维护资源记录，在 `UIForm.xlsx` 维护 UI 与 Asset/Group 的映射；分组定义放在 `UIFormGroup.xlsx`。
3. 在 `Client/Assets/Scripts/GameApp/Hotfix/Code/Runtime/UI/<Feature>/` 创建 `<Name>FormView.cs` 和 `<Name>FormLogic.cs`。
4. 在预制体上按 CodeBind 命名规则配置节点，使用项目 CodeBind 工具生成 `<Name>FormView.Bind.cs`。
5. 执行 `GameApp/DataTable/Generate/Gen All By Bin`，刷新运行时表数据、表行代码和热更枚举。
6. 通过 `GameEntry.UI.OpenUIForm((int)EUIFormID.<Name>, userData)` 打开，不直接硬编码资源路径。

若只改了其中一个环节，先检查跨层指南，避免出现“枚举存在但表无记录”“预制体存在但 AssetId 无法解析”等半成品。

## View 与 Logic 分工

`View`：

- 继承 `UGuiFormView`，声明为 `sealed partial`。
- 使用 `[MonoCodeBind('-')]`。
- 在 `OnInit()` 中把 Unity 控件事件转换为 `GameFrameworkAction` 或领域事件。
- 只处理控件引用和轻量展示行为，不持有 Procedure、Model 或全局业务流程。

`Logic`：

- 继承 `UGuiFormLogic`。
- 在 `OnInit` 连接 View 事件与业务动作。
- 在 `OnOpen` 校验并保存 `userData`；类型无效时记录 Warning 并停止后续逻辑。
- 在 `OnClose` 清空上下文、停止异步操作，并解除非永久事件关系。
- 通过 `GameEntry`、Model 或 Procedure 驱动业务。

`MainMenuFormView` 与 `MainMenuFormLogic` 是首选参考；不要把两者合并成一个大 MonoBehaviour。

## CodeBind 规则

- `*.Bind.cs` 文件头明确标注自动生成，禁止手工编辑。
- 手写代码放在同名非 Bind 的 partial 文件。
- 绑定字段为空时应修复预制体/命名并重新生成，不在 Bind 文件里补引用。
- 项目扩展的绑定类型定义在 `Client/Assets/Scripts/GameApp/Editor/CodeBindConfig/GameAppCodeBindNameTypeConfig.cs`，例如 `TMPText`、`PlayerInput`。
- 新增全局命名映射时修改该配置并重新生成；单个界面的特殊逻辑不要写进全局 CodeBind 配置。

## 打开、关闭和异步

- 普通打开使用 `UIExtension.OpenUIForm(int, object)`，它会解析 `DRUIForm`、`DRUIFormGroup` 和 `DRAsset`。
- 需要等待结果或取消时使用 `OpenUIFormAsync`，传入所属生命周期的 `CancellationToken`。
- 配置为不允许多实例时，不绕过扩展直接重复打开相同资源。
- UI 自己关闭使用 `CloseSelf()`；外部关闭已保存的 Logic 时使用 `GameEntry.UI.CloseUIForm(logic.UIForm)`。
- 打开成功事件必须用 `UserData` 过滤归属，参考 `ProcedureGame.OnOpenUIFormSuccess`。
- `Image` / `RawImage` 的运行时换图遵循 `resource-and-localization-guidelines.md`，不要让单个界面重新包装资源加载与释放。

## 常见错误

- 手改 `MainMenuFormView.Bind.cs` 一类生成文件。
- 直接写 `Assets/...` 字符串打开 UI，绕过 Asset/UIForm 表。
- 在 View 中切换 Procedure、读写 Model 或执行网络业务。
- 在 `OnOpen` 保存的引用没有在 `OnClose` 清空。
- 新建预制体和代码后忘记更新 Excel、生成枚举/表数据或 CodeBind。
