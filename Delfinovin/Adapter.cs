using BitStreams;
using System;

namespace Delfinovin
{
    public class Adapter
    {
        // Byte 0
        private const byte _Identifier = 0x21;
        private BitStream _Stream;

        public ControllerInstance[] Controllers;

        public Adapter()
        {
            Controllers = new ControllerInstance[4];
            for (int i = 0; i < 4; i++)
            {
                Controllers[i] = new ControllerInstance();
            }
        }

        public void UpdateAdapter(byte[] data)
        {
            _Stream = BitStream.Create(data);
            if (_Stream.ReadByte() != _Identifier)
            {
                throw new Exception(Strings.EXCEPTION_IDENTIFIER);
            }

            for (int i = 0; i < 4; i++)
            {
                Controllers[i].ReadControllerData(_Stream);
                Controllers[i].UpdateCenter();
            }
        }
    }
}
