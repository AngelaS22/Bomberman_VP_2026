using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bomberman
{
    internal class Player
    {
        public int Rows;
        public int Columns;
        public int GoleminaKelija;
        public Color Boja;

        public Player(int Rows, int Columns, int GoleminaKelija, Color Boja)
        {
            this.Rows = Rows;
            this.Columns = Columns;
            this.GoleminaKelija = GoleminaKelija;
            this.Boja = Boja;
        }
        public void Draw(Graphics g)
        {
            g.FillEllipse(new SolidBrush(Boja), Columns * GoleminaKelija + 4, Rows * GoleminaKelija + 4, GoleminaKelija - 8, GoleminaKelija - 8);
        }
    }
}
