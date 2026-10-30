<!-- TRELLIS:START -->
# Trellis Instructions

These instructions are for AI assistants working in this project.

This project is managed by Trellis. The working knowledge you need lives under `.trellis/`:

- `.trellis/workflow.md` — development phases, when to create tasks, skill routing
- `.trellis/spec/` — package- and layer-scoped coding guidelines (read before writing code in a given layer)
- `.trellis/workspace/` — per-developer journals and session traces
- `.trellis/tasks/` — active and archived tasks (PRDs, research, jsonl context)

If a Trellis command is available on your platform (e.g. `/trellis:finish-work`, `/trellis:continue`), prefer it over manual steps. Not every platform exposes every command.

If you're using Codex or another agent-capable tool, additional project-scoped helpers may live in:
- `.agents/skills/` — reusable Trellis skills
- `.codex/agents/` — optional custom subagents

Managed by Trellis. Edits outside this block are preserved; edits inside may be overwritten by a future `trellis update`.

<!-- TRELLIS:END -->

## Project Tool Routing

主会话和子代理在已配置且可用的 MCP 能覆盖任务时，优先使用对应 MCP。C# 语义分析优先 Rider MCP 或其他代码语义 MCP；Unity 编辑器状态与操作按 **Unity MCP / 其他 Unity 编辑器 MCP → 可用的 Unity Agent Bridge → 必要的其他工具** 选择。Unity MCP 未配置、不可用或缺少当前任务所需能力时，必须检查本工程的 AgentBridge；Bridge 可用且支持所需操作时直接使用，无需为此安装 MCP 或重复询问工具选择。详细降级、可用性判断和避免重复写入的规则以 `.trellis/workflow.md` 为准。

## Unity Agent Bridge 接入位置

Unity 工程位于 `Client/`，通过 UPM 安装 `me.xw.unityagentbridge`。首次安装后，在 Unity 的 `Window > Agent Bridge` 点击 `Enable Bridge`，由插件创建 `Client/.agentbridge/` 并启动宿主。需要后台驱动时，在该窗口启用失焦不节流。

已安装包的 `AGENT.md` 通过 `Client/Packages/` 或 `Client/Library/PackageCache/` 定位。通讯文件、截图和测试结果位于 `Client/.agentbridge/`，不提交 Git。

下方受管理的官方协议片段适用于选择 AgentBridge 执行操作的场景；选择 Unity MCP 时遵循对应 MCP 的协议。通过文件系统或命令行完成 Bridge 通讯属于真实的 Unity 编辑器调用，不应因没有 MCP 而跳过已有 Bridge、直接手改 Scene/Prefab YAML，或声称无法自动验证。项目扩展和验证入口见 `.trellis/spec/client/agent-bridge-guidelines.md`。

<!-- BEGIN UNITY_AGENT_BRIDGE -->
## Unity Agent Bridge（驱动 Unity Editor）

本工程通过 Unity Agent Bridge 驱动编辑器。执行任何 Unity 查询或修改前，必须先阅读已安装包的 `AGENT.md`，并遵守其中的 fixed-slot single-flight exchange、原子请求写入、先等 `processing.json` 消失再删除响应的 ack、唯一 id、session 首次 `list_commands` 和 `commandsVersion` 刷新规则。

先寻找 `Assets/` 与已存在 `.agentbridge/` 同级的 Unity 工程；Bridge root 直接使用固定的 `request.json`、`processing.json`、`response.json` 槽位，空闲时这些文件可以不存在。目录存在只表示桥接配置过，不保证宿主当前已启用。找不到 Bridge root 时停止并报告 Unity 没有安装或运行 AgentBridge，不得自行创建目录。实际 command、params schema 和 batch policy 只从运行时 `list_commands` 获取，不得使用硬编码清单。
<!-- END UNITY_AGENT_BRIDGE -->
