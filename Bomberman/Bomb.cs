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

        public Bomb(int Rows, int Columns, int GoleminaKelija)
        {
            this.Rows = Rows;
            this.Columns = Columns;
            this.GoleminaKelija = GoleminaKelija;
            this.VremeNaPostavuvanje = DateTime.Now;
        }
        public bool TrebaEksplozija()
        {
            return (DateTime.Now - VremeNaPostavuvanje).TotalSeconds >= 3;
        }
        public void Draw(Graphics g)
        {
            g.FillEllipse(Brushes.Black, Columns * GoleminaKelija + 8, Rows * GoleminaKelija + 8, GoleminaKelija - 16, GoleminaKelija - 16);
        }
    }
}
