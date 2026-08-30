# GameTemplate 代码复用指南

新增类型前先查找项目已有扩展点。GF 项目很容易为同一资源、UI 或事件回调重复写一套包装，最终造成生命周期处理不一致。

## 查找顺序

1. `Client/Assets/Scripts/GameApp/*Extension.cs` 与 `*.Awaitable.cs`
2. `Client/Assets/Scripts/GameApp/Container/`
3. UI/Entity/Procedure/HotfixComponent 基类
4. `GameEntry` 已注册组件
5. `Client/Assets/Scripts/GameApp/Utility/` 和 `Definition/`
6. `Core/UnityGameFramework.Extensions/` 的通用能力

使用 `rg` 搜索领域词和基类，例如：

```powershell
rg -n "OpenUIForm|ShowEntity|LoadAsset|Subscribe" Client/Assets/Scripts/GameApp -g "*.cs"
rg -n "class .*Extension|class .*Container" Client/Assets/Scripts/GameApp -g "*.cs"
```

## 优先复用的项目能力

### GameEntry

`GameEntry.Custom.cs` 已集中提供 Camera、Platform、Screen、WebSocket、Timer、CodeRunner 等组件。不要在业务代码再次用场景查找或创建平行单例。

### Extension

- UI：`UIExtension` 和 `UIExtension.Awaitable`
- Entity：`EntityExtension` 和 `EntityExtension.Awaitable`
- Scene：`SceneExtension.Awaitable`
- Event：`EventExtension.TryUnsubscribe`
- Sound、Localization、SpriteCollection、TextureSet 等领域也已有扩展

新增重载时保留已有的表解析、优先级、CancellationToken 和错误语义。

### Container

`ResourceContainer`、`EventContainer`、`EntityContainer` 把资源/事件/实体与 Owner 生命周期绑定。一个对象需要管理多项同类资源时，复用 Container，不要散落 List、token 和清理回调。

### UI/Entity 基类

`UGuiFormLogic` 已处理 Canvas、字体、本地化和深度；`UGFEntityLogic` 已处理 View/Data、位置旋转和 ReferencePool 回收。业务子类只补充领域逻辑，不复制基类生命周期。

### Model 与 Procedure

热更全局数据遵循 `IModel` + `ModelComponent`；流程切换遵循 `ProcedureBase` + `HotfixProcedureComponent`。不要用静态字段或新的状态机绕开既有生命周期，除非现有抽象确实无法表达需求并有明确设计记录。

## 何时新增能力

新增前满足至少一项：

- 两个以上模块需要相同操作，并且生命周期/错误语义一致。
- 现有 Extension 缺少一个自然重载。
- 需要把重复的 acquire/release、subscribe/unsubscribe 或 load/unload 配对封装起来。
- 新能力属于项目级 GF 适配，而不是单个业务界面的细节。

放置位置：

- 单领域调用便利方法：对应 `*Extension`。
- 随 Owner 管理多项状态：`Container`。
- 跨热更层的稳定能力：`GameApp.Runtime`。
- 只在热更业务使用：`GameApp.Hotfix.Runtime`。
- 仅生成/检查资产：`GameApp.Editor`。

## 不要复用错层

- 不把业务代码塞进 `Core/` 或 `Library/`，即使那里有相似工具。
- 不直接复制参考项目的 ET、Luban 或服务端抽象。
- 不继承生成类型来规避修改源配置。
- 不用反射或字符串路径绕过已有强类型 ID/扩展，除非现有框架入口明确要求。

## Review 问题

- 是否已经存在同领域 Extension/Awaitable？
- 是否应该由 Container 管理清理？
- 是否重复了 UI/Entity 基类已做的工作？
- 是否能通过已有 GameEntry 组件完成？
- 新抽象是否有两个以上真实调用方，还是只把单个调用包装了一层？
- 复用后是否仍保持程序集依赖方向和热更新边界？
