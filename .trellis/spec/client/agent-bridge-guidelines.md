# Unity Agent Bridge 项目扩展

## 1. 范围与入口

`Client/` 通过 UPM 引用 `me.xw.unityagentbridge`。项目命令位于
`Assets/Scripts/GameApp/Editor/AgentBridge/`，属于 `GameApp.Editor`，分组为 `GameApp`。
该程序集显式引用 `AgentBridge.Editor`、`CodeBind.Editor` 和 `StateController`。
所有命令只在 Editor 中运行，不增加 Runtime/热更程序集依赖。

### 工具选择与可用性

主会话和子代理遵循 `.trellis/workflow.md` 的路由：Unity MCP / 其他 Unity 编辑器 MCP 优先，
缺失、不可用或缺少目标能力时检查并使用 AgentBridge。已授权范围内不重复询问工具选择。
先阅读已安装包 `AGENT.md`，确认既有 `Client/.agentbridge/` 并成功完成实时 `list_commands`，
再根据启用命令和 schema 执行；不能仅凭包已安装或 root 存在认定可用。
无 root 时不自行创建；未启用、无响应或目标命令缺失时如实说明，已有 exchange 按协议继续处理。
同一 root 由一个调用方串行协调，MCP 与 Bridge 之间切换也不能重复提交结果不明的写操作。
官方协议片段约束 Bridge 通讯；使用 MCP 不要求额外走一次 Bridge。

来源是 GameDevelopmentKit 的项目级实现：CodeBind、StateController 经过本项目 API 适配；
Excel 保留五种操作，改用项目已有的 EPPlus。当前没有 Luban、UXTool、NPOI，
不得通过复制无效处理器或引入整个工具链来伪装接入。

## 2. 接口

处理器实现 `ICommandHandler`，入口为 `Task<object> ExecuteAsync(JObject params)`。
`Task<object>` 是外部协议边界，不改变项目业务使用 UniTask 的约定。

能力概览：

| 命令 | 操作 |
| --- | --- |
| `codebind` | `all`、`generate_code`、`set_serialization`、`rename_node` |
| `statecontroller` | `list`、`list_nodes`、`set_state`、`add_data`、`add_state`、`add_state_node` |
| `excel` | `inspect`、`read_range`、`find_rows`、`set_cells`、`upsert_rows` |

此表用于理解所有权，不是调用协议。执行前从实时 `list_commands` 获取 schema 和策略，
这些项目命令均不允许 Batch；`CanDisable=true`，不可绕过用户禁用状态。

## 3. 契约

- Bridge root 为 `Client/.agentbridge/`，由 Unity 窗口启用桥接时创建。
  调用前读取已安装包 `AGENT.md`；遵守原子发布、single-flight、唯一 id、版本刷新和 ack 顺序。
- `codebind` 使用现有内部 Binder 的 `GenerateBindingSource` / `UpdateSerializedBindings`。
  不使用参考项目的旧方法名。内部类型通过该库既有反射入口实例化，不修改第三方库。
  `all` 返回 `serializationDeferred=true` 时先编译，再单独执行序列化绑定。
  现有 `UpdateSerializedBindings` 内部调用 `AssetDatabase.SaveAssets()`，会同时保存已标记 dirty 的资产；
  验证时应留意插件设置的自动序列化差异，不覆盖其他编辑器操作产生的改动。
- 节点和 Prefab 参数沿用处理器的 schema；Prefab 修改自动保存，Scene 修改记录 Undo 并标记 dirty。
  `rename_node` 必须记录 GameObject 的 Undo，Transform 名称实际代理的是 GameObject 名称。
- StateController 的 `dataName` 是状态组名。布尔绑定使用 `m_PrimaryGroupName` /
  `m_SecondaryGroupName` 及对应 ValueMappings。Edit Mode 使用 StateGroup 的编辑器刷新和选择入口，
  使控制器缓存建立后新加的绑定也生效。当前选中状态名不是序列化字段；保存的是应用后的对象属性。
- Excel 只访问仓库内已有 `.xlsx`，拒绝仓库外路径、符号链接与目录联接，单文件上限 32 MiB。
  不支持旧 `.xls`。工作簿使用内存副本处理；写入前再次核对 SHA-256，使用同目录临时文件和
  `File.Replace` 替换。失败不保存内存中的部分变更；Excel 写入不能通过 Unity Undo 撤销。
- `ClientExcel/` 默认读取 `DataTableConfig.NameRow + 1` / `ContentStartRow + 1`。
  显式传入的行号采用 Excel 的 1 基行号；其他工作簿识别 `##var` 或默认第 1 行表头。
- `dryRun=true` 只计算变化；`expectedVersion` 校验文件版本；`expectedValue` /
  `expectedFormula` 校验旧单元格，两者不可同时传入。`upsert_rows` 拒绝重复键，
  追加行可复制样式、相对公式并清除模板常量，不承诺扩展模板的数据验证范围。

## 4. 校验与错误

| 情况 | 行为 |
| --- | --- |
| 非法路径、格式、A1 地址、字段、重复键、超范围查询 | `INVALID_PARAMS`，不保存 |
| SHA-256 或旧单元格不匹配 | `EXCEL_CONFLICT`，重新读取后再决定写入 |
| 工作簿锁定、损坏或读写异常 | `EXCEL_ERROR`，保留可定位信息 |
| CodeBind 目标不存在或没有绑定组件 | `CODEBIND_ERROR` |
| 状态组/状态/控制器不存在或不唯一 | `STATECONTROLLER_ERROR` 或参数错误 |

读取范围最多 2000 格，查询每次最多返回 200 行，扫描最多 100000 行。
Excel 结果序列化后超过预算必须在保存前拒绝，避免操作已经成功却返回过大响应。

## 5. 正常、边界与失败场景

- 正常：先查询 `ClientExcel/Game/UIForm.xlsx` 获得实际 sheet 和版本，再预演、写入。
- 边界：空工作簿可以 inspect；空表头不允许按列名 upsert；相同值写入不重写文件。
- 失败：请求后发现文件版本变化、后续单元格冲突、重复输入键，整次磁盘写入均不发生。
- Prefab 选中状态不会跨重新加载保留；验证实际激活、颜色等应用后的属性。

## 6. 必需验证

通过 `list_agent_methods` 发现 `AgentBridgeCommandChecks` 的实际方法 ID 后调用：

- `ExcelRoundTrip`：隔离临时工作簿，验证查询、预演不改字节、旧值/版本冲突不部分保存、
  upsert 更新与追加、样式和相对公式、无变化不保存、地址与路径拒绝。
- `CodeBindRoundTrip`：临时场景复用真实 `MainMenuFormView`，验证已有字段引用、节点重命名和 Undo。
- `StateControllerRoundTrip`：临时场景与临时 Prefab，验证状态组、状态、布尔绑定、应用与持久化，
  并验证在控制器缓存初始化后新加 Scene 绑定仍然生效。

检查正常结束表示通过；异常必须包含断言步骤。所有临时状态在 `finally` 中恢复并清理。
文件增删后先触发资源刷新，再检查 Unity 编译，不能对旧程序集调用测试并当成新代码证据。
窗口中的 AgentCallable 按钮会从 `OnGUI` 同步启动方法；三个 RoundTrip 入口必须先
`await WaitForEditorUpdate()`，通过 `EditorApplication.delayCall` 等当前布局事件结束，
再执行场景切换、Undo 或资产操作。否则 Bridge 调用可能通过，手动点击却产生
`EndLayoutGroup: BeginLayoutGroup must be called first`。这些检查仅允许在编译完成后的 Edit Mode 执行。
修改相关检查时，除了 Bridge 调用，还要从 AgentCallable 页签手动点击验证成功状态及 Console 无新增布局错误。

## 7. 错误与正确方式

- 错误：照抄参考项目 Binder 方法名或布尔字段名；正确：核对当前已安装库，再编译和验证实际操作。
- 错误：`Undo.RecordObject(transform)` 后改节点名字；正确：记录 `transform.gameObject`。
- 错误：把测试 MonoBehaviour 放在 Editor 目录后直接挂载；正确：复用临时对象上的现有 Runtime 组件。
- 错误：在 AgentCallable 的首个同步执行段里切换场景或 Undo；正确：先等待下一次 Editor 调度，并覆盖窗口按钮调用路径。
- 错误：直接修改真实配置表做冒烟测试；正确：使用唯一命名的临时 `.xlsx`，并验证文件字节是否变化。
- 错误：通过手改 `.Bind.cs` 修复绑定；正确：修改源节点或标记，再调用现有 CodeBind 生成入口。
