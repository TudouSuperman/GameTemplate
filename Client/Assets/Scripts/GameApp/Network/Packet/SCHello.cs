using System;
using ProtoBuf;

namespace GameApp
{
    [Serializable, ProtoContract(Name = @"SCHello")]
    public sealed class SCHello : SCPacketBase
    {
        public override int Id
        {
            get
            {
                return 10;
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