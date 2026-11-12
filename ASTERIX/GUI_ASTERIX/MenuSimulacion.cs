using Archivos;
using Archvios;
using DatosDecod;
using DatosDecod21;
using DatosDecod48;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using Gst;
using Microsoft.VisualBasic;
using Simulacion;
using System.Data.Entity.Core.Mapping;
using System.Diagnostics;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using P3;
using MultiCAT6.Utils;

namespace GUI_ASTERIX
{
    public partial class MenuSimulacion : Form
    {
        private BarraTiempoPersonalizada barraTiempoNueva;
        private BarraTiempoPersonalizada barraVelocidadNueva;
        private double TiempoActual = 0;
        private double tiempoInicioSimulacion = 0;
        private double tiempoFinSimulacion = 0;
        private List<Mensaje> ListaMensajes = new List<Mensaje>();
        private List<Mensaje> ListaMensajesFiltrados = new List<Mensaje>();
        private List<AvionSimulacion> AvionesActuales = new List<AvionSimulacion>();
        private List<AvionSimulacion> MensajesActuales = new List<AvionSimulacion>();
        private List<PlanVueloP3> planesVueloP3 = new List<PlanVueloP3>();
        private List<AvionSimulacion> avionesP3 = new List<AvionSimulacion>();
        private bool mostrarSoloAvionesP3 = false;
        private HashSet<Mensaje> mensajesVisiblesP3 = new HashSet<Mensaje>();
        private bool Reproduciendo = false;
        private GMapOverlay capaAviones;
        private GMapOverlay capaTrayectorias;
        private GMapOverlay capaHeatmap;
        private bool mostrarHeatmapDinamico = false;
        private double ultimoTiempoHeatmap = -1;
        private const double intervaloHeatmap = 10;
        private bool mostrarHeatmap = false;
        private List<AvionSimulacion> avionesTrayectoriaSeleccionados = new List<AvionSimulacion>();
        private bool mostrarTodasTrayectorias = false;
        private ScrollBarASTERIX barraDataGrid;
        public MenuSimulacion()
        {
            // inicializar el formulario y configurar el temporizador y el mapa
            InitializeComponent();

            CrearEscalaHeatmap();
            panelLeyendaHeatmap.Visible = false;

            timerSimulacion.Interval = 1000;
            gMapControl1.OnMarkerClick += GMapControl1_OnMarkerClick;

            AjustarMapa();
            this.Resize += (sender, e) => AjustarMapa();

            // crear la barra personalizada para controlar el tiempo de simulación
            barraTiempoNueva = new BarraTiempoPersonalizada();

            barraTiempoNueva.Location = trackBarTiempo.Location;
            barraTiempoNueva.Size = new Size(trackBarTiempo.Width, 36);

            barraTiempoNueva.Anchor = trackBarTiempo.Anchor;
            barraTiempoNueva.BackColor = Color.FromArgb(18, 28, 45);

            barraTiempoNueva.TiempoMinimo = trackBarTiempo.Minimum;
            barraTiempoNueva.TiempoMaximo = trackBarTiempo.Maximum;
            barraTiempoNueva.TiempoValor = trackBarTiempo.Value;

            // sustituir visualmente la barra original por la personalizada
            trackBarTiempo.Parent.Controls.Add(barraTiempoNueva);
            trackBarTiempo.Visible = false;
            barraTiempoNueva.BringToFront();

            // actualizar el tiempo de simulación al soltar la barra
            barraTiempoNueva.ValorConfirmado += (sender, e) =>
            {
                bool estabaReproduciendo = Reproduciendo;

                trackBarTiempo.Value = barraTiempoNueva.TiempoValor;

                trackBarTiempo_Scroll(sender, EventArgs.Empty);

                if (estabaReproduciendo)
                {
                    Reproduciendo = true;
                    pictureBox1.Image = Properties.Resources.botonPausa;
                    timerSimulacion.Start();
                }
            };

            // mostrar la hora seleccionada mientras se arrastra la barra
            barraTiempoNueva.ValorCambiado += (sender, e) =>
            {
                if (!barraTiempoNueva.EstaArrastrando)
                    return;

                double tiempoPrevisualizado =
                    tiempoInicioSimulacion + barraTiempoNueva.TiempoValor;

                TimeSpan hora = TimeSpan.FromSeconds(tiempoPrevisualizado);

                textBoxHora.Text = ((int)hora.TotalHours).ToString("00");
                textBoxMinuto.Text = hora.Minutes.ToString("00");
                textBoxSegundo.Text = hora.Seconds.ToString("00");
            };

            // permitir introducir manualmente la hora de simulación
            textBoxHora.KeyDown += HoraTextBox_KeyDown;
            textBoxMinuto.KeyDown += HoraTextBox_KeyDown;
            textBoxSegundo.KeyDown += HoraTextBox_KeyDown;

            // crear la barra personalizada para controlar la velocidad
            barraVelocidadNueva = new BarraTiempoPersonalizada();

            barraVelocidadNueva.Location = trackBarVelSimulacion.Location;
            barraVelocidadNueva.Size = new Size(
                trackBarVelSimulacion.Width, 32);

            barraVelocidadNueva.Anchor = trackBarVelSimulacion.Anchor;
            barraVelocidadNueva.BackColor = Color.FromArgb(18, 28, 45);

            barraVelocidadNueva.TiempoMinimo = 1;
            barraVelocidadNueva.TiempoMaximo = 8;
            barraVelocidadNueva.TiempoValor = trackBarVelSimulacion.Value;

            // sustituir visualmente la barra de velocidad original
            trackBarVelSimulacion.Parent.Controls.Add(barraVelocidadNueva);
            trackBarVelSimulacion.Visible = false;
            barraVelocidadNueva.BringToFront();

            // aplicar la velocidad seleccionada al soltar la barra
            barraVelocidadNueva.ValorConfirmado += (sender, e) =>
            {
                trackBarVelSimulacion.Value = barraVelocidadNueva.TiempoValor;

                trackBarVelSimulacion_Scroll(sender, EventArgs.Empty);
            };

            // mostrar el multiplicador de velocidad mientras se mueve la barra
            barraVelocidadNueva.ValorCambiado += (sender, e) =>
            {
                int valor = barraVelocidadNueva.TiempoValor;
                int velocidad = (int)Math.Pow(2, valor - 1);

                labelVelocidad.Text = "x" + velocidad;
            };

            // ajustar la tabla de aviones cuando cambia el tamaño o la posición del mapa
            AjustarDataGrid();

            gMapControl1.Resize += (sender, e) =>
            {
                AjustarDataGrid();
            };

            gMapControl1.LocationChanged += (sender, e) =>
            {
                AjustarDataGrid();
            };
            // aplicar el diseño personalizado al menú

            // aplicar el diseño personalizado
            menuStrip1.Renderer = new ColoresMenu();

            // colores de la barra superior
            menuStrip1.BackColor = Color.FromArgb(16, 27, 45);
            menuStrip1.ForeColor = Color.White;

            // configurar los elementos principales y desplegables
            foreach (ToolStripMenuItem item in menuStrip1.Items)
            {
                item.ForeColor = Color.White;

                foreach (ToolStripItem subItem in item.DropDownItems)
                {
                    subItem.ForeColor = Color.White;
                }

            }

            ConfigurarScrollBarDataGrid();
            EstiloDataGrid.PersonalizarDataGrid(dataGridAviones);

            this.Shown += MenuSimulacion_Shown;
        }

        private void AjustarDataGrid()
        {
            dataGridAviones.Location = gMapControl1.Location;
            dataGridAviones.Size = gMapControl1.Size;
        }

        private void MenuSimulacion_Load(object sender, EventArgs e)
        {
            pictureBoxContorles.Location = gMapControl1.Location;
            pictureBoxContorles.Size = gMapControl1.Size;

            //-----------parte del datagrid.-------------------------------
            dataGridAviones.Columns.Clear();

            dataGridAviones.Columns.Add("direccion", "Dirección");
            dataGridAviones.Columns.Add("identificador", "Identificador");
            dataGridAviones.Columns.Add("latitud", "Latitud");
            dataGridAviones.Columns.Add("longitud", "Longitud");
            dataGridAviones.Columns.Add("fl", "FL");
            dataGridAviones.Columns.Add("altitud", "Altitud ft");
            dataGridAviones.Columns.Add("sistema", "Sistema");
            dataGridAviones.Columns.Add("ultimoTiempo", "Último mensaje");

            dataGridAviones.AllowUserToAddRows = false;
            dataGridAviones.ReadOnly = true;
            dataGridAviones.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            //---------------parte del mapañ.........................
            GMapProvider.UserAgent = "PGTA-ASTERIX-Simulator/1.0";

            GMaps.Instance.Mode = AccessMode.ServerOnly;
            gMapControl1.MapProvider = OpenStreetMapProvider.Instance;

            gMapControl1.MinZoom = 2;
            gMapControl1.MaxZoom = 18;

            gMapControl1.DragButton = MouseButtons.Left;
            gMapControl1.ShowCenter = false;

            // Zona de interés con un poco de margen
            RectLatLng zona = RectLatLng.FromLTRB(
                1.7,    // Oeste
                41.5,   // Norte
                2.4,    // Este
                41.1    // Sur
            );

            gMapControl1.SetZoomToFitRect(zona);

            //parte del heatmap
            capaHeatmap = new GMapOverlay("heatmap");
            gMapControl1.Overlays.Add(capaHeatmap);

            capaTrayectorias = new GMapOverlay("trayectorias");
            gMapControl1.Overlays.Add(capaTrayectorias);

            capaAviones = new GMapOverlay("aviones");
            gMapControl1.Overlays.Add(capaAviones);
        }

        private void timerSimulacion_Tick(object sender, EventArgs e)
        {
            AvanzarUnSegundo();
        }
        private void ReconstruirAvionesFiltrados()
        {
            ReconstruirAviones(TiempoActual);
        }

        private void ReconstruirAviones(double tiempoSeleccionado)
        {
            // Guardar las trayectorias seleccionadas
            List<string> clavesSeleccionadas = new List<string>();

            foreach (AvionSimulacion avion in avionesTrayectoriaSeleccionados)
            {
                clavesSeleccionadas.Add(ObtenerClaveAvion(avion));
            }

            // Limpiar el estado actual
            AvionesActuales.Clear();
            MensajesActuales.Clear();

            capaAviones.Markers.Clear();
            capaTrayectorias.Routes.Clear();

            avionesTrayectoriaSeleccionados.Clear();

            // Reconstruir los últimos 10 segundos
            double tiempoInicial = Math.Max(
                tiempoInicioSimulacion,
                Math.Floor(tiempoSeleccionado) - 10
            );

            for (double tiempo = tiempoInicial;
                 tiempo <= tiempoSeleccionado;
                 tiempo++)
            {
                TiempoActual = tiempo;

                ActualizarAviones();
                EliminarAvionesInactivos();
            }

            // Recuperar el instante solicitado
            TiempoActual = tiempoSeleccionado;

            EliminarAvionesInactivos();

            // Reconstruir los mensajes visibles
            ActualizarMensajesActuales();

            // Recuperar las trayectorias seleccionadas
            foreach (AvionSimulacion avion in AvionesActuales)
            {
                if (clavesSeleccionadas.Contains(ObtenerClaveAvion(avion)))
                {
                    avionesTrayectoriaSeleccionados.Add(avion);
                }
            }

            // Actualizar la barra de tiempo
            trackBarTiempo.Value = Math.Max(
                trackBarTiempo.Minimum,
                Math.Min(
                    trackBarTiempo.Maximum,
                    (int)(TiempoActual - tiempoInicioSimulacion)
                )
            );

            barraTiempoNueva.TiempoValor = trackBarTiempo.Value;

            // Actualizar la interfaz
            ActualizarHoraTextBox();
            ActualizarDataGrid();
            ActualizarMapa();
            MostrarTrayectorias();
        }
        private void AvanzarUnSegundo()
        {
            // guardar el estado actual para poder retroceder en la simulación

            // avanzar un segundo y actualizar la barra de tiempo
            TiempoActual = TiempoActual + 1;
            if (TiempoActual <= tiempoFinSimulacion)
            {
                trackBarTiempo.Value = (int)(
                    TiempoActual - tiempoInicioSimulacion
                );
            }
            barraTiempoNueva.TiempoValor = trackBarTiempo.Value;

            // convertir el tiempo actual a formato de horas, minutos y segundos
            TimeSpan hora = TimeSpan.FromSeconds(TiempoActual);

            //actualiza todo lo del form
            ActualizarHoraTextBox();
            ActualizarAviones();
            EliminarAvionesInactivos();
            ActualizarDataGrid();
            ActualizarMapa();
            MostrarTrayectorias();
            ActualizarHeatmapDinamico();
        }

        private void trackBarVelSimulacion_Scroll(object sender, EventArgs e)
        {
            //cada posicion es una velocidad diferente
            if (trackBarVelSimulacion.Value == 1)
            {
                timerSimulacion.Interval = 1000;
                labelVelocidad.Text = "x1";
            }

            if (trackBarVelSimulacion.Value == 2)
            {
                timerSimulacion.Interval = 500;
                labelVelocidad.Text = "x2";
            }

            if (trackBarVelSimulacion.Value == 3)
            {
                timerSimulacion.Interval = 250;
                labelVelocidad.Text = "x4";
            }

            if (trackBarVelSimulacion.Value == 4)
            {
                timerSimulacion.Interval = 125;
                labelVelocidad.Text = "x8";
            }
            if (trackBarVelSimulacion.Value == 5)
            {
                timerSimulacion.Interval = 62;
                labelVelocidad.Text = "x16";
            }

            if (trackBarVelSimulacion.Value == 6)
            {
                timerSimulacion.Interval = 31;
                labelVelocidad.Text = "x32";
            }

            if (trackBarVelSimulacion.Value == 7)
            {
                timerSimulacion.Interval = 16;
                labelVelocidad.Text = "x64";
            }

            if (trackBarVelSimulacion.Value == 8)
            {
                timerSimulacion.Interval = 8;
                labelVelocidad.Text = "x128";
            }
        }


        private void ActualizarAviones()
        {
            // recorrer los mensajes filtrados de la simulación
            foreach (Mensaje mensaje in ListaMensajesFiltrados)
            {
                double tiempoMensaje = -1;

                string direccion = "";
                string identificador = "";

                double latitud = double.NaN;
                double longitud = double.NaN;
                double altitud = double.NaN;
                double flightLevel = double.NaN;

                double bps = double.NaN;
                double altitudPresionFt = double.NaN;
                double altitudQNHFt = double.NaN;
                double qnhUtilizado = double.NaN;

                int trackNumber = -1;

                double velocidad = double.NaN;
                double rumbo = double.NaN;
                string mode3A = "";
                string estadoVuelo = "";

                bool esADSB = false;
                bool esRadar = false;

                // obtener los datos de los mensajes ads-b
                if (mensaje.categoria == 21)
                {
                    TiraDatosDecod021 mensaje21 = (TiraDatosDecod021)mensaje.tira;

                    mode3A = mensaje21.Mode3ACode;

                    tiempoMensaje = mensaje21.TimeReceptionPosition;

                    direccion = mensaje21.TargetAddress;
                    identificador = mensaje21.TargetIdentification;

                    latitud = mensaje21.PosWGS84HighRes[0];
                    longitud = mensaje21.PosWGS84HighRes[1];

                    if (!double.IsNaN(mensaje21.FlightLevel))
                    {
                        flightLevel = mensaje21.FlightLevel;

                        bps = mensaje21.BPS;
                        altitudPresionFt = mensaje21.altitudPresionFt;
                        altitudQNHFt = mensaje21.altitudQNHFt;
                        qnhUtilizado = mensaje21.qnhUtilizado;

                        // utilizar la altitud corregida si está disponible
                        if (!double.IsNaN(altitudQNHFt))
                        {
                            altitud = altitudQNHFt;
                        }
                        else
                        {
                            altitud = altitudPresionFt;
                        }
                    }

                    esADSB = true;
                }

                // obtener los datos de los mensajes radar
                else if (mensaje.categoria == 48)
                {
                    TiraDatosDecod048 mensaje48 = (TiraDatosDecod048)mensaje.tira;

                    // descartar los mensajes radar no confirmados o sin identificar
                    bool trackSinIdentificar = string.IsNullOrEmpty(mensaje48.AircrftAddrs) && string.IsNullOrEmpty(mensaje48.AircrftIddent) && mensaje48.TargetRep.TYP == "001";

                    if (mensaje48.TrckStatus.CNF == 1 || trackSinIdentificar)
                    {
                        continue;
                    }

                    if (mensaje48.tieneTrackVelocity)
                    {
                        velocidad = mensaje48.TrckVelPolRepr[0];
                        rumbo = mensaje48.TrckVelPolRepr[1];
                    }

                    mode3A = mensaje48.mode3.reply;

                    tiempoMensaje = mensaje48.TimeOfDay;

                    // utilizar una cadena vacía si la dirección del avión es nula
                    direccion = mensaje48.AircrftAddrs ?? "";
                    identificador = mensaje48.AircrftIddent;
                    trackNumber = mensaje48.TrackNum;

                    latitud = mensaje48.PositionCorrectedLatLonAlt[0];
                    longitud = mensaje48.PositionCorrectedLatLonAlt[1];

                    if (!double.IsNaN(mensaje48.FL.FL))
                    {
                        flightLevel = mensaje48.FL.FL;

                        bps = mensaje48.BPS;
                        altitudPresionFt = mensaje48.altitudPresionFt;
                        altitudQNHFt = mensaje48.altitudQNHFt;
                        qnhUtilizado = mensaje48.qnhUtilizado;

                        // utilizar la altitud corregida si está disponible
                        altitud = !double.IsNaN(altitudQNHFt)
                            ? altitudQNHFt
                            : altitudPresionFt;
                    }

                    esRadar = true;
                }

                // procesar únicamente los mensajes correspondientes al segundo actual
                if (tiempoMensaje >= TiempoActual &&
                    tiempoMensaje < TiempoActual + 1)
                {
                    // crear un mensaje visual para representar la detección en el mapa
                    if (!double.IsNaN(latitud) && !double.IsNaN(longitud))
                    {
                        AvionSimulacion mensajeVisual =
                            new AvionSimulacion();

                        mensajeVisual.direccion = direccion;
                        mensajeVisual.identificador = identificador;
                        mensajeVisual.trackNumber = trackNumber;

                        mensajeVisual.latitud = latitud;
                        mensajeVisual.longitud = longitud;

                        mensajeVisual.altitud = altitud;
                        mensajeVisual.flightLevel = flightLevel;
                        mensajeVisual.bps = bps;
                        mensajeVisual.altitudPresionFt = altitudPresionFt;
                        mensajeVisual.altitudQNHFt = altitudQNHFt;
                        mensajeVisual.qnhUtilizado = qnhUtilizado;

                        mensajeVisual.velocidad = velocidad;
                        mensajeVisual.rumbo = rumbo;
                        mensajeVisual.mode3A = mode3A;

                        mensajeVisual.ultimoTiempo = tiempoMensaje;

                        mensajeVisual.mensajeOriginal = mensaje;

                        if (esADSB)
                        {
                            mensajeVisual.detectadoADSB = true;
                            mensajeVisual.ultimoTiempoADSB = tiempoMensaje;
                        }

                        if (esRadar)
                        {
                            mensajeVisual.detectadoRadar = true;
                            mensajeVisual.ultimoTiempoRadar = tiempoMensaje;
                        }

                        ActualizarMensajeVisual(mensajeVisual);
                    }

                    // buscar si el avión ya existe mediante su dirección o número de track
                    AvionSimulacion avionEncontrado = null;

                    foreach (AvionSimulacion avion in AvionesActuales)
                    {
                        if (direccion != "" && direccion != "-1")
                        {
                            if (avion.direccion == direccion)
                            {
                                avionEncontrado = avion;
                                break;
                            }
                        }
                        else if (esRadar && trackNumber != -1)
                        {
                            if ((avion.direccion == "" || avion.direccion == "-1") &&
                                avion.trackNumber == trackNumber)
                            {
                                avionEncontrado = avion;
                                break;
                            }
                        }
                    }

                    // añadir un nuevo avión si todavía no existe en la simulación
                    if (avionEncontrado == null)
                    {
                        AvionSimulacion nuevoAvion = new AvionSimulacion();

                        nuevoAvion.direccion = direccion;
                        nuevoAvion.identificador = identificador;
                        nuevoAvion.trackNumber = trackNumber;
                        nuevoAvion.rumbo = rumbo;

                        nuevoAvion.latitud = latitud;
                        nuevoAvion.longitud = longitud;

                        if (!double.IsNaN(latitud) && !double.IsNaN(longitud))
                        {
                            nuevoAvion.trayectoria.Add(
                            new double[] { latitud, longitud }
                            );
                        }

                        nuevoAvion.altitud = altitud;
                        nuevoAvion.flightLevel = flightLevel;
                        nuevoAvion.bps = bps;
                        nuevoAvion.altitudPresionFt = altitudPresionFt;
                        nuevoAvion.altitudQNHFt = altitudQNHFt;
                        nuevoAvion.qnhUtilizado = qnhUtilizado;

                        nuevoAvion.ultimoTiempo = tiempoMensaje;

                        nuevoAvion.velocidad = velocidad;
                        nuevoAvion.mode3A = mode3A;
                        nuevoAvion.estadoVuelo = estadoVuelo;

                        if (esADSB)
                        {
                            nuevoAvion.ultimoTiempoADSB = tiempoMensaje;
                            nuevoAvion.detectadoADSB = true;
                        }

                        if (esRadar)
                        {
                            nuevoAvion.ultimoTiempoRadar = tiempoMensaje;
                            nuevoAvion.detectadoRadar = true;
                        }

                        nuevoAvion.mensajeOriginal = mensaje;

                        if (mensaje.categoria == 21)
                            nuevoAvion.ultimoMensajeCAT21 = mensaje;
                        else if (mensaje.categoria == 48)
                            nuevoAvion.ultimoMensajeCAT48 = mensaje;

                        AvionesActuales.Add(nuevoAvion);
                    }

                    // actualizar los datos del avión si ya existe
                    else
                    {
                        if (!double.IsNaN(latitud) && !double.IsNaN(longitud))
                        {
                            bool actualizarPosicion = true;

                            // dar prioridad a la posición ads-b frente a la del radar
                            if (esRadar && avionEncontrado.detectadoADSB)
                            {
                                actualizarPosicion = false;
                            }

                            // actualizar la posición, el rumbo y la trayectoria
                            if (actualizarPosicion &&
                                !double.IsNaN(latitud) &&
                                !double.IsNaN(longitud))
                            {
                                if (!double.IsNaN(avionEncontrado.latitud) &&
                                    !double.IsNaN(avionEncontrado.longitud))
                                {
                                    avionEncontrado.rumbo = CalcularRumbo(
                                        avionEncontrado.latitud,
                                        avionEncontrado.longitud,
                                        latitud,
                                        longitud
                                    );
                                }

                                avionEncontrado.latitud = latitud;
                                avionEncontrado.longitud = longitud;
                                avionEncontrado.trayectoria.Add(
                                new double[] { latitud, longitud }
                                );
                            }

                            // actualizar los datos de vuelo disponibles
                            if (!double.IsNaN(altitud))
                            {
                                avionEncontrado.altitud = altitud;
                            }

                            if (!double.IsNaN(flightLevel))
                            {
                                avionEncontrado.flightLevel = flightLevel;
                            }

                            if (esADSB || esRadar)
                            {
                                avionEncontrado.bps = bps;
                                avionEncontrado.altitudPresionFt = altitudPresionFt;
                                avionEncontrado.altitudQNHFt = altitudQNHFt;
                                avionEncontrado.qnhUtilizado = qnhUtilizado;
                            }

                            if (!double.IsNaN(velocidad))
                            {
                                avionEncontrado.velocidad = velocidad;
                            }

                            if (!string.IsNullOrWhiteSpace(mode3A))
                            {
                                avionEncontrado.mode3A = mode3A;
                            }

                            if (!string.IsNullOrWhiteSpace(estadoVuelo))
                            {
                                avionEncontrado.estadoVuelo = estadoVuelo;
                            }

                            // guardar el último mensaje recibido del avión
                            avionEncontrado.ultimoTiempo = tiempoMensaje;
                            avionEncontrado.mensajeOriginal = mensaje;

                            if (mensaje.categoria == 21)
                                avionEncontrado.ultimoMensajeCAT21 = mensaje;
                            else if (mensaje.categoria == 48)
                                avionEncontrado.ultimoMensajeCAT48 = mensaje;

                            if (identificador != "")
                            {
                                avionEncontrado.identificador = identificador;
                            }

                            // actualizar los sistemas que han detectado el avión
                            if (esADSB)
                            {
                                avionEncontrado.ultimoTiempoADSB = tiempoMensaje;
                                avionEncontrado.detectadoADSB = true;
                            }

                            if (esRadar)
                            {
                                avionEncontrado.ultimoTiempoRadar = tiempoMensaje;
                                avionEncontrado.detectadoRadar = true;
                            }

                            if (esRadar && trackNumber != -1)
                            {
                                avionEncontrado.trackNumber = trackNumber;
                            }
                        }
                    }
                }

                // desactivar las detecciones que llevan 10 segundos sin actualizarse
                for (int i = AvionesActuales.Count - 1; i >= 0; i--)
                {
                    AvionSimulacion avion = AvionesActuales[i];

                    if (avion.ultimoTiempoADSB >= 0 &&
                        TiempoActual - avion.ultimoTiempoADSB >= 10)
                    {
                        avion.detectadoADSB = false;
                    }

                    if (avion.ultimoTiempoRadar >= 0 &&
                        TiempoActual - avion.ultimoTiempoRadar >= 10)
                    {
                        avion.detectadoRadar = false;
                    }
                }
            }
        }

        private double CalcularRumbo(double lat1, double lon1, double lat2, double lon2)
        {
            double lat1Rad = lat1 * Math.PI / 180.0;
            double lat2Rad = lat2 * Math.PI / 180.0;
            double diferenciaLon = (lon2 - lon1) * Math.PI / 180.0;

            double y = Math.Sin(diferenciaLon) * Math.Cos(lat2Rad);

            double x =
                Math.Cos(lat1Rad) * Math.Sin(lat2Rad) -
                Math.Sin(lat1Rad) * Math.Cos(lat2Rad) *
                Math.Cos(diferenciaLon);

            double rumbo = Math.Atan2(y, x) * 180.0 / Math.PI;

            if (rumbo < 0)
            {
                rumbo += 360;
            }

            return rumbo;
        }
        private void BuscarTiempoInicial()
        {
            // buscar el primer y último tiempo de los mensajes filtrados
            double primerTiempo = double.MaxValue;
            double ultimoTiempo = double.MinValue;

            foreach (Mensaje mensaje in ListaMensajesFiltrados)
            {
                double tiempo = double.NaN;

                // obtener el tiempo del mensaje según su categoría
                if (mensaje.categoria == 21)
                {
                    TiraDatosDecod021 mensaje21 =
                        (TiraDatosDecod021)mensaje.tira;

                    tiempo = mensaje21.TimeReceptionPosition;
                }
                else if (mensaje.categoria == 48)
                {
                    TiraDatosDecod048 mensaje48 =
                        (TiraDatosDecod048)mensaje.tira;

                    tiempo = mensaje48.TimeOfDay;
                }

                // descartar los tiempos no válidos --> los Nan
                if (double.IsNaN(tiempo) ||
                    double.IsInfinity(tiempo) ||
                    tiempo < 0)
                {
                    continue;
                }

                // actualizar los límites de tiempo de la simulación
                if (tiempo < primerTiempo)
                    primerTiempo = tiempo;

                if (tiempo > ultimoTiempo)
                    ultimoTiempo = tiempo;
            }

            // configurar el intervalo de simulación y las barras de tiempo
            if (primerTiempo != double.MaxValue)
            {
                tiempoInicioSimulacion = Math.Floor(primerTiempo);
                tiempoFinSimulacion = Math.Ceiling(ultimoTiempo);
                trackBarTiempo.Minimum = 0;

                trackBarTiempo.Maximum = (int)(
                    tiempoFinSimulacion - tiempoInicioSimulacion
                );

                trackBarTiempo.Value = 0;
                barraTiempoNueva.TiempoMinimo = trackBarTiempo.Minimum;
                barraTiempoNueva.TiempoMaximo = trackBarTiempo.Maximum;
                barraTiempoNueva.TiempoValor = trackBarTiempo.Value;

                // establecer el tiempo inicial y actualizar las horas mostradas
                TiempoActual = tiempoInicioSimulacion;

                TimeSpan hora = TimeSpan.FromSeconds(TiempoActual);
                ActualizarHoraTextBox();

                TimeSpan horaFinal = TimeSpan.FromSeconds(tiempoFinSimulacion);

                labelTiempoFinal.Text = horaFinal.ToString(@"hh\:mm\:ss");
            }
        }

        private void trackBarTiempo_Scroll(object sender, EventArgs e)
        {
            if (ListaMensajesFiltrados.Count == 0)
                return;

            // pausar la simulación
            timerSimulacion.Stop();
            Reproduciendo = false;
            pictureBox1.Image = Properties.Resources.botonReproducir;

            // obtener el tiempo seleccionado
            double tiempoSeleccionado =
                tiempoInicioSimulacion + trackBarTiempo.Value;

            // limpiar los datos anteriores
            AvionesActuales.Clear();
            MensajesActuales.Clear();

            avionesTrayectoriaSeleccionados.Clear();

            capaAviones.Markers.Clear();
            capaTrayectorias.Routes.Clear();

            // reconstruir los aviones de los últimos 10 segundos
            double tiempoInicial = Math.Max(
                tiempoInicioSimulacion,
                Math.Floor(tiempoSeleccionado) - 10
            );

            for (double tiempo = tiempoInicial;
                 tiempo <= tiempoSeleccionado;
                 tiempo++)
            {
                TiempoActual = tiempo;

                ActualizarAviones();
                EliminarAvionesInactivos();
            }

            // recuperar el tiempo seleccionado
            TiempoActual = tiempoSeleccionado;

            // eliminar los aviones que ya no están activos
            EliminarAvionesInactivos();

            // actualizar los mensajes visibles
            ActualizarMensajesActuales();

            // actualizar la hora
            ActualizarHoraTextBox();

            // actualizar la interfaz inmediatamente
            ActualizarDataGrid();
            ActualizarMapa();
            MostrarTrayectorias();

            ActualizarHeatmapDinamico(true);
        }


        private void buttonAvanzar_Click(object sender, EventArgs e)
        {
            AvanzarUnSegundo();
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            //limpia todos los vectores y esas cosas
            timerSimulacion.Stop();
            Reproduciendo = false;

            AvionesActuales.Clear();
            MensajesActuales.Clear();

            dataGridAviones.Rows.Clear();

            capaTrayectorias.Routes.Clear();
            avionesTrayectoriaSeleccionados.Clear();

            //busca el inicio de la simulacion 
            BuscarTiempoInicial();

            TimeSpan hora = TimeSpan.FromSeconds(TiempoActual);
            ActualizarHoraTextBox();

            ActualizarAviones();
            ActualizarDataGrid();
            ActualizarMapa();

            pictureBox1.Image = Properties.Resources.botonReproducir;

            capaTrayectorias.Routes.Clear();

            densidadesDinamicas.Clear();
            siguientePuntoHeatmap = 0;
            ultimoTiempoHeatmap = -1;

            ActualizarHeatmapDinamico(true);
        }



        private async void cargaDatosrToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Abrir el explorador para seleccionar archivos ASTERIX
            using (OpenFileDialog explorador = new OpenFileDialog())
            {
                explorador.Multiselect = true;
                explorador.Title = "Seleccionar archivos ASTERIX";

                if (explorador.ShowDialog() != DialogResult.OK)
                    return;

                await CargarArchivosAsterix(explorador.FileNames);
            }
        }


        // Cargar y procesar los archivos ASTERIX
        private async Task CargarArchivosAsterix(string[] archivos)
        {
            try
            {
                List<Mensaje> nuevosMensajes;

                // Leer y decodificar los archivos mostrando el progreso
                using (FormCargando cargando = new FormCargando())
                {
                    cargando.Show(this);
                    cargando.ActualizarProgreso(0);

                    IProgress<int> progreso = new Progress<int>(porcentaje =>
                    {
                        if (!cargando.IsDisposed)
                            cargando.ActualizarProgreso(porcentaje);
                    });

                    try
                    {
                        nuevosMensajes = await Task.Run(() =>
                        {
                            LeerDatos lector = new LeerDatos();

                            // Combinar los archivos eliminando solapamientos
                            // y decodificar los mensajes resultantes
                            return lector.DatosProcesadosMultiples(
                                archivos,
                                progreso
                            );
                        });
                    }
                    finally
                    {
                        cargando.Close();
                    }
                }

                // Comprobar que los archivos contienen mensajes válidos
                if (nuevosMensajes == null || nuevosMensajes.Count == 0)
                {
                    MessageBox.Show(
                        "Los archivos seleccionados no contienen mensajes válidos."
                    );
                    return;
                }

                // Detener la simulación anterior
                timerSimulacion.Stop();
                Reproduciendo = false;

                pictureBox1.Image =
                    Properties.Resources.botonReproducir;

                // Eliminar los datos de la simulación anterior
                AvionesActuales.Clear();
                MensajesActuales.Clear();

                avionesTrayectoriaSeleccionados.Clear();

                dataGridAviones.Rows.Clear();

                capaAviones.Markers.Clear();
                capaTrayectorias.Routes.Clear();

                // Guardar los nuevos mensajes y restablecer los filtros
                RestablecerFiltros();

                ListaMensajes = nuevosMensajes;
                // Desactivar el filtro P3 al cambiar los datos radar
                mostrarSoloAvionesP3 = false;
                mensajesVisiblesP3.Clear();
                avionesP3.Clear();

                Filtros gestorFiltros = new Filtros();

                ListaMensajesFiltrados =
                    gestorFiltros.PrepararMensajes(ListaMensajes);

                // Preparar los datos del mapa de calor
                InvalidarHeatmap();
                PrepararPuntosHeatmap();

                // Reiniciar el tiempo al comienzo de los archivos
                TiempoActual = 0;
                BuscarTiempoInicial();

                TimeSpan hora = TimeSpan.FromSeconds(TiempoActual);
                ActualizarHoraTextBox();

                // Actualizar la simulación y los elementos visuales
                ActualizarAviones();
                ActualizarDataGrid();
                ActualizarMapa();
                MostrarTrayectorias();

                ActualizarHeatmapDinamico(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al cargar los archivos:\n" + ex.Message
                );
            }
        }


        private async void MenuSimulacion_Shown(object sender, EventArgs e)
        {
            // Ruta del archivo ASTERIX que queremos cargar automáticamente
            string rutaArchivo = @"C:\Users\user\Desktop\asterix_radar.ast";

            // Comprobar que existe
            if (!File.Exists(rutaArchivo))
            {
                MessageBox.Show(
                    "No se encuentra el archivo ASTERIX:\n" +
                    rutaArchivo
                );

                return;
            }

            // Cargar automáticamente el archivo
            await CargarArchivosAsterix(
                new string[] { rutaArchivo }
            );
        }


        private void ActualizarDataGrid()
        {
            int primeraFila = dataGridAviones.FirstDisplayedScrollingRowIndex;
            dataGridAviones.Rows.Clear();

            foreach (AvionSimulacion avion in AvionesActuales)
            {
                string sistema = "";

                if (avion.detectadoRadar)
                {
                    sistema = "Radar";
                }
                else if (avion.detectadoADSB)
                {
                    sistema = "ADS-B";
                }

                dataGridAviones.Rows.Add(
                    avion.direccion,
                    avion.identificador,
                    avion.latitud,
                    avion.longitud,
                    avion.flightLevel,
                    avion.altitud,
                    sistema,
                    avion.ultimoTiempo
                );
            }
            // restaurar la posición del datagrid
            if (dataGridAviones.Rows.Count > 0 && primeraFila >= 0)
            {
                int indice = Math.Min(primeraFila, dataGridAviones.Rows.Count - 1);

                try
                {
                    dataGridAviones.FirstDisplayedScrollingRowIndex = indice;
                }
                catch (InvalidOperationException)
                {
                    // ignorar si la fila no puede mostrarse
                }
            }

            barraDataGrid?.Actualizar();
            barraDataGrid?.Actualizar();
        }

        private void ActualizarMapa()
        {
            // eliminar los marcadores anteriores del mapa
            capaAviones.Markers.Clear();

            foreach (AvionSimulacion mensaje in MensajesActuales)
            {
                // Filtrar los mensajes visibles cuando está activo el P3

                if (mostrarSoloAvionesP3)
                {
                    // Solo mostrar detecciones radar CAT48
                    if (!mensaje.detectadoRadar)
                        continue;

                    // Comprobar que corresponde a un vuelo del Excel
                    bool esAvionP3 = avionesP3.Any(avion =>
                        avion.planVuelo != null &&
                        avion.mensajesRadarP3.Count > 0 &&

                        string.Equals(
                            avion.identificador?.Trim(),
                            mensaje.identificador?.Trim(),
                            StringComparison.OrdinalIgnoreCase) &&

                        mensaje.ultimoTiempo >=
                            Math.Max(0, avion.planVuelo.atot - 60) &&

                        mensaje.ultimoTiempo <=
                            Math.Min(86400, avion.planVuelo.atot + 1200)
                    );

                    if (!esAvionP3)
                        continue;
                }

                if (!double.IsNaN(mensaje.latitud) &&
                    !double.IsNaN(mensaje.longitud))
                {
                    PointLatLng posicion =
                        new PointLatLng(
                            mensaje.latitud,
                            mensaje.longitud
                        );

                    // buscar el avión correspondiente para obtener sus datos actualizados
                    AvionSimulacion avionReal = null;

                    foreach (AvionSimulacion avion in AvionesActuales)
                    {
                        if (!string.IsNullOrEmpty(mensaje.direccion) &&
                            mensaje.direccion != "-1")
                        {
                            if (avion.direccion == mensaje.direccion)
                            {
                                avionReal = avion;
                                break;
                            }
                        }
                        else if (mensaje.trackNumber != -1)
                        {
                            if ((avion.direccion == "" ||
                                 avion.direccion == "-1") &&
                                avion.trackNumber == mensaje.trackNumber)
                            {
                                avionReal = avion;
                                break;
                            }
                        }
                    }

                    // asignar el color del marcador según el sistema de detección
                    Color colorAvion;

                    if (mensaje.detectadoRadar)
                    {
                        colorAvion = Color.Red;
                    }
                    else
                    {
                        colorAvion = Color.Green;
                    }

                    // obtener el rumbo del avión para orientar el marcador
                    double rumbo = mensaje.rumbo;

                    if (avionReal != null && !double.IsNaN(avionReal.rumbo))
                    {
                        rumbo = avionReal.rumbo;
                    }

                    if (double.IsNaN(rumbo))
                    {
                        rumbo = 0;
                    }

                    // crear el marcador del avión con su posición, rumbo y color
                    MarcadorAvion marcador =
                        new MarcadorAvion(
                            posicion,
                            rumbo,
                            colorAvion
                        );

                    marcador.Tag = mensaje;

                    // preparar la información que aparece al pasar el ratón sobre el avión
                    string fl = "N/A";

                    if (!double.IsNaN(mensaje.flightLevel))
                    {
                        fl = mensaje.flightLevel.ToString();
                    }

                    string sistema = "";

                    if (mensaje.detectadoRadar)
                    {
                        sistema = "Radar";
                    }
                    else if (mensaje.detectadoADSB)
                    {
                        sistema = "ADS-B";
                    }

                    marcador.ToolTipText =
                        "\n" +
                        mensaje.identificador +
                        "\nFL " +
                        fl +
                        "\n" +
                        sistema;

                    marcador.ToolTipMode =
                        MarkerTooltipMode.OnMouseOver;

                    // añadir el marcador a la capa de aviones
                    capaAviones.Markers.Add(marcador);
                }
            }

            // refrescar el mapa para mostrar los marcadores actualizados
            gMapControl1.Refresh();
        }

        private void GMapControl1_OnMarkerClick(GMapMarker item, MouseEventArgs e)
        {
            AvionSimulacion mensaje = item.Tag as AvionSimulacion;

            if (mensaje == null)
            {
                return;
            }


            // CLICK IZQUIERDO -> mostrar trayectoria
            if (e.Button == MouseButtons.Left)
            {
                // Buscamos el avión real, que contiene
                // toda la trayectoria acumulada
                AvionSimulacion avionTrayectoria =
                    BuscarAvionSeleccionado(mensaje);

                if (avionTrayectoria == null)
                {
                    return;
                }

                if (avionesTrayectoriaSeleccionados.Contains(avionTrayectoria))
                {
                    avionesTrayectoriaSeleccionados.Remove(avionTrayectoria);
                }
                else
                {
                    avionesTrayectoriaSeleccionados.Add(avionTrayectoria);
                }

                MostrarTrayectorias();
            }


            // CLICK DERECHO -> información del mensaje concreto
            else if (e.Button == MouseButtons.Right)
            {
                AvionSimulacion avionReal = BuscarAvionSeleccionado(mensaje);

                if (avionReal == null)
                    return;

                InformacionAvion ventana = new InformacionAvion();

                ventana.avion = avionReal;

                ventana.categoriaSeleccionada =
                    mensaje.detectadoRadar ? 48 : 21;

                ventana.Show(this);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)   //boton de reproducir/pausar
        {
            if (Reproduciendo == false)
            {
                Reproduciendo = true;
                pictureBox1.Image = Properties.Resources.botonPausa;
                timerSimulacion.Start();
            }
            else if (Reproduciendo == true)
            {
                Reproduciendo = false;
                timerSimulacion.Stop();
                pictureBox1.Image = Properties.Resources.botonReproducir;
            }
        }
        private void pictureBox2_Click(object sender, EventArgs e)   //boton de avanzar un segundo
        {
            AvanzarUnSegundo();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            if (ListaMensajesFiltrados.Count == 0)
                return;

            // No retroceder antes del inicio
            if (TiempoActual <= tiempoInicioSimulacion)
                return;

            // Retroceder un segundo
            double nuevoTiempo = TiempoActual - 1;

            // Reconstruir los aviones en ese instante
            ReconstruirAviones(nuevoTiempo);

            // Actualizar el mapa de calor
            ActualizarHeatmapDinamico(true);
        }
        private void ActualizarMensajesActuales()
        {
            // limpiar los mensajes visuales anteriores
            MensajesActuales.Clear();

            // recorrer los mensajes filtrados y obtener sus datos
            foreach (Mensaje mensaje in ListaMensajesFiltrados)
            {
                double tiempoMensaje = -1;

                string direccion = "";
                string identificador = "";

                double latitud = double.NaN;
                double longitud = double.NaN;
                double altitud = double.NaN;
                double flightLevel = double.NaN;
                double rumbo = double.NaN;

                double bps = double.NaN;
                double altitudPresionFt = double.NaN;
                double altitudQNHFt = double.NaN;
                double qnhUtilizado = double.NaN;

                int trackNumber = -1;

                bool esADSB = false;
                bool esRadar = false;

                double velocidad = double.NaN;
                string mode3A = "";

                // obtener los datos de los mensajes ads-b
                if (mensaje.categoria == 21)
                {
                    TiraDatosDecod021 mensaje21 =
                        (TiraDatosDecod021)mensaje.tira;

                    tiempoMensaje =
                        mensaje21.TimeReceptionPosition;

                    direccion =
                        mensaje21.TargetAddress;

                    identificador =
                        mensaje21.TargetIdentification;

                    latitud =
                        mensaje21.PosWGS84HighRes[0];

                    longitud =
                        mensaje21.PosWGS84HighRes[1];

                    flightLevel =
                        mensaje21.FlightLevel;

                    altitud = !double.IsNaN(mensaje21.altitudQNHFt)
                                 ? mensaje21.altitudQNHFt
                                 : mensaje21.altitudPresionFt;

                    bps = mensaje21.BPS;
                    altitudPresionFt = mensaje21.altitudPresionFt;
                    altitudQNHFt = mensaje21.altitudQNHFt;
                    qnhUtilizado = mensaje21.qnhUtilizado;

                    esADSB = true;

                    mode3A = mensaje21.Mode3ACode;
                }

                // obtener los datos de los mensajes radar y descartar los no válidos
                else if (mensaje.categoria == 48)
                {
                    TiraDatosDecod048 mensaje48 =
                        (TiraDatosDecod048)mensaje.tira;

                    bool trackSinIdentificar =
                        string.IsNullOrEmpty(mensaje48.AircrftAddrs) &&
                        string.IsNullOrEmpty(mensaje48.AircrftIddent) &&
                        mensaje48.TargetRep.TYP == "001";

                    if (mensaje48.TrckStatus.CNF == 1 ||
                        trackSinIdentificar)
                    {
                        continue;
                    }

                    tiempoMensaje =
                        mensaje48.TimeOfDay;

                    direccion = mensaje48.AircrftAddrs ?? "";

                    identificador =
                        mensaje48.AircrftIddent;

                    trackNumber =
                        mensaje48.TrackNum;

                    latitud =
                        mensaje48.PositionCorrectedLatLonAlt[0];

                    longitud =
                        mensaje48.PositionCorrectedLatLonAlt[1];

                    if (!double.IsNaN(mensaje48.FL.FL))
                    {
                        flightLevel = mensaje48.FL.FL;

                        bps = mensaje48.BPS;
                        altitudPresionFt = mensaje48.altitudPresionFt;
                        altitudQNHFt = mensaje48.altitudQNHFt;
                        qnhUtilizado = mensaje48.qnhUtilizado;

                        altitud = !double.IsNaN(altitudQNHFt)
                            ? altitudQNHFt
                            : altitudPresionFt;
                    }

                    esRadar = true;

                    if (mensaje48.tieneTrackVelocity)
                    {
                        velocidad = mensaje48.TrckVelPolRepr[0];
                        rumbo = mensaje48.TrckVelPolRepr[1];
                    }

                    mode3A = mensaje48.mode3.reply;
                }

                // seleccionar los mensajes recibidos durante los últimos 10 segundos
                if (tiempoMensaje <= TiempoActual &&
                    tiempoMensaje > TiempoActual - 10)
                {
                    if (double.IsNaN(latitud) ||
                        double.IsNaN(longitud))
                    {
                        continue;
                    }

                    // crear el objeto visual con los datos del mensaje
                    AvionSimulacion mensajeVisual =
                        new AvionSimulacion();

                    mensajeVisual.mensajeOriginal = mensaje;

                    mensajeVisual.direccion = direccion;
                    mensajeVisual.identificador = identificador;

                    mensajeVisual.trackNumber = trackNumber;

                    mensajeVisual.latitud = latitud;
                    mensajeVisual.longitud = longitud;

                    mensajeVisual.altitud = altitud;
                    mensajeVisual.flightLevel = flightLevel;
                    mensajeVisual.bps = bps;
                    mensajeVisual.altitudPresionFt = altitudPresionFt;
                    mensajeVisual.altitudQNHFt = altitudQNHFt;
                    mensajeVisual.qnhUtilizado = qnhUtilizado;

                    mensajeVisual.ultimoTiempo = tiempoMensaje;

                    mensajeVisual.detectadoADSB = esADSB;
                    mensajeVisual.detectadoRadar = esRadar;
                    mensajeVisual.rumbo = rumbo;

                    if (esADSB)
                    {
                        mensajeVisual.ultimoTiempoADSB =
                            tiempoMensaje;
                    }

                    if (esRadar)
                    {
                        mensajeVisual.ultimoTiempoRadar =
                            tiempoMensaje;
                    }

                    mensajeVisual.velocidad = velocidad;
                    mensajeVisual.mode3A = mode3A;

                    ActualizarMensajeVisual(mensajeVisual);
                }
            }

            // calcular el rumbo de los aviones que no tienen uno disponible
            foreach (AvionSimulacion avion in MensajesActuales)
            {
                if (!double.IsNaN(avion.rumbo))
                    continue;

                double mejorTiempo = double.MinValue;
                double latitudAnterior = double.NaN;
                double longitudAnterior = double.NaN;

                // buscar la posición anterior más reciente del mismo avión
                foreach (Mensaje mensaje in ListaMensajesFiltrados)
                {
                    double tiempo;
                    string direccion;
                    double latitud;
                    double longitud;

                    if (mensaje.categoria == 21)
                    {
                        TiraDatosDecod021 datos = (TiraDatosDecod021)mensaje.tira;

                        tiempo = datos.TimeReceptionPosition;
                        direccion = datos.TargetAddress;
                        latitud = datos.PosWGS84HighRes[0];
                        longitud = datos.PosWGS84HighRes[1];
                    }
                    else if (mensaje.categoria == 48)
                    {
                        TiraDatosDecod048 datos = (TiraDatosDecod048)mensaje.tira;

                        tiempo = datos.TimeOfDay;
                        direccion = datos.AircrftAddrs;
                        latitud = datos.PositionCorrectedLatLonAlt[0];
                        longitud = datos.PositionCorrectedLatLonAlt[1];
                    }
                    else
                    {
                        continue;
                    }

                    if (direccion != avion.direccion ||
                        tiempo >= avion.ultimoTiempo ||
                        tiempo <= mejorTiempo)
                    {
                        continue;
                    }

                    if (double.IsNaN(latitud) || double.IsNaN(longitud))
                        continue;

                    mejorTiempo = tiempo;
                    latitudAnterior = latitud;
                    longitudAnterior = longitud;
                }

                // calcular el rumbo a partir de las dos posiciones del avión
                if (!double.IsNaN(latitudAnterior))
                {
                    avion.rumbo = CalcularRumbo(
                        latitudAnterior,
                        longitudAnterior,
                        avion.latitud,
                        avion.longitud
                    );
                }
            }
        }

        private void ActualizarMensajeVisual(AvionSimulacion nuevoMensaje)
        {
            // buscar un mensaje anterior del mismo avión y sistema de detección
            AvionSimulacion mensajeAnterior = null;

            foreach (AvionSimulacion mensaje in MensajesActuales)
            {
                bool mismoAvion = false;

                // identificar el avión por su dirección o número de track
                if (!string.IsNullOrEmpty(nuevoMensaje.direccion) &&
                    nuevoMensaje.direccion != "-1")
                {
                    mismoAvion =
                        mensaje.direccion == nuevoMensaje.direccion;
                }
                else if (nuevoMensaje.trackNumber != -1)
                {
                    mismoAvion =
                        (mensaje.direccion == "" ||
                         mensaje.direccion == "-1") &&
                        mensaje.trackNumber == nuevoMensaje.trackNumber;
                }

                if (!mismoAvion)
                {
                    continue;
                }

                // comprobar que ambos mensajes pertenecen al mismo sistema
                bool mismoSistema =
                    (mensaje.detectadoADSB &&
                     nuevoMensaje.detectadoADSB)
                    ||
                    (mensaje.detectadoRadar &&
                     nuevoMensaje.detectadoRadar);

                if (mismoSistema)
                {
                    mensajeAnterior = mensaje;
                    break;
                }
            }

            // actualizar el mensaje anterior con los nuevos datos disponibles
            if (mensajeAnterior != null)
            {
                mensajeAnterior.direccion = nuevoMensaje.direccion;
                mensajeAnterior.identificador = nuevoMensaje.identificador;
                mensajeAnterior.trackNumber = nuevoMensaje.trackNumber;

                mensajeAnterior.latitud = nuevoMensaje.latitud;
                mensajeAnterior.longitud = nuevoMensaje.longitud;

                mensajeAnterior.mensajeOriginal = nuevoMensaje.mensajeOriginal;

                if (!double.IsNaN(nuevoMensaje.altitud))
                {
                    mensajeAnterior.altitud = nuevoMensaje.altitud;
                }

                if (!double.IsNaN(nuevoMensaje.flightLevel))
                {
                    mensajeAnterior.flightLevel = nuevoMensaje.flightLevel;
                }

                if (nuevoMensaje.detectadoADSB || nuevoMensaje.detectadoRadar)
                {
                    mensajeAnterior.bps = nuevoMensaje.bps;
                    mensajeAnterior.altitudPresionFt = nuevoMensaje.altitudPresionFt;
                    mensajeAnterior.altitudQNHFt = nuevoMensaje.altitudQNHFt;
                    mensajeAnterior.qnhUtilizado = nuevoMensaje.qnhUtilizado;
                }

                if (!double.IsNaN(nuevoMensaje.velocidad))
                {
                    mensajeAnterior.velocidad = nuevoMensaje.velocidad;
                }

                if (!double.IsNaN(nuevoMensaje.rumbo))
                {
                    mensajeAnterior.rumbo = nuevoMensaje.rumbo;
                }

                if (!string.IsNullOrWhiteSpace(nuevoMensaje.mode3A))
                {
                    mensajeAnterior.mode3A = nuevoMensaje.mode3A;
                }

                // actualizar el tiempo y el sistema de detección
                mensajeAnterior.ultimoTiempo = nuevoMensaje.ultimoTiempo;

                mensajeAnterior.detectadoADSB = nuevoMensaje.detectadoADSB;
                mensajeAnterior.detectadoRadar = nuevoMensaje.detectadoRadar;

                if (nuevoMensaje.detectadoADSB)
                {
                    mensajeAnterior.ultimoTiempoADSB =
                        nuevoMensaje.ultimoTiempoADSB;
                }

                if (nuevoMensaje.detectadoRadar)
                {
                    mensajeAnterior.ultimoTiempoRadar =
                        nuevoMensaje.ultimoTiempoRadar;
                }
            }
            else
            {
                // añadir el mensaje si no existe otro del mismo avión y sistema
                MensajesActuales.Add(nuevoMensaje);
            }
        }
        private string ObtenerClaveAvion(AvionSimulacion avion)
        {
            if (!string.IsNullOrEmpty(avion.direccion) &&
                avion.direccion != "-1")
            {
                return avion.direccion;
            }

            return "TRACK_" + avion.trackNumber;
        }

        private bool filtroBlancoPuro = false;
        private bool filtroTransFijo = false;
        private bool filtroGround = false;

        private bool filtroCat21 = true;
        private bool filtroCat48 = true;

        private bool filtroTrayectoria = false;
        private double filtroTrayectoriaMin = 0;
        private double filtroTrayectoriaMax = 360;

        private bool filtroAltura = false;
        private double filtroAlturaMin = 0;
        private double filtroAlturaMax = 600;

        private void aplicarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormFiltro ventana = new FormFiltro(
                filtroBlancoPuro,
                filtroTransFijo,
                filtroGround,
                filtroCat21,
                filtroCat48,
                filtroTrayectoria,
                filtroTrayectoriaMin,
                filtroTrayectoriaMax,
                filtroAltura,
                filtroAlturaMin,
                filtroAlturaMax
            );

            if (ventana.ShowDialog() == DialogResult.OK)
            {
                // Guardar los valores seleccionados
                filtroBlancoPuro = ventana.BlancoPuro;
                filtroTransFijo = ventana.transFijo;
                filtroGround = ventana.ground;

                filtroCat21 = ventana.Cat21;
                filtroCat48 = ventana.Cat48;

                filtroTrayectoria = ventana.FiltrarTrayectoria;
                filtroTrayectoriaMin = ventana.trayectoriaMin;
                filtroTrayectoriaMax = ventana.trayectoriaMax;

                // guardar los valores del filtro de nivel de vuelo
                filtroAltura = ventana.FiltrarAltura;
                filtroAlturaMin = ventana.AlturaMin;
                filtroAlturaMax = ventana.AlturaMax;

                // Aplicar filtros
                Filtros gestorFiltros = new Filtros();
                //MessageBox.Show("1 - antes de aplicar filtros");

                ListaMensajesFiltrados = gestorFiltros.AplicarFiltros(
                    ListaMensajes,
                    filtroCat48,
                    filtroCat21,
                    filtroBlancoPuro,
                    filtroTransFijo,
                    filtroTrayectoria,
                    filtroTrayectoriaMin,
                    filtroTrayectoriaMax,
                    filtroGround,
                    filtroAltura,
                    filtroAlturaMin,
                    filtroAlturaMax
                );

                //MessageBox.Show("2 - filtros aplicados");

                ReconstruirAvionesFiltrados();

                //MessageBox.Show("3 - aviones reconstruidos");

                InvalidarHeatmap();

                //MessageBox.Show("4 - heatmap invalidado");

                PrepararPuntosHeatmap();
                ActualizarHeatmapDinamico(true);
                AplicarFiltroMensajesP3();

                //MessageBox.Show("5 - terminado");
            }
        }

        private void limpiarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Restablecer los filtros
            RestablecerFiltros();

            Filtros gestorFiltros = new Filtros();

            ListaMensajesFiltrados =
                gestorFiltros.PrepararMensajes(ListaMensajes);

            // Reconstruir los aviones en el tiempo actual
            ReconstruirAvionesFiltrados();

            // Actualizar el mapa de calor
            InvalidarHeatmap();
            PrepararPuntosHeatmap();
            ActualizarHeatmapDinamico(true);
            AplicarFiltroMensajesP3();

            gMapControl1.Refresh();
        }

        private void RestablecerFiltros()
        {
            filtroBlancoPuro = false;
            filtroTransFijo = false;
            filtroGround = false;

            filtroCat21 = true;
            filtroCat48 = true;

            filtroTrayectoria = false;
            filtroTrayectoriaMin = 0;
            filtroTrayectoriaMax = 360;

            filtroAltura = false;
            filtroAlturaMin = 0;
            filtroAlturaMax = 999;
        }

        private void guardarEnCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // configurar el explorador para guardar el archivo csv
            SaveFileDialog guardar = new SaveFileDialog();

            guardar.Filter = "Archivo CSV (*.csv)|*.csv";
            guardar.DefaultExt = "csv";
            guardar.FileName = "ASTERIX.csv";

            // exportar los mensajes filtrados al archivo seleccionado
            if (guardar.ShowDialog() == DialogResult.OK)
            {
                int cat21 = ListaMensajesFiltrados.Count(m => m.categoria == 21);
                int cat48 = ListaMensajesFiltrados.Count(m => m.categoria == 48);

                ExportarCSV exportador = new ExportarCSV();

                exportador.Exportar(
                    ListaMensajesFiltrados,
                    guardar.FileName
                );

                MessageBox.Show("CSV guardado correctamente");
            }
        }

        private AvionSimulacion BuscarAvionSeleccionado(AvionSimulacion avionAnterior)
        {
            // comprobar que el avión seleccionado existe
            if (avionAnterior == null)
            {
                return null;
            }

            // buscar el avión en la lista actual mediante su dirección o número de track
            foreach (AvionSimulacion avion in AvionesActuales)
            {
                if (!string.IsNullOrEmpty(avionAnterior.direccion) &&
                    avionAnterior.direccion != "-1")
                {
                    if (avion.direccion == avionAnterior.direccion)
                    {
                        return avion;
                    }
                }

                else
                {
                    if (avion.trackNumber == avionAnterior.trackNumber)
                    {
                        return avion;
                    }
                }
            }

            // devolver null si el avión ya no se encuentra en la simulación
            return null;
        }

        private void MostrarTrayectorias()
        {
            // eliminar las trayectorias anteriores del mapa
            capaTrayectorias.Routes.Clear();

            // seleccionar todos los aviones o únicamente los seleccionados
            List<AvionSimulacion> avionesMostrar = mostrarTodasTrayectorias
                ? AvionesActuales
                : avionesTrayectoriaSeleccionados;

            foreach (AvionSimulacion avion in avionesMostrar)
            {
                if (avion.trayectoria == null || avion.trayectoria.Count < 2)
                {
                    continue;
                }

                // convertir las posiciones del avión en puntos del mapa
                List<PointLatLng> puntos = new List<PointLatLng>();

                foreach (double[] punto in avion.trayectoria)
                {
                    puntos.Add(new PointLatLng(punto[0], punto[1]));
                }

                // crear la trayectoria del avión y dibujarla en azul
                GMapRoute ruta = new GMapRoute(
                    puntos,
                    "Trayectoria " + avion.identificador
                );

                ruta.Stroke = new Pen(Color.Blue, 2);

                capaTrayectorias.Routes.Add(ruta);
            }

            // actualizar el mapa para mostrar las trayectorias
            gMapControl1.Refresh();
        }

        private void EliminarAvionesInactivos()
        {
            // Eliminar mensajes antiguos del mapa
            MensajesActuales.RemoveAll(mensaje =>
                TiempoActual - mensaje.ultimoTiempo >= 10
            );

            // Eliminar aviones que llevan 10 segundos sin enviar mensajes
            AvionesActuales.RemoveAll(avion =>
                TiempoActual - avion.ultimoTiempo >= 10
            );
        }

        private void buttonSimularDesdeHora_Click(object sender, EventArgs e)
        {
            //posibles errores al poner la hora
            if (!int.TryParse(textBoxHora.Text, out int horas) ||
                !int.TryParse(textBoxMinuto.Text, out int minutos) ||
                !int.TryParse(textBoxSegundo.Text, out int segundos))
            {
                MessageBox.Show("Introduce valores numéricos válidos.");
                return;
            }

            if (horas < 0 || horas > 23 ||
                minutos < 0 || minutos > 59 ||
                segundos < 0 || segundos > 59)
            {
                MessageBox.Show("La hora introducida no es válida.");
                return;
            }

            if (ListaMensajesFiltrados.Count == 0)
            {
                MessageBox.Show("Primero debes cargar un archivo ASTERIX.");
                return;
            }

            double nuevoTiempo = horas * 3600 + minutos * 60 + segundos;

            if (nuevoTiempo < tiempoInicioSimulacion ||
                nuevoTiempo > tiempoFinSimulacion)
            {
                MessageBox.Show("La hora está fuera del intervalo de la simulación.");
                return;
            }

            bool estabaReproduciendo = Reproduciendo;

            // Actualizar la barra de reproducción
            trackBarTiempo.Value = Math.Max(
                trackBarTiempo.Minimum,
                Math.Min(
                    trackBarTiempo.Maximum,
                    (int)(nuevoTiempo - tiempoInicioSimulacion)
                )
            );

            barraTiempoNueva.TiempoValor = trackBarTiempo.Value;

            // Aprovechar el método existente para actualizar mapa y datos
            trackBarTiempo_Scroll(sender, EventArgs.Empty);

            // Mostrar la hora introducida
            textBoxHora.Text = horas.ToString("00");
            textBoxMinuto.Text = minutos.ToString("00");
            textBoxSegundo.Text = segundos.ToString("00");

            // Mantener la reproducción si estaba activa
            if (estabaReproduciendo)
            {
                Reproduciendo = true;
                pictureBox1.Image = Properties.Resources.botonPausa;
                timerSimulacion.Start();
            }
        }

        private void mostrarDatosEnCeldasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridAviones.BringToFront();
            dataGridAviones.Visible = !dataGridAviones.Visible;
            // actualizar la barra después de mostrar la tabla
            if (dataGridAviones.Visible)
            {
                barraDataGrid.BringToFront();
            }

            barraDataGrid.Actualizar();
            mostrarDatosEnCeldasToolStripMenuItem.Checked = dataGridAviones.Visible;
        }

        private void trayectoriasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mostrarTodasTrayectorias = !mostrarTodasTrayectorias;

            trayectoriasToolStripMenuItem.Checked =
                mostrarTodasTrayectorias;

            if (mostrarTodasTrayectorias)
            {
                // Mostrar todas las trayectorias
                MostrarTrayectorias();
            }
            else
            {
                // Quitar todas las trayectorias
                avionesTrayectoriaSeleccionados.Clear();
                capaTrayectorias.Routes.Clear();
                gMapControl1.Refresh();
            }
        }

        private void mostrarHeatmapCompletoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mostrarHeatmapDinamico = false;
            mostrarHeatmapDinamicoToolStripMenuItem.Checked = false;

            mostrarHeatmap = !mostrarHeatmap;
            mostrarHeatmapCompletoToolStripMenuItem.Checked = mostrarHeatmap;
            if (mostrarHeatmap)
            {
                CalcularHeatmapCompleto();
            }
            capaHeatmap.IsVisibile = mostrarHeatmap;
            gMapControl1.Refresh();

            ActualizarLeyendaHeatmap();
        }


        private void CalcularHeatmapCompleto()
        {
            // eliminar los datos anteriores del mapa de calor
            capaHeatmap.Markers.Clear();

            // definir el tamaño de las celdas del mapa de calor
            const double tamanoCelda = 0.0005;

            // guardar los aviones distintos que han pasado por cada celda
            Dictionary<(int, int), HashSet<string>> densidades =
                new Dictionary<(int, int), HashSet<string>>();

            // recorrer los mensajes filtrados y obtener sus posiciones
            foreach (Mensaje mensaje in ListaMensajesFiltrados)
            {
                double latitud;
                double longitud;
                string identificadorAvion;

                if (mensaje.categoria == 21)
                {
                    TiraDatosDecod021 datos =
                        (TiraDatosDecod021)mensaje.tira;

                    if (datos.PosWGS84HighRes == null ||
                        datos.PosWGS84HighRes.Count < 2)
                        continue;

                    latitud = datos.PosWGS84HighRes[0];
                    longitud = datos.PosWGS84HighRes[1];

                    identificadorAvion = datos.TargetAddress;

                    if (string.IsNullOrWhiteSpace(identificadorAvion))
                        identificadorAvion = datos.TargetIdentification;
                }
                else if (mensaje.categoria == 48)
                {
                    TiraDatosDecod048 datos =
                        (TiraDatosDecod048)mensaje.tira;

                    // descartar los mensajes radar no confirmados o sin identificar
                    bool trackSinIdentificar =
                        string.IsNullOrEmpty(datos.AircrftAddrs) &&
                        string.IsNullOrEmpty(datos.AircrftIddent) &&
                        datos.TargetRep.TYP == "001";

                    if (datos.TrckStatus.CNF == 1 || trackSinIdentificar)
                        continue;

                    if (datos.PositionCorrectedLatLonAlt == null ||
                        datos.PositionCorrectedLatLonAlt.Count < 2)
                        continue;

                    latitud = datos.PositionCorrectedLatLonAlt[0];
                    longitud = datos.PositionCorrectedLatLonAlt[1];

                    identificadorAvion = datos.AircrftAddrs;

                    if (string.IsNullOrWhiteSpace(identificadorAvion))
                        identificadorAvion = datos.AircrftIddent;
                }
                else
                {
                    continue;
                }

                // descartar las posiciones no válidas o sin identificación
                if (double.IsNaN(latitud) ||
                    double.IsNaN(longitud) ||
                    double.IsInfinity(latitud) ||
                    double.IsInfinity(longitud))
                    continue;

                if (string.IsNullOrWhiteSpace(identificadorAvion))
                    continue;

                // calcular la celda correspondiente a la posición del avión
                int fila = (int)Math.Floor(latitud / tamanoCelda);
                int columna = (int)Math.Floor(longitud / tamanoCelda);

                var clave = (fila, columna);

                // crear la celda si todavía no existe
                if (!densidades.TryGetValue(clave, out HashSet<string> aviones))
                {
                    aviones = new HashSet<string>();
                    densidades.Add(clave, aviones);
                }

                // registrar el avión sin contar varias veces el mismo en una celda
                aviones.Add(identificadorAvion);
            }

            // dibujar el mapa de calor con las densidades calculadas
            DibujarHeatmap(densidades);
        }

        private void DibujarHeatmap(
    Dictionary<(int, int), HashSet<string>> densidades)
        {
            capaHeatmap.Markers.Clear();

            if (densidades.Count == 0)
            {
                ActualizarNumerosLeyenda(0);
                gMapControl1.Refresh();
                return;
            }

            // Máximo real de aviones distintos en una celda
            int maximo = densidades.Values.Max(aviones => aviones.Count);

            ActualizarNumerosLeyenda(maximo);

            foreach (var celda in densidades)
            {
                int cantidad = celda.Value.Count;

                if (cantidad == 0)
                    continue;

                double intensidad = maximo > 1
                    ? (double)(cantidad - 1) / (maximo - 1)
                    : 0;

                Color color;

                if (intensidad < 0.33)
                {
                    double t = intensidad / 0.33;

                    color = Color.FromArgb(
                        110, (int)(255 * t), 220, 0);
                }
                else if (intensidad < 0.66)
                {
                    double t = (intensidad - 0.33) / 0.33;

                    color = Color.FromArgb(
                        130, 255, (int)(220 - 100 * t), 0);
                }
                else
                {
                    double t = (intensidad - 0.66) / 0.34;

                    color = Color.FromArgb(
                        160, 255, (int)(120 * (1 - t)), 0);
                }

                double latMin = celda.Key.Item1 * tamanoCeldaHeatmap;
                double lonMin = celda.Key.Item2 * tamanoCeldaHeatmap;

                CeldaHeatmap marcador = new CeldaHeatmap(
                    latMin,
                    latMin + tamanoCeldaHeatmap,
                    lonMin,
                    lonMin + tamanoCeldaHeatmap,
                    color
                );

                capaHeatmap.Markers.Add(marcador);
            }

            gMapControl1.Refresh();
        }

        private void InvalidarHeatmap()
        {
            if (capaHeatmap == null)
                return;

            capaHeatmap.Markers.Clear();

            if (mostrarHeatmap)
                CalcularHeatmapCompleto();
        }

        private void ActualizarHeatmapDinamico(bool forzar = false)
        {
            if (!mostrarHeatmapDinamico)
                return;

            if (!forzar &&
                ultimoTiempoHeatmap >= 0 &&
                TiempoActual >= ultimoTiempoHeatmap &&
                TiempoActual - ultimoTiempoHeatmap < intervaloHeatmap)
            {
                return;
            }
            CalcularDensidadesDinamicas();
            DibujarHeatmap(densidadesDinamicas);

            ultimoTiempoHeatmap = TiempoActual;
            ActualizarLeyendaHeatmap();
        }


        private class PuntoHeatmap
        {
            public double tiempo;
            public double latitud;
            public double longitud;
            public string identificador;
        }
        private List<PuntoHeatmap> puntosHeatmap = new List<PuntoHeatmap>();
        private Dictionary<(int, int), HashSet<string>> densidadesDinamicas = new Dictionary<(int, int), HashSet<string>>();
        private int siguientePuntoHeatmap = 0;
        private const double tamanoCeldaHeatmap = 0.0005;


        private void PrepararPuntosHeatmap()
        {
            // eliminar los puntos anteriores del mapa de calor
            puntosHeatmap.Clear();

            // recorrer los mensajes filtrados y obtener sus datos
            foreach (Mensaje mensaje in ListaMensajesFiltrados)
            {
                double tiempo;
                double latitud;
                double longitud;
                string identificador;

                // obtener los datos de los mensajes ads-b
                if (mensaje.categoria == 21)
                {
                    TiraDatosDecod021 datos =
                        (TiraDatosDecod021)mensaje.tira;

                    if (datos.PosWGS84HighRes == null ||
                        datos.PosWGS84HighRes.Count < 2)
                        continue;

                    tiempo = datos.TimeReceptionPosition;
                    latitud = datos.PosWGS84HighRes[0];
                    longitud = datos.PosWGS84HighRes[1];

                    identificador = datos.TargetAddress;

                    if (string.IsNullOrWhiteSpace(identificador))
                        identificador = datos.TargetIdentification;
                }

                // obtener los datos de los mensajes radar y descartar los no válidos
                else if (mensaje.categoria == 48)
                {
                    TiraDatosDecod048 datos =
                        (TiraDatosDecod048)mensaje.tira;

                    bool trackSinIdentificar =
                        string.IsNullOrEmpty(datos.AircrftAddrs) &&
                        string.IsNullOrEmpty(datos.AircrftIddent) &&
                        datos.TargetRep.TYP == "001";

                    if (datos.TrckStatus.CNF == 1 || trackSinIdentificar)
                        continue;

                    if (datos.PositionCorrectedLatLonAlt == null ||
                        datos.PositionCorrectedLatLonAlt.Count < 2)
                        continue;

                    tiempo = datos.TimeOfDay;
                    latitud = datos.PositionCorrectedLatLonAlt[0];
                    longitud = datos.PositionCorrectedLatLonAlt[1];

                    identificador = datos.AircrftAddrs;

                    if (string.IsNullOrWhiteSpace(identificador))
                        identificador = datos.AircrftIddent;
                }
                else
                {
                    continue;
                }

                // descartar los mensajes con tiempos, posiciones o identificadores no válidos
                if (double.IsNaN(tiempo) ||
                    double.IsInfinity(tiempo) ||
                    tiempo < 0 ||
                    double.IsNaN(latitud) ||
                    double.IsInfinity(latitud) ||
                    double.IsNaN(longitud) ||
                    double.IsInfinity(longitud) ||
                    string.IsNullOrWhiteSpace(identificador))
                {
                    continue;
                }

                // guardar los puntos válidos para generar el mapa de calor
                puntosHeatmap.Add(new PuntoHeatmap
                {
                    tiempo = tiempo,
                    latitud = latitud,
                    longitud = longitud,
                    identificador = identificador
                });
            }

            // ordenar los puntos cronológicamente
            puntosHeatmap.Sort((a, b) =>
                a.tiempo.CompareTo(b.tiempo));

            // reiniciar los datos del mapa de calor dinámico
            densidadesDinamicas.Clear();
            siguientePuntoHeatmap = 0;
            ultimoTiempoHeatmap = -1;
        }

        private void CalcularDensidadesDinamicas()
        {
            // Si hemos retrocedido, reconstruir desde el principio
            if (ultimoTiempoHeatmap >= 0 &&
                TiempoActual < ultimoTiempoHeatmap)
            {
                densidadesDinamicas.Clear();
                siguientePuntoHeatmap = 0;
            }

            // Incorporar solo las posiciones hasta TiempoActual
            while (siguientePuntoHeatmap < puntosHeatmap.Count)
            {
                PuntoHeatmap punto =
                    puntosHeatmap[siguientePuntoHeatmap];

                if (punto.tiempo > TiempoActual)
                    break;

                int fila = (int)Math.Floor(
                    punto.latitud / tamanoCeldaHeatmap);

                int columna = (int)Math.Floor(
                    punto.longitud / tamanoCeldaHeatmap);

                var clave = (fila, columna);

                if (!densidadesDinamicas.TryGetValue(
                    clave, out HashSet<string> aviones))
                {
                    aviones = new HashSet<string>();
                    densidadesDinamicas.Add(clave, aviones);
                }

                aviones.Add(punto.identificador);

                siguientePuntoHeatmap++;
            }
        }


        private void mostrarHeatmapDinamicoToolStripMenuItem_Click(object sender, EventArgs e)
        {

            mostrarHeatmapDinamico = !mostrarHeatmapDinamico;

            if (mostrarHeatmapDinamico)
            {
                // Desactivar el modo completo
                mostrarHeatmap = false;
                mostrarHeatmapCompletoToolStripMenuItem.Checked = false;

                mostrarHeatmapDinamicoToolStripMenuItem.Checked = true;

                capaHeatmap.IsVisibile = true;

                PrepararPuntosHeatmap();
                ActualizarHeatmapDinamico(true);
            }
            else
            {
                mostrarHeatmapDinamicoToolStripMenuItem.Checked = false;

                capaHeatmap.Markers.Clear();
                capaHeatmap.IsVisibile = false;

                ultimoTiempoHeatmap = -1;
            }

            gMapControl1.Refresh();
        }
        private void AjustarMapa()
        {
            int margenDerecho = 25;
            int margenInferior = 230;

            gMapControl1.Width =
                this.ClientSize.Width - gMapControl1.Left - margenDerecho;

            gMapControl1.Height =
                this.ClientSize.Height - gMapControl1.Top - margenInferior;

            PosicionarLeyendaHeatmap();
        }

        private void trackBarTiempo_MouseUp(object sender, MouseEventArgs e)
        {
            // Guardar si la simulación estaba reproduciéndose
            bool estabaReproduciendo = Reproduciendo;

            // Actualizar la simulación al tiempo seleccionado
            trackBarTiempo_Scroll(sender, EventArgs.Empty);

            // Recuperar el estado de reproducción
            if (estabaReproduciendo)
            {
                Reproduciendo = true;
                pictureBox1.Image = Properties.Resources.botonPausa;
                timerSimulacion.Start();
            }
        }
        private void ActualizarHoraTextBox()
        {
            // No sobrescribir la hora mientras el usuario la está editando
            if (textBoxHora.Focused ||
                textBoxMinuto.Focused ||
                textBoxSegundo.Focused)
            {
                return;
            }

            TimeSpan hora = TimeSpan.FromSeconds(TiempoActual);

            textBoxHora.Text = ((int)hora.TotalHours).ToString("00");
            textBoxMinuto.Text = hora.Minutes.ToString("00");
            textBoxSegundo.Text = hora.Seconds.ToString("00");
        }
        private void HoraTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                buttonSimularDesdeHora_Click(sender, EventArgs.Empty);
                this.ActiveControl = null;
            }
        }

        public bool controles = false;
        private void moToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (controles == false)
            {
                pictureBoxContorles.BringToFront();
                pictureBoxContorles.Visible = true;
                controles = true;
                moToolStripMenuItem.Checked = true;
            }
            else
            {
                pictureBoxContorles.Visible = false;
                controles = false;
                moToolStripMenuItem.Checked = false;
            }
        }

        private void pictureBoxContorles_Click(object sender, EventArgs e)
        {
            controles = false;
            pictureBoxContorles.Visible = false;
            moToolStripMenuItem.Checked = false;
        }

        private void ConfigurarScrollBarDataGrid()
        {
            // crear la barra personalizada
            barraDataGrid = new ScrollBarASTERIX();

            // colocarla junto al borde derecho del datagrid
            barraDataGrid.Parent = dataGridAviones.Parent;

            barraDataGrid.Height = dataGridAviones.Height;
            barraDataGrid.Left = dataGridAviones.Right - barraDataGrid.Width;
            barraDataGrid.Top = dataGridAviones.Top;

            barraDataGrid.Anchor = AnchorStyles.Top |
                                   AnchorStyles.Bottom |
                                   AnchorStyles.Right;

            // vincular la barra con la tabla
            barraDataGrid.Vincular(dataGridAviones);
            // actualizar la barra cuando se muestre u oculte el datagrid
            dataGridAviones.VisibleChanged += (sender, e) =>
            {
                barraDataGrid.Actualizar();
            };
            barraDataGrid.BringToFront();

            // mantener su posición si cambia el tamaño de la tabla
            dataGridAviones.SizeChanged += (sender, e) =>
            {
                barraDataGrid.Left =
                    dataGridAviones.Right - barraDataGrid.Width;

                barraDataGrid.Top = dataGridAviones.Top;
                barraDataGrid.Height = dataGridAviones.Height;

                barraDataGrid.Actualizar();
            };

            dataGridAviones.LocationChanged += (sender, e) =>
            {
                barraDataGrid.Left =
                    dataGridAviones.Right - barraDataGrid.Width;

                barraDataGrid.Top = dataGridAviones.Top;
            };
        }

        private void guiaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form guia = new MenuTutorial();
            guia.Show();
        }

        private void PosicionarLeyendaHeatmap()
        {
            panelLeyendaHeatmap.Location = new Point(
                gMapControl1.Right - panelLeyendaHeatmap.Width - 15,
                gMapControl1.Bottom - panelLeyendaHeatmap.Height - 15
            );

            panelLeyendaHeatmap.BringToFront();
        }

        private void CrearEscalaHeatmap()
        {
            Bitmap imagen = new Bitmap(
                pictureBoxEscalaHeatmap.Width,
                pictureBoxEscalaHeatmap.Height
            );

            using (Graphics g = Graphics.FromImage(imagen))
            {
                for (int x = 0; x < imagen.Width; x++)
                {
                    double intensidad = (double)x / (imagen.Width - 1);

                    Color color;

                    if (intensidad < 0.33)
                    {
                        double t = intensidad / 0.33;

                        color = Color.FromArgb(
                            (int)(255 * t), 220, 0);
                    }
                    else if (intensidad < 0.66)
                    {
                        double t = (intensidad - 0.33) / 0.33;

                        color = Color.FromArgb(
                            255, (int)(220 - 100 * t), 0);
                    }
                    else
                    {
                        double t = (intensidad - 0.66) / 0.34;

                        color = Color.FromArgb(
                            255, (int)(120 * (1 - t)), 0);
                    }

                    using (Pen lapiz = new Pen(color))
                    {
                        g.DrawLine(
                            lapiz,
                            x, 0,
                            x, imagen.Height
                        );
                    }
                }
            }

            Image imagenAnterior = pictureBoxEscalaHeatmap.Image;
            pictureBoxEscalaHeatmap.Image = imagen;
            imagenAnterior?.Dispose();
        }

        private void ActualizarLeyendaHeatmap()
        {
            panelLeyendaHeatmap.Visible =
                mostrarHeatmap || mostrarHeatmapDinamico;

            if (panelLeyendaHeatmap.Visible)
            {
                PosicionarLeyendaHeatmap();
            }
        }

        private void ActualizarNumerosLeyenda(int maximo)
        {
            if (maximo == 0)
            {
                labelHeatmap0.Text = "0";
                labelHeatmap25.Text = "0";
                labelHeatmap50.Text = "0";
                labelHeatmap75.Text = "0";
                labelHeatmap100.Text = "0";
                return;
            }

            labelHeatmap0.Text = "1";

            labelHeatmap25.Text =
                (1 + (int)Math.Round((maximo - 1) * 0.25)).ToString();

            labelHeatmap50.Text =
                (1 + (int)Math.Round((maximo - 1) * 0.50)).ToString();

            labelHeatmap75.Text =
                (1 + (int)Math.Round((maximo - 1) * 0.75)).ToString();

            labelHeatmap100.Text = maximo.ToString();
        }

        //==============================================================================
        //==============================================================================
        //--------------------------PARTE DEL PROYECTO 3--------------------------------
        //==============================================================================
        //==============================================================================


        private void cargarPlanDeVueloToolStripMenuItem_Click(
            object sender, EventArgs e)
        {
            using (OpenFileDialog explorador = new OpenFileDialog())
            {
                explorador.Title = "Cargar planes de vuelo P3";
                explorador.Filter = "Archivos Excel (*.xlsx)|*.xlsx";

                if (explorador.ShowDialog() != DialogResult.OK)
                    return;

                try
                {
                    // Cargar los planes de vuelo
                    LectorPlanesVueloP3 lector =
                        new LectorPlanesVueloP3();

                    planesVueloP3 = lector.LeerExcel(
                        explorador.FileName);

                    IdentificarSIDP3();
                    int reconocidas = planesVueloP3.Count(p =>
    p.procDesp != "-" &&
    !string.IsNullOrWhiteSpace(p.procDesp));

                    int inferidas = planesVueloP3.Count(p =>
                        p.procDesp == "-" &&
                        p.sidDetectada != "-");

                    int sinIdentificar = planesVueloP3.Count(p =>
                        p.sidDetectada == "-");

                    string resultado = "";

                    foreach (PlanVueloP3 plan in planesVueloP3
                        .Where(p => p.pistaDespegue == "24L" ||
                                    p.pistaDespegue == "06R")
                        .Take(50))
                    {
                        resultado +=
                            plan.indicativo +
                            " | Pista: " + plan.pistaDespegue +
                            " | Original: " + plan.procDesp +
                            " | Detectada: " + plan.sidDetectada +
                            " | Grupo: " + plan.grupoSID +
                            "\n";
                    }

                    MessageBox.Show(
                        "SID originales reconocidas: " + reconocidas +
                        "\nSID inferidas: " + inferidas +
                        "\nSin identificar: " + sinIdentificar +
                        "\n\n" + resultado,
                        "Proyecto 3 - Identificación SID"
                    );

                    // Asociar los planes y los precedentes
                    AsignarPlanesVueloP3();
                    AsignarAvionesPrecedentesP3();
                    mensajesVisiblesP3 = avionesP3
                        .SelectMany(a => a.mensajesRadarP3)
                        .ToHashSet();

                    mostrarSoloAvionesP3 = true;
                    
                    // Aplicar todos los filtros y reconstruir la simulación
                    ActualizarFiltros();

                    int separacionesCalculadas = 0;
                    int separacionesNoCalculadas = 0;
                    int minimasCalculadas = 0;

                    string resultadoSeparaciones = "";

                    foreach (AvionSimulacion avion in avionesP3
                        .Where(a => a.mensajesRadarP3.Count > 0)
                        .OrderBy(a => a.planVuelo.atot))
                    {
                        // Calcular separación TWR
                        avion.separacionTWR = CalcularSeparacionTWRP3(avion);

                        // Calcular separación mínima TMA
                        avion.separacionMinimaTMA =
                            CalcularSeparacionMinimaTMAP3(
                                avion,
                                out double tiempoMinimo,
                                out int comparaciones,
                                out double separacionFinal);

                        avion.tiempoSeparacionMinimaTMA = tiempoMinimo;

                        // Contadores
                        if (double.IsFinite(avion.separacionTWR))
                            separacionesCalculadas++;
                        else
                            separacionesNoCalculadas++;

                        if (double.IsFinite(avion.separacionMinimaTMA))
                            minimasCalculadas++;

                        // Obtener el indicativo del precedente
                        string precedente = avion.avionPrecedente != null
                            ? avion.avionPrecedente.identificador
                            : "NINGUNO";

                        // Preparar resultados para mostrar
                        string distanciaTWR = double.IsFinite(avion.separacionTWR)
                            ? avion.separacionTWR.ToString("F3") + " NM"
                            : "N/A";

                        string distanciaTMA = double.IsFinite(avion.separacionMinimaTMA)
                            ? avion.separacionMinimaTMA.ToString("F3") + " NM"
                            : "N/A";

                        string horaMinima = double.IsFinite(tiempoMinimo)
                            ? TimeSpan.FromSeconds(tiempoMinimo)
                                .ToString(@"hh\:mm\:ss")
                            : "N/A";

                        resultadoSeparaciones +=
                            avion.identificador +
                            " | TWR: " + distanciaTWR +
                            " | TMA: " + distanciaTMA +
                            " | Comparaciones: " + comparaciones +
                            " | Última: " +
                            (double.IsFinite(separacionFinal)
                                ? separacionFinal.ToString("F3") + " NM"
                                : "N/A") +
                            "\n";
                                            }

                    MessageBox.Show(
                        "Separaciones TWR calculadas: " +
                        separacionesCalculadas +
                        "\nSeparaciones TWR no calculadas: " +
                        separacionesNoCalculadas +
                        "\nSeparaciones mínimas TMA calculadas: " +
                        minimasCalculadas +
                        "\n\n" + resultadoSeparaciones,
                        "Proyecto 3 - Separaciones TWR y TMA"
                    );
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Error al cargar los planes de vuelo:\n" +
                        ex.Message,
                        "Proyecto 3"
                    );
                }
            }
        }

        private void AsignarPlanesVueloP3()
        {
            avionesP3.Clear();

            // Agrupar los mensajes CAT48 por indicativo.
            // Utilizamos los mensajes ya decodificados del P2.
            var mensajesPorIndicativo = ListaMensajes
                .Where(m => m.categoria == 48 &&
                            m.tira is TiraDatosDecod048)
                .GroupBy(
                    m => ((TiraDatosDecod048)m.tira)
                            .AircrftIddent?.Trim() ?? "",
                    StringComparer.OrdinalIgnoreCase
                )
                .Where(g => !string.IsNullOrWhiteSpace(g.Key))
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderBy(m =>
                        ((TiraDatosDecod048)m.tira).TimeOfDay)
                        .ToList(),
                    StringComparer.OrdinalIgnoreCase
                );

            // Recorrer los planes del Excel
            foreach (PlanVueloP3 plan in planesVueloP3)
            {
                AvionSimulacion avion = new AvionSimulacion
                {
                    identificador = plan.indicativo,
                    planVuelo = plan
                };

                // Buscar mensajes con el mismo indicativo
                if (mensajesPorIndicativo.TryGetValue(
                    plan.indicativo.Trim(),
                    out List<Mensaje> mensajes))
                {
                    // Ventana temporal provisional:
                    // 60 s antes y 20 min después del ATOT
                    double inicio = Math.Max(0, plan.atot - 60);
                    double fin = Math.Min(86400, plan.atot + 1200);

                    avion.mensajesRadarP3 = mensajes
                        .Where(m =>
                        {
                            double tiempo =
                                ((TiraDatosDecod048)m.tira).TimeOfDay;

                            return double.IsFinite(tiempo) &&
                                   tiempo >= inicio &&
                                   tiempo <= fin;
                        })
                        .ToList();
                }

                // Conservar también los planes sin detecciones
                avionesP3.Add(avion);
            }
        }

        private void AsignarAvionesPrecedentesP3()
        {
            // Limpiar las relaciones anteriores
            foreach (AvionSimulacion avion in avionesP3)
            {
                avion.avionPrecedente = null;
            }

            // Separar los despegues por pista
            string[] pistas = { "24L", "06R" };

            foreach (string pista in pistas)
            {
                // Obtener los aviones de esa pista
                // y ordenarlos según su hora de despegue
                List<AvionSimulacion> despegues = avionesP3
                    .Where(a =>
                        a.planVuelo != null &&
                        a.planVuelo.pistaDespegue == pista &&
                        double.IsFinite(a.planVuelo.atot))
                    .OrderBy(a => a.planVuelo.atot)
                    .ToList();

                // Asignar a cada avión el despegue anterior
                for (int i = 1; i < despegues.Count; i++)
                {
                    AvionSimulacion avionActual = despegues[i];
                    AvionSimulacion avionAnterior = despegues[i - 1];

                    avionActual.avionPrecedente = avionAnterior;
                }
            }
        }

        private Mensaje? BuscarPrimeraDeteccionTWRP3(
            AvionSimulacion avion,
            out double distanciaTWR,
            bool exigirFueraDER = true)
        {
            distanciaTWR = double.NaN;

            if (avion.planVuelo == null ||
                avion.mensajesRadarP3.Count < 2)
                return null;

            string pista = avion.planVuelo.pistaDespegue;
            double atot = avion.planVuelo.atot;

            if ((pista != "24L" && pista != "06R") ||
                !double.IsFinite(atot))
                return null;

            double inicio = Math.Max(0, atot - 60);
            double fin = Math.Min(86400, atot + 1200);

            var detecciones = avion.mensajesRadarP3
                .Where(m =>
                    m.categoria == 48 &&
                    m.tira is TiraDatosDecod048)
                .Select(m => new
                {
                    mensaje = m,
                    datos = (TiraDatosDecod048)m.tira
                })
                .Where(d =>
                    double.IsFinite(d.datos.TimeOfDay) &&
                    d.datos.TimeOfDay >= inicio &&
                    d.datos.TimeOfDay <= fin)
                .OrderBy(d => d.datos.TimeOfDay)
                .ToList();

            double distanciaAnterior = double.NaN;
            double ejeAnterior = double.NaN;
            double tiempoAnterior = double.NaN;

            bool cruceDetectado = false;
            double tiempoCruce = double.NaN;

            foreach (var deteccion in detecciones)
            {
                TiraDatosDecod048 datos = deteccion.datos;

                double tiempo = datos.TimeOfDay;

                double distancia = CalcularDistanciaDesdeDERP3(
                    datos, pista);

                if (!PosicionRespectoPistaP3(
                    datos, pista,
                    out double posicionEje,
                    out bool fueraDER))
                    continue;

                if (!double.IsFinite(distancia))
                    continue;

                // Comprobar continuidad temporal
                bool consecutivas =
                    double.IsFinite(tiempoAnterior) &&
                    tiempo > tiempoAnterior &&
                    tiempo - tiempoAnterior <= 12;

                // Avanzar en el sentido de la pista
                bool alejandose =
                    consecutivas &&
                    posicionEje > ejeAnterior &&
                    posicionEje >= 0;

                // Detectar el cruce real de 0,5 NM
                if (!cruceDetectado &&
                    consecutivas &&
                    distanciaAnterior < 0.5 &&
                    distancia >= 0.5 &&
                    alejandose &&
                    fueraDER)
                {
                    cruceDetectado = true;
                    tiempoCruce = tiempo;
                }

                if (cruceDetectado)
                {
                    // No buscar indefinidamente tras el cruce
                    if (tiempo - tiempoCruce > 60)
                        break;

                    int stat = datos.CommACAScapability.STAT;

                    bool enVuelo =
                        stat == 0 || stat == 2;

                    bool flValido =
                        double.IsFinite(datos.FL.FL) &&
                        datos.FL.V != 1;

                    // Aplicar el límite de 6000 ft cuando
                    // disponemos de altitud QNH válida
                    bool altitudValida =
                        double.IsFinite(datos.altitudQNHFt) &&
                        datos.altitudQNHFt <= 6000;

                    // Condición opcional: haber pasado el DER
                    bool cumpleDER =
                        !exigirFueraDER || fueraDER;

                    if (distancia >= 0.5 &&
                        alejandose &&
                        enVuelo &&
                        flValido &&
                        altitudValida &&
                        cumpleDER)
                    {
                        distanciaTWR = distancia;

                        return deteccion.mensaje;
                    }
                }

                distanciaAnterior = distancia;
                ejeAnterior = posicionEje;
                tiempoAnterior = tiempo;
            }

            return null;
        }

        private double CalcularDistanciaDesdeDERP3(
            TiraDatosDecod048 datos,
            string pista)
        {
            // Posición del avión sobre el eje de pista
            if (!PosicionRespectoPistaP3(
                datos, pista,
                out double posicionEje,
                out bool fueraDER))
            {
                return double.NaN;
            }

            // Coordenadas de los extremos de pista
            double lat24L = 41 + 17.0 / 60 + 31.99 / 3600;
            double lon24L = 2 + 6.0 / 60 + 11.81 / 3600;

            double lat06R = 41 + 16.0 / 60 + 56.32 / 3600;
            double lon06R = 2 + 4.0 / 60 + 27.66 / 3600;

            var extremo24L = ProyectarCoordenadasP3(
                lat24L, lon24L);

            var extremo06R = ProyectarCoordenadasP3(
                lat06R, lon06R);

            // Longitud entre los dos extremos
            double dx = extremo24L.x - extremo06R.x;
            double dy = extremo24L.y - extremo06R.y;

            double longitudPista = Math.Sqrt(dx * dx + dy * dy);

            // Distancia longitudinal desde el DER.
            // Negativa: antes de llegar al DER.
            // Positiva: después de superar el DER.
            double distanciaMetros = posicionEje - longitudPista;

            return distanciaMetros / 1852.0;
        }


        private (double x, double y) ProyectarCoordenadasP3(
            double latitud, double longitud)
        {
            // Centro de proyección del TMA Barcelona
            double latCentro = 41 + 6.0 / 60 + 56.56 / 3600;
            double lonCentro = 1 + 41.0 / 60 + 33.01 / 3600;

            CoordinatesWGS84 centro = new CoordinatesWGS84(
                latCentro * GeoUtils.DEGS2RADS,
                lonCentro * GeoUtils.DEGS2RADS,
                0
            );

            GeoUtils geo = new GeoUtils(
                Math.Sqrt(0.00669437999013),
                6378137.0,
                centro
            );

            CoordinatesWGS84 posicion = new CoordinatesWGS84(
                latitud * GeoUtils.DEGS2RADS,
                longitud * GeoUtils.DEGS2RADS,
                0
            );

            CoordinatesXYZ geocentrica =
                geo.change_geodesic2geocentric(posicion);

            CoordinatesXYZ cartesiana =
                geo.change_geocentric2system_cartesian(geocentrica);

            CoordinatesUVH proyectada =
                geo.change_system_cartesian2stereographic(cartesiana);

            return (proyectada.U, proyectada.V);
        }

        private bool PosicionRespectoPistaP3(
            TiraDatosDecod048 datos,
            string pista,
            out double distanciaEjeMetros,
            out bool fueraDER)
        {
            distanciaEjeMetros = double.NaN;
            fueraDER = false;

            if (datos.PositionCorrectedLatLonAlt == null ||
                datos.PositionCorrectedLatLonAlt.Count < 2)
                return false;

            double latAvion = datos.PositionCorrectedLatLonAlt[0];
            double lonAvion = datos.PositionCorrectedLatLonAlt[1];

            if (!double.IsFinite(latAvion) ||
                !double.IsFinite(lonAvion))
                return false;

            // Extremos de la pista
            double lat24L = 41 + 17.0 / 60 + 31.99 / 3600;
            double lon24L = 2 + 6.0 / 60 + 11.81 / 3600;

            double lat06R = 41 + 16.0 / 60 + 56.32 / 3600;
            double lon06R = 2 + 4.0 / 60 + 27.66 / 3600;

            double latTHR, lonTHR;
            double latDER, lonDER;

            if (pista == "24L")
            {
                latTHR = lat24L;
                lonTHR = lon24L;

                latDER = lat06R;
                lonDER = lon06R;
            }
            else if (pista == "06R")
            {
                latTHR = lat06R;
                lonTHR = lon06R;

                latDER = lat24L;
                lonDER = lon24L;
            }
            else
            {
                return false;
            }

            var thr = ProyectarCoordenadasP3(latTHR, lonTHR);
            var der = ProyectarCoordenadasP3(latDER, lonDER);
            var avion = ProyectarCoordenadasP3(latAvion, lonAvion);

            // Vector desde el THR hacia el DER
            double dx = der.x - thr.x;
            double dy = der.y - thr.y;

            double longitudPista = Math.Sqrt(dx * dx + dy * dy);

            if (longitudPista <= 0)
                return false;

            // Vector desde el THR hasta el avión
            double ax = avion.x - thr.x;
            double ay = avion.y - thr.y;

            // Proyección del avión sobre el eje de pista
            distanciaEjeMetros =
                (ax * dx + ay * dy) / longitudPista;

            // Ha sobrepasado el plano perpendicular
            // al eje de pista que pasa por el DER
            fueraDER = distanciaEjeMetros > longitudPista;

            return true;
        }

        private bool ObtenerPosicionInterpoladaP3(
            AvionSimulacion avion,
            double tiempo,
            out double x,
            out double y)
        {
            x = double.NaN;
            y = double.NaN;

            if (avion == null ||
                !double.IsFinite(tiempo) ||
                avion.mensajesRadarP3 == null ||
                avion.mensajesRadarP3.Count == 0)
                return false;

            // Obtener detecciones CAT48 con posición y tiempo válidos
            var detecciones = avion.mensajesRadarP3
                .Where(m => m.categoria == 48 &&
                            m.tira is TiraDatosDecod048)
                .Select(m => (TiraDatosDecod048)m.tira)
                .Where(d =>
                    double.IsFinite(d.TimeOfDay) &&
                    d.TimeOfDay >= 0 &&
                    d.PositionCorrectedLatLonAlt != null &&
                    d.PositionCorrectedLatLonAlt.Count >= 2 &&
                    double.IsFinite(d.PositionCorrectedLatLonAlt[0]) &&
                    double.IsFinite(d.PositionCorrectedLatLonAlt[1]))
                .OrderBy(d => d.TimeOfDay)
                .ToList();

            // Buscar una detección exacta o dos que rodeen
            // el instante de la primera detección TWR
            for (int i = 0; i < detecciones.Count; i++)
            {
                TiraDatosDecod048 actual = detecciones[i];

                // Si existe una detección en el instante solicitado
                if (Math.Abs(actual.TimeOfDay - tiempo) < 0.001)
                {
                    var posicion = ProyectarCoordenadasP3(
                        actual.PositionCorrectedLatLonAlt[0],
                        actual.PositionCorrectedLatLonAlt[1]);

                    x = posicion.x;
                    y = posicion.y;

                    return true;
                }

                if (i == 0)
                    continue;

                TiraDatosDecod048 anterior = detecciones[i - 1];

                double t1 = anterior.TimeOfDay;
                double t2 = actual.TimeOfDay;

                // El instante debe estar entre las dos muestras
                if (t1 > tiempo || t2 < tiempo)
                    continue;

                // Evitar interpolar entre detecciones demasiado
                // separadas o con tiempos repetidos
                if (t2 <= t1 || t2 - t1 > 12)
                    return false;

                // Proyectar las dos posiciones radar
                var p1 = ProyectarCoordenadasP3(
                    anterior.PositionCorrectedLatLonAlt[0],
                    anterior.PositionCorrectedLatLonAlt[1]);

                var p2 = ProyectarCoordenadasP3(
                    actual.PositionCorrectedLatLonAlt[0],
                    actual.PositionCorrectedLatLonAlt[1]);

                // Factor de interpolación lineal
                double factor = (tiempo - t1) / (t2 - t1);

                // Posición estimada en el instante solicitado
                x = p1.x + factor * (p2.x - p1.x);
                y = p1.y + factor * (p2.y - p1.y);

                return true;
            }

            // No hay datos para estimar la posición
            return false;
        }

        private double CalcularSeparacionTWRP3(
            AvionSimulacion avion)
        {
            // Comprobar que existe un avión precedente
            if (avion == null || avion.avionPrecedente == null)
                return double.NaN;

            // Obtener la primera detección válida del avión
            // a 0,5 NM del DER de su pista
            Mensaje? mensajeTWR =
                BuscarPrimeraDeteccionTWRP3(
                    avion,
                    out double distanciaDER);

            if (mensajeTWR == null)
                return double.NaN;

            TiraDatosDecod048 datos =
                (TiraDatosDecod048)mensajeTWR.tira;

            // Instante en el que queremos comparar ambos aviones
            double tiempo = datos.TimeOfDay;

            // Posición del avión posterior
            var posterior = ProyectarCoordenadasP3(
                datos.PositionCorrectedLatLonAlt[0],
                datos.PositionCorrectedLatLonAlt[1]);

            // Posición interpolada del avión precedente
            bool encontrado = ObtenerPosicionInterpoladaP3(
                avion.avionPrecedente,
                tiempo,
                out double xPrecedente,
                out double yPrecedente);

            if (!encontrado)
                return double.NaN;

            // Calcular distancia horizontal
            double dx = posterior.x - xPrecedente;
            double dy = posterior.y - yPrecedente;

            double distanciaMetros = Math.Sqrt(
                dx * dx + dy * dy);

            return distanciaMetros / 1852.0;
        }

        private double CalcularSeparacionMinimaTMAP3(
            AvionSimulacion avion,
            out double tiempoMinimo,
            out int comparaciones,
            out double separacionFinal)
        {
            tiempoMinimo = double.NaN;
            tiempoMinimo = double.NaN;
            comparaciones = 0;
            separacionFinal = double.NaN;

            if (avion == null || avion.avionPrecedente == null)
                return double.NaN;

            // Primera detección válida del avión posterior
            Mensaje? mensajeTWR = BuscarPrimeraDeteccionTWRP3(
                avion, out double distanciaDER);

            if (mensajeTWR == null)
                return double.NaN;

            double inicioTMA =
                ((TiraDatosDecod048)mensajeTWR.tira).TimeOfDay;

            double separacionMinima = double.MaxValue;

            // Recorrer las detecciones posteriores al inicio TWR
            var detecciones = avion.mensajesRadarP3
                .Where(m => m.categoria == 48 &&
                            m.tira is TiraDatosDecod048)
                .Select(m => (TiraDatosDecod048)m.tira)
                .Where(d => double.IsFinite(d.TimeOfDay) &&
                            d.TimeOfDay > inicioTMA)
                .OrderBy(d => d.TimeOfDay);

            foreach (TiraDatosDecod048 datos in detecciones)
            {
                if (datos.PositionCorrectedLatLonAlt == null ||
                    datos.PositionCorrectedLatLonAlt.Count < 2)
                    continue;

                double lat = datos.PositionCorrectedLatLonAlt[0];
                double lon = datos.PositionCorrectedLatLonAlt[1];

                if (!double.IsFinite(lat) || !double.IsFinite(lon))
                    continue;

                // Filtro geográfico del P3
                if (lat < 40.9 || lat > 41.7 ||
                    lon < 1.5 || lon > 2.6)
                    continue;

                // Comprobar estado y Flight Level
                int stat = datos.CommACAScapability.STAT;

                if (stat != 0 && stat != 2)
                    continue;

                if (!double.IsFinite(datos.FL.FL) ||
                    datos.FL.V == 1)
                    continue;

                // Altitud corregida por QNH
                if (!double.IsFinite(datos.altitudQNHFt))
                    continue;

                // Finalizar el análisis al superar 6000 ft
                if (datos.altitudQNHFt > 6000)
                    break;

                // Proyectar la posición del avión posterior
                var posterior = ProyectarCoordenadasP3(lat, lon);

                // Interpolar el precedente al mismo instante
                bool encontrado = ObtenerPosicionInterpoladaP3(
                    avion.avionPrecedente,
                    datos.TimeOfDay,
                    out double xPrecedente,
                    out double yPrecedente);

                if (!encontrado)
                    continue;

                // Calcular separación horizontal
                double dx = posterior.x - xPrecedente;
                double dy = posterior.y - yPrecedente;

                double separacion =
                    Math.Sqrt(dx * dx + dy * dy) / 1852.0;

                // Registrar cuántas distancias hemos calculado
                comparaciones++;

                // Guardar la última distancia disponible
                separacionFinal = separacion;

                // Conservar únicamente la menor separación
                if (separacion < separacionMinima)
                {
                    separacionMinima = separacion;
                    tiempoMinimo = datos.TimeOfDay;
                }
            }

            if (separacionMinima == double.MaxValue)
                return double.NaN;

            return separacionMinima;
        }
        private void AplicarFiltroMensajesP3()
        {
            if (!mostrarSoloAvionesP3)
                return;

            Filtros gestorFiltros = new Filtros();

            ListaMensajesFiltrados =
                gestorFiltros.FiltrarDespeguesP3(
                    ListaMensajesFiltrados,
                    mensajesVisiblesP3);
        }

        private void ActualizarFiltros()
        {
            Filtros gestorFiltros = new Filtros();

            ListaMensajesFiltrados = gestorFiltros.AplicarFiltros(
                ListaMensajes,
                filtroCat48,
                filtroCat21,
                filtroBlancoPuro,
                filtroTransFijo,
                filtroTrayectoria,
                filtroTrayectoriaMin,
                filtroTrayectoriaMax,
                filtroGround,
                filtroAltura,
                filtroAlturaMin,
                filtroAlturaMax,
                mostrarSoloAvionesP3,
                mensajesVisiblesP3
            );

            ReconstruirAviones(TiempoActual);
        }


        private void IdentificarSIDP3()
        {
            // Grupos de SID según la pista de despegue
            string[][] grupos24L =
            {
        new[] { "OLOXO", "NATPI", "MOPAS", "GRAUS",
                "LOBAR", "MAMUK", "REBUL", "VIBOK", "DUQQI" },

        new[] { "DUNES", "LARPA", "LOTOS", "SENIA" },

        new[] { "DALIN", "AGENA", "DIPES" }
    };

            string[][] grupos06R =
            {
        new[] { "OLOXO", "NATPI", "MOPAS", "LOBAR",
                "GRAUS", "MAMUK", "REBUL", "VIBOK", "DUQQI" },

        new[] { "LARPA", "DUNES", "SENIA", "LOTOS" },

        new[] { "AGENA", "DALIN", "DIPES" }
    };

            foreach (PlanVueloP3 plan in planesVueloP3)
            {
                plan.sidDetectada = "-";
                plan.grupoSID = "-";

                string pista = plan.pistaDespegue;
                string ruta = plan.rutaSACTA?.ToUpperInvariant() ?? "";

                // Solo analizar las pistas del P3
                if (pista != "24L" && pista != "06R")
                    continue;

                string[][] grupos = pista == "24L"
                    ? grupos24L
                    : grupos06R;

                // 1. Si el Excel ya contiene una SID, utilizarla directamente
                if (!string.IsNullOrWhiteSpace(plan.procDesp) &&
                    plan.procDesp.Trim() != "-")
                {
                    plan.sidDetectada = plan.procDesp.Trim();

                    string procedimiento =
                        plan.procDesp.Trim().ToUpperInvariant();

                    // Determinar el grupo a partir de la SID original
                    for (int i = 0; i < grupos.Length; i++)
                    {
                        if (grupos[i].Any(punto =>
                            System.Text.RegularExpressions.Regex.IsMatch(
                                procedimiento,
                                "^" + punto + @"\d+[A-Z]$")))
                        {
                            plan.grupoSID = "G" + (i + 1);
                            break;
                        }
                    }

                    // No hace falta analizar RutaSACTA
                    continue;
                }

                // 2. Si no hay SID original, buscar puntos en RutaSACTA
                var coincidencias = new List<(string punto, int posicion)>();

                for (int i = 0; i < grupos.Length; i++)
                {
                    foreach (string punto in grupos[i])
                    {
                        var encontrados =
                            System.Text.RegularExpressions.Regex.Matches(
                                ruta,
                                @"\b" + punto + @"\b");

                        foreach (System.Text.RegularExpressions.Match m
                            in encontrados)
                        {
                            coincidencias.Add((punto, m.Index));
                        }
                    }
                }

                // DUQQI y VIBOK pueden ser puntos intermedios.
                // Si existe otro candidato, priorizar ese.
                var candidatos = coincidencias
                    .Where(c => c.punto != "DUQQI" &&
                                c.punto != "VIBOK")
                    .ToList();

                // Si no encontramos ningún punto, dejar SID sin identificar
                if (candidatos.Count == 0)
                    continue;

                // Seleccionar el último punto candidato de la ruta
                string puntoSID = candidatos
                    .OrderBy(c => c.posicion)
                    .Last().punto;

                // 3. Determinar el grupo de la SID inferida
                for (int i = 0; i < grupos.Length; i++)
                {
                    if (grupos[i].Contains(puntoSID))
                    {
                        plan.grupoSID = "G" + (i + 1);
                        break;
                    }
                }

                // 4. Buscar una SID conocida de la misma pista
                // y asociada al mismo punto de salida
                string sidConocida = planesVueloP3
                    .Where(p =>
                        p.pistaDespegue == pista &&
                        !string.IsNullOrWhiteSpace(p.procDesp) &&
                        p.procDesp.Trim() != "-" &&
                        System.Text.RegularExpressions.Regex.IsMatch(
                            p.procDesp.Trim(),
                            "^" + puntoSID + @"\d+[A-Z]$",
                            System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                    .Select(p => p.procDesp.Trim())
                    .GroupBy(s => s, StringComparer.OrdinalIgnoreCase)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key)
                    .FirstOrDefault();

                if (!string.IsNullOrEmpty(sidConocida))
                {
                    // SID probable según los procedimientos del Excel
                    plan.sidDetectada = sidConocida;
                }
                else
                {
                    // Se conoce la familia, pero no la versión exacta
                    plan.sidDetectada = puntoSID +
                        (pista == "24L" ? "-C" : "-R");
                }
            }
        }
    }
}
