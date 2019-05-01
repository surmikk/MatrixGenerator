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

    struct Coordinates
    {
        public int line;
        public int column;
        public Coordinates(int line, int column)
        {
            this.line = line;
            this.column = column;
        }
    }

    class Matrix01
    {
        public readonly int linesNumber;
        public readonly int columnNumber;
        public int numberOf1;
        public bool[][] element;
        public Matrix01(MatrixSize size)
        {
            linesNumber = size.linesNumber;
            columnNumber = size.columnNumber;
            element = new bool[linesNumber][];
            for (int i = 0; i < linesNumber; i++)
            {
                element[i] = new bool[columnNumber];
            }
        }
        public Matrix01(Matrix01 originalMatrix)
        {
            linesNumber = originalMatrix.linesNumber;
            columnNumber = originalMatrix.columnNumber;
            element = new bool[linesNumber][];
            for (int i = 0; i < linesNumber; i++)
            {
                element[i] = new bool[columnNumber];
            }
            for (int i = 0; i < linesNumber; i++)
            {
                for(int j = 0; j < columnNumber; j++)
                {
                    element[i][j] = originalMatrix.element[i][j];
                }
            }
        }
    }

    /// <summary>
    /// Element [i][j] is true if and only if the line in RndmMatrix beginning in this element in the given direction contains an 1
    /// </summary>
    class LineMatrix : Matrix01
    {
        readonly bool directionLeft;
        readonly bool directionRight;
        readonly bool directionUpper;
        readonly bool directionLower;
        public LineMatrix(MatrixSize size, bool left, bool right, bool upper, bool lower) : base(size)
        {
            directionLeft = left;
            directionRight = right;
            directionUpper = upper;
            directionLower = lower;
        }
        public void Add1ToLine(int line, int column)
        {
            if (directionLeft)
            {
                for (int j = column; j < columnNumber; j++) { element[line][j] = true; }
            }
            if (directionRight)
            {
                for (int j = 0; j <= column; j++) { element[line][j] = true; }
            }
            if (directionUpper)
            {
                for (int i = line; i < linesNumber; i++) { element[i][column] = true; }
            }
            if (directionLower)
            {
                for (int i = 0; i <= line; i++) { element[i][column] = true; }
            }
        }
        public void Remove1FromLine(int line, int column, Matrix01 rndmMatrix)
        {
            if (directionLeft)
            {
                int j = 0;
                while (j < columnNumber && !rndmMatrix.element[line][j])
                {
                    element[line][j] = false;
                    j++;
                }
            }
            if (directionRight)
            {
                int j = columnNumber - 1;
                while (j >= 0 && !rndmMatrix.element[line][j])
                {
                    element[line][j] = false;
                    j--;
                }
            }
            if (directionUpper)
            {
                int i = 0;
                while (i < linesNumber && !rndmMatrix.element[i][column])
                {
                    element[i][column] = false;
                    i++;
                }
            }
            if (directionLower)
            {
                int i = linesNumber - 1;
                while (i >= 0 && !rndmMatrix.element[i][column])
                {
                    element[i][column] = false;
                    i--;
                }
            }
        }
    }
    
    /// <summary>
    /// Element [i][j] is true if and only if the rectangle in RndmMatrix beginning in this element and ending in a corner depending on given directions contains an 1
    /// </summary>
    class CornerMatrix : Matrix01
    {
        readonly bool directionLeft;
        readonly bool directionUpper;
        public CornerMatrix(MatrixSize size, bool directionLeft, bool directionUpper) : base(size)
        {
            this.directionLeft = directionLeft;
            this.directionUpper = directionUpper;
        }
        /// <summary>
        /// Repairs corner matrix after adding 1 element to RndmMatrix
        /// </summary>
        public void SetCornerTo1(int line, int col)
        {
            if (element[line][col]) return;
            int i = line;
            while (i >= 0 && i < linesNumber)
            {
                int j = col;
                while (j >= 0 && j < columnNumber && !element[i][j])
                {
                    element[i][j] = true;
                    if (directionLeft)
                    {
                        j++;
                    }
                    else j--;
                }
                if (directionUpper)
                {
                    i++;
                }
                else i--;
            }
        }
        /// <summary>
        /// Repairs corner matrix after deleting 1 element from RndmMatrix
        /// </summary>
        public void SetCornerTo0(int m, int n, Matrix01 rndmMatrix)
        {
            if (directionLeft)
            {
                if (directionUpper)
                {
                    if (m > 0 && n > 0)
                    {
                        if (element[m - 1][n] || element[m][n - 1])
                        {
                            return;
                        }
                    }
                    int i = 0;
                    while (i < linesNumber)
                    {
                        int j = 0;
                        while (j < columnNumber && !rndmMatrix.element[i][j])
                        {
                            if (i > 0 && element[i - 1][j])
                            {
                                break;
                            }
                            else
                            {
                                element[i][j] = false;
                            }
                            j++;
                        }
                        i++;
                    }
                }
                else
                {
                    if (m < linesNumber - 1 && n > 0)
                    {
                        if (element[m + 1][n] || element[m][n - 1])
                        {
                            return;
                        }
                    }
                    int i = linesNumber - 1;
                    while (i >= 0)
                    {
                        int j = 0;
                        while (j < columnNumber && !rndmMatrix.element[i][j])
                        {
                            if (i < linesNumber - 1 && element[i + 1][j])
                            {
                                break;
                            }
                            else
                            {
                                element[i][j] = false;
                            }
                            j++;
                        }
                        i--;
                    }
                }
            }
            else
            {
                if (directionUpper)
                {
                    if (m > 0 && n < columnNumber - 1)
                    {
                        if (element[m - 1][n] || element[m][n + 1])
                        {
                            return;
                        }
                    }
                    int i = 0;
                    while (i < linesNumber)
                    {
                        int j = columnNumber - 1;
                        while (j >= 0 && !rndmMatrix.element[i][j])
                        {
                            if (i > 0 && element[i - 1][j])
                            {
                                break;
                            }
                            else
                            {
                                element[i][j] = false;
                            }
                            j--;
                        }
                        i++;
                    }
                }
                else
                {
                    if (m < linesNumber - 1 && n < columnNumber - 1)
                    {
                        if (element[m + 1][n] || element[m][n + 1])
                        {
                            return;
                        }
                    }
                    int i = linesNumber - 1;
                    while (i >= 0)
                    {
                        int j = columnNumber - 1;
                        while (j >= 0 && !rndmMatrix.element[i][j])
                        {
                            if (i < linesNumber - 1 && element[i + 1][j])
                            {
                                break;
                            }
                            else
                            {
                                element[i][j] = false;
                            }
                            j--;
                        }
                        i--;
                    }
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
        public bool FindPattern()
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
        public Matrix01 RndmMatrix;
        public Matrix01 PatternMatrix;
        public CornerMatrix UpperLeft;
        public CornerMatrix UpperRight;
        public CornerMatrix LowerLeft;
        public CornerMatrix LowerRight;
        public LineMatrix Upper;
        public LineMatrix Lower;
        public LineMatrix Left;
        public LineMatrix Right;

        public Context(Matrix01 RandomMatrix, Matrix01 PatternMatrix, CornerMatrix UpperLeft, CornerMatrix UpperRight, CornerMatrix LowerLeft, CornerMatrix LowerRight,
            LineMatrix Upper, LineMatrix Lower, LineMatrix Left, LineMatrix Right)
        {
            RndmMatrix = RandomMatrix;
            this.PatternMatrix = PatternMatrix;
            this.UpperLeft = UpperLeft;
            this.UpperRight = UpperRight;
            this.LowerLeft = LowerLeft;
            this.LowerRight = LowerRight;
            this.Upper = Upper;
            this.Lower = Lower;
            this.Left = Left;
            this.Right = Right;
        }

        public bool SwapElement(int i, int j)
        {
            if (RndmMatrix.element[i][j] == true)
            {
                RndmMatrix.element[i][j] = false;
                RndmMatrix.numberOf1--;
                RepairCornerMatrices(i, j, false);
                RepairLineMatrices(i, j, false);
                return false;
            }
            else
            {
                RndmMatrix.element[i][j] = true;
                RndmMatrix.numberOf1++;
                RepairCornerMatrices(i, j, true);
                RepairLineMatrices(i, j, true);
                return true;
            }
        }

        void RepairLineMatrices(int i, int j, bool element1Created)
        {

            if (!element1Created)
            {
                //element 1 removed
                Left.Remove1FromLine(i, j, RndmMatrix);
                Right.Remove1FromLine(i, j, RndmMatrix);
                Upper.Remove1FromLine(i, j, RndmMatrix);
                Lower.Remove1FromLine(i, j, RndmMatrix);
            }
            else
            {
                Left.Add1ToLine(i, j);
                Right.Add1ToLine(i, j);
                Upper.Add1ToLine(i, j);
                Lower.Add1ToLine(i, j);
            }
        }

        void RepairCornerMatrices(int i, int j, bool element1Created)
        {
            if (element1Created)
            {
                UpperLeft.SetCornerTo1(i, j);
                UpperRight.SetCornerTo1(i, j);
                LowerLeft.SetCornerTo1(i, j);
                LowerRight.SetCornerTo1(i, j);
            }
            else
            {
                UpperLeft.SetCornerTo0(i, j, RndmMatrix);
                UpperRight.SetCornerTo0(i, j, RndmMatrix);
                LowerLeft.SetCornerTo0(i, j, RndmMatrix);
                LowerRight.SetCornerTo0(i, j, RndmMatrix);
            }
        }

        /// <summary>
        /// Returns true if there is an 1 element in given rectangle
        /// </summary>
        public bool CheckRectangle(int[] lineIndices, int[] columnIndices, int lineNum, int colNum)
        {
            int cdMax = columnIndices.Length - 2;
            int ldMax = lineIndices.Length - 2;

            //using CornerMatrices and LineMatrices
            if (lineNum == 0)
            {
                if (colNum == 0)
                {
                    if (UpperLeft.element[lineIndices[1] - 1][columnIndices[1] - 1])
                    {
                        return true;
                    }
                    else return false;
                }
                else if (colNum == PatternMatrix.columnNumber - 1)
                {
                    if (UpperRight.element[lineIndices[1] - 1][columnIndices[cdMax]])
                    {
                        return true;
                    }
                    else return false;
                }
                else if (UpperLeft.element[lineIndices[1] - 1][columnIndices[colNum + 1] - 1] && UpperRight.element[lineIndices[1] - 1][columnIndices[colNum]])
                {
                    for (int j = columnIndices[colNum]; j < columnIndices[colNum + 1]; j++)
                    {
                        if (Upper.element[lineIndices[1] - 1][j]) return true;
                    }
                    return false;
                }
                else return false;
            }
            if (lineNum == PatternMatrix.linesNumber - 1)
            {
                if (colNum == 0)
                {
                    if (LowerLeft.element[lineIndices[ldMax]][columnIndices[1] - 1])
                    {
                        return true;
                    }
                    else return false;
                }
                else if (colNum == PatternMatrix.columnNumber - 1)
                {
                    if (LowerRight.element[lineIndices[ldMax]][columnIndices[cdMax]])
                    {
                        return true;
                    }
                    else return false;
                }
                else if (LowerLeft.element[lineIndices[ldMax]][columnIndices[colNum + 1] - 1] && LowerRight.element[lineIndices[ldMax]][columnIndices[colNum]])
                {
                    for (int j = columnIndices[colNum]; j < columnIndices[colNum + 1]; j++)
                    {
                        if (Lower.element[lineIndices[ldMax]][j]) return true;
                    }
                    return false;
                }
                else return false;
            }
            if (colNum == 0)
            {
                if (UpperLeft.element[lineIndices[lineNum + 1] - 1][columnIndices[1] - 1] && LowerLeft.element[lineIndices[lineNum]][columnIndices[1] - 1])
                {
                    for (int i = lineIndices[lineNum]; i < lineIndices[lineNum + 1]; i++)
                    {
                        if (Left.element[i][columnIndices[1] - 1]) return true;
                    }
                    return false;
                }
                else return false;
            }
            if (colNum == PatternMatrix.columnNumber - 1)
            {
                if (UpperRight.element[lineIndices[lineNum + 1] - 1][columnIndices[cdMax]] && LowerRight.element[lineIndices[lineNum]][columnIndices[cdMax]])
                {
                    for (int i = lineIndices[lineNum]; i < lineIndices[lineNum + 1]; i++)
                    {
                        if (Right.element[i][columnIndices[cdMax]]) return true;
                    }
                    return false;
                }
                else return false;
            }

            //inner rectangle check
            if (lineIndices[lineNum + 1] - lineIndices[lineNum] < columnIndices[colNum + 1] - columnIndices[colNum]) //rectangle is wide
            {
                for (int i = lineIndices[lineNum]; i < lineIndices[lineNum + 1]; i++)
                {
                    if (Right.element[i][columnIndices[colNum]] && !Right.element[i][columnIndices[colNum + 1]]) return true;
                    if (!Left.element[i][columnIndices[colNum] - 1] && Left.element[i][columnIndices[colNum + 1] - 1]) return true;
                }
            }
            else
            {
                for (int j = columnIndices[colNum]; j < columnIndices[colNum + 1]; j++)
                {
                    if (Lower.element[lineIndices[lineNum]][j] && !Lower.element[lineIndices[lineNum + 1]][j]) return true;
                    if (!Upper.element[lineIndices[lineNum] - 1][j] && Upper.element[lineIndices[lineNum + 1] - 1][j]) return true;
                }
            }

            //searching element by element in case that previous check is not succesfull
            for (int i = lineIndices[lineNum]; i < lineIndices[lineNum + 1]; i++)
            {
                for (int j = columnIndices[colNum]; j < columnIndices[colNum + 1]; j++)
                {
                    if (RndmMatrix.element[i][j])
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }

    class Program
    {
        static Matrix01 RndmMatrix;
        static Matrix01 PatternMatrix;
        static CornerMatrix UpperLeft;
        static CornerMatrix UpperRight;
        static CornerMatrix LowerLeft;
        static CornerMatrix LowerRight;
        static LineMatrix Upper;
        static LineMatrix Lower;
        static LineMatrix Left;
        static LineMatrix Right;

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

        static void PrintMatrix(Matrix01 matrix)
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

                        if (distributor.FindPattern())
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

            RndmMatrix = new Matrix01(size);

            UpperLeft = new CornerMatrix(size, true, true);
            UpperRight = new CornerMatrix(size, false, true);
            LowerLeft = new CornerMatrix(size, true, false);
            LowerRight = new CornerMatrix(size, false, false);

            Upper = new LineMatrix(size, false, false, true, false);
            Lower = new LineMatrix(size, false, false, false, true);
            Left = new LineMatrix(size, true, false, false, false);
            Right = new LineMatrix(size, false, true, false, false);

            if (!loadingFromFile) Console.WriteLine("Enter sizes of pattern matrix.");

            if ((size = LoadMatrixSize(reader.ReadLine())) == null) return;
            else if (size.linesNumber > RndmMatrix.linesNumber || size.columnNumber > RndmMatrix.columnNumber)
            {
                ReportError("Pattern matrix can't be larger than generating matrix.");
                return;
            }

            PatternMatrix = new Matrix01(size);
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

            if (!loadingFromFile) Console.WriteLine("Enter number of random iterations.");
            if (!int.TryParse(reader.ReadLine(), out iterationsNumber) || iterationsNumber < 1)
            {
                ReportError("Wrong format of integer.");
                return;
            }
            Console.WriteLine("**Enter \"end\" to exit or any key to continue**");

            Stopwatch sw = new Stopwatch();

            Context context = new Context(RndmMatrix, PatternMatrix, UpperLeft, UpperRight, LowerLeft, LowerRight, Upper, Lower, Left, Right);
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