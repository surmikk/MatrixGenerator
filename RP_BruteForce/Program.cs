using System;
using System.IO;
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
        /// <summary>
        /// an array that contains bounds of the current matrix side distribution
        /// </summary>
        public bool distributionSuccesfull;
        public int[] indices;
        public int[] lowerBorder;
        public int[] upperBorder;
        int rndmMatrixSize;
        int[] indicesX;
        public LinesDistributor(int matrixSize, int numberOfSectionSepararators, int patternLine, int swappedLine)
        {
            indices = new int[numberOfSectionSepararators + 2];
            lowerBorder = new int[numberOfSectionSepararators + 2];
            upperBorder = new int[numberOfSectionSepararators + 2];
            indicesX = new int[numberOfSectionSepararators + 2];
            rndmMatrixSize = matrixSize;

            lowerBorder[1] = 0;
            indicesX[indicesX.Length - 1] = matrixSize;
            upperBorder[upperBorder.Length - 2] = matrixSize;
            if (patternLine == 0)
            {
                lowerBorder[1] = swappedLine + 1;
            }
            else if (patternLine == numberOfSectionSepararators)
            {
                upperBorder[upperBorder.Length - 2] = swappedLine + 1;
            }
            else
            {
                upperBorder[patternLine] = swappedLine + 1; 
                lowerBorder[patternLine + 1] = swappedLine + 1;
            }
            distributionSuccesfull = TryInitialize();
        }
        public void Initialize()
        {
            distributionSuccesfull = true;
            for (int i = 0; i < indicesX.Length; i++)
            {
                indices[i] = indicesX[i];
            }
        }
        bool TryInitialize()
        {
            for(int i = 1; i < lowerBorder.Length - 1; i++)
            {
                if(lowerBorder[i] == 0)
                {
                    lowerBorder[i] = lowerBorder[i - 1] + 1;
                }
            }
            for (int i = upperBorder.Length - 2; i > 0; i--)
            {
                if (upperBorder[i] == 0)
                {
                    upperBorder[i] = upperBorder[i + 1] - 1;
                }
            }
            for(int i = 1; i < indicesX.Length - 1; i++)
            {
                if (lowerBorder[i] <= upperBorder[i] && lowerBorder[i] < rndmMatrixSize && upperBorder[i] > 1) 
                {
                    indicesX[i] = lowerBorder[i];
                }
                else return false;
            }
            Initialize();
            return true;
        }

        /// <summary>
        /// Tries to make another distribution. Returns false if there isn't another distribution
        /// </summary>
        public bool NextPosition()
        {
            bool overflow = false;
            int i = indices.Length - 2;
            indices[i]++;
            while(indices[i] == upperBorder[i])
            {
                overflow = true;
                i--;
                if (i < 1)
                {
                    Initialize();
                    return false;
                }
                indices[i]++;
            }
            if(overflow)
            {
                for (int j = i + 1; j < indices.Length - 1; j++) 
                {
                    if (indices[j - 1] + 1 < lowerBorder[j])
                    {
                        indices[j] = lowerBorder[j];
                    }
                    else
                    {
                        indices[j] = indices[j - 1] + 1;
                    }
                }
            }
            return true;
        }
    }

    class Program
    {
        static Matrix01 RndmMatrix;
        static Matrix01 PatternMatrix;
        static Random rndm = new Random(1);

        static MatrixSize LoadMatrixSize(string inputLine)
        {
            int linesNumber;
            int columnNumber;
            string[] tokens = inputLine.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length != 2 ||
                !int.TryParse(tokens[0], out linesNumber) ||
                !int.TryParse(tokens[1], out columnNumber))
            {
                Console.WriteLine("Wrong format. Press any key to exit.");
                Console.ReadLine();
                return null;
            }
            if (linesNumber < 1 || columnNumber < 1) return null;
            return new MatrixSize(linesNumber, columnNumber);
        }

        static void LoadPatternMatrix(TextReader sr)
        {
            for (int i = 0; i < PatternMatrix.linesNumber; i++)
            {
                string inputLine = sr.ReadLine();
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

        static bool SwapElement(int i, int j)
        {
            if (RndmMatrix.element[i][j] == true)
            {
                RndmMatrix.element[i][j] = false;
                RndmMatrix.numberOf1--;
                return false;
            }
            else
            {
                RndmMatrix.element[i][j] = true;
                RndmMatrix.numberOf1++;
                return true;
            }
        }
        static bool DistributionPossible(int randomMatrixLine, int patternLine, int rndmSize, int patternSize)
        {
            if (randomMatrixLine + patternSize - patternLine <= rndmSize &&
                randomMatrixLine - patternLine >= 0)
            {
                return true;
            }
            else return false;
        }
        /// <summary>
        /// Returns true if there is an 1 element in given rectangle
        /// </summary>
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
        /// <summary>
        /// Tests whether every rectangle inside random matrix contains an 1 for given distribution and pattern.
        /// </summary>
        /// <param name="Distribution of lines"></param>
        /// <param name="Distribution of columns"></param>
        static bool CheckPattern(LinesDistributor ld, LinesDistributor cd, int patternLine, int patternCol)
        {
            for (int i = 0; i < PatternMatrix.linesNumber; i++)
            {
                for (int j = 0; j < PatternMatrix.columnNumber; j++)
                {
                    if(i != patternLine || j != patternCol)
                    {
                        //if there must be a 1 element in specified rectangle
                        if (PatternMatrix.element[i][j])
                        {
                            if (!CheckRectangle(ld, cd, i, j))
                            {
                                //forbidden patern does'n exist in this distribution
                                return false;
                            }
                        }
                    }
                }
            }
            //forbidden patern found
            return true;
        }
        /// <summary>
        /// Returns true if the new matrix doesn't contain forbidden pattern, otherwise keeps original matrix.
        /// </summary>
        static bool ChangeAndTestMatrix()
        {
            int rndmLine = rndm.Next(RndmMatrix.linesNumber);
            int rndmColumn = rndm.Next(RndmMatrix.columnNumber);
            bool element1Created = SwapElement(rndmLine, rndmColumn);

            //tests whether forbidden pattern wasn't made
            if(element1Created && RndmMatrix.numberOf1 >= PatternMatrix.numberOf1)
            {
                LinesDistributor linesDistributor;
                LinesDistributor columnDistributor;

                for (int i = 0; i < PatternMatrix.linesNumber; i++)
                {
                    for (int j = 0; j < PatternMatrix.columnNumber; j++)
                    {
                        if (PatternMatrix.element[i][j])
                        {
                            if (DistributionPossible(rndmLine, i, RndmMatrix.linesNumber, PatternMatrix.linesNumber))
                            {
                                linesDistributor = new LinesDistributor(RndmMatrix.linesNumber, PatternMatrix.linesNumber - 1, i, rndmLine);
                            }
                            else goto ENDLines;
                            if (DistributionPossible(rndmColumn, j, RndmMatrix.columnNumber, PatternMatrix.columnNumber))
                            {
                                columnDistributor = new LinesDistributor(RndmMatrix.columnNumber, PatternMatrix.columnNumber - 1, j, rndmColumn);
                            }
                            else goto ENDColumns;
                            if(linesDistributor.distributionSuccesfull && columnDistributor.distributionSuccesfull) //SMAZAT!!!!!!!!!!!!!!
                            {
                                do
                                {
                                    do
                                    {
                                        if (CheckPattern(linesDistributor, columnDistributor,i,j))
                                        {
                                            SwapElement(rndmLine, rndmColumn);
                                            return false;
                                        }

                                    } while (columnDistributor.NextPosition());

                                } while (linesDistributor.NextPosition());
                            }
                        }
                        ENDColumns:;
                    }
                    ENDLines:;
                }
            }
            return true;
        }
       
        static void Main(string[] args)
        {
            int iterationsNumber;
            MatrixSize size;
            if (args.Length > 0)
            {
                StreamReader sr = new StreamReader(args[0]);
                string input = sr.ReadLine();
                size = LoadMatrixSize(input);
                if (size == null) return;
                RndmMatrix = new Matrix01(size);

                if ((size = LoadMatrixSize(sr.ReadLine())) == null ||
                    size.linesNumber > RndmMatrix.linesNumber ||
                    size.columnNumber > RndmMatrix.columnNumber)
                {
                    return;
                }
                PatternMatrix = new Matrix01(size);
                LoadPatternMatrix(sr);
                iterationsNumber = int.Parse(sr.ReadLine());
            }
            else
            {
                Console.WriteLine("**Enter \"help\" for illustration of a valid input**");
                Console.WriteLine("Enter sizes of random matrix.");
                string input = Console.ReadLine();
                if (input == "help")
                {
                    Console.WriteLine("->Enter sizes of random matrix. (lines \"space\" columns)");
                    Console.WriteLine("3 5");
                    Console.WriteLine("->Enter sizes of pattern matrix.");
                    Console.WriteLine("2 3");
                    Console.WriteLine("->Enter pattern 01-matrix of given sizes. (every row on a new line, without spaces)");
                    Console.WriteLine("101");
                    Console.WriteLine("010");
                    Console.WriteLine("->Enter number of random iterations.");
                    Console.WriteLine("200");
                    Console.WriteLine("**END of the illustration**");
                    Console.WriteLine();
                    Console.WriteLine("Enter sizes of random matrix.");
                    input = Console.ReadLine();
                    size = LoadMatrixSize(input);
                }
                else
                {
                    size = LoadMatrixSize(input);
                }
                if (size == null) return;
                RndmMatrix = new Matrix01(size);

                Console.WriteLine("Enter sizes of pattern matrix.");
                if ((size = LoadMatrixSize(Console.ReadLine())) == null ||
                    size.linesNumber > RndmMatrix.linesNumber ||
                    size.columnNumber > RndmMatrix.columnNumber)
                {
                    return;
                }
                PatternMatrix = new Matrix01(size);
                Console.WriteLine("Enter pattern 01-matrix of given sizes.");
                LoadPatternMatrix(Console.In);

                Console.WriteLine("Enter number of random iterations.");
                iterationsNumber = int.Parse(Console.ReadLine());
                Console.WriteLine("**Enter \"end\" to exit or any key to continue**");
            }

            Stopwatch sw = new Stopwatch();

            while(PatternMatrix.numberOf1 > RndmMatrix.numberOf1)
            {
                ChangeAndTestMatrix();
            }

            string entry;
            do
            {
                sw.Restart();
                for (int i = 0; i < iterationsNumber; i++)
                {
                    ChangeAndTestMatrix();
                }
                sw.Stop();
                Console.WriteLine(sw.Elapsed);
                PrintMatrix(RndmMatrix);
            } while((entry = Console.ReadLine()) != "end");
        }
    }
}
