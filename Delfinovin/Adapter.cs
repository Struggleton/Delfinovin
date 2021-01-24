using BitStreams;
using System;

namespace Delfinovin
{
    public class Adapter
    {
        // Byte 0
        private const byte _Identifier = 0x21;
        private BitStream _Stream;

        public Controller[] Controllers;

        public Adapter()
        {
            // Initialize 4 controllers, one for each port
            Controllers = new Controller[4];
            for (int i = 0; i < 4; i++)
            {
                Controllers[i] = new Controller();
            }
        }

        public void UpdateAdapter(byte[] data)
        {
            _Stream = BitStream.Create(data); // Pass bitstream controller data
            if (_Stream.ReadByte() != _Identifier) // Magic identifier check 
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
