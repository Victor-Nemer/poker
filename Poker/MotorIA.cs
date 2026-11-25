
using System;
using System.Collections.Generic;

namespace Poker
{
    public enum AccionApuesta
    {
        PasarIgualar,
        Subir,
        Retirarse
    }

    public class DecisionApuestaIA
    {
        public AccionApuesta Accion { get; set; }
        public int MontoSubida { get; set; }
        public string Explicacion { get; set; }
    }

    public class DecisionIA
    {
        public List<int> IndicesACambiar { get; set; }
        public string ReglaAplicada { get; set; }
        public int Outs { get; set; }
        public double ProbabilidadMejora { get; set; }
        public string Explicacion { get; set; }

        public DecisionIA()
        {
            IndicesACambiar = new List<int>();
        }
    }

    public class MotorIA
    {
        private const double CartasDesconocidas = 47.0;

        public DecisionIA DecidirCambios(List<Carta> manoIA)
        {
            DecisionIA decision = new DecisionIA();
            if (manoIA == null || manoIA.Count != 5)
            {
                return decision;
            }

            ResultadoEvaluacion eval = EvaluadorManos.Evaluar(manoIA);
            if (eval == null)
            {
                return decision;
            }

            // Mano hecha fuerte
            if (eval.Categoria >= CategoriaMano.Escalera)
            {
                decision.ReglaAplicada = "Conservar mano formada";
                decision.Outs = 47;
                decision.ProbabilidadMejora = 100.0;
                decision.Explicacion = string.Format(
                    "IA detecto {0}. Conserva las 5 cartas (100%).",
                    eval.DescripcionDetallada);
                return decision;
            }

            // Tercia: conservar las 3 iguales y cambiar 2
            if (eval.Categoria == CategoriaMano.Tercia)
            {
                int valorTercia = eval.ValoresDesempate[0];
                for (int i = 0; i < manoIA.Count; i++)
                {
                    if (manoIA[i].Valor != valorTercia)
                    {
                        decision.IndicesACambiar.Add(i);
                    }
                }

                decision.ReglaAplicada = "Tercia: buscar Full o Poker";
                decision.Outs = 7;
                decision.ProbabilidadMejora = Math.Round((7.0 / CartasDesconocidas) * 100.0, 2);
                decision.Explicacion = string.Format(
                    "IA detecto {0}. Cambia 2 cartas (Outs: 7/47, Prob: {1:0.00}%).",
                    eval.DescripcionDetallada, decision.ProbabilidadMejora);
                return decision;
            }

            // Doble Par: conservar pares y cambiar kicker
            if (eval.Categoria == CategoriaMano.DoblePar)
            {
                int valorKicker = eval.ValoresDesempate[2];
                for (int i = 0; i < manoIA.Count; i++)
                {
                    if (manoIA[i].Valor == valorKicker)
                    {
                        decision.IndicesACambiar.Add(i);
                        break;
                    }
                }

                decision.ReglaAplicada = "Doble Par: buscar Full House";
                decision.Outs = 4;
                decision.ProbabilidadMejora = Math.Round((4.0 / CartasDesconocidas) * 100.0, 2);
                decision.Explicacion = string.Format(
                    "IA detecto {0}. Cambia 1 carta (Outs: 4/47, Prob: {1:0.00}%).",
                    eval.DescripcionDetallada, decision.ProbabilidadMejora);
                return decision;
            }

            // Proyecto de Color (4 cartas del mismo palo)
            if (eval.Categoria == CategoriaMano.CartaAlta ||
                (eval.Categoria == CategoriaMano.Par && eval.ValoresDesempate[0] < 11))
            {
                int indiceDescarteColor = BuscarIndiceDescarteProyectoColor(manoIA);
                if (indiceDescarteColor >= 0)
                {
                    decision.IndicesACambiar.Add(indiceDescarteColor);
                    decision.ReglaAplicada = "Proyecto de Color (4 cartas)";
                    decision.Outs = 9;
                    decision.ProbabilidadMejora = Math.Round((9.0 / CartasDesconocidas) * 100.0, 2);
                    decision.Explicacion = string.Format(
                        "IA detecto Proyecto de Color. Cambia 1 carta (Outs: 9/47, Prob: {0:0.00}%).",
                        decision.ProbabilidadMejora);
                    return decision;
                }
            }

            // Proyecto de Escalera Abierta (4 consecutivas)
            if (eval.Categoria == CategoriaMano.CartaAlta)
            {
                int indiceDescarteAbierta = BuscarIndiceDescarteEscaleraAbierta(manoIA);
                if (indiceDescarteAbierta >= 0)
                {
                    decision.IndicesACambiar.Add(indiceDescarteAbierta);
                    decision.ReglaAplicada = "Proyecto de Escalera Abierta";
                    decision.Outs = 8;
                    decision.ProbabilidadMejora = Math.Round((8.0 / CartasDesconocidas) * 100.0, 2);
                    decision.Explicacion = string.Format(
                        "IA detecto Escalera Abierta. Cambia 1 carta (Outs: 8/47, Prob: {0:0.00}%).",
                        decision.ProbabilidadMejora);
                    return decision;
                }
            }

            // Un Par: conservar el par y cambiar 3
            if (eval.Categoria == CategoriaMano.Par)
            {
                int valorPar = eval.ValoresDesempate[0];
                for (int i = 0; i < manoIA.Count; i++)
                {
                    if (manoIA[i].Valor != valorPar)
                    {
                        decision.IndicesACambiar.Add(i);
                    }
                }

                decision.ReglaAplicada = "Un Par: conservar par y cambiar 3";
                decision.Outs = 2;
                decision.ProbabilidadMejora = Math.Round((2.0 / CartasDesconocidas) * 100.0, 2);
                decision.Explicacion = string.Format(
                    "IA detecto {0}. Cambia 3 cartas (Outs a tercia: 2/47, Prob: {1:0.00}%).",
                    eval.DescripcionDetallada, decision.ProbabilidadMejora);
                return decision;
            }

            // Escalera Interna
            if (eval.Categoria == CategoriaMano.CartaAlta)
            {
                int indiceDescarteInterna = BuscarIndiceDescarteEscaleraInterna(manoIA);
                if (indiceDescarteInterna >= 0)
                {
                    decision.IndicesACambiar.Add(indiceDescarteInterna);
                    decision.ReglaAplicada = "Proyecto de Escalera Interna";
                    decision.Outs = 4;
                    decision.ProbabilidadMejora = Math.Round((4.0 / CartasDesconocidas) * 100.0, 2);
                    decision.Explicacion = string.Format(
                        "IA detecto Escalera Interna. Cambia 1 carta (Outs: 4/47, Prob: {0:0.00}%).",
                        decision.ProbabilidadMejora);
                    return decision;
                }
            }

            // Carta Alta: conservar cartas altas (J, Q, K, A) o la mayor
            List<int> indicesOrdenados = new List<int> { 0, 1, 2, 3, 4 };
            indicesOrdenados.Sort((a, b) => manoIA[b].Valor.CompareTo(manoIA[a].Valor));

            int altas = 0;
            for (int i = 0; i < manoIA.Count; i++)
            {
                if (manoIA[i].Valor >= 11)
                {
                    altas++;
                }
            }

            int cartasAConservar = (altas >= 2) ? 2 : 1;
            for (int i = cartasAConservar; i < indicesOrdenados.Count; i++)
            {
                decision.IndicesACambiar.Add(indicesOrdenados[i]);
            }
            decision.IndicesACambiar.Sort();

            decision.Outs = (cartasAConservar == 2) ? 6 : 3;
            decision.ReglaAplicada = (cartasAConservar == 2)
                ? "Carta Alta: conservar 2 cartas altas"
                : "Carta Alta: conservar carta mayor";
            decision.ProbabilidadMejora = Math.Round((decision.Outs / CartasDesconocidas) * 100.0, 2);

            decision.Explicacion = string.Format(
                "IA tiene {0}. Conserva {1} carta(s) y cambia {2} (Outs: {3}/47, Prob: {4:0.00}%).",
                eval.DescripcionDetallada, cartasAConservar, decision.IndicesACambiar.Count,
                decision.Outs, decision.ProbabilidadMejora);
            return decision;
        }

        public DecisionApuestaIA DecidirApuesta(List<Carta> manoIA, int apuestaJugador, int fichasMaximas)
        {
            DecisionApuestaIA decision = new DecisionApuestaIA();
            if (manoIA == null || manoIA.Count != 5)
            {
                decision.Accion = AccionApuesta.PasarIgualar;
                decision.Explicacion = "IA pasa.";
                return decision;
            }

            ResultadoEvaluacion eval = EvaluadorManos.Evaluar(manoIA);
            if (eval == null)
            {
                decision.Accion = AccionApuesta.PasarIgualar;
                decision.Explicacion = "IA pasa.";
                return decision;
            }

            if (eval.Categoria == CategoriaMano.CartaAlta)
            {
                if (apuestaJugador > 100)
                {
                    decision.Accion = AccionApuesta.Retirarse;
                    decision.Explicacion = "IA tiene Carta Alta y se retira ante una apuesta de " + apuestaJugador + ".";
                }
                else
                {
                    decision.Accion = AccionApuesta.PasarIgualar;
                    decision.Explicacion = (apuestaJugador == 0)
                        ? "IA tiene Carta Alta y pasa."
                        : "IA tiene Carta Alta e iguala las " + apuestaJugador + " fichas.";
                }
                return decision;
            }

            if (eval.Categoria == CategoriaMano.Par || eval.Categoria == CategoriaMano.DoblePar)
            {
                decision.Accion = AccionApuesta.PasarIgualar;
                decision.Explicacion = "IA tiene " + eval.NombreJugada + " e iguala la apuesta.";
                return decision;
            }

            if (fichasMaximas >= 50)
            {
                decision.Accion = AccionApuesta.Subir;
                decision.MontoSubida = Math.Min(100, fichasMaximas);
                decision.Explicacion = "IA tiene " + eval.NombreJugada + " y sube la apuesta +" + decision.MontoSubida + " fichas.";
            }
            else
            {
                decision.Accion = AccionApuesta.PasarIgualar;
                decision.Explicacion = "IA tiene " + eval.NombreJugada + " e iguala la apuesta.";
            }

            return decision;
        }

        private int BuscarIndiceDescarteProyectoColor(List<Carta> mano)
        {
            Dictionary<Palo, int> conteoPalos = new Dictionary<Palo, int>();
            for (int i = 0; i < mano.Count; i++)
            {
                Palo p = mano[i].PaloCarta;
                conteoPalos[p] = conteoPalos.ContainsKey(p) ? conteoPalos[p] + 1 : 1;
            }

            foreach (KeyValuePair<Palo, int> par in conteoPalos)
            {
                if (par.Value == 4)
                {
                    for (int i = 0; i < mano.Count; i++)
                    {
                        if (mano[i].PaloCarta != par.Key)
                        {
                            return i;
                        }
                    }
                }
            }

            return -1;
        }

        private int BuscarIndiceDescarteEscaleraAbierta(List<Carta> mano)
        {
            for (int descarte = 0; descarte < mano.Count; descarte++)
            {
                List<int> cuatro = ObtenerValoresSinIndice(mano, descarte);
                cuatro.Sort();

                if (TieneValoresDistintos(cuatro) &&
                    cuatro[1] == cuatro[0] + 1 &&
                    cuatro[2] == cuatro[1] + 1 &&
                    cuatro[3] == cuatro[2] + 1 &&
                    cuatro[0] >= 2 &&
                    cuatro[3] <= 13)
                {
                    return descarte;
                }
            }

            return -1;
        }

        private int BuscarIndiceDescarteEscaleraInterna(List<Carta> mano)
        {
            for (int descarte = 0; descarte < mano.Count; descarte++)
            {
                List<int> cuatro = ObtenerValoresSinIndice(mano, descarte);
                cuatro.Sort();

                if (!TieneValoresDistintos(cuatro))
                {
                    continue;
                }

                if (cuatro[3] - cuatro[0] <= 4)
                {
                    return descarte;
                }

                if (cuatro.Contains(14))
                {
                    List<int> conAsBajo = new List<int>();
                    for (int i = 0; i < cuatro.Count; i++)
                    {
                        conAsBajo.Add(cuatro[i] == 14 ? 1 : cuatro[i]);
                    }
                    conAsBajo.Sort();

                    if (conAsBajo[3] - conAsBajo[0] <= 4)
                    {
                        return descarte;
                    }
                }
            }

            return -1;
        }

        private List<int> ObtenerValoresSinIndice(List<Carta> mano, int indiceExcluido)
        {
            List<int> resultado = new List<int>();
            for (int i = 0; i < mano.Count; i++)
            {
                if (i != indiceExcluido)
                {
                    resultado.Add(mano[i].Valor);
                }
            }
            return resultado;
        }

        private bool TieneValoresDistintos(List<int> ordenados)
        {
            for (int i = 1; i < ordenados.Count; i++)
            {
                if (ordenados[i] == ordenados[i - 1])
                {
                    return false;
                }
            }
            return true;
        }
    }
}