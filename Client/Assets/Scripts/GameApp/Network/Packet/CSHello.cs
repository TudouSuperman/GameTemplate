using System;
using ProtoBuf;

namespace GameApp
{
    [Serializable, ProtoContract(Name = @"CSHello")]
    public sealed class CSHello : CSPacketBase
    {
        public override int Id
        {
            get
            {
                return 5;
            }
        }

        [ProtoMember(1)]
        public string Name { get; set; }

        [ProtoMember(2)]
        public string Text { get; set; }

        public override void Clear()
        {
        }
    }
}