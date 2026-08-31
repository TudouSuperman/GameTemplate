//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System;
using ProtoBuf;

namespace GameApp
{
    [Serializable, ProtoContract(Name = "@SCPacketHeader")]
    public sealed class SCPacketHeader : PacketHeaderBase
    {
        public override PacketType PacketType
        {
            get
            {
                return PacketType.ServerToClient;
            }
        }

        [ProtoMember(1)]
        public override int Id
        {
            get; 
            set;
        }

        [ProtoMember(2)]
        public override int PacketLength
        {
            get; 
            set;
        }
    }
}