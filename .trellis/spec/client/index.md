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
| [HybridCLR 热更新](./hotfix-guidelines.md) | 修改热更入口、Loader、程序集或构建流程 |
| [UI 开发](./ui-guidelines.md) | 新建或修改 UIForm、View、CodeBind 和 UI 表 |
| [Entity 开发](./entity-guidelines.md) | 新建实体、数据、逻辑、View、预制体和配置 |
| [配置与生成代码](./data-and-generated-code.md) | 修改 Excel、表代码、枚举、Input 和 Bind 文件 |
| [质量检查](./quality-guidelines.md) | 提交前编译、生成、Play Mode 和差异检查 |

## 开发前检查

- 目标代码属于常驻层还是热更新层？
- 是否已有基类、Extension、Container 或 Utility 可以复用？
- 是否涉及 Excel、预制体、CodeBind 或其他生成产物？
- 是否需要在生命周期结束时取消异步、退订事件、释放资源或归还引用池对象？
- 是否跨越 `.asmdef` 边界，且依赖方向正确？

涉及多个环节时，同时阅读 `../guides/cross-layer-thinking-guide.md`。
