# 客户端网络规范

## 适用范围

本规范只描述 `Client/Assets/Scripts/GameApp/Network/` 中已经存在的客户端数据包、处理器和通道代码。仓库当前没有服务端工程或协议生成流水线；不要据此虚构服务端目录、部署流程或另一套协议工具。

## 数据包契约

- 客户端发出的数据包放在 `Network/Packet/`，继承 `CSPacketBase` 并使用 `CS*` 命名。
- 客户端接收的数据包继承 `SCPacketBase` 并使用 `SC*` 命名。
- Protobuf 数据包使用 `[Serializable, ProtoContract]`；字段序号通过 `[ProtoMember(n)]` 明确声明，已发布序号不得复用给不同语义。
- 每个数据包覆盖 `Id` 和 `Clear()`。`Id` 必须在当前协议空间唯一；`Clear()` 必须重置所有可变字段，因为数据包由 GF `ReferencePool` 复用。
- 数据包只承载序列化数据，不访问 Unity 对象，也不放玩法逻辑。

现有最小参考：

- `Client/Assets/Scripts/GameApp/Network/Packet/CSHeartBeat.cs`
- `Client/Assets/Scripts/GameApp/Network/Packet/SCHeartBeat.cs`
- `Client/Assets/Scripts/GameApp/Network/PacketType.cs`

## 接收处理器契约

接收处理器放在 `Network/PacketHandler/`，直接继承 `PacketHandlerBase`：

```csharp
public sealed class SCExampleHandler : PacketHandlerBase
{
    public override int Id => ExamplePacketId;

    public override void Handle(object sender, Packet packet)
    {
        SCExample response = (SCExample)packet;
        // 在调用期间读取 response，不保存池对象引用。
    }
}
```

- 处理器 `Id` 必须与对应 `SC*` 数据包的 `Id` 一致。
- `NetworkChannelHelper.Initialize` 通过反射发现直接继承 `SCPacketBase` 和 `PacketHandlerBase` 的具体类型；不要另建平行注册体系，也不要增加会绕过当前直接基类检查的中间基类。
- `Handle` 返回后不得长期持有数据包引用；反序列化对象属于引用池生命周期。

现有参考：`Client/Assets/Scripts/GameApp/Network/PacketHandler/SCHeartBeatHandler.cs`。

## 通道和协议边界

- 复用 `NetworkChannelHelper`、`NetworkServiceHelper` 和已有 `GameEntry` 网络组件，不在业务模块直接创建第二套 Socket/通道生命周期。
- 包头长度、字节序、序列化方式或 ID 布局属于线协议契约。修改其中任何一项时，必须同步检查 `PacketHeaderBase`、`CSPacketHeader`、`SCPacketHeader`、`NetworkChannelHelper` 和实际对端协议；不能只改一个数据包就假定兼容。
- `NetworkServiceHelper.SendAsync` 当前是否可用以源码实现为准；若仍抛出 `NotImplementedException`，不得把异步请求/响应写成已完成能力。
- 连接地址属于运行配置，不要把临时本机测试地址写进项目规范或业务逻辑。

## 修改后检查

1. 搜索现有 `CS*`、`SC*` 和处理器，确认新增 `Id` 唯一且请求/响应方向正确。
2. 检查所有 `[ProtoMember]` 序号稳定、不重复，`Clear()` 覆盖每个可变字段。
3. Unity 编译无错误，通道初始化时没有重复数据包或处理器警告。
4. 若修改线格式，使用明确存在的对端或固定字节样本验证序列化/反序列化；没有对端时必须如实说明未完成联调。
5. 断开、超时、取消和框架关闭后，不应继续回写已销毁的业务对象。
