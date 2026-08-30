# HybridCLR 热更新规范

## 两层职责

项目把热更新拆成 Loader 与 Code：

```text
GameApp/Hotfix/
├── Loader/Runtime/          # 随主包发布，负责加载 DLL 和创建入口
└── Code/Runtime/            # GameApp.Hotfix.Runtime，可由 HybridCLR 更新
```

- `Loader/Runtime/Init.cs` 根据 `Define.EnableHotfix` 和 CodeRunner 模式加载 DLL/PDB，再实例化 `HotfixEntry.prefab`。
- `Code/Runtime/Base/HotfixEntry.cs` 初始化热更组件、启动 `ProcedureLaunch`，并在 Unity 生命周期中驱动 Update/Shutdown。
- `HotfixComponentEntry` 统一管理热更组件；新增长生命周期热更系统时优先实现现有 `HotfixComponent` 模式。

## 放置规则

适合放在 `Client/Assets/Scripts/GameApp/Hotfix/Code/Runtime`：

- 游戏流程、玩法、UI 业务逻辑、Entity 业务逻辑。
- Model 和可热更新的数值/配置消费逻辑。
- 只依赖稳定常驻契约的业务代码。

必须放在常驻层或 Loader：

- 热更 DLL/PDB 的加载与入口实例化。
- 必须在热更程序集加载前执行的代码。
- 需要提供给多个程序集的稳定基类、GF 扩展和资源契约。
- 使用 Unity Editor API 的生成或构建工具应放在 `Client/Assets/Scripts/GameApp/Editor`，不进入任何 Runtime 程序集。

不要为了调用热更实现而让 `GameApp.Runtime` 或 Loader 反向引用 `GameApp.Hotfix.Runtime`。

## 程序集约束

`GameApp.Hotfix.Runtime.asmdef` 当前引用：

- `GameApp.Runtime`
- `GameApp.Hotfix.Loader.Runtime`
- GF/UGF、UniTask、CodeBind、protobuf-net、ZString 和 UI 相关包

修改 `.asmdef` 前确认依赖确实属于该层。新增包引用会影响热更编译、AOT 补充元数据和构建产物，必须完成 HybridCLR 构建验证。

## 热更入口生命周期

- `HotfixEntry.Start`：初始化组件并启动首个流程。
- `HotfixEntry.Update`：传入 `Time.deltaTime` 与 `Time.unscaledDeltaTime`。
- `HotfixEntry.OnDestroy`：关闭所有热更组件。
- Loader 销毁时必须销毁入口实例并通过 `GameEntry.Resource.UnloadAsset` 卸载入口资源。

新增全局热更组件必须具备明确优先级，并在 `OnShutdown` 释放 FSM、事件、资源和静态引用。

## 宏和运行模式

`Client/Assets/Scripts/GameApp/Hotfix/Loader/Runtime/Define.cs` 定义当前判断方式：

- `UNITY_EDITOR`：编辑器模式。
- `UNITY_HOTFIX`：是否启用热更 DLL 加载。
- `ENABLE_IL2CPP`：IL2CPP 模式。

不要在业务代码散布不同的宏判断方式；需要统一行为时扩展 `Define` 或相邻的构建配置。

## Editor 工具入口

项目已有以下入口：

- `HybridCLR/Do All`：完整 HybridCLR 准备流程。
- `HybridCLR/CopyAotDlls`：复制 AOT DLL。
- `GameApp/Copy Compile Dll`：复制编译 DLL。
- `GameApp/Build Tool Editor`：项目构建窗口。

实现位于：

- `Client/Assets/Scripts/GameApp/Editor/HybridCLR/`
- `Client/Assets/Scripts/GameApp/Editor/Build/`

不要手工复制某一个 DLL 后就认定构建链路完整；程序集、AOT 元数据、热更资源和 BuildSettings 应通过项目工具保持一致。

## 修改后验证

1. Unity 完成主程序集、Loader 和 Hotfix 程序集编译且无错误。
2. 若改动热更程序集依赖、AOT 泛型或构建逻辑，执行 `HybridCLR/Do All`。
3. 在编辑器运行，确认 Loader 能创建 `HotfixEntry` 并进入 `ProcedureLaunch`。
4. 退出 Play Mode，确认入口销毁且没有重复订阅、未释放资源或重复加载程序集错误。
