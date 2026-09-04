# Unity 客户端开发规范

本目录适用于 `Client/` 下的 Unity 客户端开发。

## 技术基线

| 项目 | 当前值 |
| --- | --- |
| Unity | `2022.3.15f1c1` |
| 主框架 | UnityGameFramework / Game Framework |
| 热更新 | HybridCLR |
| 异步 | UniTask |
| UI 绑定 | CodeBind |
| 配置源 | `ClientExcel/` |
| 主程序集 | `GameApp.Runtime` |
| 热更程序集 | `GameApp.Hotfix.Runtime` |

Odin Inspector 是项目已有的付费依赖；不要用未经提交的本机插件替代项目依赖。

## 文件导航

| 规范 | 适用场景 |
| --- | --- |
| [架构与目录](./architecture.md) | 判断代码放置位置、命名空间和程序集依赖 |
| [C# 与生命周期](./coding-guidelines.md) | 编写业务代码、异步逻辑、事件和错误处理 |
| [运行时性能](./performance-guidelines.md) | 高频更新、对象池、缓存、生命周期和性能验证 |
| [HybridCLR 热更新](./hotfix-guidelines.md) | 修改热更入口、Loader、程序集或构建流程 |
| [UI 开发](./ui-guidelines.md) | 新建或修改 UIForm、View、CodeBind 和 UI 表 |
| [Entity 开发](./entity-guidelines.md) | 新建实体、数据、逻辑、View、预制体和配置 |
| [客户端网络](./network-guidelines.md) | 新增或修改客户端数据包、处理器和网络通道 |
| [资源与本地化](./resource-and-localization-guidelines.md) | 设置 Sprite/Texture、管理资源生命周期、修改语言表和切换语言 |
| [配置与生成代码](./data-and-generated-code.md) | 修改 Excel、表代码、枚举、Input 和 Bind 文件 |
| [Editor 与构建工具](./editor-and-build-guidelines.md) | 修改 Toolbar、场景入口、AssetBundle、Player 或热更构建工具 |
| [质量检查](./quality-guidelines.md) | 提交前编译、生成、Play Mode 和差异检查 |

## 开发前检查

- 目标代码属于常驻层还是热更新层？
- 是否已有基类、Extension、Container 或 Utility 可以复用？
- 是否涉及 Excel、预制体、CodeBind 或其他生成产物？
- 图片/纹理是否应走 SpriteCollection、TextureSet 或 Owner 级 `ResourceContainer`？
- 是否触及 Build Target、资源收集、热更 DLL 或 AOT 元数据？
- 是否需要在生命周期结束时取消异步、退订事件、释放资源或归还引用池对象？
- 代码是否运行在高频路径，是否需要阅读[运行时性能](./performance-guidelines.md)？
- 网络包的方向、协议 ID、处理器 ID 和 `Clear()` 是否一致？
- 是否跨越 `.asmdef` 边界，且依赖方向正确？

涉及多个环节时，同时阅读 `../guides/cross-layer-thinking-guide.md`。

## Quality Check

- 详细质量门见[质量检查](./quality-guidelines.md)。
- 在 Unity 2022.3.15f1c1 中完成脚本刷新，确认 Console 没有与本次改动相关的编译错误。
- 按改动范围执行最小 Play Mode、Editor 工具或资源流程冒烟验证；没有实际运行时必须说明未验证部分。
- 检查 Git 差异、生成文件和资源配置，只保留本次目标所需变更。
