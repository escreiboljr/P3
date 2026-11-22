using System;

namespace DatosDecod48
{
    public class BDS50
    {
        public double Roll { get; private set; } = double.NaN;
        public double Track { get; private set; } = double.NaN;
        public double GroundSpeed { get; private set; } = double.NaN;
        public double TrackRate { get; private set; } = double.NaN;
        public double TrueAirspeed { get; private set; } = double.NaN;

        // Leer una cantidad determinada de bits
        private int LeerBits(byte[] mb, int inicio, int cantidad)
        {
            int valor = 0;

            for (int i = 0; i < cantidad; i++)
            {
                int posicion = inicio - 1 + i;

                int indiceByte = posicion / 8;
                int indiceBit = 7 - (posicion % 8);

                int bit = (mb[indiceByte] >> indiceBit) & 1;

                valor = (valor << 1) | bit;
            }

            return valor;
        }

        public void Decode(byte[] mb)
        {
            if (mb == null || mb.Length < 7)
                throw new ArgumentException("BDS50 requiere 7 bytes MB.");

            // ROLL ANGLE
            bool rollValido = LeerBits(mb, 1, 1) == 1;

            if (rollValido)
            {
                int rawRoll = LeerBits(mb, 2, 10);

                rawRoll = BDS.TwosComplement(rawRoll, 10);

                Roll = rawRoll * (45.0 / 256.0);
            }

            // TRUE TRACK ANGLE
            bool trackValido = LeerBits(mb, 12, 1) == 1;

            if (trackValido)
            {
                int rawTrack = LeerBits(mb, 13, 11);

                rawTrack = BDS.TwosComplement(rawTrack, 11);

                Track = rawTrack * (90.0 / 512.0);
            }

            // GROUND SPEED
            bool gsValido = LeerBits(mb, 24, 1) == 1;

            if (gsValido)
            {
                int rawGS = LeerBits(mb, 25, 10);

                GroundSpeed = rawGS * 2.0;
            }

            // TRACK ANGLE RATE
            bool tarValido = LeerBits(mb, 35, 1) == 1;

            if (tarValido)
            {
                int rawTAR = LeerBits(mb, 36, 10);

                rawTAR = BDS.TwosComplement(rawTAR, 10);

                TrackRate = rawTAR * (8.0 / 256.0);
            }

            // TRUE AIRSPEED
            bool tasValido = LeerBits(mb, 46, 1) == 1;

            if (tasValido)
            {
                int rawTAS = LeerBits(mb, 47, 10);

                TrueAirspeed = rawTAS * 2.0;
            }
        }
    }
}