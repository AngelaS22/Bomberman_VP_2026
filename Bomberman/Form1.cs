using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bomberman
{
    public enum GameState { Menu, Playing, GameOver }

    public partial class Form1 : Form
    {
        NacrtajMapa nacrtajMapa;
        Player player1;
        Player player2;
        List<Bomb> bombi = new List<Bomb>();
        List<PowerUp> powerUps = new List<PowerUp>();

        GameState gameState = GameState.Menu;
        string winnerText = "";

        double menuPulse = 0;

        int p1StartRow = 1, p1StartCol = 1;
        int p2StartRow = 1, p2StartCol = 13;

        const int HUD_HEIGHT = 60;
        const int RESPAWN_DELAY_MS = 1500;
        DateTime? p1RespawnTime = null;
        DateTime? p2RespawnTime = null;
        DateTime? p1DeathFlash = null;
        DateTime? p2DeathFlash = null;
        public Form1()
        {
            InitializeComponent();
            this.Text = "Bomberman";
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            DoubleBuffered = true;
            SetupGame();
        }
        private void SetupGame()
        {
            nacrtajMapa = new NacrtajMapa();
            int mapWidth = nacrtajMapa.Columns * nacrtajMapa.GoleminaKelija;
            int mapHeight = nacrtajMapa.Rows * nacrtajMapa.GoleminaKelija;
            this.ClientSize = new Size(mapWidth, mapHeight + HUD_HEIGHT);

            player1 = new Player(p1StartRow, p1StartCol, nacrtajMapa.GoleminaKelija, Color.FromArgb(70, 130, 255));
            player2 = new Player(p2StartRow, p2StartCol, nacrtajMapa.GoleminaKelija, Color.FromArgb(255, 70, 70));

            bombi.Clear();
            powerUps.Clear();
            p1DeathFlash = null;
            p2DeathFlash = null;
            p1RespawnTime = null;
            p2RespawnTime = null;

            timer1.Interval = 50;
            timer1.Start();
        }

        private void StartNewRound()
        {
            nacrtajMapa = new NacrtajMapa();
            bombi.Clear();
            powerUps.Clear();

            player1.Rows = p1StartRow; player1.Columns = p1StartCol;
            player1.Ziv = true; player1.Eliminiran = false; player1.Zivoti = 3;
            player1.AktivniBombi = 0; player1.BombOpseg = 2; player1.MaxBombi = 1;

            player2.Rows = p2StartRow; player2.Columns = p2StartCol;
            player2.Ziv = true; player2.Eliminiran = false; player2.Zivoti = 3;
            player2.AktivniBombi = 0; player2.BombOpseg = 2; player2.MaxBombi = 1;

            p1DeathFlash = null; p2DeathFlash = null;
            p1RespawnTime = null; p2RespawnTime = null;

            gameState = GameState.Playing;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            switch (gameState)
            {
                case GameState.Menu: DrawMenu(g); break;
                case GameState.Playing: DrawGame(g); break;
                case GameState.GameOver: DrawGame(g); DrawGameOver(g); break;
            }
        }


        
        private void DrawMenu(Graphics g)
        {
            // Background gradient
            using (var bg = new LinearGradientBrush(ClientRectangle,
                Color.FromArgb(15, 15, 35), Color.FromArgb(35, 15, 55), 45))
                g.FillRectangle(bg, ClientRectangle);

            // Naslov
            string title = "BOMBERMAN";
            using (var f = new Font("Impact", 48, FontStyle.Bold))
            using (var brush = new SolidBrush(Color.FromArgb(255, 220, 50)))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Center };
                g.DrawString(title, f, brush, new RectangleF(0, 80, ClientSize.Width, 100), sf);
            }

            DrawMenuCard(g, 40, 220, 220, 200, "ИГРАЧ 1",
                new[] { "W A S D - Движење", "SPACE - Бомба" },
                Color.FromArgb(70, 130, 255));
            DrawMenuCard(g, ClientSize.Width - 260, 220, 220, 200, "ИГРАЧ 2",
                new[] { "↑ ↓ ← → - Движење", "ENTER - Бомба" },
                Color.FromArgb(255, 70, 70));
            DrawPowerUpLegend(g);

            menuPulse += 0.05;
            int alpha = (int)(180 + 75 * Math.Sin(menuPulse));
            using (var f = new Font("Arial", 18, FontStyle.Bold))
            using (var brush = new SolidBrush(Color.FromArgb(alpha, 255, 255, 255)))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Center };
                g.DrawString("Притисни ENTER за да почнеш", f, brush,
                    new RectangleF(0, ClientSize.Height - 80, ClientSize.Width, 40), sf);
            }
        }
        private void DrawMenuCard(Graphics g, int x, int y, int w, int h, string title, string[] lines, Color color)
        {
            using (var bg = new SolidBrush(Color.FromArgb(40, color)))
                g.FillRectangle(bg, x, y, w, h);
            using (var border = new Pen(color, 2))
                g.DrawRectangle(border, x, y, w, h);

            using (var f = new Font("Arial", 13, FontStyle.Bold))
            using (var brush = new SolidBrush(color))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Center };
                g.DrawString(title, f, brush, new RectangleF(x, y + 10, w, 30), sf);
            }

            using (var f = new Font("Arial", 10))
            using (var brush = new SolidBrush(Color.FromArgb(200, 255, 255, 255)))
            {
                for (int i = 0; i < lines.Length; i++)
                    g.DrawString(lines[i], f, brush, x + 10, y + 55 + i * 22);
            }
        }

        private void DrawPowerUpLegend(Graphics g)
        {
            int cx = ClientSize.Width / 2;
            int y = 240;
            string header = "POWER-UPS";
            using (var f = new Font("Arial", 12, FontStyle.Bold))
            using (var brush = new SolidBrush(Color.FromArgb(200, 255, 255, 255)))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Center };
                g.DrawString(header, f, brush, new RectangleF(cx - 100, y, 200, 25), sf);
            }

            var items = new[] {
                (Color.Cyan, "+R", "Поголем опсег"),
                (Color.Magenta, "+B", "Повеќе бомби"),
                (Color.LimeGreen, "+S", "Побрзо движење")
            };

            for (int i = 0; i < items.Length; i++)
            {
                int iy = y + 30 + i * 35;
                g.FillRectangle(new SolidBrush(items[i].Item1), cx - 90, iy, 28, 28);
                using (var f = new Font("Arial", 8, FontStyle.Bold))
                    g.DrawString(items[i].Item2, f, Brushes.Black, cx - 88, iy + 8);
                using (var f = new Font("Arial", 10))
                using (var brush = new SolidBrush(Color.FromArgb(200, 255, 255, 255)))
                    g.DrawString(items[i].Item3, f, brush, cx - 55, iy + 6);
            }
        }
        //private void Form1_KeyDown(object sender, KeyEventArgs e)
        //{
        //    int newRows = player1.Rows;
        //    int newColumns = player1.Columns;

        //    if(e.KeyCode == Keys.W)
        //    {
        //        newRows--;
        //    }
        //    if(e.KeyCode == Keys.S)
        //    {
        //        newRows++;
        //    }
        //    if (e.KeyCode == Keys.A)
        //    {
        //        newColumns--;
        //    }
        //    if (e.KeyCode == Keys.D)
        //    {
        //        newColumns++;
        //    }
        //    if (nacrtajMapa.Mapa[newRows, newColumns] == 0)
        //    {
        //        player1.Rows = newRows;
        //        player1.Columns = newColumns;
        //    }

        //    int newRows2 = player2.Rows;
        //    int newColumns2 = player2.Columns;
        //    if(e.KeyCode == Keys.Up)
        //    {
        //        newRows2--;
        //    }
        //    if (e.KeyCode == Keys.Down)
        //    {
        //        newRows2++;
        //    }
        //    if (e.KeyCode == Keys.Left)
        //    {
        //        newColumns2--;
        //    }
        //    if(e.KeyCode == Keys.Right)
        //    {
        //        newColumns2++;
        //    }
        //    if (nacrtajMapa.Mapa[newRows2, newColumns2] == 0)
        //    {
        //        player2.Rows = newRows2;
        //        player2.Columns = newColumns2;
        //    }

        //    if(e.KeyCode == Keys.Space)
        //    {
        //        bombi.Add(new Bomb(player1.Rows, player1.Columns, nacrtajMapa.GoleminaKelija));
        //    }
        //    if (e.KeyCode == Keys.Enter)
        //    {
        //        bombi.Add(new Bomb(player2.Rows, player2.Columns, nacrtajMapa.GoleminaKelija));
        //    }
       
        //Invalidate();
        //}


        //private void timer1_Tick(object sender, EventArgs e)
        //{
        //    for(int i=bombi.Count-1; i>=0; i--)
        //    {
        //        if (bombi[i].TrebaEksplozija())
        //        {
        //            bombi.RemoveAt(i);
        //        }
        //    }
        //    Invalidate();
        //}

        private void DrawGame(Graphics g)
        {
            g.TranslateTransform(0, HUD_HEIGHT);

            for (int i = 0; i < nacrtajMapa.Rows; i++)
            {
                for (int j = 0; j < nacrtajMapa.Columns; j++)
                {
                    int cel = nacrtajMapa.Mapa[i, j];
                    int x = j * nacrtajMapa.GoleminaKelija;
                    int y = i * nacrtajMapa.GoleminaKelija;
                    int s = nacrtajMapa.GoleminaKelija;

                    if (cel == 1)
                    {
                        using (var b = new LinearGradientBrush(
                            new Rectangle(x, y, s, s),
                            Color.FromArgb(100, 100, 110),
                            Color.FromArgb(60, 60, 70), 45))
                            g.FillRectangle(b, x, y, s, s);
                        using (var p = new Pen(Color.FromArgb(80, 80, 90), 1))
                            g.DrawRectangle(p, x, y, s, s);
                    }
                    else if (cel == 2)
                    {
                        using (var b = new SolidBrush(Color.FromArgb(160, 100, 50)))
                            g.FillRectangle(b, x, y, s, s);
                        using (var p = new Pen(Color.FromArgb(130, 80, 30), 1))
                        {
                            g.DrawLine(p, x + s / 3, y, x + s / 3, y + s);
                            g.DrawLine(p, x + s * 2 / 3, y, x + s * 2 / 3, y + s);
                            g.DrawLine(p, x, y + s / 2, x + s, y + s / 2);
                        }
                    }
                    else
                    {
                        using (var b = new SolidBrush(Color.FromArgb(34, 85, 34)))
                            g.FillRectangle(b, x, y, s, s);
                        using (var p = new Pen(Color.FromArgb(28, 70, 28), 1))
                            g.DrawRectangle(p, x, y, s, s);
                    }
                }
            }

            foreach (var pu in powerUps)
                if (!pu.Zemen) pu.Draw(g);

            foreach (var b in bombi)
                b.Draw(g);

            DrawPlayerWithFlash(g, player1, p1DeathFlash);
            DrawPlayerWithFlash(g, player2, p2DeathFlash);

            g.ResetTransform();

            DrawHUD(g);
        }
        private void DrawPlayerWithFlash(Graphics g, Player p, DateTime? flashTime)
        {
            if (!p.Ziv) return;
            if (flashTime.HasValue && (DateTime.Now - flashTime.Value).TotalMilliseconds < 300)
            {
                int x = p.Columns * p.GoleminaKelija;
                int y = p.Rows * p.GoleminaKelija;
                using (var b = new SolidBrush(Color.FromArgb(180, 255, 0, 0)))
                    g.FillEllipse(b, x + 2, y + 2, p.GoleminaKelija - 4, p.GoleminaKelija - 4);
            }
            p.Draw(g);
        }

        private void DrawHUD(Graphics g)
        {
            using (var b = new LinearGradientBrush(new Rectangle(0, 0, ClientSize.Width, HUD_HEIGHT),
                Color.FromArgb(20, 20, 40), Color.FromArgb(35, 35, 60), 90))
                g.FillRectangle(b, 0, 0, ClientSize.Width, HUD_HEIGHT);

            using (var p = new Pen(Color.FromArgb(80, 80, 120), 1))
                g.DrawLine(p, 0, HUD_HEIGHT - 1, ClientSize.Width, HUD_HEIGHT - 1);

            DrawPlayerHUD(g, player1, 10, "ИГРАЧ 1 (WASD+SPACE)");

            DrawPlayerHUD(g, player2, ClientSize.Width / 2 + 10, "ИГРАЧ 2 (Стрелки+ENTER)");

            using (var p = new Pen(Color.FromArgb(60, 60, 90), 1))
                g.DrawLine(p, ClientSize.Width / 2, 5, ClientSize.Width / 2, HUD_HEIGHT - 5);
        }
        private void DrawPlayerHUD(Graphics g, Player p, int x, string label)
        {
            Color col = p.Boja;

            using (var f = new Font("Arial", 8))
            using (var b = new SolidBrush(Color.FromArgb(150, 200, 200, 200)))
                g.DrawString(label, f, b, x, 5);

            using (var f = new Font("Segoe UI Emoji", 14))
            {
                string hearts = "";
                for (int i = 0; i < 3; i++)
                    hearts += i < p.Zivoti ? "❤ " : "🖤 ";
                using (var b = new SolidBrush(Color.White))
                    g.DrawString(hearts, f, b, x, 18);
            }

            using (var f = new Font("Arial", 9))
            using (var b = new SolidBrush(Color.FromArgb(180, 255, 220, 100)))
            {
                g.DrawString($"Опсег: {p.BombOpseg}   Бомби: {p.MaxBombi}", f, b, x, 42);
            }
        }


        private void DrawGameOver(Graphics g)
        {
            using (var b = new SolidBrush(Color.FromArgb(170, 0, 0, 0)))
                g.FillRectangle(b, ClientRectangle);

            using (var f = new Font("Impact", 42, FontStyle.Bold))
            using (var b = new SolidBrush(Color.FromArgb(255, 220, 50)))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(winnerText, f, b, new RectangleF(0, ClientSize.Height / 2 - 80, ClientSize.Width, 100), sf);
            }

            menuPulse += 0.05;
            int alpha = (int)(180 + 75 * Math.Sin(menuPulse));
            using (var f = new Font("Arial", 16))
            using (var b = new SolidBrush(Color.FromArgb(alpha, 255, 255, 255)))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Center };
                g.DrawString("ENTER - Нова игра   |   ESC - Мени", f, b,
                    new RectangleF(0, ClientSize.Height / 2 + 40, ClientSize.Width, 40), sf);
            }
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            if (gameState != GameState.Playing) { Invalidate(); return; }

            for (int i = bombi.Count - 1; i >= 0; i--)
            {
                var b = bombi[i];
                if (!b.Eksplodirala && b.TrebaEksplozija())
                {
                    b.Eksplodiraj(nacrtajMapa);
                    ProveriPowerUpSpawn(b);
                }
                else if (b.Eksplodirala && b.EksplozijataZavrsi())
                {
                    bombi.RemoveAt(i);
                    continue;
                }
                if (b.EJasenaEksplozija())
                {
                    ProveriSmrt(player1, b, ref p1DeathFlash, ref p1RespawnTime);
                    ProveriSmrt(player2, b, ref p2DeathFlash, ref p2RespawnTime);
                }
            }

            foreach (var pu in powerUps)
            {
                if (pu.Zemen) continue;
                if (player1.Ziv && player1.Rows == pu.Rows && player1.Columns == pu.Columns)
                { pu.PrimeniNaIgrac(player1); pu.Zemen = true; }
                else if (player2.Ziv && player2.Rows == pu.Rows && player2.Columns == pu.Columns)
                { pu.PrimeniNaIgrac(player2); pu.Zemen = true; }
            }
            powerUps.RemoveAll(pu => pu.Zemen);

            if (p1RespawnTime.HasValue && (DateTime.Now - p1RespawnTime.Value).TotalMilliseconds >= RESPAWN_DELAY_MS)
            {
                player1.Respawn(p1StartRow, p1StartCol);
                p1RespawnTime = null;
            }
            if (p2RespawnTime.HasValue && (DateTime.Now - p2RespawnTime.Value).TotalMilliseconds >= RESPAWN_DELAY_MS)
            {
                player2.Respawn(p2StartRow, p2StartCol);
                p2RespawnTime = null;
            }


            if (player1.Eliminiran && player2.Eliminiran)
            { winnerText = "🤝 НЕРЕШЕНО!"; gameState = GameState.GameOver; }
            else if (player1.Eliminiran)
            { winnerText = "🔴 ИГРАЧ 2 ПОБЕДИ!"; gameState = GameState.GameOver; }
            else if (player2.Eliminiran)
            { winnerText = "🔵 ИГРАЧ 1 ПОБЕДИ!"; gameState = GameState.GameOver; }

            Invalidate();
        }

        private void ProveriSmrt(Player p, Bomb b, ref DateTime? flashTime, ref DateTime? respawnTime)
        {
            if (!p.Ziv || p.Eliminiran) return;
            foreach (var cell in b.EksplozijaCelii)
            {
                if (cell.Y == p.Rows && cell.X == p.Columns)
                {
                    flashTime = DateTime.Now;
                    p.Umri();
                    if (!p.Eliminiran)
                        respawnTime = DateTime.Now;
                    break;
                }
            }
        }

        private void ProveriPowerUpSpawn(Bomb b)
        {
            Random rnd = new Random();
            if (rnd.NextDouble() < 0.4)
            {
                var tip = (PowerUpTip)rnd.Next(0, 3);
                powerUps.Add(new PowerUp(b.Rows, b.Columns, nacrtajMapa.GoleminaKelija, tip));
            }
        }


        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (gameState == GameState.Menu)
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
                    StartNewRound();
                return;
            }

            if (gameState == GameState.GameOver)
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
                    StartNewRound();
                else if (e.KeyCode == Keys.Escape)
                    gameState = GameState.Menu;
                return;
            }

            // Player 1 - WASD
            if (player1.Ziv && !player1.Eliminiran)
            {
                int r = player1.Rows, c = player1.Columns;
                if (e.KeyCode == Keys.W) r--;
                else if (e.KeyCode == Keys.S) r++;
                else if (e.KeyCode == Keys.A) c--;
                else if (e.KeyCode == Keys.D) c++;

                if ((r != player1.Rows || c != player1.Columns) &&
                    r >= 0 && r < nacrtajMapa.Rows && c >= 0 && c < nacrtajMapa.Columns &&
                    nacrtajMapa.Mapa[r, c] == 0 && !ImaBombаNа(r, c))
                {
                    player1.Rows = r; player1.Columns = c;
                }

                if (e.KeyCode == Keys.Space && player1.AktivniBombi < player1.MaxBombi
                    && !ImaBombаNа(player1.Rows, player1.Columns))
                {
                    bombi.Add(new Bomb(player1.Rows, player1.Columns, nacrtajMapa.GoleminaKelija,
                        player1.BombOpseg, player1));
                    player1.AktivniBombi++;
                }
            }

            // Player 2 - Strelki
            if (player2.Ziv && !player2.Eliminiran)
            {
                int r = player2.Rows, c = player2.Columns;
                if (e.KeyCode == Keys.Up) r--;
                else if (e.KeyCode == Keys.Down) r++;
                else if (e.KeyCode == Keys.Left) c--;
                else if (e.KeyCode == Keys.Right) c++;

                if ((r != player2.Rows || c != player2.Columns) &&
                    r >= 0 && r < nacrtajMapa.Rows && c >= 0 && c < nacrtajMapa.Columns &&
                    nacrtajMapa.Mapa[r, c] == 0 && !ImaBombаNа(r, c))
                {
                    player2.Rows = r; player2.Columns = c;
                }

                if ((e.KeyCode == Keys.Return) && player2.AktivniBombi < player2.MaxBombi
                    && !ImaBombаNа(player2.Rows, player2.Columns))
                {
                    bombi.Add(new Bomb(player2.Rows, player2.Columns, nacrtajMapa.GoleminaKelija,
                        player2.BombOpseg, player2));
                    player2.AktivniBombi++;
                }
            }

            Invalidate();
        }

        private bool ImaBombаNа(int row, int col)
        {
            foreach (var b in bombi)
                if (!b.Eksplodirala && b.Rows == row && b.Columns == col) return true;
            return false;
        }
    }
}
