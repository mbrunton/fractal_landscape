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
        private Random r;

        public HeightMap(int sideLength, List<float> corners, List<float> randRange)
        {
            if (!isPow2Plus1(sideLength)) {
                sideLength = getNextPow2Plus1(sideLength);
            }
            this.sideLength = sideLength;
            this.r = new Random();

            fillGrid(corners, randRange);
        }

        public List<List<float>> getGrid()
        {
            return this.grid;
        }

        /**
         * Using the Diamond-square algorithm
         */
        private void fillGrid(List<float> corners, List<float> randRange)
        {
            // fill in corner values

            int side = this.sideLength - 1;
            int i, j;
            while (side > 0)
            {
                // square step
                // for each square of size side*side
                for (i = 0; i < this.sideLength - side; i += side)
                {
                    for (j = 0; j < this.sideLength - side; j += side)
                    {
                        // square has corners (i,j), (i+side,j), (i,j+side), (i+side,j+side)
                        float topLeft = this.grid[i][j];
                        float topRight = this.grid[i][j + side];
                        float bottomLeft = this.grid[i + side][j];
                        float bottomRight = this.grid[i + side][j + side];
                        float centre = (topLeft + topRight + bottomLeft + bottomRight) / 4.0f;
                        float adjustment = getRandInRange(randRange);
                        this.grid[i + side / 2][j + side / 2] = centre + adjustment;
                    }
                }
                // diamond step
                

                randRange[0] = randRange[0] / 2.0f;
                randRange[1] = randRange[1] / 2.0f;
                side = side / 2;
            }
        }

        /**
         * randRange is of form {a, b}
         * function returns float fl such that a <= fl <= b
         */
        private float getRandInRange(List<float> randRange) {
            float fl = (float) r.NextDouble();
            fl = fl * (randRange[1] - randRange[0]);
            fl = fl + randRange[0];
            return fl;
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
