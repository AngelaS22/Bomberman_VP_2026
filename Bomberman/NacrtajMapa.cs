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
                  
                        else
                        {
                            Mapa[i, j] = 0;
                        }
                    
                }
            }
        }
    }
}
