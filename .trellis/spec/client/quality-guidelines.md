# 质量检查规范

## 基线事实

- Unity 版本：`2022.3.15f1c1`，见 `Client/ProjectSettings/ProjectVersion.txt`。
- 当前没有正式的 Unity Test Framework 测试程序集；`Client/Assets/Scripts/Test/` 只有辅助脚本。
- `Client.sln` 和 `*.csproj` 由 Unity 生成，不能用修改生成工程文件解决程序集问题。

因此不要声称“测试已通过”，除非确实新增并运行了对应测试。当前最低质量门是静态差异检查、Unity 编译和与改动匹配的 Play Mode/Editor 工具验证。

## 每次改动都要做

1. 检查 Git 差异，只包含目标源码、资源、配置和预期生成产物。
2. 检查没有手改 `*.Bind.cs`、`Generate/` 代码或 `*.csproj`。
3. 在 Unity 2022.3.15f1c1 打开 `Client/`，等待脚本刷新并确认 Console 无编译错误。
4. 检查目标 `.asmdef` 的依赖方向，没有 Runtime 引用 Editor、常驻层引用热更层。
5. 进入 Play Mode 做目标流程的最小烟雾验证。
6. 退出 Play Mode，确认无未退订事件、未取消任务、未释放资源和重复加载异常。

可在命令行做的辅助检查：

```powershell
git diff --check
git status --short
rg -n "automatically generated|自动生成，请勿直接修改" Client/Assets/Scripts/GameApp
```

这些检查不能代替 Unity 编译。

## 按改动类型增加验证

### UI

- Prefab 能加载，CodeBind 字段没有空引用警告。
- Excel 中 Asset/UIForm/Group 关系正确，`EUIFormID` 已重新生成。
- 打开、重复打开限制、关闭和 `userData` 校验符合预期。
- UI 关闭后异步和事件不再回写该实例。

### Entity

- Prefab、Asset/Entity/Group 配置和 `EEntityID` 一致。
- Data 从 ReferencePool 获取并在回收后清空。
- Entity 隐藏/回收后资源、事件、父节点和异步状态正确。

### 配置表

- 从 Excel 源修改并执行 `GameApp/DataTable/Generate/Gen All By Bin`。
- 检查 `DR*`、枚举、bytes/XML 的差异，不提交无关的大面积重生成。
- Play Mode 实际读取目标数据行。

### HybridCLR

- 修改热更程序集依赖、AOT 泛型、Loader 或构建逻辑时执行 `HybridCLR/Do All`。
- 验证编辑器直跑模式和目标热更模式所需路径。
- Loader 能创建/销毁入口，热更流程从 `ProcedureLaunch` 正常进入业务流程。

### Editor 工具

- 菜单项可见并能在目标选择状态运行。
- 文件写入限制在预期目录，失败时提供可定位的错误。
- 生成后调用必要的 `AssetDatabase.Refresh()`，但避免无条件重复刷新。

## Review 重点

- 逻辑是否放在正确程序集，而不是因为“能编译”就跨层引用。
- 是否复用了 `GameEntry`、Extension、Container、UI/Entity 基类和项目 Utility。
- 事件订阅、资源加载、ReferencePool、CancellationToken 是否有对称清理。
- 表格、预制体、生成代码和运行时代码是否作为一个整体交付。
- 是否误引入 ET、服务端、Luban 或 Web 全栈模板约定。

## 不接受的验证方式

- 只运行 `dotnet build Client/Client.sln` 就认定 Unity 项目可用。
- 因为没有测试失败就声称测试通过。
- 手改生成文件消除编译错误。
- 忽略 Unity Console 中与本次改动相关的 Warning/Error。
- 在未检查资源和表格差异的情况下提交整批重新生成产物。
