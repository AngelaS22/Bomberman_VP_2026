using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bomberman
{
    internal class Bomb
    {
        public int Rows;
        public int Columns;
        public int GoleminaKelija;
        public DateTime VremeNaPostavuvanje;
        public int Opseg;
        public bool Eksplodirala = false;
        public List<Point> EksplozijaCelii = new List<Point>();
        public DateTime VremeNaEksplozija;
        private const double EKSPLOZIJA_TRAJANJE = 0.6;
        private const double VREME_PRED_EKSPLOZIJA = 3.0;

        public Player Sopstvenik;


        public Bomb(int rows, int columns, int goleminaKelija, int opseg, Player sopstvenik)
        {
            Rows = rows;
            Columns = columns;
            GoleminaKelija = goleminaKelija;
            Opseg = opseg;
            VremeNaPostavuvanje = DateTime.Now;
            Sopstvenik = sopstvenik;
        }
        public bool TrebaEksplozija()
        {
            return !Eksplodirala && (DateTime.Now - VremeNaPostavuvanje).TotalSeconds >= VREME_PRED_EKSPLOZIJA;
        }

        public bool EksplozijataZavrsi()
        {
            return Eksplodirala && (DateTime.Now - VremeNaEksplozija).TotalSeconds >= EKSPLOZIJA_TRAJANJE;
        }

        public void Eksplodiraj(NacrtajMapa mapa)
        {
            Eksplodirala = true;
            VremeNaEksplozija = DateTime.Now;
            EksplozijaCelii = mapa.GoleminaEksplozija(Rows, Columns, Opseg);
            Sopstvenik.AktivniBombi--;
        }

        public bool EJasenaEksplozija()
        {
            return Eksplodirala && (DateTime.Now - VremeNaEksplozija).TotalSeconds < EKSPLOZIJA_TRAJANJE;
        }

        public void Draw(Graphics g)
        {
            if (Eksplodirala)
            {
                // plamen
                double elapsed = (DateTime.Now - VremeNaEksplozija).TotalSeconds;
                float alpha = (float)(1.0 - elapsed / EKSPLOZIJA_TRAJANJE);
                int a = (int)(255 * alpha);
                if (a < 0) a = 0;

                foreach (var cell in EksplozijaCelii)
                {
                    // Nadvoresna portokалova
                    using (var b1 = new SolidBrush(Color.FromArgb(a, 255, 140, 0)))
                        g.FillRectangle(b1, cell.X * GoleminaKelija + 2, cell.Y * GoleminaKelija + 2,
                            GoleminaKelija - 4, GoleminaKelija - 4);
                    // Vnatresna zolta
                    using (var b2 = new SolidBrush(Color.FromArgb(a, 255, 255, 0)))
                        g.FillRectangle(b2, cell.X * GoleminaKelija + 8, cell.Y * GoleminaKelija + 8,
                            GoleminaKelija - 16, GoleminaKelija - 16);
                }
            }
            else
            {
                // trepkanje pred eksplozija
                double elapsed = (DateTime.Now - VremeNaPostavuvanje).TotalSeconds;
                double preostanato = VREME_PRED_EKSPLOZIJA - elapsed;
                bool trepkaj = preostanato < 1.0 && ((int)(elapsed * 6) % 2 == 0);

                Color bombaBoja = trepkaj ? Color.Red : Color.Black;
                using (var brush = new SolidBrush(bombaBoja))
                    g.FillEllipse(brush,
                        Columns * GoleminaKelija + 8, Rows * GoleminaKelija + 8,
                        GoleminaKelija - 16, GoleminaKelija - 16);

                // Fitil
                using (var pen = new Pen(Color.SaddleBrown, 2))
                    g.DrawLine(pen,
                        Columns * GoleminaKelija + GoleminaKelija / 2,
                        Rows * GoleminaKelija + 8,
                        Columns * GoleminaKelija + GoleminaKelija / 2 + 4,
                        Rows * GoleminaKelija + 4);
            }
        }
    }
}
