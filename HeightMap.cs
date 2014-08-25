using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project1
{
    class HeightMap
    {
        private List<List<float>> grid;
        private int sideLength;

        public HeightMap(int sideLength)
        {
            if (!isPow2Plus1(sideLength)) {
                sideLength = getNextPow2Plus1(sideLength);
            }
            this.sideLength = sideLength;

            fillGrid();
        }

        public List<List<float>> getGrid()
        {
            return this.grid;
        }

        /**
         * Using the Diamond-square algorithm
         */
        private void fillGrid()
        {

        }

        private Boolean isPow2Plus1(int n) {
            if (n <= 0)
            {
                return false;
            }
            n--;
            while (n > 2)
            {
                if (n % 2 != 0)
                {
                    return false;
                }
                n = n / 2;
            }
            return true;
        }

        private int getNextPow2Plus1(int n)
        {
            n++;
            while (!isPow2Plus1(n))
            {
                n++;
            }
            return n;
        }
    }
}
