using System;
using System.Collections.Generic;
using Archivos;
using P3;
namespace Simulacion
{
    public class AvionSimulacion
    {
        public string direccion {  get; set; }
        public string identificador {  get; set; }
        public double latitud { get; set; }
        public double longitud { get; set; }
        public double altitud { get; set; }
        public double ultimoTiempo { get; set; }
        public double ultimoTiempoRadar { get; set; }
        public double ultimoTiempoADSB { get; set; }
        public bool detectadoRadar { get; set; }
        public bool detectadoADSB { get; set; }
        public double flightLevel { get; set; }
        public int trackNumber { get; set; }
        public double rumbo { get; set; }
        public List<double[]> trayectoria { get; set; }
        public double velocidad;
        public string estadoVuelo;
        public string mode3A;
        public Mensaje mensajeOriginal { get; set; }
        public Mensaje ultimoMensajeCAT21;
        public Mensaje ultimoMensajeCAT48;
        public double bps { get; set; } = double.NaN;
        public double altitudPresionFt { get; set; } = double.NaN;
        public double altitudQNHFt { get; set; } = double.NaN;
        public double qnhUtilizado { get; set; } = double.NaN;

        // Atributos del Proyecto 3
        public PlanVueloP3 planVuelo { get; set; }
        public AvionSimulacion avionPrecedente { get; set; }
        public List<Mensaje> mensajesRadarP3 { get; set; }
            = new List<Mensaje>();
        public double separacionTWR { get; set; } = double.NaN;
        public double separacionMinimaTMA { get; set; } = double.NaN;
        public double tiempoSeparacionMinimaTMA { get; set; } = double.NaN;

        public AvionSimulacion()
        {
            identificador = "";
            latitud = double.NaN;
            longitud = double.NaN;
            altitud = double.NaN;

            ultimoTiempo = 0;
            ultimoTiempoRadar = -1;
            ultimoTiempoADSB = -1;

            detectadoRadar = false;
            detectadoADSB = false;
            flightLevel = double.NaN;
            trackNumber = -1;

            trayectoria = new List<double[]>();

            rumbo = double.NaN;

            velocidad = double.NaN;
            estadoVuelo = "";
            mode3A = "";
        }
        public AvionSimulacion Copiar()
        {
            AvionSimulacion copia = new AvionSimulacion();

            copia.direccion = direccion;
            copia.identificador = identificador;

            copia.latitud = latitud;
            copia.longitud = longitud;
            copia.altitud = altitud;

            copia.ultimoTiempo = ultimoTiempo;
            copia.ultimoTiempoRadar = ultimoTiempoRadar;
            copia.ultimoTiempoADSB = ultimoTiempoADSB;

            copia.detectadoRadar = detectadoRadar;
            copia.detectadoADSB = detectadoADSB;

            copia.flightLevel = flightLevel;
            copia.trackNumber = trackNumber;
            copia.rumbo = rumbo;

            copia.trayectoria = new List<double[]>();

            foreach (double[] punto in trayectoria)
            {
                copia.trayectoria.Add(new double[]
                { punto[0], punto[1]
                });
            }

            copia.velocidad = velocidad;
            copia.estadoVuelo = estadoVuelo;
            copia.mode3A = mode3A;

            copia.mensajeOriginal = mensajeOriginal;

            copia.bps = bps;
            copia.altitudPresionFt = altitudPresionFt;
            copia.altitudQNHFt = altitudQNHFt;
            copia.qnhUtilizado = qnhUtilizado;

            return copia;
        }
    }
}
