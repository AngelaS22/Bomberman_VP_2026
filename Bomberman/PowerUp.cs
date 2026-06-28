using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bomberman
{
    public enum PowerUpTip
    {
        PovekeOpseg,     
        PovekeBombi,     
        PobrzoMovenje    
    }
    internal class PowerUp
    {
        public int Rows;
        public int Columns;
        public int GoleminaKelija;
        public PowerUpTip Tip;
        public bool Zemen = false;

        private static readonly Color[] Boji = {
            Color.Cyan,
            Color.Magenta,
            Color.LimeGreen
        };

        private static readonly string[] Ikoni = { "+R", "+B", "+S" };

        public PowerUp(int rows, int columns, int goleminaKelija, PowerUpTip tip)
        {
            Rows = rows;
            Columns = columns;
            GoleminaKelija = goleminaKelija;
            Tip = tip;
        }

        public void Draw(Graphics g)
        {
            if (Zemen) return;
            int x = Columns * GoleminaKelija + 4;
            int y = Rows * GoleminaKelija + 4;
            int s = GoleminaKelija - 8;

            using (var b = new SolidBrush(Boji[(int)Tip]))
                g.FillRectangle(b, x, y, s, s);

            using (var font = new Font("Arial", 9, FontStyle.Bold))
            using (var brush = new SolidBrush(Color.Black))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(Ikoni[(int)Tip], font, brush, new RectangleF(x, y, s, s), sf);
            }
        }

        public void PrimeniNaIgrac(Player p)
        {
            switch (Tip)
            {
                case PowerUpTip.PovekeOpseg: p.BombOpseg++; break;
                case PowerUpTip.PovekeBombi: p.MaxBombi++; break;
                case PowerUpTip.PobrzoMovenje: break;
            }
        }
    }
}
