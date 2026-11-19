using System;
using System.Collections.Generic;

namespace Poker
{
    public class Baraja
    {
        private List<Carta> cartas;
        private Random random;

        public int CartasRestantes
        {
            get { return cartas.Count; }
        }

        public Baraja()
        {
            cartas = new List<Carta>();
            random = new Random();
            Reiniciar();
        }

        public void Reiniciar()
        {
            cartas.Clear();

            foreach (Palo palo in Enum.GetValues(typeof(Palo)))
            {
                for (int valor = 2; valor <= 14; valor++)
                {
                    cartas.Add(new Carta(valor, palo));
                }
            }
        }

        public void Barajar()
        {
            for (int i = cartas.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                Carta temp = cartas[i];
                cartas[i] = cartas[j];
                cartas[j] = temp;
            }
        }

        public Carta RepartirCarta()
        {
            if (cartas.Count == 0)
            {
                return null;
            }

            Carta carta = cartas[0];
            cartas.RemoveAt(0);
            return carta;
        }

        public List<Carta> RepartirMano(int cantidad)
        {
            List<Carta> mano = new List<Carta>();
            for (int i = 0; i < cantidad; i++)
            {
                Carta c = RepartirCarta();
                if (c != null)
                {
                    mano.Add(c);
                }
            }

            return mano;
        }

        public Carta ExtraerCarta(int valor, Palo palo)
        {
            for (int i = 0; i < cartas.Count; i++)
            {
                if (cartas[i].Valor == valor && cartas[i].PaloCarta == palo)
                {
                    Carta encontrada = cartas[i];
                    cartas.RemoveAt(i);
                    return encontrada;
                }
            }

            return null;
        }
    }
}
