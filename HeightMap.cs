using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;

namespace Project1
{
    using SharpDX.Toolkit.Graphics;
    class HeightMap
    {
        private List<List<float>> grid;
        private int sideLength;
        private List<float> randRange;
        private Random r;

        public HeightMap(int sideLength, List<float> corners, List<float> randRange)
        {
            if (!isPow2Plus1(sideLength)) {
                sideLength = getNextPow2Plus1(sideLength);
            }
            this.sideLength = sideLength;
            this.randRange = randRange;
            this.r = new Random();

            fillGrid(corners);
        }

        public List<List<float>> getGrid()
        {
            return this.grid;
        }

        /**
         * Using the Diamond-square algorithm
         * note: corner indices 0..4 correspond to: 
         *       top left, bottom left, bottom right, top right
         */
        private void fillGrid(List<float> corners)
        {
            if (corners.Count() != 4 || this.randRange.Count() != 2)
            {
                throw new ArgumentException("corners must have 4 elements, and randRange must have 2");
            }
            // fill in corner values
            this.grid = new List<List<float>>();
            List<float> randRangeCopy = new List<float>() { this.randRange[0], this.randRange[1] };
            
            for (int i = 0; i < this.sideLength; i++)
            {
                List<float> row = new List<float>();
                for (int j = 0; j < this.sideLength; j++)
                {
                    row.Add(0f);
                }
                this.grid.Add(row);
            }
            this.grid[0][0] = corners[0];
            this.grid[this.sideLength-1][0] = corners[1];
            this.grid[0][this.sideLength - 1] = corners[2];
            this.grid[this.sideLength - 1][this.sideLength - 1] = corners[3];

            int side = this.sideLength - 1;
            while (side > 1)
            {
                // square step
                // for each square of size side*side
                for (int i = 0; i < this.sideLength - side; i += side)
                {
                    for (int j = 0; j < this.sideLength - side; j += side)
                    {
                        float average = getAverageOfSquareCorners(i, j, side);
                        float adjustment = getRandInRange(randRangeCopy);
                        this.grid[i + side / 2][j + side / 2] = average + adjustment;
                    }
                }
                // diamond step
                for (int i = side / 2; i < this.sideLength - side/2; i += side)
                {
                    for (int j = 0; j < this.sideLength; j += side)
                    {
                        float average = getAverageOfDiamondNeighbours(i, j, side);
                        float adjustment = getRandInRange(randRangeCopy);
                        this.grid[i][j] = average + adjustment;
                    }
                }
                for (int i = 0; i < this.sideLength; i += side)
                {
                    for (int j = side / 2; j < this.sideLength - side/2; j += side)
                    {
                        float average = getAverageOfDiamondNeighbours(i, j, side);
                        float adjustment = getRandInRange(randRangeCopy);
                        this.grid[i][j] = average + adjustment;
                    }
                }

                randRangeCopy[0] = randRangeCopy[0] / 2.0f;
                randRangeCopy[1] = randRangeCopy[1] / 2.0f;
                side = side / 2;
            }
        }

        private float getAverageOfSquareCorners(int i, int j, int side)
        {
            // square has corners (i,j), (i+side,j), (i,j+side), (i+side,j+side)
            float topLeft = this.grid[i][j];
            float topRight = this.grid[i][j + side];
            float bottomLeft = this.grid[i + side][j];
            float bottomRight = this.grid[i + side][j + side];
            float average = (topLeft + topRight + bottomLeft + bottomRight) / 4.0f;
            return average;
        }

        private float getAverageOfDiamondNeighbours(int i, int j, int side)
        {
            // diamond centre is at (i,j)
            List<float> neighbours = new List<float>();
            if (i - side / 2 >= 0)
            {
                neighbours.Add(this.grid[i - side / 2][j]);
            }
            if (i + side / 2 < this.sideLength)
            {
                neighbours.Add(this.grid[i + side / 2][j]);
            }
            if (j - side / 2 >= 0)
            {
                neighbours.Add(this.grid[i][j - side / 2]);
            }
            if (j + side / 2 < this.sideLength)
            {
                neighbours.Add(this.grid[i][j + side / 2]);
            }
            float average = neighbours.Sum() / neighbours.Count();
            return average;
        }

        public float getMinHeight() {
            float min = grid[0][0];
            for (int i = 0; i < sideLength; i++)
            {
                for (int j = 0; j < sideLength; j++)
                {
                    if (grid[i][j] < min)
                    {
                        min = grid[i][j];
                    }
                }
            }
            return min;
        }
        public float getMaxHeight() {
            float max = grid[0][0];
            for (int i = 0; i < sideLength; i++)
            {
                for (int j = 0; j < sideLength; j++)
                {
                    if (grid[i][j] > max)
                    {
                        max = grid[i][j];
                    }
                }
            }
            return max;
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

        public override string ToString()
        {
            string str = "";
            for (int i = 0; i < this.sideLength; i++)
            {
                for (int j = 0; j < this.sideLength; j++)
                {
                    str += this.grid[i][j].ToString() + " ";
                }
                str += "\n";
            }
            return str;
        }

    }
}
