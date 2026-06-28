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
        public bool Ziv = true;        
        public bool Eliminiran = false; public int Zivoti = 3;
        public int BombOpseg = 2;       
        public int MaxBombi = 1;        
        public int AktivniBombi = 0;

        public Player(int Rows, int Columns, int GoleminaKelija, Color Boja)
        {
            this.Rows = Rows;
            this.Columns = Columns;
            this.GoleminaKelija = GoleminaKelija;
            this.Boja = Boja;
        }
        public void Draw(Graphics g)
        {
            if (!Ziv) return;

            int x = Columns * GoleminaKelija + 4;
            int y = Rows * GoleminaKelija + 4;
            int size = GoleminaKelija - 8;

            g.FillEllipse(new SolidBrush(Boja), x, y, size, size);

            g.FillEllipse(Brushes.White, x + size / 4 - 2, y + size / 4, 6, 6);
            g.FillEllipse(Brushes.White, x + size * 3 / 4 - 4, y + size / 4, 6, 6);
            g.FillEllipse(Brushes.Black, x + size / 4 - 1, y + size / 4 + 1, 4, 4);
            g.FillEllipse(Brushes.Black, x + size * 3 / 4 - 3, y + size / 4 + 1, 4, 4);
        }
        public void Umri()
        {
            Zivoti--;
            Ziv = false;
            AktivniBombi = 0;
            if (Zivoti <= 0)
                Eliminiran = true;
        }

        public void Respawn(int startRow, int startCol)
        {
            Rows = startRow;
            Columns = startCol;
            Ziv = true;
        }

    }
}
