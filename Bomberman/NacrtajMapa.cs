using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bomberman
{
    internal class NacrtajMapa
    {
        public int Rows = 13;
        public int Columns = 15;
        public int GoleminaKelija = 40;

        public int[,] Mapa;

        public NacrtajMapa()
        {
            Mapa = new int[Rows, Columns];
            GenerirajMapa();
        }
        private void GenerirajMapa()
        {
            Random random = new Random();
            for (int i=0; i<Rows; i++)
            {
                for(int j=0; j<Columns; j++)
                {
                    if(i==0 || j==0 || i==Rows-1 || j == Columns - 1)
                    {
                        Mapa[i, j] = 1;
                    }
                    else if(i%2==0 && j % 2 == 0)
                    {
                        Mapa[i, j] = 1;
                    }

                    bool p1SafeZone = (i <= 2 && j <= 2);
                    bool p2SafeZone = (i <= 2 && j >= Columns - 3);
                    bool p3SafeZone = (i >= Rows - 3 && j <= 2);
                    bool p4SafeZone = (i >= Rows - 3 && j >= Columns - 3);

                    if (p1SafeZone || p2SafeZone || p3SafeZone || p4SafeZone)
                        Mapa[i, j] = 0;
                    else
                        Mapa[i, j] = random.Next(0, 3) == 0 ? 2 : 0; 

                }
            }
        }
        public List<System.Drawing.Point> GoleminaEksplozija(int row, int col, int opseg)
        {
            var zasegnati = new List<System.Drawing.Point>();
            zasegnati.Add(new System.Drawing.Point(col, row));

            int[] dr = { -1, 1, 0, 0 };
            int[] dc = { 0, 0, -1, 1 };

            for (int d = 0; d < 4; d++)
            {
                for (int k = 1; k <= opseg; k++)
                {
                    int nr = row + dr[d] * k;
                    int nc = col + dc[d] * k;

                    if (nr < 0 || nr >= Rows || nc < 0 || nc >= Columns) break;
                    if (Mapa[nr, nc] == 1) break; 

                    zasegnati.Add(new System.Drawing.Point(nc, nr));

                    if (Mapa[nr, nc] == 2) 
                    {
                        Mapa[nr, nc] = 0; 
                        break;
                    }
                }
            }
            return zasegnati;
        }
    }
}
