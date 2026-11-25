using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Poker
{
    public partial class Form1 : Form
    {
        private const int PosicionYJugador = 484;
        private const int DesplazamientoSeleccion = 15;
        private const int Ante = 50;

        private Baraja baraja;
        private MotorIA motorIA;
        private DecisionIA decisionActualIA;
        private List<Carta> manoJugador;
        private List<Carta> manoIA;

        private PictureBox[] cuadrosJugador;
        private PictureBox[] cuadrosIA;
        private Label[] etiquetasSeleccion;

        private bool rondaActiva;
        private bool cambioRealizado;
        private bool faseApuesta;

        private int fichasJugador = 1000;
        private int fichasIA = 1000;
        private int pozo = 0;
        private int apuestaActual = 0;

        private int victoriasJugador = 0;
        private int victoriasIA = 0;
        private int empates = 0;

        public Form1()
        {
            InitializeComponent();
            baraja = new Baraja();
            motorIA = new MotorIA();
            manoJugador = new List<Carta>();
            manoIA = new List<Carta>();

            cuadrosJugador = new PictureBox[] { picJugador0, picJugador1, picJugador2, picJugador3, picJugador4 };
            cuadrosIA = new PictureBox[] { picIA0, picIA1, picIA2, picIA3, picIA4 };
            etiquetasSeleccion = new Label[] { lblSel0, lblSel1, lblSel2, lblSel3, lblSel4 };

            CargarImagenesInterfaz();
            CargarListaEscenarios();
        }

        private void CargarImagenesInterfaz()
        {
            AsignarImagenPictureBox(picPozo, CargarImagenCarta("ficha_pozo.png"));
            AsignarImagenPictureBox(picMazo, CargarImagenCarta("reverso.png"));
            AsignarImagenPictureBox(picFicha25, CargarImagenCarta("ficha_25.png"));
            AsignarImagenPictureBox(picFicha50, CargarImagenCarta("ficha_50.png"));
            AsignarImagenPictureBox(picFicha100, CargarImagenCarta("ficha_100.png"));
            AsignarImagenPictureBox(picFicha250, CargarImagenCarta("ficha_250.png"));
        }

        private void CargarListaEscenarios()
        {
            List<EscenarioDemo> escenarios = PruebasPoker.ObtenerEscenariosDemo();
            cmbEscenarios.Items.Clear();
            for (int i = 0; i < escenarios.Count; i++)
            {
                cmbEscenarios.Items.Add(escenarios[i]);
            }
            if (cmbEscenarios.Items.Count > 0)
            {
                cmbEscenarios.SelectedIndex = 0;
            }
        }

        private void btnRepartir_Click(object sender, EventArgs e)
        {
            baraja.Reiniciar();
            baraja.Barajar();

            manoJugador = baraja.RepartirMano(5);
            manoIA = baraja.RepartirMano(5);

            IniciarRonda("Selecciona las cartas a cambiar y presiona 'Cambiar Cartas'.",
                "Esperando tu turno...");
        }

        private void btnCargarEscenario_Click(object sender, EventArgs e)
        {
            EscenarioDemo escenario = cmbEscenarios.SelectedItem as EscenarioDemo;
            if (escenario == null)
            {
                return;
            }

            PruebasPoker.PrepararEscenarioEnBaraja(escenario, baraja, out manoJugador, out manoIA);
            IniciarRonda("Escenario cargado: " + escenario.Nombre,
                "IA lista para evaluar su mano.");
        }

        private void IniciarRonda(string mensajeEstado, string mensajeIA)
        {
            if (fichasJugador < Ante || fichasIA < Ante)
            {
                fichasJugador = 1000;
                fichasIA = 1000;
            }

            fichasJugador -= Ante;
            fichasIA -= Ante;
            pozo = Ante * 2;
            apuestaActual = 0;

            rondaActiva = true;
            cambioRealizado = false;
            faseApuesta = false;
            decisionActualIA = null;

            btnCambiar.Enabled = true;
            btnCambiar.Text = "Cambiar Cartas (0)";
            HabilitarControlesApuesta(false);

            MostrarCartasJugador();
            MostrarCartasIA(true);

            ResultadoEvaluacion evalInicial = EvaluadorManos.Evaluar(manoJugador);
            lblManoJugador.Text = "Tu jugada actual: " + evalInicial.DescripcionDetallada;
            lblEstado.Text = mensajeEstado;
            lblDecisionIA.Text = mensajeIA;
            ActualizarTextoApuesta();
            ActualizarTextoFichas();
            ActualizarTextoMazo();
        }

        private void CuadroJugador_Click(object sender, EventArgs e)
        {
            if (!rondaActiva || cambioRealizado)
            {
                return;
            }

            PictureBox cuadro = sender as PictureBox;
            int indice = Array.IndexOf(cuadrosJugador, cuadro);
            if (indice < 0 || indice >= manoJugador.Count)
            {
                return;
            }

            Carta carta = manoJugador[indice];
            carta.Seleccionada = !carta.Seleccionada;

            cuadro.Top = carta.Seleccionada ? PosicionYJugador - DesplazamientoSeleccion : PosicionYJugador;
            etiquetasSeleccion[indice].Visible = carta.Seleccionada;
            btnCambiar.Text = "Cambiar Cartas (" + ContarSeleccionadas() + ")";
        }

        private void btnCambiar_Click(object sender, EventArgs e)
        {
            if (!rondaActiva || cambioRealizado)
            {
                return;
            }

            for (int i = 0; i < manoJugador.Count; i++)
            {
                if (manoJugador[i].Seleccionada)
                {
                    manoJugador[i] = baraja.RepartirCarta();
                }
                manoJugador[i].Seleccionada = false;
                etiquetasSeleccion[i].Visible = false;
            }

            decisionActualIA = motorIA.DecidirCambios(manoIA);
            for (int i = 0; i < decisionActualIA.IndicesACambiar.Count; i++)
            {
                int idx = decisionActualIA.IndicesACambiar[i];
                manoIA[idx] = baraja.RepartirCarta();
            }

            cambioRealizado = true;
            faseApuesta = true;
            btnCambiar.Enabled = false;
            btnCambiar.Text = "Cambiar Cartas (0)";

            MostrarCartasJugador();
            MostrarCartasIA(true);
            ActualizarTextoMazo();

            ResultadoEvaluacion evalJugador = EvaluadorManos.Evaluar(manoJugador);
            lblManoJugador.Text = "Tu jugada actual: " + evalJugador.DescripcionDetallada;

            int maxApuesta = Math.Min(fichasJugador, fichasIA);
            apuestaActual = Math.Min(50, maxApuesta);
            ActualizarTextoApuesta();
            HabilitarControlesApuesta(true);

            lblDecisionIA.Text = "La IA cambio " + decisionActualIA.IndicesACambiar.Count + " carta(s). Esperando tu apuesta...";
            lblEstado.Text = "Usa las fichas para apostar o presiona 'Retirarse'.";
        }

        private void picFicha25_Click(object sender, EventArgs e)
        {
            AgregarApuesta(25);
        }

        private void picFicha50_Click(object sender, EventArgs e)
        {
            AgregarApuesta(50);
        }

        private void picFicha100_Click(object sender, EventArgs e)
        {
            AgregarApuesta(100);
        }

        private void picFicha250_Click(object sender, EventArgs e)
        {
            AgregarApuesta(250);
        }

        private void btnLimpiarApuesta_Click(object sender, EventArgs e)
        {
            if (!rondaActiva || !faseApuesta)
            {
                return;
            }

            apuestaActual = 0;
            ActualizarTextoApuesta();
        }

        private void AgregarApuesta(int monto)
        {
            if (!rondaActiva || !faseApuesta)
            {
                return;
            }

            int maximo = Math.Min(fichasJugador, fichasIA);
            apuestaActual += monto;
            if (apuestaActual > maximo)
            {
                apuestaActual = maximo;
            }
            ActualizarTextoApuesta();
        }

        private void btnApostar_Click(object sender, EventArgs e)
        {
            if (!rondaActiva || !faseApuesta)
            {
                return;
            }

            int apuestaJugador = apuestaActual;
            fichasJugador -= apuestaJugador;
            pozo += apuestaJugador;

            int fichasMaximasSubida = Math.Min(fichasJugador, fichasIA - apuestaJugador);
            DecisionApuestaIA decApuesta = motorIA.DecidirApuesta(manoIA, apuestaJugador, fichasMaximasSubida);

            HabilitarControlesApuesta(false);
            faseApuesta = false;
            rondaActiva = false;

            string textoCambioIA = decisionActualIA != null ? decisionActualIA.Explicacion + " | " : "";

            if (decApuesta.Accion == AccionApuesta.Retirarse)
            {
                fichasJugador += pozo;
                pozo = 0;
                victoriasJugador++;

                MostrarCartasIA(false);
                ActualizarTextoFichas();
                ActualizarMarcador();

                lblDecisionIA.Text = textoCambioIA + decApuesta.Explicacion;
                lblEstado.Text = "La IA se retiro. Ganaste el pozo.";
                return;
            }

            fichasIA -= apuestaJugador;
            pozo += apuestaJugador;

            if (decApuesta.Accion == AccionApuesta.Subir && decApuesta.MontoSubida > 0)
            {
                int subida = Math.Min(decApuesta.MontoSubida, Math.Min(fichasJugador, fichasIA));
                fichasJugador -= subida;
                fichasIA -= subida;
                pozo += subida * 2;
            }

            MostrarCartasIA(false);

            ResultadoEvaluacion evalJugador = EvaluadorManos.Evaluar(manoJugador);
            ResultadoEvaluacion evalIA = EvaluadorManos.Evaluar(manoIA);

            lblManoJugador.Text = "Tu jugada final: " + evalJugador.DescripcionDetallada;
            lblDecisionIA.Text = textoCambioIA + decApuesta.Explicacion + " | Mano final IA: " + evalIA.DescripcionDetallada;

            int comparacion = evalJugador.CompararCon(evalIA);
            if (comparacion > 0)
            {
                victoriasJugador++;
                fichasJugador += pozo;
                lblEstado.Text = string.Format("Ganaste +${0} con {1} contra {2}.",
                    pozo, evalJugador.DescripcionDetallada, evalIA.DescripcionDetallada);
                pozo = 0;
            }
            else if (comparacion < 0)
            {
                victoriasIA++;
                fichasIA += pozo;
                lblEstado.Text = string.Format("Gana la Computadora (+${0}) con {1} contra {2}.",
                    pozo, evalIA.DescripcionDetallada, evalJugador.DescripcionDetallada);
                pozo = 0;
            }
            else
            {
                empates++;
                int mitad = pozo / 2;
                fichasJugador += mitad;
                fichasIA += (pozo - mitad);
                pozo = 0;
                lblEstado.Text = string.Format("Empate con {0}. Se reparte el pozo.", evalJugador.DescripcionDetallada);
            }

            ActualizarTextoFichas();
            ActualizarMarcador();
        }

        private void btnRetirarse_Click(object sender, EventArgs e)
        {
            if (!rondaActiva || !faseApuesta)
            {
                return;
            }

            fichasIA += pozo;
            pozo = 0;
            victoriasIA++;

            HabilitarControlesApuesta(false);
            faseApuesta = false;
            rondaActiva = false;

            MostrarCartasIA(false);
            ActualizarTextoFichas();
            ActualizarMarcador();

            lblDecisionIA.Text = decisionActualIA != null ? decisionActualIA.Explicacion : "La IA gana por retiro.";
            lblEstado.Text = "Te retiraste. La computadora gana el pozo.";
        }

        private void btnPruebas_Click(object sender, EventArgs e)
        {
            string reporte;
            PruebasPoker.EjecutarBateriaPruebas(out reporte);
            MessageBox.Show(reporte, "Pruebas");
        }

        private int ContarSeleccionadas()
        {
            int total = 0;
            for (int i = 0; i < manoJugador.Count; i++)
            {
                if (manoJugador[i].Seleccionada)
                {
                    total++;
                }
            }
            return total;
        }

        private void HabilitarControlesApuesta(bool habilitar)
        {
            picFicha25.Enabled = habilitar;
            picFicha50.Enabled = habilitar;
            picFicha100.Enabled = habilitar;
            picFicha250.Enabled = habilitar;
            btnLimpiarApuesta.Enabled = habilitar;
            btnApostar.Enabled = habilitar;
            btnRetirarse.Enabled = habilitar;
        }

        private void MostrarCartasJugador()
        {
            for (int i = 0; i < 5; i++)
            {
                cuadrosJugador[i].Top = manoJugador[i].Seleccionada
                    ? PosicionYJugador - DesplazamientoSeleccion
                    : PosicionYJugador;
                etiquetasSeleccion[i].Visible = manoJugador[i].Seleccionada;
                AsignarImagenPictureBox(cuadrosJugador[i], CargarImagenCarta(manoJugador[i].ObtenerNombreArchivo()));
            }
        }

        private void MostrarCartasIA(bool bocaAbajo)
        {
            for (int i = 0; i < 5; i++)
            {
                string archivo = bocaAbajo ? "reverso.png" : manoIA[i].ObtenerNombreArchivo();
                AsignarImagenPictureBox(cuadrosIA[i], CargarImagenCarta(archivo));
            }
        }

        private Image CargarImagenCarta(string nombreArchivo)
        {
            string ruta = Path.Combine(Application.StartupPath, "Cartas", nombreArchivo);
            if (!File.Exists(ruta))
            {
                return null;
            }

            return Image.FromFile(ruta);
        }

        private void AsignarImagenPictureBox(PictureBox cuadro, Image nuevaImagen)
        {
            Image imagenAnterior = cuadro.Image;
            cuadro.Image = nuevaImagen;
            if (imagenAnterior != null)
            {
                imagenAnterior.Dispose();
            }
        }

        private void ActualizarTextoApuesta()
        {
            lblApuestaActual.Text = "Apuesta:\r\n$" + apuestaActual;
            btnApostar.Text = (apuestaActual == 0) ? "Pasar" : "Apostar";
        }

        private void ActualizarTextoMazo()
        {
            lblMazo.Text = "Baraja:\r\n" + baraja.CartasRestantes + " cartas";
        }

        private void ActualizarTextoFichas()
        {
            lblPozo.Text = "$" + pozo;
            lblFichasJugador.Text = "Jugador: $" + fichasJugador;
            lblFichasIA.Text = "Computadora: $" + fichasIA;
        }

        private void ActualizarMarcador()
        {
            lblMarcador.Text = string.Format(
                "Jugador: {0}\r\nComputadora: {1}\r\nEmpates: {2}",
                victoriasJugador, victoriasIA, empates);
        }
    }
}