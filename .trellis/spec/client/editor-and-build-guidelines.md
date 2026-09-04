# Editor 与构建工具规范

本规范适用于 `Client/Assets/Scripts/GameApp/Editor/`。Editor 工具可以修改资源、生成代码和构建产物，因此必须明确入口、输出目录、Build Target 和失败边界。

## 资源引用与编辑器验证

- 移动或重命名 Unity 资源时，必须同时保留对应的 `.meta` 文件和 GUID；资源移动优先通过 Unity 编辑器或 Unity 资源 API 完成。
- 修改 Scene、Prefab 或其他 Unity 序列化资源后，必须在 Unity 编辑器中刷新或重新导入，检查序列化引用、Prefab 覆盖项和 Console。
- 文本 YAML 或 Git diff 只能作为辅助，不能替代 Unity 编辑器中的实际验证。

## 当前入口

| 入口 | 作用 | 实现 |
| --- | --- | --- |
| Toolbar `Launcher` | 刷新 BuildSettings 场景并从 `Assets/Launcher.unity` 进入 Play Mode | `Editor/ToolBar/LauncherSceneToolBar.cs` |
| Toolbar `UI-Res` | 聚焦 `Assets/Res/Artwork/UI` | `Editor/ToolBar/FocusFolderToolBar.cs` |
| `GameApp/DataTable/Generate/*` | 从 `ClientExcel` 生成表代码、数据、枚举和本地化 | `Editor/DataTableGenerator/` |
| `HybridCLR/Do All` | 生成 HybridCLR 数据并复制 AOT DLL | `Editor/HybridCLR/HybridCLREditor.cs` |
| `GameApp/Copy Compile Dll` | 复制已有热更 DLL/PDB 到资源目录 | `Editor/Build/BuildAssemblyTool.cs` |
| `GameApp/Build Tool Editor` | 构建资源、Player 或刷新现有 Windows64 包资源 | `Editor/Build/BuildToolEditor.cs` |

## Toolbar 约定

- Toolbar 方法必须是静态、无参数并位于 Editor 程序集，使用 `[Toolbar(OnGUISide, priority)]` 注册。
- GUIContent 使用静态缓存，避免 OnGUI 每帧分配。
- 播放、编译或构建期间不可执行的操作要显式禁用并防止重复点击。
- `Launcher` 必须先调用 `BuildSceneSetting.AllScenes()`；场景入口以 `BuildSceneSetting.EntryScenePath` 为准，不在多个 Toolbar 中维护不同路径。
- 新按钮只负责轻量入口，耗时生成和构建逻辑放入独立工具类。

## 资源与 Player 构建

`BuildHelper.BuildPkg(platform)` 的当前流程：

1. 清理 `../ClientBuild/Build_Package/<Platform>`。
2. `RefreshResourceCollectionWithOptimize()` 后构建资源。
3. 强制刷新 AssetDatabase。
4. 只用 `Assets/Launcher.unity` 构建 Player。
5. 构建失败时抛出 `GameFrameworkException`，成功后打开输出目录。

`Build Resource With Refresh Optimize Collection` 只构建资源；`Build And Refresh Windows64 Package Resource` 只替换已存在 Windows64 包的 `StreamingAssets`，不能代替完整 Player 构建。

切换平台可能触发长时间资源重导入。必须先确认 Active Build Target，再生成 DLL、AOT、AssetBundle 和 Player，不能混用不同平台产物。

## 热更构建边界

- `HybridCLR/Do All` 当前不复制 `GameApp.Hotfix.Runtime` 业务 DLL。
- `GameApp/Copy Compile Dll` 当前不调用 `CompilePlayerScripts`；它从 `HybridCLRData/HotfixDlls/<BuildTarget>` 复制已有 DLL/PDB。
- 热更 DLL 输出为 `Assets/Res/Hotfix/Code/*.dll.bytes`，外部 HybridCLR 目录为 `Temp/HybridCLRBin`。
- AOT DLL 输出为 `Assets/Res/HybridCLR/*.dll.bytes`，并写入 `HybridCLRConfig.asset`。

修改构建工具时不得把这三个阶段合并成未经验证的隐式行为。若恢复或更换编译步骤，要验证 Development/Release 选项、宏、目标平台、PDB 和资源收集规则。

## 可更新资源版本

`UGFBuildEvent.GenerateLocalUpdatableVersion` 会把版本信息写到：

```text
../ClientBuild/Build_AssetBundle/<Platform>Version.txt
```

其中 UpdatePrefixUri 属于开发环境配置。不要新增个人机器路径或地址；修改时应明确目标环境，并验证 URI、版本文件和客户端读取逻辑一致。

## 文件写入规则

- 生成/构建输出只能落在工具声明的 `Assets/Res/Generate`、`Assets/Res/Hotfix`、`Assets/Res/HybridCLR`、`HybridCLRData`、`Temp` 和 `ClientBuild` 范围。
- 不修改 Unity 生成的 `.sln` / `.csproj` 来解决程序集问题。
- 构建前必须检查输入目录、输出目录和 Active Build Target；前置条件失败时应立即停止，不得静默继续并生成看似成功的产物。
- 清理目录前必须解析并确认目标属于本次构建输出，不得删除源码资源或用户未授权的目录。
- 生成后只在确有新资产时刷新 AssetDatabase，避免循环导入。

## 验证清单

- [ ] 菜单/Toolbar 在 Unity 2022.3.15f1c1 可见，播放时禁用状态正确。
- [ ] Active Build Target 与 DLL、AOT、AssetBundle、Player 平台一致。
- [ ] `Launcher` 和 Build Player 都使用 `Assets/Launcher.unity` 入口。
- [ ] 生成差异只包含预期资源与代码，没有把 `Library/`、`Temp/` 或 `ClientBuild/` 提交进 Git。
- [ ] 构建失败保留最早错误并停止后续复制，不输出看似成功的不完整包。
