using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Poker
{
    public partial class Form1 : Form
    {
        private const int PosicionYJugador = 452;
        private const int DesplazamientoSeleccion = 15;
        private const int Ante = 50;

        private Baraja baraja;
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
            manoJugador = new List<Carta>();
            manoIA = new List<Carta>();

            cuadrosJugador = new PictureBox[] { picJugador0, picJugador1, picJugador2, picJugador3, picJugador4 };
            cuadrosIA = new PictureBox[] { picIA0, picIA1, picIA2, picIA3, picIA4 };
            etiquetasSeleccion = new Label[] { lblSel0, lblSel1, lblSel2, lblSel3, lblSel4 };

            CargarImagenesInterfaz();
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

        private void btnRepartir_Click(object sender, EventArgs e)
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

            baraja.Reiniciar();
            baraja.Barajar();

            manoJugador = baraja.RepartirMano(5);
            manoIA = baraja.RepartirMano(5);

            rondaActiva = true;
            cambioRealizado = false;
            faseApuesta = false;

            btnCambiar.Enabled = true;
            btnCambiar.Text = "Cambiar Cartas (0)";
            HabilitarControlesApuesta(false);

            MostrarCartasJugador();
            MostrarCartasIA(true);

            lblEstado.Text = "Selecciona las cartas que quieras cambiar y presiona 'Cambiar Cartas'.";
            lblDecisionIA.Text = "Turno del jugador...";
            lblManoJugador.Text = "";
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

            int cantidadCambiada = 0;
            for (int i = 0; i < manoJugador.Count; i++)
            {
                if (manoJugador[i].Seleccionada)
                {
                    manoJugador[i] = baraja.RepartirCarta();
                    cantidadCambiada++;
                }
                manoJugador[i].Seleccionada = false;
                etiquetasSeleccion[i].Visible = false;
            }

            cambioRealizado = true;
            faseApuesta = true;
            btnCambiar.Enabled = false;
            btnCambiar.Text = "Cambiar Cartas (0)";

            MostrarCartasJugador();
            MostrarCartasIA(true);
            ActualizarTextoMazo();

            int maxApuesta = Math.Min(fichasJugador, fichasIA);
            apuestaActual = Math.Min(50, maxApuesta);
            ActualizarTextoApuesta();
            HabilitarControlesApuesta(true);

            lblEstado.Text = "Cambiaste " + cantidadCambiada + " carta(s). Usa las fichas para apostar o presiona 'Retirarse'.";
            lblDecisionIA.Text = "Fase de apuesta...";
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

            fichasJugador -= apuestaActual;
            fichasIA -= apuestaActual;
            pozo += apuestaActual * 2;

            HabilitarControlesApuesta(false);
            faseApuesta = false;
            rondaActiva = false;

            MostrarCartasIA(false);
            ActualizarTextoFichas();

            lblEstado.Text = (apuestaActual == 0)
                ? "Pasaste (Pozo final: $" + pozo + "). Fin de la ronda."
                : "Apostaste $" + apuestaActual + " (Pozo final: $" + pozo + "). Fin de la ronda.";
            lblDecisionIA.Text = "Cartas de la computadora reveladas.";
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

            lblEstado.Text = "Te retiraste. La computadora gana el pozo.";
            lblDecisionIA.Text = "Cartas de la computadora reveladas.";
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
