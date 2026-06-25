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
        List<Bomb> bombi = new List<Bomb>();
        public Form1()
        {
            InitializeComponent();
            this.ClientSize = new Size(600, 520);
            nacrtajMapa = new NacrtajMapa();
            player1 = new Player(1, 1, nacrtajMapa.GoleminaKelija, Color.Blue);
            player2 = new Player(1, nacrtajMapa.Columns - 2, nacrtajMapa.GoleminaKelija, Color.Red);
            DoubleBuffered = true;
            Timer timer = new Timer();
            timer.Interval = 100;
            timer.Tick+=new EventHandler(timer1_Tick);
            timer.Start();
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
            foreach(Bomb b in bombi)
            {
                b.Draw(g);
            }
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

            if(e.KeyCode == Keys.Space)
            {
                bombi.Add(new Bomb(player1.Rows, player1.Columns, nacrtajMapa.GoleminaKelija));
            }
            if (e.KeyCode == Keys.Enter)
            {
                bombi.Add(new Bomb(player2.Rows, player2.Columns, nacrtajMapa.GoleminaKelija));
            }
       
        Invalidate();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            for(int i=bombi.Count-1; i>=0; i--)
            {
                if (bombi[i].TrebaEksplozija())
                {
                    bombi.RemoveAt(i);
                }
            }
            Invalidate();
        }
    }
}
