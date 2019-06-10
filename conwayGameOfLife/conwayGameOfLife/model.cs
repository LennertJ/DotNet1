using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace conwayGameOfLife
{
    class Model
    {

        private int arraySize;

        public Model()
        {
        }

        public int Mod(int x, int m)
        {
            return (x % m + m) % m;
        }
    }
}
