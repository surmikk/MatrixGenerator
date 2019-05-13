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

    class Matrix<T>
    {
        public readonly int linesNumber;
        public readonly int columnNumber;
        public T[][] element;
        public int numberOf1;
        public Matrix(MatrixSize size)
        {
            linesNumber = size.linesNumber;
            columnNumber = size.columnNumber;
            element = new T[linesNumber][];
            for (int i = 0; i < linesNumber; i++)
            {
                element[i] = new T[columnNumber];
            }
        }
        public Matrix(Matrix<T> originalMatrix)
        {
            linesNumber = originalMatrix.linesNumber;
            columnNumber = originalMatrix.columnNumber;
            element = new T[linesNumber][];
            for (int i = 0; i < linesNumber; i++)
            {
                element[i] = new T[columnNumber];
            }
            for (int i = 0; i < linesNumber; i++)
            {
                for (int j = 0; j < columnNumber; j++)
                {
                    element[i][j] = originalMatrix.element[i][j];
                }
            }
        }
    }  

    class Distributor
    {
        class LinesDistributor
        {
            /// <summary>
            /// an array that contains bounds of the current matrix side distribution
            /// </summary>
            public readonly int[] indices;
            public int[] lowerBorder;
            public int[] upperBorder;
            readonly int rndmMatrixSize;
            public LinesDistributor(int matrixSize, int numberOfSectionSepararators, int patternLine, int swappedLine)
            {
                indices = new int[numberOfSectionSepararators + 2];
                lowerBorder = new int[numberOfSectionSepararators + 2];
                upperBorder = new int[numberOfSectionSepararators + 2];
                rndmMatrixSize = matrixSize;

                lowerBorder[1] = 0;
                upperBorder[upperBorder.Length - 2] = matrixSize;
                indices[indices.Length - 1] = matrixSize;
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
                SetBorders();
                InitializeIndices();
            }
            public void InitializeIndices()
            {
                for (int i = 1; i < indices.Length - 1; i++)
                {
                    indices[i] = lowerBorder[i];
                }
            }
            public void SetBorders()
            {
                for (int i = 1; i < lowerBorder.Length - 1; i++)
                {
                    if (lowerBorder[i] == 0)
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
            }
            
            /// <summary>
            /// Tries to make another distribution. Returns false if there isn't another distribution
            /// </summary>
            public bool NextPosition()
            {
                bool overflow = false;
                int i = indices.Length - 2;
                indices[i]++;
                while (indices[i] == upperBorder[i])
                {
                    overflow = true;
                    i--;
                    if (i < 1)
                    {
                        InitializeIndices();
                        return false;
                    }
                    indices[i]++;
                }
                if (overflow)
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

        Context context;
        LinesDistributor linesDistributor;
        LinesDistributor columnDistributor;

        // remembers values from column distributor's constructor
        int[] originalColDistLowerBorder;
        int[] originalColDistUpperBorder;

        readonly int patternLine;
        readonly int patternColumn;
        public readonly bool linesDistributorLoaded = false;
        public readonly bool columnDistributorLoaded = false;

        public Distributor(Context context, int patternLine, int patternColumn, int swapedLine, int swapedColumn)
        {
            this.context = context;
            this.patternColumn = patternColumn;
            this.patternLine = patternLine;

            //lines distributor constructor
            if (DistributionPossible(swapedLine, patternLine, context.RndmMatrix.linesNumber, context.PatternMatrix.linesNumber))
            {
                linesDistributor = new LinesDistributor(context.RndmMatrix.linesNumber, context.PatternMatrix.linesNumber - 1, patternLine, swapedLine);
                linesDistributorLoaded = true;
            }
            else return;

            //column distributor constructor
            if (DistributionPossible(swapedColumn, patternColumn, context.RndmMatrix.columnNumber, context.PatternMatrix.columnNumber))
            {
                columnDistributor = new LinesDistributor(context.RndmMatrix.columnNumber, context.PatternMatrix.columnNumber - 1, patternColumn, swapedColumn);
                originalColDistLowerBorder = new int[columnDistributor.indices.Length];
                Array.Copy(columnDistributor.indices, originalColDistLowerBorder, columnDistributor.indices.Length);
                originalColDistUpperBorder = new int[columnDistributor.upperBorder.Length];
                Array.Copy(columnDistributor.upperBorder, originalColDistUpperBorder, columnDistributor.upperBorder.Length);
                columnDistributorLoaded = true;
            }
            else return;
        }

        /// <summary>
        /// For every possible line distribution tries to find forbidden pattern
        /// </summary>
        /// <returns></returns>
        public bool TryFindPattern()
        {
            do
            {
                if(MoveWithColumnSeparators())
                {
                    return true;
                }
            } while (linesDistributor.NextPosition());

            return false;
        }

        /// <summary>
        /// Returns true if there exists forbidden pattern
        /// </summary>
        /// <returns></returns>
        bool MoveWithColumnSeparators()
        {
            Array.Copy(originalColDistUpperBorder, columnDistributor.upperBorder, originalColDistUpperBorder.Length);
            int [] auxiliaryColumnIndices = new int[originalColDistLowerBorder.Length];
            Array.Copy(originalColDistLowerBorder, auxiliaryColumnIndices, originalColDistLowerBorder.Length);

            //sets last separator most right
            auxiliaryColumnIndices[auxiliaryColumnIndices.Length - 2] = originalColDistUpperBorder[originalColDistUpperBorder.Length - 2] - 1;

            //last separator initialization (moving to the left as much as necessary)
            for (int i = 0; i < context.PatternMatrix.linesNumber; i++)
            {
                if (context.PatternMatrix.element[i][context.PatternMatrix.columnNumber - 1])
                {
                    while (auxiliaryColumnIndices[auxiliaryColumnIndices.Length - 2] >= originalColDistLowerBorder[originalColDistLowerBorder.Length - 2])
                    {
                        if(context.CheckRectangle(linesDistributor.indices, auxiliaryColumnIndices, i, context.PatternMatrix.columnNumber - 1))
                        {
                            break;
                        }
                        else
                        {
                            auxiliaryColumnIndices[auxiliaryColumnIndices.Length - 2]--;
                        }
                    }
                }
            }
            if (auxiliaryColumnIndices[auxiliaryColumnIndices.Length - 2] < originalColDistLowerBorder[originalColDistLowerBorder.Length - 2])
            {
                //last separator was moved to the most left possible possition but still the last column doesn't contain given pattern's part
                return false;
            }

            // set correct upper borders for other separators
            columnDistributor.upperBorder[columnDistributor.upperBorder.Length - 2] = auxiliaryColumnIndices[auxiliaryColumnIndices.Length - 2] + 1;
            for (int i = columnDistributor.upperBorder.Length - 3; i > 0; i--) 
            {
                columnDistributor.upperBorder[i] = Minimum(columnDistributor.upperBorder[i + 1] - 1, originalColDistUpperBorder[i]);
            }

            //moving separators,from the first to the last but one, to the right as much as necessary for containing all 1 elements in given column
            for (int j = 0; j < context.PatternMatrix.columnNumber - 1; j++)
            {
                auxiliaryColumnIndices[j + 1] = Maximum(originalColDistLowerBorder[j + 1], auxiliaryColumnIndices[j]);
                for (int i = 0; i < context.PatternMatrix.linesNumber; i++)
                {
                    if (context.PatternMatrix.element[i][j])
                    {
                        while (auxiliaryColumnIndices[j + 1] < columnDistributor.upperBorder[j + 1])
                        {
                            if (context.CheckRectangle(linesDistributor.indices, auxiliaryColumnIndices, i, j))
                            {
                                break;
                            }
                            else
                            {
                                auxiliaryColumnIndices[j + 1]++;
                            }
                        }
                    }
                }
                if (auxiliaryColumnIndices[j + 1] == columnDistributor.upperBorder[j + 1])
                {
                    // the j-th separator was moved to the most right possible possition but still the j-th column doesn't contain the j-th column of pattern matrix
                    return false;
                }
            }
            // separators can be moved to positions so that matrix contains the forbiden pattern
            return true;
        }

        int Maximum(int x, int y)
        {
            if (x > y)
            {
                return x;
            }
            else return y;
        }

        int Minimum(int x, int y)
        {
            if (x < y)
            {
                return x;
            }
            else return y;
        }

        bool DistributionPossible(int randomMatrixLine, int patternLine, int rndmSize, int patternSize)
        {
            if (randomMatrixLine + patternSize - patternLine <= rndmSize &&
                randomMatrixLine - patternLine >= 0)
            {
                return true;
            }
            else return false;
        }
    }

    /// <summary>
    /// Contains all necessary matrices and methods for their checking and repairing
    /// </summary>
    class Context
    {
        public Matrix<bool> RndmMatrix;
        public Matrix<bool> PatternMatrix;
        /// <summary>
        /// element[i][j] contains sum of all 1 in the left-upper direction from this position
        /// </summary>
        public Matrix<int> CountingMatrix;

        public Context(Matrix<bool> RandomMatrix, Matrix<bool> PatternMatrix, Matrix<int> matrix)
        {
            RndmMatrix = RandomMatrix;
            this.PatternMatrix = PatternMatrix;
            this.CountingMatrix = matrix;
        }

        public bool SwapElement(int i, int j)
        {
            if (RndmMatrix.element[i][j] == true)
            {
                RndmMatrix.element[i][j] = false;
                RndmMatrix.numberOf1--;
                RepairCountingMatrix(i, j, false);
                return false;
            }
            else
            {
                RndmMatrix.element[i][j] = true;
                RndmMatrix.numberOf1++;
                RepairCountingMatrix(i, j, true);
                return true;
            }
        }

        void RepairCountingMatrix(int x, int y, bool element1Created)
        {
            for (int i = x; i < CountingMatrix.linesNumber; i++)
            {
                for (int j = y; j < CountingMatrix.columnNumber; j++)
                {
                    if (element1Created)
                    {
                        CountingMatrix.element[i][j]++;
                    }
                    else CountingMatrix.element[i][j]--;
                }
            }
        }

        /// <summary>
        /// Returns true if there is an 1 element in given rectangle
        /// </summary>
        public bool CheckRectangle(int[] lineIndices, int[] columnIndices, int lineNum, int colNum)
        {
            int numberOf1InRectangle = 0;
            if (lineNum == 0)
            {
                //left upper rectangle
                if (colNum == 0)
                {
                    numberOf1InRectangle = CountingMatrix.element[lineIndices[1] - 1][columnIndices[1] - 1];
                }
                else
                {
                    //upper rectangles
                    numberOf1InRectangle = CountingMatrix.element[lineIndices[1] - 1][columnIndices[colNum + 1] - 1]
                                         - CountingMatrix.element[lineIndices[1] - 1][columnIndices[colNum] - 1];
                }
            }
            else if (colNum == 0)
            {
                //left rectangles
                numberOf1InRectangle = CountingMatrix.element[lineIndices[lineNum + 1] - 1][columnIndices[1] - 1]
                                     - CountingMatrix.element[lineIndices[lineNum] - 1][columnIndices[1] - 1];
            }
            else
            {
                //other possibilities
                numberOf1InRectangle = CountingMatrix.element[lineIndices[lineNum + 1] - 1][columnIndices[colNum + 1] - 1]
                                     - CountingMatrix.element[lineIndices[lineNum + 1] - 1][columnIndices[colNum] - 1]
                                     - CountingMatrix.element[lineIndices[lineNum] - 1][columnIndices[colNum + 1] - 1]
                                     + CountingMatrix.element[lineIndices[lineNum] - 1][columnIndices[colNum] - 1];
            }

            if (numberOf1InRectangle > 0)
            {
                return true;
            }
            else return false;
        }
    }

    class Program
    {
        static Matrix<bool> RndmMatrix;
        static Matrix<bool> PatternMatrix;
        static Matrix<int> CountingMatrix;

        static bool transpositionIsNeeded = false;
        static Random rndm = new Random(1);

        static void ReportError(string message)
        {
            Console.WriteLine(message + " Press any key to exit.");
            Console.ReadLine();
        }

        static MatrixSize LoadMatrixSize(string inputLine)
        {
            int linesNumber;
            int columnNumber;
            string[] tokens = inputLine.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length != 2 ||
                !int.TryParse(tokens[0], out linesNumber) ||
                !int.TryParse(tokens[1], out columnNumber))
            {
                ReportError("Wrong format.");
                return null;
            }
            if (linesNumber < 1 || columnNumber < 1)
            {
                ReportError("Values must be positive.");
                return null;
            }
            return new MatrixSize(linesNumber, columnNumber);
        }

        static void LoadPatternMatrix(TextReader reader)
        {
            for (int i = 0; i < PatternMatrix.linesNumber; i++)
            {
                string inputLine = reader.ReadLine();
                if (inputLine.Length < PatternMatrix.columnNumber)
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

        static Matrix<bool> TransposeMatrix(Matrix<bool> matrix)
        {
            Matrix<bool> result = new Matrix<bool>(new MatrixSize(matrix.columnNumber, matrix.linesNumber));
            for (int i = 0; i < matrix.linesNumber; i++)
            {
                for (int j = 0; j < matrix.columnNumber; j++)
                {
                    result.element[j][i] = matrix.element[i][j];
                }
            }
            return result;
        }

        static void PrintMatrix(Matrix<bool> matrix)
        {
            if(transpositionIsNeeded)
            {
                for (int j = 0; j < matrix.columnNumber; j++)
                {
                    for (int i = 0; i < matrix.linesNumber; i++)
                    {
                        if (matrix.element[i][j] == true)
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
            else
            {
                for (int i = 0; i < matrix.linesNumber; i++)
                {
                    for (int j = 0; j < matrix.columnNumber; j++)
                    {
                        if (matrix.element[i][j] == true)
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
        }

        /// <summary>
        /// Returns true if the new matrix doesn't contain forbidden pattern, otherwise keeps original matrix.
        /// </summary>
        static bool ChangeAndTestMatrix(Context context)
        {
            int rndmLine = rndm.Next(RndmMatrix.linesNumber);
            int rndmColumn = rndm.Next(RndmMatrix.columnNumber);
            bool element1Created = context.SwapElement(rndmLine, rndmColumn);

            if (!element1Created || RndmMatrix.numberOf1 < PatternMatrix.numberOf1)
            {
                //trivial situation when forbidden pattern definitely wasn't made
                return true;
            }

            for (int i = 0; i < PatternMatrix.linesNumber; i++)
            {
                for (int j = 0; j < PatternMatrix.columnNumber; j++)
                {
                    if (PatternMatrix.element[i][j]) //tests all distributions where the new 1 element (and it's rectangular neighborhood) is contracted to element [i][j] of PatternMatrix
                    {
                        Distributor distributor = new Distributor(context, i, j, rndmLine, rndmColumn);
                        if (!distributor.linesDistributorLoaded)
                        {
                            break;
                        }
                        if (!distributor.columnDistributorLoaded)
                        {
                            continue;
                        }

                        if (distributor.TryFindPattern())
                        {
                            // forbidden pattern found
                            context.SwapElement(rndmLine, rndmColumn);
                            return false;
                        }
                    }
                }
            }
            // forbidden pattern wasn't found
            return true;
        }

        static void Main(string[] args)
        {
            MatrixSize size;
            int iterationsNumber;
            bool loadingFromFile = false;
            TextReader reader;
            if (args.Length > 0)
            {
                loadingFromFile = true;
                try
                {
                    reader = new StreamReader(args[0]);
                }
                catch (IOException)
                {
                    ReportError("Error while opening the file.");
                    return;
                }
            }
            else reader = Console.In;

            if (!loadingFromFile)
            {
                Console.WriteLine("**Enter \"help\" for illustration of a valid input**");
                Console.WriteLine("Enter sizes of random matrix.");
            }
            string input = reader.ReadLine();
            if (!loadingFromFile && input == "help")
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
                input = reader.ReadLine();
            }
            if ((size = LoadMatrixSize(input)) == null) return;

            RndmMatrix = new Matrix<bool>(size);
            CountingMatrix = new Matrix<int>(size);

            if (!loadingFromFile) Console.WriteLine("Enter sizes of pattern matrix.");

            if ((size = LoadMatrixSize(reader.ReadLine())) == null) return;
            else if (size.linesNumber > RndmMatrix.linesNumber || size.columnNumber > RndmMatrix.columnNumber)
            {
                ReportError("Pattern matrix can't be larger than generating matrix.");
                return;
            }

            PatternMatrix = new Matrix<bool>(size);
            if (!loadingFromFile) Console.WriteLine("Enter pattern 01-matrix of given sizes.");
            try
            {
                LoadPatternMatrix(reader);
            }
            catch (FormatException)
            {
                ReportError("The row is too short.");
                return;
            }

            // transpose matrix in case the pattern has more rows than columns
            if(PatternMatrix.linesNumber > PatternMatrix.columnNumber)
            {
                MatrixSize transposedSize = new MatrixSize(RndmMatrix.columnNumber, RndmMatrix.linesNumber);
                RndmMatrix = new Matrix<bool>(transposedSize);
                CountingMatrix = new Matrix<int>(transposedSize);
                PatternMatrix = TransposeMatrix(PatternMatrix);
                transpositionIsNeeded = true;
            }

            if (!loadingFromFile) Console.WriteLine("Enter number of random iterations.");
            if (!int.TryParse(reader.ReadLine(), out iterationsNumber) || iterationsNumber < 1)
            {
                ReportError("Wrong format of integer.");
                return;
            }
            Console.WriteLine("**Enter \"end\" to exit or any key to continue**");

            Stopwatch sw = new Stopwatch();

            Context context = new Context(RndmMatrix, PatternMatrix, CountingMatrix);
            while (PatternMatrix.numberOf1 > RndmMatrix.numberOf1)
            {
                ChangeAndTestMatrix(context);
            }

            string entry;
            do
            {
                sw.Restart();
                for (int i = 0; i < iterationsNumber; i++)
                {
                    ChangeAndTestMatrix(context);
                }
                sw.Stop();
                Console.WriteLine(sw.Elapsed);
                PrintMatrix(RndmMatrix);

            } while ((entry = Console.ReadLine()) != "end");
        }
    }
}