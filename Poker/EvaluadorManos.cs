using System;
using System.Collections.Generic;

namespace Poker
{
    public enum CategoriaMano
    {
        CartaAlta = 0,
        Par = 1,
        DoblePar = 2,
        Tercia = 3,
        Escalera = 4,
        Color = 5,
        FullHouse = 6,
        Poker = 7,
        EscaleraColor = 8
    }

    public class ResultadoEvaluacion
    {
        public CategoriaMano Categoria { get; set; }
        public string NombreJugada { get; set; }
        public List<int> ValoresDesempate { get; set; }
        public string DescripcionDetallada { get; set; }

        public ResultadoEvaluacion()
        {
            ValoresDesempate = new List<int>();
        }

        public int CompararCon(ResultadoEvaluacion otra)
        {
            if (otra == null)
            {
                return 1;
            }

            int compCategoria = ((int)Categoria).CompareTo((int)otra.Categoria);
            if (compCategoria != 0)
            {
                return compCategoria;
            }

            int limite = Math.Min(ValoresDesempate.Count, otra.ValoresDesempate.Count);
            for (int i = 0; i < limite; i++)
            {
                int compValor = ValoresDesempate[i].CompareTo(otra.ValoresDesempate[i]);
                if (compValor != 0)
                {
                    return compValor;
                }
            }

            return 0;
        }
    }

    public static class EvaluadorManos
    {
        public static ResultadoEvaluacion Evaluar(List<Carta> mano)
        {
            if (mano == null || mano.Count != 5)
            {
                return null;
            }

            Dictionary<int, int> conteo = new Dictionary<int, int>();
            for (int i = 0; i < mano.Count; i++)
            {
                int v = mano[i].Valor;
                if (conteo.ContainsKey(v))
                {
                    conteo[v]++;
                }
                else
                {
                    conteo[v] = 1;
                }
            }

            List<KeyValuePair<int, int>> grupos = new List<KeyValuePair<int, int>>(conteo);
            grupos.Sort((a, b) =>
            {
                int compFrec = b.Value.CompareTo(a.Value);
                if (compFrec != 0)
                {
                    return compFrec;
                }
                return b.Key.CompareTo(a.Key);
            });

            bool esColor = VerificarColor(mano);
            int valorAltoEscalera;
            bool esEscalera = VerificarEscalera(mano, grupos.Count, out valorAltoEscalera);

            ResultadoEvaluacion resultado = new ResultadoEvaluacion();

            if (esColor && esEscalera)
            {
                resultado.Categoria = CategoriaMano.EscaleraColor;
                resultado.NombreJugada = "Escalera de Color";
                resultado.ValoresDesempate.Add(valorAltoEscalera);
                resultado.DescripcionDetallada = "Escalera de Color al " + FormatearValor(valorAltoEscalera);
                return resultado;
            }

            if (grupos[0].Value == 4)
            {
                resultado.Categoria = CategoriaMano.Poker;
                resultado.NombreJugada = "Poker";
                resultado.ValoresDesempate.Add(grupos[0].Key);
                resultado.ValoresDesempate.Add(grupos[1].Key);
                resultado.DescripcionDetallada = "Poker de " + FormatearValor(grupos[0].Key);
                return resultado;
            }

            if (grupos[0].Value == 3 && grupos[1].Value == 2)
            {
                resultado.Categoria = CategoriaMano.FullHouse;
                resultado.NombreJugada = "Full House";
                resultado.ValoresDesempate.Add(grupos[0].Key);
                resultado.ValoresDesempate.Add(grupos[1].Key);
                resultado.DescripcionDetallada = "Full House de " + FormatearValor(grupos[0].Key) + " y " + FormatearValor(grupos[1].Key);
                return resultado;
            }

            if (esColor)
            {
                resultado.Categoria = CategoriaMano.Color;
                resultado.NombreJugada = "Color";
                AgregarValoresOrdenados(resultado.ValoresDesempate, grupos);
                resultado.DescripcionDetallada = "Color carta alta " + FormatearValor(resultado.ValoresDesempate[0]);
                return resultado;
            }

            if (esEscalera)
            {
                resultado.Categoria = CategoriaMano.Escalera;
                resultado.NombreJugada = "Escalera";
                resultado.ValoresDesempate.Add(valorAltoEscalera);
                resultado.DescripcionDetallada = "Escalera al " + FormatearValor(valorAltoEscalera);
                return resultado;
            }

            if (grupos[0].Value == 3)
            {
                resultado.Categoria = CategoriaMano.Tercia;
                resultado.NombreJugada = "Tercia";
                AgregarValoresOrdenados(resultado.ValoresDesempate, grupos);
                resultado.DescripcionDetallada = "Tercia de " + FormatearValor(grupos[0].Key);
                return resultado;
            }

            if (grupos[0].Value == 2 && grupos[1].Value == 2)
            {
                resultado.Categoria = CategoriaMano.DoblePar;
                resultado.NombreJugada = "Doble Par";
                AgregarValoresOrdenados(resultado.ValoresDesempate, grupos);
                resultado.DescripcionDetallada = "Doble Par (" + FormatearValor(grupos[0].Key) + " y " + FormatearValor(grupos[1].Key) + ")";
                return resultado;
            }

            if (grupos[0].Value == 2)
            {
                resultado.Categoria = CategoriaMano.Par;
                resultado.NombreJugada = "Par";
                AgregarValoresOrdenados(resultado.ValoresDesempate, grupos);
                resultado.DescripcionDetallada = "Par de " + FormatearValor(grupos[0].Key);
                return resultado;
            }

            resultado.Categoria = CategoriaMano.CartaAlta;
            resultado.NombreJugada = "Carta Alta";
            AgregarValoresOrdenados(resultado.ValoresDesempate, grupos);
            resultado.DescripcionDetallada = "Carta Alta " + FormatearValor(resultado.ValoresDesempate[0]);
            return resultado;
        }

        private static bool VerificarColor(List<Carta> mano)
        {
            Palo primerPalo = mano[0].PaloCarta;
            for (int i = 1; i < mano.Count; i++)
            {
                if (mano[i].PaloCarta != primerPalo)
                {
                    return false;
                }
            }
            return true;
        }

        private static bool VerificarEscalera(List<Carta> mano, int valoresDistintos, out int valorAlto)
        {
            valorAlto = 0;
            if (valoresDistintos != 5)
            {
                return false;
            }

            List<int> valores = new List<int>();
            for (int i = 0; i < mano.Count; i++)
            {
                valores.Add(mano[i].Valor);
            }
            valores.Sort();

            if (valores[4] - valores[0] == 4)
            {
                valorAlto = valores[4];
                return true;
            }

            if (valores[0] == 2 && valores[1] == 3 && valores[2] == 4 && valores[3] == 5 && valores[4] == 14)
            {
                valorAlto = 5;
                return true;
            }

            return false;
        }

        private static void AgregarValoresOrdenados(List<int> destino, List<KeyValuePair<int, int>> grupos)
        {
            for (int i = 0; i < grupos.Count; i++)
            {
                destino.Add(grupos[i].Key);
            }
        }

        private static string FormatearValor(int valor)
        {
            switch (valor)
            {
                case 11: return "J";
                case 12: return "Q";
                case 13: return "K";
                case 14: return "A";
                default: return valor.ToString();
            }
        }
    }
}