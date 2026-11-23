using DatosDecod48;
using MultiCAT6.Utils;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using MultiCAT6.Utils;

namespace DatosDecod
{
    public class TiraDatosDecod048
    {
        public List<byte> DataSourceID { get; set; }
        public double TimeOfDay { get; set; }
        public TargetReportDescriptor TargetRep { get; set; }
        public List<double> PosSlantPolarCoord { get; set; }
        public List<double> PositionCorrectedLatLonAlt { get; set; }
        public Mode3 mode3 { get; set; }
        public FlightLevel FL {  get; set; }
        public RadarPlotCharacteristics RadarPlot { get; set; }
        public string AircrftAddrs { get; set; }
        public string AircrftIddent { get; set; }
        public class ModeSData      //se supone que no podran ser negativos y si no se crea no se pone ?? revisar
        {
            public BDS40? BDS40 { get; set; }
            public BDS50? BDS50 { get; set; }
            public BDS60? BDS60 { get; set; }
        }
        public ModeSData ModeS { get; set; }
        public int TrackNum { get; set; }   //siempre es positivo
        public List<double> TrckVelPolRepr { get; set; }    //siempre positivo
        public TrackStatus TrckStatus {  get; set; }
        public ACAScap CommACAScapability { get; set; }
        public bool tieneTrackVelocity { get; set; }
        public double altitudPresionFt { get; set; } = double.NaN;
        public double altitudEstimadaM { get; set; } = double.NaN;
        public double elevacionRadarRad { get; set; } = double.NaN;
        public double BPS { get; set; } = double.NaN;
        public double altitudQNHFt { get; set; } = double.NaN;
        public double qnhUtilizado { get; set; } = double.NaN;

        public TiraDatosDecod048()
        {
            this.DataSourceID = new List<byte> { 0, 0 };
            this.TimeOfDay = -1;
            this.TargetRep = new TargetReportDescriptor();      //todo positivo
            this.PosSlantPolarCoord = new List<double> { double.NaN, double.NaN };
            this.PositionCorrectedLatLonAlt =new List<double> { double.NaN, double.NaN, double.NaN };
            this.mode3 = new Mode3();       //todo positivo
            this.FL = new FlightLevel();    //revisar
            this.RadarPlot = new RadarPlotCharacteristics();    //ya esta cambiado para que no salga -1
            this.AircrftAddrs = null;
            this.AircrftIddent = null;
            this.ModeS = new ModeSData();
            this.TrackNum = -1;
            this.TrckVelPolRepr = new List<double> { double.NaN, double.NaN };
            this.TrckStatus = new TrackStatus();    //siempre positivo
            this.CommACAScapability = new ACAScap();    //siempre positivo
            this.tieneTrackVelocity = false;
        }

        public static byte BitsToByte(List<int> bits)
        {
            byte result = 0;
            for (int i = 0; i < 8; i++)
            {
                result <<= 1;               // desplaza a la izquierda
                result |= (byte)bits[i];    // añade el bit
            }
            return result;
        }

        public static List<int> ByteToBits(byte b)
        {
            List<int> bits = new List<int>(8);

            for (int i = 7; i >= 0; i--)
            {
                int bit = (b >> i) & 1;
                bits.Add(bit);
            }
            return bits;
        }

        public List<int> JoinBytesToBits(List<byte> listaBytes)
        {
            List<int> resultado = new List<int>();

            foreach (byte b in listaBytes)
            {
                List<int> bits = ByteToBits(b);   // tu función existente
                resultado.AddRange(bits);         // concatenar los 8 bits
            }

            return resultado;  // devuelve todos los bits juntos
        }

        public void DecDataSourceID(Queue<byte> ColaBytes)
        {
            byte SAC = ColaBytes.Dequeue();
            byte SIC = ColaBytes.Dequeue();

            this.DataSourceID[0] = SAC;
            this.DataSourceID[1] = SIC;
        }

        public void DecodeTimeOfDayFromBits(Queue<byte> ColaBytes)
        {
            byte byte1 = ColaBytes.Dequeue();
            byte byte2 = ColaBytes.Dequeue();
            byte byte3 = ColaBytes.Dequeue();

            // Unir los 3 bytes en un entero de 24 bits
            int raw = (byte1 << 16) | (byte2 << 8) | byte3;

            // Convertir a segundos desde medianoche
            this.TimeOfDay =  raw / 128.0;
        }

        public void DecodePosSlantPolarCoord(Queue<byte> ColaBytes)
        {
            byte b1 = ColaBytes.Dequeue();
            byte b2 = ColaBytes.Dequeue();
            byte b3 = ColaBytes.Dequeue();
            byte b4 = ColaBytes.Dequeue();

            int rhoRaw = (b1 << 8) | b2;     // RHO = 16 bits
            int thetaRaw = (b3 << 8) | b4;   // THETA = 16 bits

            double rhoNM = rhoRaw / 256.0;                     // RHO en millas náuticas
            double thetaDeg = thetaRaw * (360.0 / 65536.0);    // THETA en grados

            this.PosSlantPolarCoord[0] = rhoNM;
            this.PosSlantPolarCoord[1] = thetaDeg;
        }



        public void ConvertToWGS84(double radarLat, double radarLon, double radarAlt)
        {
            double rhoNM = this.PosSlantPolarCoord[0];
            double thetaDeg = this.PosSlantPolarCoord[1];

            if (double.IsNaN(rhoNM) || double.IsNaN(thetaDeg) ||
                double.IsInfinity(rhoNM) || double.IsInfinity(thetaDeg) ||
                rhoNM < 0)
            {
                return;
            }

            GeoUtils geo = new GeoUtils();

            CoordinatesWGS84 radar = new CoordinatesWGS84(
                radarLat * GeoUtils.DEGS2RADS,
                radarLon * GeoUtils.DEGS2RADS,
                radarAlt
            );

            double rho = rhoNM * GeoUtils.NM2METERS;
            double theta = thetaDeg * GeoUtils.DEGS2RADS;

            double elevacion = 0.0;

            this.altitudEstimadaM = double.NaN;

            if (!double.IsNaN(this.altitudPresionFt) &&
                !double.IsInfinity(this.altitudPresionFt) &&
                rho > 0)
            {
                double alturaEstimada =
                    this.altitudPresionFt * GeoUtils.FEET2METERS;

                double radioTierra = 6371000.0;

                double diferenciaAltura = alturaEstimada - radarAlt;

                double senoElevacion =
                    (2.0 * radioTierra * diferenciaAltura + alturaEstimada * alturaEstimada - radarAlt * radarAlt - rho * rho) /
                    (2.0 * rho * (radioTierra + radarAlt));

                if (senoElevacion >= -1.0 && senoElevacion <= 1.0)
                {
                    elevacion = Math.Asin(senoElevacion);
                    this.altitudEstimadaM = alturaEstimada;
                }
            }

            this.elevacionRadarRad = elevacion;

            CoordinatesPolar polar = new CoordinatesPolar(rho,theta,elevacion);

            CoordinatesXYZ cartesiana =
                GeoUtils.change_radar_spherical2radar_cartesian(polar);

            CoordinatesXYZ geocentrica =
                geo.change_radar_cartesian2geocentric(radar, cartesiana);

            CoordinatesWGS84 posicion =
                geo.change_geocentric2geodesic(geocentrica);

            this.PositionCorrectedLatLonAlt[0] =
                posicion.Lat * GeoUtils.RADS2DEGS;

            this.PositionCorrectedLatLonAlt[1] =
                posicion.Lon * GeoUtils.RADS2DEGS;

            if (!double.IsNaN(this.altitudEstimadaM))
            {
                this.PositionCorrectedLatLonAlt[2] = posicion.Height;
            }
            else
            {
                this.PositionCorrectedLatLonAlt[2] = double.NaN;
            }
        }



        public void DecodeTargetRep(Queue<byte> ColaBytes)    //preguntar lo del third extent
        {
            byte b = ColaBytes.Dequeue();
            List<int> listaBits = ByteToBits(b);
            this.TargetRep.TYP = string.Join("", listaBits.GetRange(0, 3));
            this.TargetRep.SIM = listaBits[3];
            this.TargetRep.RDP = listaBits[4];
            this.TargetRep.SPI = listaBits[5];
            this.TargetRep.RAB = listaBits[6];
            if (listaBits[7] == 1)
            {
                b = ColaBytes.Dequeue();
                listaBits = ByteToBits(b);
                this.TargetRep.TST = listaBits[0];
                this.TargetRep.ERR = listaBits[1];
                this.TargetRep.XPP = listaBits[2];
                this.TargetRep.ME = listaBits[3];
                this.TargetRep.MI = listaBits[4];
                this.TargetRep.FOE_FRI = string.Join("", listaBits.GetRange(5, 2));
                if (listaBits[7] == 1)
                {
                    b = ColaBytes.Dequeue();
                    listaBits = ByteToBits(b);
                    this.TargetRep.ADSB_EP = listaBits[0];
                    this.TargetRep.ADSB_VAL = listaBits[1];
                    this.TargetRep.SCN_EP = listaBits[2];
                    this.TargetRep.SCN_VAL = listaBits[3];
                    this.TargetRep.PAI_EP = listaBits[4];
                    this.TargetRep.PAI_VAL = listaBits[5];
                }
            }
        }

        public void DecodeMode3A(Queue<byte> ColaBytes)
        {
            byte byte1 = ColaBytes.Dequeue();
            byte byte2 = ColaBytes.Dequeue();

            List<byte> listaByte = new List<byte> { byte1, byte2 };
            List<int> bits = JoinBytesToBits(listaByte);

            this.mode3.V = bits[0];
            this.mode3.G = bits[1];
            this.mode3.L = bits[2];

            int A = bits[4] * 4 + bits[5] * 2 + bits[6];
            int B = bits[7] * 4 + bits[8] * 2 + bits[9];
            int C = bits[10] * 4 + bits[11] * 2 + bits[12];
            int D = bits[13] * 4 + bits[14] * 2 + bits[15];

            this.mode3.reply = $"{A}{B}{C}{D}";
        }

        public void DecodeFL(Queue<byte> colaBytes)
        {
            byte b1 = colaBytes.Dequeue();
            byte b2 = colaBytes.Dequeue();

            List<byte> listaByte = new List<byte> { b1, b2 };
            List<int> listaBits = JoinBytesToBits(listaByte);

            this.FL.V = listaBits[0];
            this.FL.G = listaBits[1];

            if (this.FL.V == 1)
            {
                this.FL.FL = double.NaN;
                this.altitudPresionFt = double.NaN;
                return;
            }

            int raw = ((b1 & 0x3F) << 8) | b2;

            if ((raw & 0x2000) != 0)
            {
                raw = raw - 16384;
            }

            double flightLevel = raw * 0.25;

            this.FL.FL = flightLevel;

            double altitudFt = flightLevel * 100.0;

            this.altitudPresionFt = altitudFt;
        }

        private int TwosComplementToInt(byte b)    //esta funcion es para el decodeRadarPlot
        {
            // Si el MSB está a 1 → número negativo
            if ((b & 0x80) != 0)
                return b - 256;   // ejemplo: 0xF0 → -16

            return b;             // positivo normal
        }

        public void DecodeRadarPlot(Queue<byte> ColaBytes)
        {
            byte PrimerOcteto = ColaBytes.Dequeue();
            List<int> listaBitsPrimerOcteto = ByteToBits(PrimerOcteto);

            if (listaBitsPrimerOcteto[0] == 1)
            {
                this.RadarPlot.SRL = ColaBytes.Dequeue() * 0.044;
            }
            if (listaBitsPrimerOcteto[1] == 1)
            {
                this.RadarPlot.SRR = ColaBytes.Dequeue() * 1;
            }
            if (listaBitsPrimerOcteto[2] == 1)
            {
                byte raw = ColaBytes.Dequeue();
                int signed = TwosComplementToInt(raw);
                this.RadarPlot.SAM = signed * 1;
            }
            if (listaBitsPrimerOcteto[3] == 1)
            {
                this.RadarPlot.PRL = ColaBytes.Dequeue() * 0.044;
            }
            if (listaBitsPrimerOcteto[4] == 1)
            {
                byte raw = ColaBytes.Dequeue();
                int signed = TwosComplementToInt(raw);
                this.RadarPlot.PAM = signed * 1;
            }
            if (listaBitsPrimerOcteto[5] == 1)
            {
                byte raw = ColaBytes.Dequeue();
                int signed = TwosComplementToInt(raw);
                this.RadarPlot.RPD = signed * (1.0 / 256.0);
            }
            if (listaBitsPrimerOcteto[6] == 1)
            {
                byte raw = ColaBytes.Dequeue();
                int signed = TwosComplementToInt(raw);
                this.RadarPlot.APD = signed * (360.0 / Math.Pow(2, 14));
            }
        }

        public void DecodeAircraftAddress(Queue<byte> ColaBytes)
        {
            byte b1 = ColaBytes.Dequeue();
            byte b2 = ColaBytes.Dequeue();
            byte b3 = ColaBytes.Dequeue();

            int raw = (b1 << 16) | (b2 << 8) | b3;
            this.AircrftAddrs = raw.ToString("X6");
        }

        private char DecodeCharICAO(int v)
        {
            if (v == 0) return ' ';
            if (v >= 1 && v <= 26) return (char)('A' + (v - 1));
            if (v >= 48 && v <= 57) return (char)('0' + (v - 48));
            return ' ';
        }

        public void DecodeAircraftID(Queue<byte> cola)
        {
            List<byte> bytes = new List<byte>();      //lee los 6 bytes
            for (int i = 0; i < 6; i++)
            {
                bytes.Add(cola.Dequeue());
            }

            List<int> bits = JoinBytesToBits(bytes);     //los une en la lista en forma de bits

            string result = "";
            for (int i = 0; i < 8; i++)
            {
                int start = i * 6;
                int value = 0;

                for (int b = 0; b < 6; b++)
                {
                    value = (value << 1) | bits[start + b];
                }

                result += DecodeCharICAO(value);
            }

            this.AircrftIddent = result.Trim();
        }

        public void DecodeI048_250(Queue<byte> cola)
        {
            // Primer byte = REP
            int rep = cola.Dequeue();

            for (int i = 0; i < rep; i++)
            {
                // Cada bloque contiene 8 bytes:
                // 7 bytes MB Data + 1 byte BDS
                byte[] mb = new byte[8];

                for (int j = 0; j < 8; j++)
                {
                    mb[j] = cola.Dequeue();
                }

                // El último byte contiene BDS1 y BDS2
                byte bds = mb[7];

                int major = (bds >> 4) & 0x0F;
                int minor = bds & 0x0F;

                if (major == 4 && minor == 0)
                {
                    this.ModeS.BDS40 = new BDS40();
                    this.ModeS.BDS40.Decode(mb);
                    this.BPS = this.ModeS.BDS40.Baro;
                }
                else if (major == 5 && minor == 0)
                {
                    this.ModeS.BDS50 = new BDS50();
                    this.ModeS.BDS50.Decode(mb);
                }
                else if (major == 6 && minor == 0)
                {
                    this.ModeS.BDS60 = new BDS60();
                    this.ModeS.BDS60.Decode(mb);
                }
            }
        }

        public void DecodeTrack(Queue<byte> ColaBytes)
        {
            byte b1 = ColaBytes.Dequeue();
            byte b2 = ColaBytes.Dequeue();

            int raw = (b1 << 8) | b2;

            this.TrackNum = raw & 0x0FFF;
        }

        public void DecodeTrackVelocityPolar(Queue<byte> ColaBytes)
        {
            byte b1 = ColaBytes.Dequeue();
            byte b2 = ColaBytes.Dequeue();
            byte b3 = ColaBytes.Dequeue();
            byte b4 = ColaBytes.Dequeue();

            int gsRaw = (b1 << 8) | b2;
            double groundSpeedKt = gsRaw * 3600.0 / 16384.0;

            int headingRaw = (b3 << 8) | b4;
            double heading = headingRaw * (360.0 / 65536.0);

            this.TrckVelPolRepr[0] = groundSpeedKt;
            this.TrckVelPolRepr[1] = heading;

            this.tieneTrackVelocity = true;
        }

        public void DecodeTrackStatus(Queue<byte> ColaBytes)
        {
            byte b1 = ColaBytes.Dequeue();
            List<int> listaBits = ByteToBits(b1);

            this.TrckStatus.CNF = listaBits[0];         // Primer octeto
            this.TrckStatus.RAD = string.Join("", listaBits.GetRange(1, 2)); 
            this.TrckStatus.DOU = listaBits[3];
            this.TrckStatus.MAH = listaBits[4];
            this.TrckStatus.CDM = string.Join("", listaBits.GetRange(5, 2));

            if (listaBits[7] == 1)          // FX del primer octeto
            {
                b1 = ColaBytes.Dequeue();
                listaBits = ByteToBits(b1);
                this.TrckStatus.TRE = listaBits[0];          // Segundo octeto
                this.TrckStatus.GHO = listaBits[1];
                this.TrckStatus.SUP = listaBits[2];
                this.TrckStatus.TCC = listaBits[3];
            }
        }

        public void DecodeCommACAScapability(Queue<byte> ColaBytes)
        {
            byte b1 = ColaBytes.Dequeue();
            byte b2 = ColaBytes.Dequeue();

            List<byte> list = new List<byte> { b1,b2};
            List<int> listaBits = JoinBytesToBits(list);

            this.CommACAScapability.COM = 4 * listaBits[0] + 2 * listaBits[1] + listaBits[2];
            int STAT = 4 * listaBits[3] + 2 * listaBits[4] + listaBits[5];
            this.CommACAScapability.STAT = STAT;
            this.CommACAScapability.SI = listaBits[6];
            this.CommACAScapability.MSSC = listaBits[8];
            this.CommACAScapability.ARC = listaBits[9];
            this.CommACAScapability.AIC = listaBits[10];
            this.CommACAScapability.B1A = listaBits[11];
            this.CommACAScapability.B1B = string.Join("", listaBits.GetRange(12, 4));
        }
    }
}
    