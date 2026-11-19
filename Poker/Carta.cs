using System;

namespace Poker
{
    public enum Palo
    {
        Corazones,
        Diamantes,
        Treboles,
        Espadas
    }

    public class Carta
    {
        public int Valor { get; set; }
        public Palo PaloCarta { get; set; }
        public bool Seleccionada { get; set; }

        public Carta(int valor, Palo palo)
        {
            Valor = valor;
            PaloCarta = palo;
            Seleccionada = false;
        }

        public string ObtenerTextoValor()
        {
            switch (Valor)
            {
                case 11: return "J";
                case 12: return "Q";
                case 13: return "K";
                case 14: return "A";
                default: return Valor.ToString();
            }
        }

        public string ObtenerNombreArchivo()
        {
            return Valor + "_" + PaloCarta.ToString() + ".png";
        }

        public override string ToString()
        {
            return ObtenerTextoValor() + " de " + PaloCarta.ToString();
        }
    }
}
