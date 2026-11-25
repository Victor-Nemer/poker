using System;
using System.Collections.Generic;
using System.Text;

namespace Poker
{
    public class EscenarioDemo
    {
        public string Nombre { get; set; }
        public List<Carta> ManoJugador { get; set; }
        public List<Carta> ManoIA { get; set; }

        public EscenarioDemo(string nombre, List<Carta> manoJugador, List<Carta> manoIA)
        {
            Nombre = nombre;
            ManoJugador = manoJugador;
            ManoIA = manoIA;
        }

        public override string ToString()
        {
            return Nombre;
        }
    }

    public static class PruebasPoker
    {
        public static List<EscenarioDemo> ObtenerEscenariosDemo()
        {
            List<EscenarioDemo> lista = new List<EscenarioDemo>();

            lista.Add(new EscenarioDemo(
                "Caso 1: IA con Proyecto de Color (9/47 = 19.15%)",
                new List<Carta>
                {
                    new Carta(10, Palo.Espadas), new Carta(10, Palo.Diamantes),
                    new Carta(6, Palo.Treboles), new Carta(4, Palo.Corazones), new Carta(2, Palo.Espadas)
                },
                new List<Carta>
                {
                    new Carta(2, Palo.Corazones), new Carta(7, Palo.Corazones),
                    new Carta(11, Palo.Corazones), new Carta(14, Palo.Corazones), new Carta(9, Palo.Treboles)
                }
            ));

            lista.Add(new EscenarioDemo(
                "Caso 2: IA con Escalera Abierta (8/47 = 17.02%)",
                new List<Carta>
                {
                    new Carta(12, Palo.Espadas), new Carta(12, Palo.Treboles),
                    new Carta(3, Palo.Corazones), new Carta(5, Palo.Diamantes), new Carta(8, Palo.Corazones)
                },
                new List<Carta>
                {
                    new Carta(6, Palo.Diamantes), new Carta(7, Palo.Treboles),
                    new Carta(8, Palo.Espadas), new Carta(9, Palo.Corazones), new Carta(13, Palo.Diamantes)
                }
            ));

            lista.Add(new EscenarioDemo(
                "Caso 3: IA con Escalera Interna (4/47 = 8.51%)",
                new List<Carta>
                {
                    new Carta(9, Palo.Treboles), new Carta(9, Palo.Diamantes),
                    new Carta(4, Palo.Espadas), new Carta(2, Palo.Corazones), new Carta(7, Palo.Diamantes)
                },
                new List<Carta>
                {
                    new Carta(5, Palo.Corazones), new Carta(6, Palo.Espadas),
                    new Carta(8, Palo.Diamantes), new Carta(9, Palo.Espadas), new Carta(13, Palo.Treboles)
                }
            ));

            lista.Add(new EscenarioDemo(
                "Caso 4: IA con Doble Par busca Full (4/47 = 8.51%)",
                new List<Carta>
                {
                    new Carta(13, Palo.Corazones), new Carta(13, Palo.Espadas),
                    new Carta(5, Palo.Treboles), new Carta(3, Palo.Diamantes), new Carta(2, Palo.Treboles)
                },
                new List<Carta>
                {
                    new Carta(11, Palo.Corazones), new Carta(11, Palo.Diamantes),
                    new Carta(4, Palo.Espadas), new Carta(4, Palo.Treboles), new Carta(9, Palo.Corazones)
                }
            ));

            lista.Add(new EscenarioDemo(
                "Caso 5: Desempate Escalera Baja (A-2-3-4-5 vs 2-3-4-5-6)",
                new List<Carta>
                {
                    new Carta(14, Palo.Espadas), new Carta(2, Palo.Corazones),
                    new Carta(3, Palo.Diamantes), new Carta(4, Palo.Treboles), new Carta(5, Palo.Espadas)
                },
                new List<Carta>
                {
                    new Carta(2, Palo.Diamantes), new Carta(3, Palo.Corazones),
                    new Carta(4, Palo.Espadas), new Carta(5, Palo.Treboles), new Carta(6, Palo.Corazones)
                }
            ));

            lista.Add(new EscenarioDemo(
                "Caso 6: Desempate por Kicker (Par de Q con As vs con J)",
                new List<Carta>
                {
                    new Carta(12, Palo.Corazones), new Carta(12, Palo.Espadas),
                    new Carta(14, Palo.Diamantes), new Carta(8, Palo.Treboles), new Carta(3, Palo.Corazones)
                },
                new List<Carta>
                {
                    new Carta(12, Palo.Diamantes), new Carta(12, Palo.Treboles),
                    new Carta(11, Palo.Espadas), new Carta(10, Palo.Corazones), new Carta(9, Palo.Diamantes)
                }
            ));

            return lista;
        }

        public static void PrepararEscenarioEnBaraja(EscenarioDemo escenario, Baraja baraja, out List<Carta> manoJugador, out List<Carta> manoIA)
        {
            baraja.Reiniciar();
            baraja.Barajar();

            manoJugador = new List<Carta>();
            manoIA = new List<Carta>();

            for (int i = 0; i < escenario.ManoJugador.Count; i++)
            {
                Carta c = escenario.ManoJugador[i];
                manoJugador.Add(baraja.ExtraerCarta(c.Valor, c.PaloCarta));
            }

            for (int i = 0; i < escenario.ManoIA.Count; i++)
            {
                Carta c = escenario.ManoIA[i];
                manoIA.Add(baraja.ExtraerCarta(c.Valor, c.PaloCarta));
            }
        }

        public static bool EjecutarBateriaPruebas(out string reporte)
        {
            StringBuilder sb = new StringBuilder();
            int total = 0;
            int aprobadas = 0;

            sb.AppendLine("Resumen de pruebas del proyecto:");
            sb.AppendLine();

            // 1. Baraja completa sin duplicados
            total++;
            Baraja b = new Baraja();
            HashSet<string> unicas = new HashSet<string>();
            while (b.CartasRestantes > 0)
            {
                unicas.Add(b.RepartirCarta().ToString());
            }
            bool okBaraja = (unicas.Count == 52);
            if (okBaraja) aprobadas++;
            sb.AppendLine(string.Format("- Baraja sin duplicados: {0} ({1}/52 cartas)",
                okBaraja ? "OK" : "Error", unicas.Count));

            // 2. Orden de las 9 jugadas
            List<List<Carta>> manosJerarquia = new List<List<Carta>>
            {
                CrearMano(2, Palo.Corazones, 5, Palo.Diamantes, 7, Palo.Treboles, 9, Palo.Espadas, 14, Palo.Corazones),
                CrearMano(8, Palo.Corazones, 8, Palo.Diamantes, 3, Palo.Treboles, 10, Palo.Espadas, 14, Palo.Corazones),
                CrearMano(4, Palo.Corazones, 4, Palo.Diamantes, 11, Palo.Treboles, 11, Palo.Espadas, 2, Palo.Corazones),
                CrearMano(7, Palo.Corazones, 7, Palo.Diamantes, 7, Palo.Treboles, 13, Palo.Espadas, 2, Palo.Corazones),
                CrearMano(14, Palo.Espadas, 2, Palo.Corazones, 3, Palo.Diamantes, 4, Palo.Treboles, 5, Palo.Espadas),
                CrearMano(2, Palo.Espadas, 6, Palo.Espadas, 9, Palo.Espadas, 11, Palo.Espadas, 13, Palo.Espadas),
                CrearMano(6, Palo.Corazones, 6, Palo.Diamantes, 6, Palo.Treboles, 12, Palo.Espadas, 12, Palo.Corazones),
                CrearMano(9, Palo.Corazones, 9, Palo.Diamantes, 9, Palo.Treboles, 9, Palo.Espadas, 14, Palo.Corazones),
                CrearMano(9, Palo.Corazones, 10, Palo.Corazones, 11, Palo.Corazones, 12, Palo.Corazones, 13, Palo.Corazones)
            };

            CategoriaMano[] esperadas = new CategoriaMano[]
            {
                CategoriaMano.CartaAlta, CategoriaMano.Par, CategoriaMano.DoblePar,
                CategoriaMano.Tercia, CategoriaMano.Escalera, CategoriaMano.Color,
                CategoriaMano.FullHouse, CategoriaMano.Poker, CategoriaMano.EscaleraColor
            };

            bool okCategorias = true;
            for (int i = 0; i < manosJerarquia.Count; i++)
            {
                ResultadoEvaluacion res = EvaluadorManos.Evaluar(manosJerarquia[i]);
                if (res.Categoria != esperadas[i])
                {
                    okCategorias = false;
                }
                if (i > 0)
                {
                    ResultadoEvaluacion ant = EvaluadorManos.Evaluar(manosJerarquia[i - 1]);
                    if (res.CompararCon(ant) <= 0)
                    {
                        okCategorias = false;
                    }
                }
            }
            total++;
            if (okCategorias) aprobadas++;
            sb.AppendLine(string.Format("- Jerarquia de 9 jugadas: {0}", okCategorias ? "OK" : "Error"));

            // 3. Escalera baja A-2-3-4-5
            total++;
            ResultadoEvaluacion rueda = EvaluadorManos.Evaluar(manosJerarquia[4]);
            ResultadoEvaluacion esc6 = EvaluadorManos.Evaluar(
                CrearMano(2, Palo.Diamantes, 3, Palo.Corazones, 4, Palo.Espadas, 5, Palo.Treboles, 6, Palo.Corazones));
            bool okRueda = (rueda.ValoresDesempate[0] == 5 && esc6.CompararCon(rueda) > 0);
            if (okRueda) aprobadas++;
            sb.AppendLine(string.Format("- Escalera baja A-2-3-4-5: {0}", okRueda ? "OK" : "Error"));

            // 4. Desempate por kicker
            total++;
            ResultadoEvaluacion parConAs = EvaluadorManos.Evaluar(
                CrearMano(12, Palo.Corazones, 12, Palo.Espadas, 14, Palo.Diamantes, 8, Palo.Treboles, 3, Palo.Corazones));
            ResultadoEvaluacion parConK = EvaluadorManos.Evaluar(
                CrearMano(12, Palo.Diamantes, 12, Palo.Treboles, 13, Palo.Espadas, 10, Palo.Corazones, 9, Palo.Diamantes));
            bool okKicker = (parConAs.CompararCon(parConK) > 0);
            if (okKicker) aprobadas++;
            sb.AppendLine(string.Format("- Desempate por kicker: {0}", okKicker ? "OK" : "Error"));

            // 5. Reglas de descarte de la IA
            MotorIA motor = new MotorIA();

            total++;
            DecisionIA dColor = motor.DecidirCambios(
                CrearMano(2, Palo.Corazones, 7, Palo.Corazones, 11, Palo.Corazones, 14, Palo.Corazones, 9, Palo.Treboles));
            bool okIaColor = (dColor.IndicesACambiar.Count == 1 && dColor.IndicesACambiar[0] == 4 && dColor.Outs == 9 && Math.Abs(dColor.ProbabilidadMejora - 19.15) < 0.05);
            if (okIaColor) aprobadas++;
            sb.AppendLine(string.Format("- IA Proyecto de Color: {0} ({1:0.00}%)", okIaColor ? "OK" : "Error", dColor.ProbabilidadMejora));

            total++;
            DecisionIA dAbierta = motor.DecidirCambios(
                CrearMano(6, Palo.Diamantes, 7, Palo.Treboles, 8, Palo.Espadas, 9, Palo.Corazones, 13, Palo.Diamantes));
            bool okIaAbierta = (dAbierta.IndicesACambiar.Count == 1 && dAbierta.IndicesACambiar[0] == 4 && dAbierta.Outs == 8 && Math.Abs(dAbierta.ProbabilidadMejora - 17.02) < 0.05);
            if (okIaAbierta) aprobadas++;
            sb.AppendLine(string.Format("- IA Escalera Abierta: {0} ({1:0.00}%)", okIaAbierta ? "OK" : "Error", dAbierta.ProbabilidadMejora));

            total++;
            DecisionIA dTopeAs = motor.DecidirCambios(
                CrearMano(11, Palo.Diamantes, 12, Palo.Treboles, 13, Palo.Espadas, 14, Palo.Corazones, 7, Palo.Diamantes));
            bool okIaTopeAs = (dTopeAs.IndicesACambiar.Count == 1 && dTopeAs.IndicesACambiar[0] == 4 && dTopeAs.Outs == 4 && Math.Abs(dTopeAs.ProbabilidadMejora - 8.51) < 0.05);
            if (okIaTopeAs) aprobadas++;
            sb.AppendLine(string.Format("- IA Escalera J-Q-K-A: {0} ({1:0.00}%)", okIaTopeAs ? "OK" : "Error", dTopeAs.ProbabilidadMejora));

            total++;
            DecisionIA dDoblePar = motor.DecidirCambios(
                CrearMano(11, Palo.Corazones, 11, Palo.Diamantes, 4, Palo.Espadas, 4, Palo.Treboles, 9, Palo.Corazones));
            bool okIaDoblePar = (dDoblePar.IndicesACambiar.Count == 1 && dDoblePar.IndicesACambiar[0] == 4 && dDoblePar.Outs == 4);
            if (okIaDoblePar) aprobadas++;
            sb.AppendLine(string.Format("- IA Doble Par: {0} ({1:0.00}%)", okIaDoblePar ? "OK" : "Error", dDoblePar.ProbabilidadMejora));

            total++;
            DecisionIA dPar = motor.DecidirCambios(
                CrearMano(10, Palo.Corazones, 10, Palo.Diamantes, 14, Palo.Espadas, 7, Palo.Treboles, 3, Palo.Corazones));
            bool okIaPar = (dPar.IndicesACambiar.Count == 3 && dPar.IndicesACambiar[0] == 2 && dPar.IndicesACambiar[1] == 3 && dPar.IndicesACambiar[2] == 4 && dPar.Outs == 2 && Math.Abs(dPar.ProbabilidadMejora - 4.26) < 0.05);
            if (okIaPar) aprobadas++;
            sb.AppendLine(string.Format("- IA Un Par: {0} ({1:0.00}%)", okIaPar ? "OK" : "Error", dPar.ProbabilidadMejora));

            // 6. Decisiones de apuesta de la IA
            total++;
            DecisionApuestaIA apAlta = motor.DecidirApuesta(manosJerarquia[0], 200, 500);
            DecisionApuestaIA apPar = motor.DecidirApuesta(manosJerarquia[1], 100, 500);
            DecisionApuestaIA apTercia = motor.DecidirApuesta(manosJerarquia[3], 50, 500);
            bool okApuestas = (apAlta.Accion == AccionApuesta.Retirarse &&
                               apPar.Accion == AccionApuesta.PasarIgualar &&
                               apTercia.Accion == AccionApuesta.Subir &&
                               apTercia.MontoSubida == 100);
            if (okApuestas) aprobadas++;
            sb.AppendLine(string.Format("- IA Heuristicas de apuesta: {0}", okApuestas ? "OK" : "Error"));

            // 7. Simulacion: IA vs Descarte Aleatorio (200 manos)
            int partidas = 200;
            int ganoIA = 0;
            int ganoAzar = 0;
            int empatesSim = 0;

            Baraja mazoSim = new Baraja();
            Random rndAzar = new Random();

            for (int r = 0; r < partidas; r++)
            {
                mazoSim.Reiniciar();
                mazoSim.Barajar();

                List<Carta> manoAzar = mazoSim.RepartirMano(5);
                List<Carta> manoHeuristica = mazoSim.RepartirMano(5);

                int cantAzar = rndAzar.Next(0, 6);
                List<int> indicesPosibles = new List<int> { 0, 1, 2, 3, 4 };
                for (int c = 0; c < cantAzar; c++)
                {
                    int pos = rndAzar.Next(indicesPosibles.Count);
                    int idx = indicesPosibles[pos];
                    indicesPosibles.RemoveAt(pos);
                    manoAzar[idx] = mazoSim.RepartirCarta();
                }

                DecisionIA dec = motor.DecidirCambios(manoHeuristica);
                for (int c = 0; c < dec.IndicesACambiar.Count; c++)
                {
                    int idx = dec.IndicesACambiar[c];
                    manoHeuristica[idx] = mazoSim.RepartirCarta();
                }

                ResultadoEvaluacion evalAzar = EvaluadorManos.Evaluar(manoAzar);
                ResultadoEvaluacion evalIA = EvaluadorManos.Evaluar(manoHeuristica);

                int comp = evalIA.CompararCon(evalAzar);
                if (comp > 0) ganoIA++;
                else if (comp < 0) ganoAzar++;
                else empatesSim++;
            }

            total++;
            bool okSimulacion = (ganoIA > ganoAzar);
            if (okSimulacion) aprobadas++;

            sb.AppendLine();
            sb.AppendLine(string.Format("Simulacion IA vs Azar ({0} manos):", partidas));
            sb.AppendLine(string.Format("  IA Heuristica: {0} victorias ({1:0.0}%)", ganoIA, ganoIA * 100.0 / partidas));
            sb.AppendLine(string.Format("  Jugador Azar: {0} victorias ({1:0.0}%)", ganoAzar, ganoAzar * 100.0 / partidas));
            sb.AppendLine(string.Format("  Empates: {0}", empatesSim));
            sb.AppendLine();
            sb.AppendLine(string.Format("Pruebas correctas: {0}/{1}", aprobadas, total));

            reporte = sb.ToString();
            return aprobadas == total;
        }

        private static List<Carta> CrearMano(
            int v1, Palo p1,
            int v2, Palo p2,
            int v3, Palo p3,
            int v4, Palo p4,
            int v5, Palo p5)
        {
            return new List<Carta>
            {
                new Carta(v1, p1),
                new Carta(v2, p2),
                new Carta(v3, p3),
                new Carta(v4, p4),
                new Carta(v5, p5)
            };
        }
    }
}
