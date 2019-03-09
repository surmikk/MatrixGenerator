using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RP_BruteForce
{
    class MatrixSize
    {
        public int linesNumber;
        public int columnNumber;
        public MatrixSize(int lines, int columns)
        {
            linesNumber = lines;
            columnNumber = columns;
        }
    }

    class Matrix01
    {
        public readonly int linesNumber;
        public readonly int columnNumber;
        public BitArray[] element;
        public Matrix01(MatrixSize size)
        {
            linesNumber = size.linesNumber;
            columnNumber = size.columnNumber;
            element = new BitArray[linesNumber];
            for (int i = 0; i < linesNumber; i++)
            {
                element[i] = new BitArray(columnNumber, false);
            }
        }

    }

    class LinesDistributor
    {
        int[] indices;
        int size;
        public LinesDistributor(int size, int numberOfSectionSepararators)
        {
            indices = new int[numberOfSectionSepararators];
            this.size = size;
            for (int i = 0; i < numberOfSectionSepararators; i++)
            {
                indices[i] = i + 1;
            }
            PrintState();
        }
        void PrintState()
        {
            for (int i = 0; i < indices.Length; i++)
            {
                Console.Write("{0} ", indices[i]);
            }
            Console.WriteLine();
        }
        public bool NextPosition()
        {
            bool overflow = false;
            int i = indices.Length - 1;
            indices[i]++;;
            while(indices[i] > size - indices.Length + i )
            {
                overflow = true;
                i--;
                if (i < 0) return false;
                indices[i]++;
            }
            if(overflow)
            {
                for (int j = i + 1; j < indices.Length; j++) 
                {
                    indices[j] = indices[j - 1] + 1;
                }
            }
            //PrintState();
            return true;
        }
    }

    class Program
    {
        static Matrix01 RndmMatrix;
        static Matrix01 PatternMatrix;
        static Random rndm = new Random();

        static MatrixSize LoadMatrixSize()
        {
            Console.WriteLine("Enter matrix size (e.g.: 3 5)");
            string input = Console.ReadLine();
            string[] tokens = input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length != 2 ||
                !int.TryParse(tokens[0], out int linesNumber) ||
                !int.TryParse(tokens[1], out int columnNumber))
            {
                Console.WriteLine("Wrong format. Press any key for exit.");
                Console.ReadLine();
                return null;
            }
            if (linesNumber < 1 || columnNumber < 1) return null;
            return new MatrixSize(linesNumber, columnNumber);
        }

        static void LoadPatternMatrix()
        {
            for (int i = 0; i < PatternMatrix.linesNumber; i++)
            {
                string inputLine = Console.ReadLine();
                if(inputLine.Length != PatternMatrix.columnNumber)
                {
                    throw new FormatException();
                }

                for (int j = 0; j < PatternMatrix.columnNumber; j++)
                {
                    if (inputLine[j] != '0')
                    {
                        PatternMatrix.element[i][j] = true;
                    }
                }
            }
        }

        static void PrintMatrix(Matrix01 matrix)
        {
            for (int i = 0; i < matrix.linesNumber; i++)
            {
                for (int j = 0; j < matrix.columnNumber; j++)
                {
                    if(matrix.element[i][j] == true)
                    {
                        Console.Write("1");
                    }
                    else
                    {
                        Console.Write("0");
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");
        }

        static void SwapElement(int i, int j)
        {
            if (RndmMatrix.element[i][j] == true)
            {
                RndmMatrix.element[i][j] = false;
            }
            else
            {
                RndmMatrix.element[i][j] = true;
            }
        }

        static void ChangeAndTestMatrix()
        {
            int rndmLine = rndm.Next(RndmMatrix.linesNumber);
            int rndmColumn = rndm.Next(RndmMatrix.columnNumber);
            SwapElement(rndmLine, rndmColumn);


        }
       
        static void Main(string[] args)
        {
            /*
            MatrixSize size;
            if ((size = LoadMatrixSize()) == null) 
            {
                return;
            }
            RndmMatrix = new Matrix01(size);
            if ((size = LoadMatrixSize()) == null || 
                size.linesNumber > RndmMatrix.linesNumber || 
                size.columnNumber > RndmMatrix.columnNumber)
            {
                return;
            }
            PatternMatrix = new Matrix01(size);
            LoadPatternMatrix();



            //PrintMatrix(RndmMatrix);
            //PrintMatrix(PatternMatrix);
            */

            Stopwatch sw = new Stopwatch();

            LinesDistributor ld = new LinesDistributor(200, 6);
            ld.NextPosition();
            sw.Start();
            while (ld.NextPosition() != false)
            {

            }
            sw.Stop();
            Console.WriteLine("Elapsed={0}", sw.Elapsed);
        }
    }
}
