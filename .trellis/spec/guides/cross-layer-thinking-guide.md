# Unity 跨层变更思考指南

Unity 功能通常不只是一份 C# 文件。预制体、Excel、生成代码、程序集和运行时对象任何一层遗漏，都会造成编译成功但运行失败。

## 先画出目标链路

常见链路：

```text
UI:
Prefab → Asset/UIForm/Group Excel → DR* + EUIFormID → UIExtension → View/Logic

Entity:
Prefab → Asset/Entity/Group Excel → DR* + EEntityID → EntityExtension → Data/Logic/View

Hotfix:
GameApp.Runtime 契约 → Hotfix.Loader → Hotfix.Runtime → HotfixEntry → Procedure

异步资源:
Owner → Container/CancellationToken → GameEntry.Resource → 完成回调 → 对称卸载

动态图片:
Image/RawImage → SpriteCollection/TextureSet → 等待项 → 目标替换或销毁时回收

本地化:
$Localization.xlsx → XML + LocalizationKey.cs → ProcedureLaunch/Preload → GameEntry.Localization

构建:
Active Build Target → 热更 DLL/AOT → ResourceCollection → AssetBundle → Launcher Player
```

在开始修改前，明确入口、配置源、生成产物、运行时消费者和清理点。

## UI/Entity 变更检查

- [ ] 预制体位于项目现有资源目录。
- [ ] `Asset.xlsx` 有正确资源记录。
- [ ] UIForm/Entity 及 Group 表关系完整。
- [ ] 执行了 `GameApp/DataTable/Generate/Gen All By Bin`。
- [ ] `EUIFormID` 或 `EEntityID` 与表 ID 一致。
- [ ] View/Logic/Data 放在正确热更目录。
- [ ] `*.Bind.cs` 由 CodeBind 生成，字段无空引用。
- [ ] 打开/显示和关闭/隐藏流程都经过项目扩展。
- [ ] 生命周期结束时事件、异步、资源和引用池对象已清理。

## 程序集边界检查

改动一个公共类型时，追踪调用方向：

- 类型只供热更业务使用：留在 `GameApp.Hotfix.Runtime`。
- 类型是 UI/Entity/GF 的稳定公共能力：考虑放入 `GameApp.Runtime`。
- 类型负责加载热更程序集：属于 Loader。
- 类型使用 `UnityEditor`：属于 `GameApp.Editor`。

禁止用新增反向引用解决架构位置错误。常驻程序集不能静态引用热更程序集。

## 配置到运行时的数据一致性

增加或改名字段时检查：

1. Excel 列定义和数据。
2. DataTableGenerator 对该类型的处理器。
3. 生成的 `DR*` 字段与解析顺序。
4. `*.bytes`/XML 是否重新生成。
5. 读取该行的 Extension 或 Procedure。
6. 热更枚举是否仍匹配 ID。

不要只修改 `DR*.cs`；下一次生成会覆盖，而且二进制布局仍旧不一致。

本地化还要检查语言枚举、资源变体、生成 Key 和语言界面的重启策略；具体规则见 `../client/resource-and-localization-guidelines.md`。

## 生命周期一致性

对每个“开始动作”找“结束动作”：

| 开始 | 对称结束 |
| --- | --- |
| `Event.Subscribe` | `Unsubscribe` / `TryUnsubscribe` |
| `ReferencePool.Acquire` | 框架回收或 `ReferencePool.Release` |
| `LoadAsset` | `UnloadAsset` |
| 创建 `CancellationTokenSource` | `Cancel` 并清除引用 |
| 创建 FSM/热更组件 | Destroy/Shutdown |
| 保存 `userData` | OnClose/OnHide 清空 |

如果完成回调可能晚于 Owner 销毁，使用版本号或 CancellationToken 阻止回写。参考 `ResourceContainer` 的 `m_Version` 和取消逻辑。

## 出错路径

成功链路和失败链路必须同时成立：

- 表行/Asset/Group 缺失时，返回值或异常任务能让调用者知道失败。
- 异步取消不是普通成功；不要在取消后继续操作已销毁的 UI/Entity。
- 失败事件使用 `UserData` 过滤归属，避免多个并发请求互相处理。
- 错误日志包含 ID、资源名、状态和错误消息。

## 完成定义

跨层改动只有在以下条件都满足后才算完成：

- 源配置、生成产物和业务代码一致。
- Unity 所有受影响程序集编译通过。
- 目标流程在 Play Mode 能进入、退出并再次进入。
- 退出后没有悬挂事件、资源、异步回调或静态状态。
- Git 差异没有无关生成文件或被误改的第三方/框架代码。
