# Entity 开发规范

本项目 Entity 使用 GF EntityComponent、`UGFEntityData`、`UGFEntityLogic`、`UGFEntityView` 和 CodeBind，不使用 ETEntity。

## 一条完整 Entity 链路

```text
Entity Prefab
  + ClientExcel/Game/Asset.xlsx
  + ClientExcel/Game/Entity.xlsx
  + ClientExcel/Game/EntityGroup.xlsx
  + EntityData / EntityLogic / EntityView
  + CodeBind 生成的 *.Bind.cs
  → EEntityID
  → GameEntry.Entity.ShowEntity(...)
```

参考实现：

- 预制体：`Client/Assets/Res/Artwork/Entity/HostEntity.prefab`
- 数据：`Client/Assets/Scripts/GameApp/Hotfix/Code/Runtime/Entity/Host/HostEntityData.cs`
- 逻辑：`Client/Assets/Scripts/GameApp/Hotfix/Code/Runtime/Entity/Host/HostEntityLogic.cs`
- 视图：`Client/Assets/Scripts/GameApp/Hotfix/Code/Runtime/Entity/Host/HostEntityView.cs`
- 绑定：`Client/Assets/Scripts/GameApp/Hotfix/Code/Runtime/Entity/Host/HostEntityView.Bind.cs`

## 创建或修改 Entity

1. 在 `Client/Assets/Res/Artwork/Entity/` 创建预制体。
2. 在 `ClientExcel/Game/Asset.xlsx` 维护资源，在 `Entity.xlsx` 和 `EntityGroup.xlsx` 维护实体类型和分组。
3. 在 `Client/Assets/Scripts/GameApp/Hotfix/Code/Runtime/Entity/<Feature>/` 创建 Data、Logic 和 View。
4. 使用 CodeBind 生成 View 的 `*.Bind.cs`，不要手填生成字段。
5. 执行 `GameApp/DataTable/Generate/Gen All By Bin`，刷新 `DREntity`、`EEntityID` 和运行时数据。
6. 添加领域扩展时，仿照热更层 `Entity/EntityExtension.cs`，把具体 Logic 类型映射集中在扩展方法中。

## Data 规则

- 继承 `UGFEntityData`。
- 使用 `ReferencePool.Acquire<T>()` 的静态 `Create` 工厂，不直接 `new` 引用池数据。
- 初始化 `m_SerialId`、`m_TypeId` 以及该实体所需字段。
- 新增引用或集合字段时覆盖 `Clear()`，调用 `base.Clear()` 并清空新增状态。
- Data 只承载显示/初始化所需的数据，不操作 GameObject、UI 或全局组件。

## Logic 与 View 分工

`Logic`：

- 继承 `UGFEntityLogic` 或需要容器能力时继承 `UGFEntityLogicEx`。
- 在 `OnShow` 先调用 `base.OnShow(userData)`，再通过 `GetData<T>()` 读取强类型数据。
- 加载的资源、订阅和子实体要跟随 Entity 生命周期释放。
- 在 `OnHide`/回收阶段恢复父节点、事件和运行时状态。

`View`：

- 继承 `UGFEntityView`，使用 `sealed partial` 与 `[MonoCodeBind('-')]`。
- 暴露必要的 Unity 组件访问，不处理实体业务规则。
- `*.Bind.cs` 只能由 CodeBind 生成。

## 显示和序列号

- 根据配置显示时使用 `GameEntry.Entity.ShowEntity<T>(entityData)` 或项目的强类型扩展。
- `EntityExtension.TryGetTableData` 会通过 `DREntity`、`DREntityGroup`、`DRAsset` 解析资源和分组，不绕过该链路硬编码。
- 当前纯客户端临时实体使用 `GenerateSerialId()` 产生递减的负数。
- 代码中保留了“正值可用于远端实体”的兼容约定，但当前没有服务端或协议实现；不要基于这个注释扩展服务端规范。未来接入真实协议时再定义远端 ID 所有权。

## 生命周期和资源

`UGFEntityLogic` 在回收时会把当前 `UGFEntityData` 归还 `ReferencePool`。因此：

- 不在外部长期保留 EntityData 引用。
- 不重复 Release 同一数据。
- 异步显示或资源加载使用 Entity/Container 提供的 CancellationToken 机制。
- Entity 隐藏或 Owner 销毁后，未完成异步结果不能重新写回已回收对象。

## 常见错误

- 用 `new HostEntityData()` 绕过 ReferencePool。
- 只创建 Logic，没有预制体、表配置、Data 或生成枚举。
- 在 View 中访问 Procedure/Model，或把业务状态存在生成绑定字段中。
- 手改 `HostEntityView.Bind.cs`、`DREntity.cs` 或 `EEntityID.cs`。
- 为当前占位 `Server/` 编写 ETEntity 或服务端同步规则。
