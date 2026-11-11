namespace GUI_ASTERIX
{
    partial class MenuSimulacion
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MenuSimulacion));
            menuStrip1 = new MenuStrip();
            archivoToolStripMenuItem = new ToolStripMenuItem();
            cargaDatosrToolStripMenuItem = new ToolStripMenuItem();
            guardarEnCSVToolStripMenuItem = new ToolStripMenuItem();
            filtrarToolStripMenuItem = new ToolStripMenuItem();
            aplicarToolStripMenuItem = new ToolStripMenuItem();
            limpiarToolStripMenuItem = new ToolStripMenuItem();
            verToolStripMenuItem = new ToolStripMenuItem();
            mostrarDatosEnCeldasToolStripMenuItem = new ToolStripMenuItem();
            trayectoriasToolStripMenuItem = new ToolStripMenuItem();
            heatmapToolStripMenuItem = new ToolStripMenuItem();
            mostrarHeatmapCompletoToolStripMenuItem = new ToolStripMenuItem();
            mostrarHeatmapDinamicoToolStripMenuItem = new ToolStripMenuItem();
            ayudaToolStripMenuItem = new ToolStripMenuItem();
            moToolStripMenuItem = new ToolStripMenuItem();
            guiaToolStripMenuItem = new ToolStripMenuItem();
            p3ToolStripMenuItem = new ToolStripMenuItem();
            cargarPlanDeVueloToolStripMenuItem = new ToolStripMenuItem();
            timerSimulacion = new System.Windows.Forms.Timer(components);
            trackBarVelSimulacion = new TrackBar();
            dataGridAviones = new DataGridView();
            gMapControl1 = new GMap.NET.WindowsForms.GMapControl();
            labelTituloVelocidadReproduccion = new Label();
            labelTituloHora = new Label();
            pictureBox1 = new PictureBox();
            pictureBox2 = new PictureBox();
            pictureBox3 = new PictureBox();
            labelTituloVerde = new Label();
            label2 = new Label();
            pictureBox4 = new PictureBox();
            pictureBox5 = new PictureBox();
            labelTituloAsterix = new Label();
            timerClick = new System.Windows.Forms.Timer(components);
            textBoxHora = new TextBox();
            textBoxSegundo = new TextBox();
            textBoxMinuto = new TextBox();
            labelVelocidad = new Label();
            trackBarTiempo = new TrackBar();
            labelTiempoFinal = new Label();
            label3 = new Label();
            label4 = new Label();
            pictureBox6 = new PictureBox();
            pictureBoxContorles = new PictureBox();
            pictureBox7 = new PictureBox();
            panelLeyendaHeatmap = new Panel();
            labeldensidad = new Label();
            labelHeatmap100 = new Label();
            labelHeatmap75 = new Label();
            labelHeatmap50 = new Label();
            labelHeatmap25 = new Label();
            labelHeatmap0 = new Label();
            pictureBoxEscalaHeatmap = new PictureBox();
            labelDensTraf = new Label();
            menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackBarVelSimulacion).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataGridAviones).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox5).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarTiempo).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox6).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxContorles).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox7).BeginInit();
            panelLeyendaHeatmap.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxEscalaHeatmap).BeginInit();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(24, 24);
            menuStrip1.Items.AddRange(new ToolStripItem[] { archivoToolStripMenuItem, filtrarToolStripMenuItem, verToolStripMenuItem, ayudaToolStripMenuItem, p3ToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(985, 29);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // archivoToolStripMenuItem
            // 
            archivoToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { cargaDatosrToolStripMenuItem, guardarEnCSVToolStripMenuItem });
            archivoToolStripMenuItem.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            archivoToolStripMenuItem.Name = "archivoToolStripMenuItem";
            archivoToolStripMenuItem.Size = new Size(75, 25);
            archivoToolStripMenuItem.Text = "Archivo";
            // 
            // cargaDatosrToolStripMenuItem
            // 
            cargaDatosrToolStripMenuItem.Image = Properties.Resources.iconoCargarArchivoTransparente;
            cargaDatosrToolStripMenuItem.Name = "cargaDatosrToolStripMenuItem";
            cargaDatosrToolStripMenuItem.Size = new Size(191, 26);
            cargaDatosrToolStripMenuItem.Text = "Cargar datos";
            cargaDatosrToolStripMenuItem.Click += cargaDatosrToolStripMenuItem_Click;
            // 
            // guardarEnCSVToolStripMenuItem
            // 
            guardarEnCSVToolStripMenuItem.Image = Properties.Resources.iconoCsv;
            guardarEnCSVToolStripMenuItem.Name = "guardarEnCSVToolStripMenuItem";
            guardarEnCSVToolStripMenuItem.Size = new Size(191, 26);
            guardarEnCSVToolStripMenuItem.Text = "Guardar en CSV";
            guardarEnCSVToolStripMenuItem.Click += guardarEnCSVToolStripMenuItem_Click;
            // 
            // filtrarToolStripMenuItem
            // 
            filtrarToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { aplicarToolStripMenuItem, limpiarToolStripMenuItem });
            filtrarToolStripMenuItem.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            filtrarToolStripMenuItem.Name = "filtrarToolStripMenuItem";
            filtrarToolStripMenuItem.Size = new Size(63, 25);
            filtrarToolStripMenuItem.Text = "Filtrar";
            // 
            // aplicarToolStripMenuItem
            // 
            aplicarToolStripMenuItem.Image = Properties.Resources.filtros;
            aplicarToolStripMenuItem.Name = "aplicarToolStripMenuItem";
            aplicarToolStripMenuItem.Size = new Size(172, 26);
            aplicarToolStripMenuItem.Text = "Aplicar filtros";
            aplicarToolStripMenuItem.Click += aplicarToolStripMenuItem_Click;
            // 
            // limpiarToolStripMenuItem
            // 
            limpiarToolStripMenuItem.Image = Properties.Resources.limpiarFiltro;
            limpiarToolStripMenuItem.Name = "limpiarToolStripMenuItem";
            limpiarToolStripMenuItem.Size = new Size(172, 26);
            limpiarToolStripMenuItem.Text = "Limpiar";
            limpiarToolStripMenuItem.Click += limpiarToolStripMenuItem_Click;
            // 
            // verToolStripMenuItem
            // 
            verToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { mostrarDatosEnCeldasToolStripMenuItem, trayectoriasToolStripMenuItem, heatmapToolStripMenuItem });
            verToolStripMenuItem.Font = new Font("Segoe UI", 12F);
            verToolStripMenuItem.Name = "verToolStripMenuItem";
            verToolStripMenuItem.Size = new Size(45, 25);
            verToolStripMenuItem.Text = "Ver";
            // 
            // mostrarDatosEnCeldasToolStripMenuItem
            // 
            mostrarDatosEnCeldasToolStripMenuItem.Image = Properties.Resources.datosDataGrid;
            mostrarDatosEnCeldasToolStripMenuItem.Name = "mostrarDatosEnCeldasToolStripMenuItem";
            mostrarDatosEnCeldasToolStripMenuItem.Size = new Size(254, 26);
            mostrarDatosEnCeldasToolStripMenuItem.Text = "Mostar aviones en celdas";
            mostrarDatosEnCeldasToolStripMenuItem.Click += mostrarDatosEnCeldasToolStripMenuItem_Click;
            // 
            // trayectoriasToolStripMenuItem
            // 
            trayectoriasToolStripMenuItem.Image = Properties.Resources.trayectorias;
            trayectoriasToolStripMenuItem.Name = "trayectoriasToolStripMenuItem";
            trayectoriasToolStripMenuItem.Size = new Size(254, 26);
            trayectoriasToolStripMenuItem.Text = "Trayectorias";
            trayectoriasToolStripMenuItem.Click += trayectoriasToolStripMenuItem_Click;
            // 
            // heatmapToolStripMenuItem
            // 
            heatmapToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { mostrarHeatmapCompletoToolStripMenuItem, mostrarHeatmapDinamicoToolStripMenuItem });
            heatmapToolStripMenuItem.Image = Properties.Resources.hetmap;
            heatmapToolStripMenuItem.Name = "heatmapToolStripMenuItem";
            heatmapToolStripMenuItem.Size = new Size(254, 26);
            heatmapToolStripMenuItem.Text = "Heatmap";
            // 
            // mostrarHeatmapCompletoToolStripMenuItem
            // 
            mostrarHeatmapCompletoToolStripMenuItem.Image = Properties.Resources.heatmapTot;
            mostrarHeatmapCompletoToolStripMenuItem.Name = "mostrarHeatmapCompletoToolStripMenuItem";
            mostrarHeatmapCompletoToolStripMenuItem.Size = new Size(269, 26);
            mostrarHeatmapCompletoToolStripMenuItem.Text = "Mostrar heatmap completo";
            mostrarHeatmapCompletoToolStripMenuItem.Click += mostrarHeatmapCompletoToolStripMenuItem_Click;
            // 
            // mostrarHeatmapDinamicoToolStripMenuItem
            // 
            mostrarHeatmapDinamicoToolStripMenuItem.Image = Properties.Resources.heatmapDin;
            mostrarHeatmapDinamicoToolStripMenuItem.Name = "mostrarHeatmapDinamicoToolStripMenuItem";
            mostrarHeatmapDinamicoToolStripMenuItem.Size = new Size(269, 26);
            mostrarHeatmapDinamicoToolStripMenuItem.Text = "Mostrar heatmap dinamico";
            mostrarHeatmapDinamicoToolStripMenuItem.Click += mostrarHeatmapDinamicoToolStripMenuItem_Click;
            // 
            // ayudaToolStripMenuItem
            // 
            ayudaToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { moToolStripMenuItem, guiaToolStripMenuItem });
            ayudaToolStripMenuItem.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ayudaToolStripMenuItem.Name = "ayudaToolStripMenuItem";
            ayudaToolStripMenuItem.Size = new Size(66, 25);
            ayudaToolStripMenuItem.Text = "Ayuda";
            // 
            // moToolStripMenuItem
            // 
            moToolStripMenuItem.Image = Properties.Resources.contorlesMapaTrans;
            moToolStripMenuItem.Name = "moToolStripMenuItem";
            moToolStripMenuItem.Size = new Size(271, 26);
            moToolStripMenuItem.Text = "Mostrar controles del mapa";
            moToolStripMenuItem.Click += moToolStripMenuItem_Click;
            // 
            // guiaToolStripMenuItem
            // 
            guiaToolStripMenuItem.Image = Properties.Resources.Círculo_de_neón_con_interrogación;
            guiaToolStripMenuItem.Name = "guiaToolStripMenuItem";
            guiaToolStripMenuItem.Size = new Size(271, 26);
            guiaToolStripMenuItem.Text = "Guia";
            guiaToolStripMenuItem.Click += guiaToolStripMenuItem_Click;
            // 
            // p3ToolStripMenuItem
            // 
            p3ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { cargarPlanDeVueloToolStripMenuItem });
            p3ToolStripMenuItem.Font = new Font("Segoe UI", 12F);
            p3ToolStripMenuItem.Name = "p3ToolStripMenuItem";
            p3ToolStripMenuItem.Size = new Size(40, 25);
            p3ToolStripMenuItem.Text = "P3";
            // 
            // cargarPlanDeVueloToolStripMenuItem
            // 
            cargarPlanDeVueloToolStripMenuItem.Name = "cargarPlanDeVueloToolStripMenuItem";
            cargarPlanDeVueloToolStripMenuItem.Size = new Size(224, 26);
            cargarPlanDeVueloToolStripMenuItem.Text = "Cargar plan de vuelo";
            cargarPlanDeVueloToolStripMenuItem.Click += cargarPlanDeVueloToolStripMenuItem_Click;
            // 
            // timerSimulacion
            // 
            timerSimulacion.Tick += timerSimulacion_Tick;
            // 
            // trackBarVelSimulacion
            // 
            trackBarVelSimulacion.Anchor = AnchorStyles.Bottom;
            trackBarVelSimulacion.LargeChange = 1;
            trackBarVelSimulacion.Location = new Point(359, 667);
            trackBarVelSimulacion.Maximum = 8;
            trackBarVelSimulacion.Minimum = 1;
            trackBarVelSimulacion.Name = "trackBarVelSimulacion";
            trackBarVelSimulacion.Size = new Size(240, 45);
            trackBarVelSimulacion.TabIndex = 8;
            trackBarVelSimulacion.Value = 1;
            trackBarVelSimulacion.Scroll += trackBarVelSimulacion_Scroll;
            // 
            // dataGridAviones
            // 
            dataGridAviones.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridAviones.Location = new Point(25, 49);
            dataGridAviones.Name = "dataGridAviones";
            dataGridAviones.RowHeadersWidth = 62;
            dataGridAviones.Size = new Size(927, 445);
            dataGridAviones.TabIndex = 8;
            dataGridAviones.Visible = false;
            // 
            // gMapControl1
            // 
            gMapControl1.Bearing = 0F;
            gMapControl1.CanDragMap = true;
            gMapControl1.EmptyTileColor = Color.Navy;
            gMapControl1.GrayScaleMode = false;
            gMapControl1.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            gMapControl1.LevelsKeepInMemory = 5;
            gMapControl1.Location = new Point(25, 49);
            gMapControl1.MarkersEnabled = true;
            gMapControl1.MaxZoom = 2;
            gMapControl1.MinZoom = 2;
            gMapControl1.MouseWheelZoomEnabled = true;
            gMapControl1.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;
            gMapControl1.Name = "gMapControl1";
            gMapControl1.NegativeMode = false;
            gMapControl1.PolygonsEnabled = true;
            gMapControl1.RetryLoadTile = 0;
            gMapControl1.RoutesEnabled = true;
            gMapControl1.ScaleMode = GMap.NET.WindowsForms.ScaleModes.Integer;
            gMapControl1.SelectedAreaFillColor = Color.FromArgb(33, 65, 105, 225);
            gMapControl1.ShowTileGridLines = false;
            gMapControl1.Size = new Size(927, 445);
            gMapControl1.TabIndex = 10;
            gMapControl1.Zoom = 0D;
            // 
            // labelTituloVelocidadReproduccion
            // 
            labelTituloVelocidadReproduccion.Anchor = AnchorStyles.Bottom;
            labelTituloVelocidadReproduccion.AutoSize = true;
            labelTituloVelocidadReproduccion.Font = new Font("Segoe UI Semibold", 15.75F, FontStyle.Bold);
            labelTituloVelocidadReproduccion.ForeColor = SystemColors.ControlLightLight;
            labelTituloVelocidadReproduccion.Location = new Point(324, 632);
            labelTituloVelocidadReproduccion.Name = "labelTituloVelocidadReproduccion";
            labelTituloVelocidadReproduccion.Size = new Size(275, 30);
            labelTituloVelocidadReproduccion.TabIndex = 15;
            labelTituloVelocidadReproduccion.Text = "Velocidad de reproducción:";
            // 
            // labelTituloHora
            // 
            labelTituloHora.Anchor = AnchorStyles.Bottom;
            labelTituloHora.AutoSize = true;
            labelTituloHora.BackColor = Color.Transparent;
            labelTituloHora.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            labelTituloHora.ForeColor = SystemColors.ControlLightLight;
            labelTituloHora.Location = new Point(75, 552);
            labelTituloHora.Name = "labelTituloHora";
            labelTituloHora.Size = new Size(147, 32);
            labelTituloHora.TabIndex = 16;
            labelTituloHora.Text = "Hora actual: ";
            // 
            // pictureBox1
            // 
            pictureBox1.Anchor = AnchorStyles.Bottom;
            pictureBox1.Image = Properties.Resources.botonReproducir;
            pictureBox1.InitialImage = (Image)resources.GetObject("pictureBox1.InitialImage");
            pictureBox1.Location = new Point(453, 568);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(44, 43);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 17;
            pictureBox1.TabStop = false;
            pictureBox1.Click += pictureBox1_Click;
            // 
            // pictureBox2
            // 
            pictureBox2.Anchor = AnchorStyles.Bottom;
            pictureBox2.Image = Properties.Resources.botonAvanzar;
            pictureBox2.InitialImage = (Image)resources.GetObject("pictureBox2.InitialImage");
            pictureBox2.Location = new Point(545, 568);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(44, 43);
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.TabIndex = 18;
            pictureBox2.TabStop = false;
            pictureBox2.Click += pictureBox2_Click;
            // 
            // pictureBox3
            // 
            pictureBox3.Anchor = AnchorStyles.Bottom;
            pictureBox3.Image = Properties.Resources.botonRetroceder;
            pictureBox3.InitialImage = (Image)resources.GetObject("pictureBox3.InitialImage");
            pictureBox3.Location = new Point(355, 568);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(44, 43);
            pictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox3.TabIndex = 19;
            pictureBox3.TabStop = false;
            pictureBox3.Click += pictureBox3_Click;
            // 
            // labelTituloVerde
            // 
            labelTituloVerde.Anchor = AnchorStyles.Bottom;
            labelTituloVerde.AutoSize = true;
            labelTituloVerde.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold);
            labelTituloVerde.ForeColor = SystemColors.ControlLightLight;
            labelTituloVerde.Location = new Point(41, 689);
            labelTituloVerde.Name = "labelTituloVerde";
            labelTituloVerde.Size = new Size(68, 25);
            labelTituloVerde.TabIndex = 20;
            labelTituloVerde.Text = "ADS-B";
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Bottom;
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold);
            label2.ForeColor = SystemColors.ControlLightLight;
            label2.Location = new Point(44, 652);
            label2.Name = "label2";
            label2.Size = new Size(62, 25);
            label2.TabIndex = 21;
            label2.Text = "Radar";
            // 
            // pictureBox4
            // 
            pictureBox4.Anchor = AnchorStyles.Bottom;
            pictureBox4.Image = Properties.Resources.large_green_square_1f7e9;
            pictureBox4.InitialImage = (Image)resources.GetObject("pictureBox4.InitialImage");
            pictureBox4.Location = new Point(7, 689);
            pictureBox4.Name = "pictureBox4";
            pictureBox4.Size = new Size(28, 28);
            pictureBox4.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox4.TabIndex = 23;
            pictureBox4.TabStop = false;
            // 
            // pictureBox5
            // 
            pictureBox5.Anchor = AnchorStyles.Bottom;
            pictureBox5.Image = Properties.Resources.images;
            pictureBox5.InitialImage = (Image)resources.GetObject("pictureBox5.InitialImage");
            pictureBox5.Location = new Point(7, 653);
            pictureBox5.Name = "pictureBox5";
            pictureBox5.Size = new Size(29, 28);
            pictureBox5.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox5.TabIndex = 24;
            pictureBox5.TabStop = false;
            // 
            // labelTituloAsterix
            // 
            labelTituloAsterix.Anchor = AnchorStyles.Bottom;
            labelTituloAsterix.AutoSize = true;
            labelTituloAsterix.Font = new Font("Segoe UI", 26.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelTituloAsterix.ForeColor = Color.Turquoise;
            labelTituloAsterix.Location = new Point(811, 667);
            labelTituloAsterix.Name = "labelTituloAsterix";
            labelTituloAsterix.Size = new Size(162, 47);
            labelTituloAsterix.TabIndex = 26;
            labelTituloAsterix.Text = "ASTERIX";
            // 
            // textBoxHora
            // 
            textBoxHora.Anchor = AnchorStyles.Bottom;
            textBoxHora.BackColor = Color.FromArgb(18, 28, 45);
            textBoxHora.BorderStyle = BorderStyle.None;
            textBoxHora.Font = new Font("Segoe UI", 26.25F, FontStyle.Bold);
            textBoxHora.ForeColor = SystemColors.ControlLightLight;
            textBoxHora.Location = new Point(70, 506);
            textBoxHora.Name = "textBoxHora";
            textBoxHora.Size = new Size(42, 47);
            textBoxHora.TabIndex = 30;
            textBoxHora.Text = "00";
            textBoxHora.TextAlign = HorizontalAlignment.Center;
            // 
            // textBoxSegundo
            // 
            textBoxSegundo.Anchor = AnchorStyles.Bottom;
            textBoxSegundo.BackColor = Color.FromArgb(18, 28, 45);
            textBoxSegundo.BorderStyle = BorderStyle.None;
            textBoxSegundo.Font = new Font("Segoe UI", 26.25F, FontStyle.Bold);
            textBoxSegundo.ForeColor = SystemColors.ControlLightLight;
            textBoxSegundo.Location = new Point(168, 506);
            textBoxSegundo.Name = "textBoxSegundo";
            textBoxSegundo.Size = new Size(42, 47);
            textBoxSegundo.TabIndex = 31;
            textBoxSegundo.Text = "00";
            textBoxSegundo.TextAlign = HorizontalAlignment.Center;
            // 
            // textBoxMinuto
            // 
            textBoxMinuto.Anchor = AnchorStyles.Bottom;
            textBoxMinuto.BackColor = Color.FromArgb(18, 28, 45);
            textBoxMinuto.BorderStyle = BorderStyle.None;
            textBoxMinuto.Font = new Font("Segoe UI", 26.25F, FontStyle.Bold);
            textBoxMinuto.ForeColor = SystemColors.ControlLightLight;
            textBoxMinuto.Location = new Point(119, 506);
            textBoxMinuto.Name = "textBoxMinuto";
            textBoxMinuto.Size = new Size(42, 47);
            textBoxMinuto.TabIndex = 32;
            textBoxMinuto.Text = "00";
            textBoxMinuto.TextAlign = HorizontalAlignment.Center;
            // 
            // labelVelocidad
            // 
            labelVelocidad.Anchor = AnchorStyles.Bottom;
            labelVelocidad.AutoSize = true;
            labelVelocidad.Font = new Font("Segoe UI Semibold", 15.75F, FontStyle.Bold);
            labelVelocidad.ForeColor = Color.Turquoise;
            labelVelocidad.Location = new Point(600, 632);
            labelVelocidad.Name = "labelVelocidad";
            labelVelocidad.Size = new Size(32, 30);
            labelVelocidad.TabIndex = 35;
            labelVelocidad.Text = "x1";
            labelVelocidad.TextAlign = ContentAlignment.TopCenter;
            // 
            // trackBarTiempo
            // 
            trackBarTiempo.Anchor = AnchorStyles.Bottom;
            trackBarTiempo.Location = new Point(233, 517);
            trackBarTiempo.Maximum = 100;
            trackBarTiempo.Name = "trackBarTiempo";
            trackBarTiempo.Size = new Size(456, 45);
            trackBarTiempo.TabIndex = 36;
            trackBarTiempo.MouseUp += trackBarTiempo_MouseUp;
            // 
            // labelTiempoFinal
            // 
            labelTiempoFinal.Anchor = AnchorStyles.Bottom;
            labelTiempoFinal.AutoSize = true;
            labelTiempoFinal.Font = new Font("Segoe UI", 26.25F, FontStyle.Bold);
            labelTiempoFinal.ForeColor = SystemColors.ControlLightLight;
            labelTiempoFinal.Location = new Point(695, 506);
            labelTiempoFinal.Name = "labelTiempoFinal";
            labelTiempoFinal.Size = new Size(158, 47);
            labelTiempoFinal.TabIndex = 37;
            labelTiempoFinal.Text = "00:00:00";
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.Bottom;
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 26.25F, FontStyle.Bold);
            label3.ForeColor = SystemColors.ControlLightLight;
            label3.Location = new Point(103, 504);
            label3.Name = "label3";
            label3.Size = new Size(29, 47);
            label3.TabIndex = 38;
            label3.Text = ":";
            // 
            // label4
            // 
            label4.Anchor = AnchorStyles.Bottom;
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 26.25F, FontStyle.Bold);
            label4.ForeColor = SystemColors.ControlLightLight;
            label4.Location = new Point(152, 503);
            label4.Name = "label4";
            label4.Size = new Size(29, 47);
            label4.TabIndex = 39;
            label4.Text = ":";
            // 
            // pictureBox6
            // 
            pictureBox6.Anchor = AnchorStyles.Bottom;
            pictureBox6.Image = Properties.Resources.botonRecargar;
            pictureBox6.InitialImage = (Image)resources.GetObject("pictureBox6.InitialImage");
            pictureBox6.Location = new Point(275, 568);
            pictureBox6.Name = "pictureBox6";
            pictureBox6.Size = new Size(44, 43);
            pictureBox6.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox6.TabIndex = 40;
            pictureBox6.TabStop = false;
            pictureBox6.Click += buttonReset_Click;
            // 
            // pictureBoxContorles
            // 
            pictureBoxContorles.Image = Properties.Resources.guiaControles;
            pictureBoxContorles.Location = new Point(25, 49);
            pictureBoxContorles.Name = "pictureBoxContorles";
            pictureBoxContorles.Size = new Size(927, 445);
            pictureBoxContorles.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBoxContorles.TabIndex = 41;
            pictureBoxContorles.TabStop = false;
            pictureBoxContorles.Visible = false;
            pictureBoxContorles.Click += pictureBoxContorles_Click;
            // 
            // pictureBox7
            // 
            pictureBox7.Anchor = AnchorStyles.Bottom;
            pictureBox7.Image = Properties.Resources.IconoAsterix;
            pictureBox7.InitialImage = (Image)resources.GetObject("pictureBox7.InitialImage");
            pictureBox7.Location = new Point(783, 679);
            pictureBox7.Name = "pictureBox7";
            pictureBox7.Size = new Size(29, 28);
            pictureBox7.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox7.TabIndex = 42;
            pictureBox7.TabStop = false;
            // 
            // panelLeyendaHeatmap
            // 
            panelLeyendaHeatmap.Controls.Add(labeldensidad);
            panelLeyendaHeatmap.Controls.Add(labelHeatmap100);
            panelLeyendaHeatmap.Controls.Add(labelHeatmap75);
            panelLeyendaHeatmap.Controls.Add(labelHeatmap50);
            panelLeyendaHeatmap.Controls.Add(labelHeatmap25);
            panelLeyendaHeatmap.Controls.Add(labelHeatmap0);
            panelLeyendaHeatmap.Controls.Add(pictureBoxEscalaHeatmap);
            panelLeyendaHeatmap.Controls.Add(labelDensTraf);
            panelLeyendaHeatmap.Location = new Point(692, 394);
            panelLeyendaHeatmap.Name = "panelLeyendaHeatmap";
            panelLeyendaHeatmap.Size = new Size(260, 100);
            panelLeyendaHeatmap.TabIndex = 43;
            // 
            // labeldensidad
            // 
            labeldensidad.AutoSize = true;
            labeldensidad.ForeColor = Color.White;
            labeldensidad.Location = new Point(77, 78);
            labeldensidad.Name = "labeldensidad";
            labeldensidad.Size = new Size(104, 15);
            labeldensidad.TabIndex = 7;
            labeldensidad.Text = "Aviones/cuadrado";
            // 
            // labelHeatmap100
            // 
            labelHeatmap100.AutoSize = true;
            labelHeatmap100.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            labelHeatmap100.ForeColor = Color.White;
            labelHeatmap100.Location = new Point(223, 59);
            labelHeatmap100.Name = "labelHeatmap100";
            labelHeatmap100.Size = new Size(37, 15);
            labelHeatmap100.TabIndex = 6;
            labelHeatmap100.Text = "H100";
            // 
            // labelHeatmap75
            // 
            labelHeatmap75.AutoSize = true;
            labelHeatmap75.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            labelHeatmap75.ForeColor = Color.White;
            labelHeatmap75.Location = new Point(177, 59);
            labelHeatmap75.Name = "labelHeatmap75";
            labelHeatmap75.Size = new Size(30, 15);
            labelHeatmap75.TabIndex = 5;
            labelHeatmap75.Text = "H75";
            // 
            // labelHeatmap50
            // 
            labelHeatmap50.AutoSize = true;
            labelHeatmap50.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            labelHeatmap50.ForeColor = Color.White;
            labelHeatmap50.Location = new Point(114, 59);
            labelHeatmap50.Name = "labelHeatmap50";
            labelHeatmap50.Size = new Size(30, 15);
            labelHeatmap50.TabIndex = 4;
            labelHeatmap50.Text = "H50";
            // 
            // labelHeatmap25
            // 
            labelHeatmap25.AutoSize = true;
            labelHeatmap25.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            labelHeatmap25.ForeColor = Color.White;
            labelHeatmap25.Location = new Point(55, 59);
            labelHeatmap25.Name = "labelHeatmap25";
            labelHeatmap25.Size = new Size(30, 15);
            labelHeatmap25.TabIndex = 3;
            labelHeatmap25.Text = "H25";
            // 
            // labelHeatmap0
            // 
            labelHeatmap0.AutoSize = true;
            labelHeatmap0.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelHeatmap0.ForeColor = Color.White;
            labelHeatmap0.Location = new Point(0, 59);
            labelHeatmap0.Name = "labelHeatmap0";
            labelHeatmap0.Size = new Size(23, 15);
            labelHeatmap0.TabIndex = 2;
            labelHeatmap0.Text = "H0";
            // 
            // pictureBoxEscalaHeatmap
            // 
            pictureBoxEscalaHeatmap.Location = new Point(15, 38);
            pictureBoxEscalaHeatmap.Name = "pictureBoxEscalaHeatmap";
            pictureBoxEscalaHeatmap.Size = new Size(230, 18);
            pictureBoxEscalaHeatmap.TabIndex = 1;
            pictureBoxEscalaHeatmap.TabStop = false;
            // 
            // labelDensTraf
            // 
            labelDensTraf.AutoSize = true;
            labelDensTraf.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelDensTraf.ForeColor = Color.White;
            labelDensTraf.Location = new Point(73, 10);
            labelDensTraf.Name = "labelDensTraf";
            labelDensTraf.Size = new Size(115, 15);
            labelDensTraf.TabIndex = 0;
            labelDensTraf.Text = "Densidad de trafico";
            // 
            // MenuSimulacion
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(18, 28, 45);
            ClientSize = new Size(985, 731);
            Controls.Add(panelLeyendaHeatmap);
            Controls.Add(pictureBox7);
            Controls.Add(pictureBox6);
            Controls.Add(labelTiempoFinal);
            Controls.Add(trackBarTiempo);
            Controls.Add(labelVelocidad);
            Controls.Add(textBoxMinuto);
            Controls.Add(textBoxSegundo);
            Controls.Add(textBoxHora);
            Controls.Add(labelTituloAsterix);
            Controls.Add(pictureBox5);
            Controls.Add(pictureBox4);
            Controls.Add(label2);
            Controls.Add(labelTituloVerde);
            Controls.Add(pictureBox3);
            Controls.Add(pictureBox2);
            Controls.Add(pictureBox1);
            Controls.Add(labelTituloHora);
            Controls.Add(labelTituloVelocidadReproduccion);
            Controls.Add(trackBarVelSimulacion);
            Controls.Add(menuStrip1);
            Controls.Add(label3);
            Controls.Add(label4);
            Controls.Add(gMapControl1);
            Controls.Add(dataGridAviones);
            Controls.Add(pictureBoxContorles);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            Name = "MenuSimulacion";
            SizeGripStyle = SizeGripStyle.Hide;
            Text = "ASTERIX";
            Load += MenuSimulacion_Load;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackBarVelSimulacion).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataGridAviones).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox5).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarTiempo).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox6).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxContorles).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox7).EndInit();
            panelLeyendaHeatmap.ResumeLayout(false);
            panelLeyendaHeatmap.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxEscalaHeatmap).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem archivoToolStripMenuItem;
        private ToolStripMenuItem cargaDatosrToolStripMenuItem;
        private ToolStripMenuItem filtrarToolStripMenuItem;
        private System.Windows.Forms.Timer timerSimulacion;
        private TrackBar trackBarVelSimulacion;
        private DataGridView dataGridAviones;
        private GMap.NET.WindowsForms.GMapControl gMapControl1;
        private Label labelTituloVelocidadReproduccion;
        private Label labelTituloHora;
        private PictureBox pictureBox1;
        private PictureBox pictureBox2;
        private PictureBox pictureBox3;
        private ToolStripMenuItem aplicarToolStripMenuItem;
        private ToolStripMenuItem limpiarToolStripMenuItem;
        private Label labelTituloVerde;
        private Label label2;
        private PictureBox pictureBox4;
        private PictureBox pictureBox5;
        private Label labelTituloAsterix;
        private ToolStripMenuItem guardarEnCSVToolStripMenuItem;
        private System.Windows.Forms.Timer timerClick;
        private TextBox textBoxHora;
        private TextBox textBoxSegundo;
        private TextBox textBoxMinuto;
        private ToolStripMenuItem verToolStripMenuItem;
        private ToolStripMenuItem mostrarDatosEnCeldasToolStripMenuItem;
        private ToolStripMenuItem trayectoriasToolStripMenuItem;
        private ToolStripMenuItem heatmapToolStripMenuItem;
        private ToolStripMenuItem mostrarHeatmapCompletoToolStripMenuItem;
        private ToolStripMenuItem mostrarHeatmapDinamicoToolStripMenuItem;
        private ToolStripMenuItem ayudaToolStripMenuItem;
        private Button buttonSalirMenuInicio;
        private Label labelVelocidad;
        private TrackBar trackBarTiempo;
        private Label labelTiempoFinal;
        private Label label3;
        private Label label4;
        private PictureBox pictureBox6;
        private ToolStripMenuItem moToolStripMenuItem;
        private PictureBox pictureBoxContorles;
        private PictureBox pictureBox7;
        private ToolStripMenuItem guiaToolStripMenuItem;
        private Panel panelLeyendaHeatmap;
        private PictureBox pictureBoxEscalaHeatmap;
        private Label labelDensTraf;
        private Label labelHeatmap0;
        private Label labelHeatmap100;
        private Label labelHeatmap75;
        private Label labelHeatmap50;
        private Label labelHeatmap25;
        private Label labeldensidad;
        private ToolStripMenuItem p3ToolStripMenuItem;
        private ToolStripMenuItem cargarPlanDeVueloToolStripMenuItem;
    }
}
