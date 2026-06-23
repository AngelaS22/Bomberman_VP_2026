using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bomberman
{
    public partial class Form1 : Form
    {
        NacrtajMapa nacrtajMapa;
        public Form1()
        {
            InitializeComponent();
            this.ClientSize = new Size(600, 520);
            nacrtajMapa = new NacrtajMapa();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Draw(e.Graphics);
        }
        private void Draw(Graphics g)
        {
            for(int i=0; i<nacrtajMapa.Rows; i++)
            {
                for(int j=0; j<nacrtajMapa.Columns; j++)
                {
                    if (nacrtajMapa.Mapa[i, j] == 1)
                    {
                        g.FillRectangle(Brushes.Gray, j * nacrtajMapa.GoleminaKelija, i * nacrtajMapa.GoleminaKelija, nacrtajMapa.GoleminaKelija, nacrtajMapa.GoleminaKelija);
                    }
                    else
                    {
                        g.FillRectangle(Brushes.Green, j * nacrtajMapa.GoleminaKelija, i * nacrtajMapa.GoleminaKelija, nacrtajMapa.GoleminaKelija, nacrtajMapa.GoleminaKelija);
                    }
                }
            }
        }

      
    }
}
