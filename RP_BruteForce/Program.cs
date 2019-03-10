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
        public int numberOf1;
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
        public int[] indices;
        int matrixSize;
        public LinesDistributor(int matrixSize, int numberOfSectionSepararators)
        {
            indices = new int[numberOfSectionSepararators + 2];
            this.matrixSize = matrixSize;
            indices[0] = 0;
            InitialPosition();
            indices[numberOfSectionSepararators + 1] = matrixSize;
        }
        void InitialPosition()
        {
            for (int i = 1; i < indices.Length - 1; i++)
            {
                indices[i] = i;
            }
        }
        public bool NextPosition()
        {
            bool overflow = false;
            int i = indices.Length - 2;
            indices[i]++;
            while(indices[i] > matrixSize - indices.Length + 2 + i )
            {
                overflow = true;
                i--;
                if (i < 1)
                {
                    InitialPosition();
                    return false;
                }
                indices[i]++;
            }
            if(overflow)
            {
                for (int j = i + 1; j < indices.Length - 1; j++) 
                {
                    indices[j] = indices[j - 1] + 1;
                }
            }
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
                Console.WriteLine("Wrong format. Press any key to exit.");
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
                        PatternMatrix.numberOf1++;
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
        }

        static void SwapElement(int i, int j)
        {
            if (RndmMatrix.element[i][j] == true)
            {
                RndmMatrix.element[i][j] = false;
                RndmMatrix.numberOf1--;
            }
            else
            {
                RndmMatrix.element[i][j] = true;
                RndmMatrix.numberOf1++;
            }
        }

        static bool CheckRectangle(LinesDistributor ld, LinesDistributor cd, int lineNum, int colNum)
        {
            for (int i = ld.indices[lineNum]; i < ld.indices[lineNum + 1]; i++)
            {
                for(int j = cd.indices[colNum]; j < cd.indices[colNum + 1]; j++)
                {
                    if(RndmMatrix.element[i][j])
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        static bool CheckPattern(LinesDistributor ld, LinesDistributor cd)
        {
            for (int i = 0; i < PatternMatrix.linesNumber; i++)
            {
                for (int j = 0; j < PatternMatrix.columnNumber; j++)
                {
                    //if there must be a 1 element in specified rectangle
                    if (PatternMatrix.element[i][j])
                    {
                        if (!CheckRectangle(ld, cd, i, j))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        static bool ChangeAndTestMatrix()
        {
            int rndmLine = rndm.Next(RndmMatrix.linesNumber);
            int rndmColumn = rndm.Next(RndmMatrix.columnNumber);
            SwapElement(rndmLine, rndmColumn);

            //test whether forbidden pattern wasn't made
            if(RndmMatrix.numberOf1 >= PatternMatrix.numberOf1)
            {
                LinesDistributor linesDistributor = new LinesDistributor(RndmMatrix.linesNumber, PatternMatrix.linesNumber - 1);
                LinesDistributor columnDistributor = new LinesDistributor(RndmMatrix.columnNumber, PatternMatrix.columnNumber - 1);

                //tests every rectangle
                do
                {
                    do
                    {
                        if(CheckPattern(linesDistributor,columnDistributor))
                        {
                            SwapElement(rndmLine, rndmColumn);
                            return false;
                        }

                    } while (columnDistributor.NextPosition());

                } while (linesDistributor.NextPosition());
            }
            return true;
        }
       
        static void Main(string[] args)
        {
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

            int counter = 0;
            for (int i = 0; i < 1000; i++)
            {
                if (ChangeAndTestMatrix()) 
                {
                    counter++;
                    if (counter % 64 == 0)
                    {
                        Console.WriteLine("___matice {0}:___", i);
                        PrintMatrix(RndmMatrix);
                    }
                }
            }
        }
    }
}
