# ReactiveBinding 接入与使用

## 1. 范围与入口

`Client/Packages/manifest.json` 通过官方 UPM Git 子目录安装 `me.xw.reactivebinding`，
版本 `2.0.2`，固定 commit `d2de4560cf6938d511e0d31962f436053c594561`。
程序集名为 `ReactiveBinding`；`GameApp.Runtime`、`GameApp.Hotfix.Runtime` 均显式引用。
本次只安装能力，未迁移现有 Model、UI 或事件，也未建立自动轮询管理器。

上游包声明最低 Unity 为 `6000.3`。当前固定版本已在本工程 Unity `2022.3.15f1c1`
实际完成导入、目标程序集生成器编译和 Edit/Play Mode 基础行为检查；这不是上游对
Unity 2022 的支持承诺。更新 commit 时必须重新验证，不修改 PackageCache 内的版本声明。

## 2. API

- 观察类声明为 `partial` 并实现 `IReactiveObserver`。
- `[ReactiveSource]` 标记字段、可读属性或无参数且有返回值的方法。
- `[ReactiveBind(nameof(Source))]` 标记刷新回调；简单值可接收新值或旧值/新值。
- `ObserveChanges()`、`ResetChanges()` 由生成器实现，不手写同名实现。
- `[VersionField] private int __Count;` 配合 `partial class : IVersion` 生成 `Count`
  属性；业务通过生成属性读写，通过 `Version` 读取版本，不直接访问 `__*` 协议成员。
- 集合需要内部变化检测时使用 `VersionList<T>` 等版本集合；普通 List 引用不代表内容版本。

## 3. 执行与生命周期契约

- CodeBind 继续负责组件引用；ReactiveBinding 只负责变化检测与展示刷新。
- 绑定对象由业务 Logic 持有并驱动，或由 Logic 向 View 提供展示数据；不让 View
  为此直接读取全局 Model、执行 Procedure/网络业务。
- 在既有 GF `OnUpdate` 中按需要调用观察；首次调用会初始化所有回调，旧值等于新值。
- 在 `OnOpen` 设置有效上下文后重置并首次观察；关闭时停止观察、清空上下文。
  `ResetChanges()` 只重置观察缓存，不替代业务引用清理。
- 两次观察之间的多次变化可合并，不能用作交易、伤害等逐次业务事件的替代。
- 数据源读取保持轻量且无副作用；不在每次观察时构建字符串、集合或执行大范围遍历。
- 数据回写 Slider/Toggle 时使用 `SetValueWithoutNotify` / `SetIsOnWithoutNotify`，
  保留用户输入事件通道，避免展示刷新反向触发业务写入。
- 生成器 DLL 由包的 `RoslynAnalyzer` 配置加载，只供编译，不作为 Player 插件引用。

## 4. 校验与错误

| 情况 | 处理 |
| --- | --- |
| 缺少 partial、接口、来源或回调签名错误 | 修正源码，不能手改生成结果 |
| 外部统一驱动触发 RB10009 | 确认真实驱动存在后使用库提供的 ReactiveObserveIgnore 标记 |
| 找不到生成方法、出现 CS8032/CS8785 等生成器诊断 | 检查包导入、asmdef 直接引用及 Roslyn 兼容性 |
| 改了磁盘脚本但编辑器仍加载旧程序集 | 先刷新资源，再独立编译、等待结束；不能对旧类型检查后宣称通过 |
| 热更首次使用新的版本集合泛型 | 按 hotfix-guidelines 重新生成 HybridCLR 数据、检查 AOT 元数据和真机行为 |

## 5. 正常、边界与失败场景

- 正常：状态在其他系统改变，下次观察触发对应 View 刷新。
- 边界：相同值不重复回调；重开/复用后再次初始化；一次多源变更只触发一次组合回调。
- 失败：关闭后仍观察已释放 Model，或把最终状态比较用于逐笔结算，属于生命周期/语义错误。

## 6. 更新时必需验证

- Unity 实际解析的包版本和 Git hash 与 manifest、自动生成的 lock 一致。
- 在两个目标程序集编译真实的观察类与 VersionField 类，确认生成器实际参与编译。
- 验证首次回调、无变化抑制、旧值/新值、VersionList 内容变化、多源合并、ResetChanges。
- Edit Mode 和 Play Mode 均执行检查，退出后清理临时探针并重新编译，检查 Console。
- 运行项目 HybridCLR 检查；发布目标的 IL2CPP/真机验证应在实际业务使用范围确定后执行。

## 7. 错误与正确方式

- 错误：每帧重新创建绑定对象；正确：绑定对象随界面生命周期复用，重开时重置缓存。
- 错误：直接改 `__Count` 绕过版本变化；正确：写生成的 `Count` 属性。
- 错误：仅看到包出现在 PackageCache 就宣称生成器有效；正确：编译真实目标类型并执行检查。
