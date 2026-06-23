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
        Player player1;
        Player player2;
        public Form1()
        {
            InitializeComponent();
            this.ClientSize = new Size(600, 520);
            nacrtajMapa = new NacrtajMapa();
            this.KeyDown += new KeyEventHandler(Form1_KeyDown);
            player1 = new Player(1, 1, nacrtajMapa.GoleminaKelija, Color.Blue);
            player2 = new Player(1, nacrtajMapa.Columns - 2, nacrtajMapa.GoleminaKelija, Color.Red);
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
            player1.Draw(g);
            player2.Draw(g);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            int newRows = player1.Rows;
            int newColumns = player1.Columns;

            if(e.KeyCode == Keys.W)
            {
                newRows--;
            }
            if(e.KeyCode == Keys.S)
            {
                newRows++;
            }
            if (e.KeyCode == Keys.A)
            {
                newColumns--;
            }
            if (e.KeyCode == Keys.D)
            {
                newColumns++;
            }
            if (nacrtajMapa.Mapa[newRows, newColumns] == 0)
            {
                player1.Rows = newRows;
                player1.Columns = newColumns;
            }

            int newRows2 = player2.Rows;
            int newColumns2 = player2.Columns;
            if(e.KeyCode == Keys.Up)
            {
                newRows2--;
            }
            if (e.KeyCode == Keys.Down)
            {
                newRows2++;
            }
            if (e.KeyCode == Keys.Left)
            {
                newColumns2--;
            }
            if(e.KeyCode == Keys.Right)
            {
                newColumns2++;
            }
            if (nacrtajMapa.Mapa[newRows2, newColumns2] == 0)
            {
                player2.Rows = newRows2;
                player2.Columns = newColumns2;
            }
       
        Invalidate();
        }
    }
}
