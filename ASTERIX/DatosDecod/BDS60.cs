using System;

namespace DatosDecod48
{
    public class BDS60
    {
        public double Heading { get; private set; } = double.NaN;
        public double IAS { get; private set; } = double.NaN;
        public double Mach { get; private set; } = double.NaN;
        public double BaroRate { get; private set; } = double.NaN;
        public double InertialVS { get; private set; } = double.NaN;

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
                throw new ArgumentException("BDS60 requiere 7 bytes MB.");

            // MAGNETIC HEADING
            bool headingValido = LeerBits(mb, 1, 1) == 1;

            if (headingValido)
            {
                int rawHeading = LeerBits(mb, 2, 11);

                rawHeading = BDS.TwosComplement(rawHeading, 11);

                Heading = rawHeading * (90.0 / 512.0);
            }

            // INDICATED AIRSPEED
            bool iasValido = LeerBits(mb, 13, 1) == 1;

            if (iasValido)
            {
                int rawIAS = LeerBits(mb, 14, 10);

                IAS = rawIAS;
            }

            // MACH NUMBER
            bool machValido = LeerBits(mb, 24, 1) == 1;

            if (machValido)
            {
                int rawMach = LeerBits(mb, 25, 10);

                Mach = rawMach * 0.004;
            }

            // BAROMETRIC ALTITUDE RATE
            bool baroValido = LeerBits(mb, 35, 1) == 1;

            if (baroValido)
            {
                int rawBaro = LeerBits(mb, 36, 10);

                rawBaro = BDS.TwosComplement(rawBaro, 10);

                BaroRate = rawBaro * 32.0;
            }

            // INERTIAL VERTICAL VELOCITY
            bool inertialValido = LeerBits(mb, 46, 1) == 1;

            if (inertialValido)
            {
                int rawIVS = LeerBits(mb, 47, 10);

                rawIVS = BDS.TwosComplement(rawIVS, 10);

                InertialVS = rawIVS * 32.0;
            }
        }
    }
}