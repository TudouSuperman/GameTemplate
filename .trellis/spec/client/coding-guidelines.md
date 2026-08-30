# C#、异步与生命周期规范

## 代码风格

以 `Client/Assets/Scripts/GameApp/` 的现有代码为准：

- 4 空格缩进，Allman 大括号。
- 具体业务类型在无需继承时声明为 `sealed`。
- 实例字段使用 `m_`，静态字段使用 `s_`；局部变量通常使用前导下划线，如 `_view`、`_data`。
- 常驻命名空间为 `GameApp`，热更命名空间为 `GameApp.Hotfix`，Editor 命名空间为 `GameApp.Editor`。
- 扩展方法按领域放入 `UIExtension`、`EntityExtension`、`SceneExtension` 等 `static partial class`。
- 不为了统一格式而批量重排 `Core/`、`Library/` 或生成文件。

参考：

- `Client/Assets/Scripts/GameApp/UI/UIExtension.cs`
- `Client/Assets/Scripts/GameApp/Entity/EntityExtension.cs`
- `Client/Assets/Scripts/GameApp/Hotfix/Code/Runtime/Procedure/ProcedureGame.cs`

## 使用项目入口和基类

- 通过 `GameEntry` 获取 GF 与项目组件，不在业务代码重复维护全局单例。
- 新增自定义常驻组件时，在 `GameEntry.Custom.cs` 的初始化模式上扩展。
- UI 继承 `UGuiFormLogic` / `UGuiFormView`；Entity 继承 `UGFEntityLogic` / `UGFEntityView`。
- 资源、事件或实体需要随 Owner 回收时，优先复用 `ResourceContainer`、`EventContainer`、`EntityContainer`。

参考：

- `Client/Assets/Scripts/GameApp/Base/GameEntry.Custom.cs`
- `Client/Assets/Scripts/GameApp/Container/ResourceContainer.cs`
- `Client/Assets/Scripts/GameApp/Container/EventContainer.cs`

## UniTask 约定

- Unity/GF 异步统一返回 `UniTask` 或 `UniTask<T>`，不要引入另一套业务异步抽象。
- 可取消操作应接收或持有 `CancellationToken`；Owner 销毁、UI 关闭、Entity 隐藏或容器清理时必须取消。
- `.Forget()` 只用于 Unity 生命周期或事件回调这类不能返回 Task 的边界，并把实际逻辑放进明确命名的异步方法。
- 资源、UI、Entity、Scene 等操作先复用已有 Awaitable 扩展，不再把 GF 回调手写包装一遍。
- 调用者需要处理失败时，返回异常任务或抛出 `GameFrameworkException`，不要静默吞掉加载失败。

参考：

- `Client/Assets/Scripts/GameApp/Hotfix/Loader/Runtime/Init.cs`
- `Client/Assets/Scripts/GameApp/UI/UIExtension.Awaitable.cs`
- `Client/Assets/Scripts/GameApp/Entity/EntityExtension.Awaitable.cs`
- `Client/Assets/Scripts/GameApp/Container/ResourceContainer.cs`

## 事件和生命周期配对

订阅、加载、缓存和引用池获取都必须有对称清理：

- Procedure 在 `OnEnter` 订阅，在 `OnLeave` 退订。
- UI 在 `OnOpen` 获取的上下文，在 `OnClose` 清空。
- Entity 在 `OnShow` 使用数据；基类会在 `OnRecycle` 归还 `ReferencePool`。
- Container 在释放时取消 token、退订事件、隐藏实体或卸载资源。
- 热更组件在 `OnInitialize` 建立状态，在 `OnShutdown` 完整销毁。

`ProcedureGame` 和 `ProcedurePreload` 是事件订阅/退订的现有参考。不要依赖对象被 GC 来解除 GF 事件或释放 Unity 资源。

## 引用池对象

实现 `IReference` 的数据通过 `ReferencePool.Acquire<T>()` 创建，并在 `Clear()` 中恢复所有字段：

```csharp
public static HostEntityData Create(int serialId, int typeId)
{
    HostEntityData data = ReferencePool.Acquire<HostEntityData>();
    data.m_SerialId = serialId;
    data.m_TypeId = typeId;
    return data;
}
```

新增字段时同步扩展 `Clear()`；派生实现应在需要时调用 `base.Clear()`。参考 `UGFEntityData.cs` 和 `HostEntityData.cs`。

## 错误和日志

- 缺少初始化、类型不合法等编程不变量使用 `GameFrameworkException` 或明确异常，例如 `HotfixProcedureComponent.CurrentProcedure`。
- 表行、资源或运行时对象缺失这类可恢复问题，按调用契约返回 `false`/`null` 并记录 Warning/Error，或返回异常 `UniTask`。
- 使用 `UnityGameFramework.Runtime.Log`；需要按领域开关时使用 `GLog.Entity`、`GLog.UI`、`GLog.Resource` 等标签。
- 不新增无上下文的 `Debug.Log`，错误日志至少包含资源名、表 ID、状态或异常信息。
- 不用空 `catch` 隐藏失败；若降级是预期行为，日志中说明降级路径。

参考：

- `Client/Assets/Scripts/GameApp/Log/Log.cs`
- `Client/Assets/Scripts/GameApp/Log/LogWithTag.cs`
- `Client/Assets/Scripts/GameApp/UI/UIExtension.Awaitable.cs`
