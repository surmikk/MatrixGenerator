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
        public int LinesNumber
        {
            get { return Element.Length; }
        }
        public int ColumnNumber
        {
            get { return Element[0].Length; }
        }
        public bool[][] Element { get; private set; }
        public int NumberOf1Elements { get; private set; }
        public Matrix01(MatrixSize size)
        {
            Element = new bool[size.linesNumber][];
            for (int i = 0; i < size.linesNumber; i++)
            {
                Element[i] = new bool[size.columnNumber];
            }
        }
        public Matrix01(Matrix01 originalMatrix)
        {
            Element = new bool[originalMatrix.LinesNumber][];
            for (int i = 0; i < LinesNumber; i++)
            {
                Element[i] = new bool[originalMatrix.ColumnNumber];
                for (int j = 0; j < ColumnNumber; j++)
                {
                    Element[i][j] = originalMatrix.Element[i][j];
                }
            }
            NumberOf1Elements = originalMatrix.NumberOf1Elements;
        }
        /// <summary>
        /// Prints the matrix or its transposition.
        /// </summary>
        public void Print(bool transpositionNecessary)
        {
            if (transpositionNecessary)
            {
                for (int j = 0; j < ColumnNumber; j++)
                {
                    for (int i = 0; i < LinesNumber; i++)
                    {
                        if (Element[i][j] == true)
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
                for (int i = 0; i < LinesNumber; i++)
                {
                    for (int j = 0; j < ColumnNumber; j++)
                    {
                        if (Element[i][j] == true)
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
        public void Transpose()
        {
            bool[][] resultElement = new bool[ColumnNumber][];
            for (int i = 0; i < ColumnNumber; i++)
            {
                resultElement[i] = new bool[LinesNumber];
            }
            for (int i = 0; i < LinesNumber; i++)
            {
                for (int j = 0; j < ColumnNumber; j++)
                {
                    resultElement[j][i] = Element[i][j];
                }
            }
            Element = resultElement;
        }
        /// <summary>
        /// Returns false if the loading failed
        /// </summary>
        public bool LoadPattern(TextReader reader)
        {
            bool failed = false;
            for (int i = 0; i < LinesNumber; i++)
            {
                if (failed) return false;
                string inputLine = reader.ReadLine();
                if (inputLine.Length < ColumnNumber)
                {
                    throw new FormatException();
                }
                for (int j = 0; j < ColumnNumber; j++)
                {
                    if (inputLine[j] == '1')
                    {
                        Element[i][j] = true;
                    }
                    else if (inputLine[j] != '0')
                    {
                        failed = true;
                    }
                }
            }
            if (failed)
            {
                return false;
            }
            else return true;
        }
        /// <summary>
        /// Changes element on the given position. Returns true if an 1 element was created.
        /// </summary>
        public bool SwapElement(int lineToChange, int columnToChange)
        {
            if (Element[lineToChange][columnToChange] == true)
            {
                Element[lineToChange][columnToChange] = false;
                NumberOf1Elements--;
                return false;
            }
            else
            {
                Element[lineToChange][columnToChange] = true;
                NumberOf1Elements++;
                return true;
            }
        }
    }

    class LinesDistributor
    {
        /// <summary>
        /// an array that contains bounds of the current matrix side distribution
        /// </summary>
        public readonly int[] indices;
        public int[] lowerBorder;
        public int[] upperBorder;
        readonly int rndmMatrixSideSize;
        public LinesDistributor(int matrixSize, int numberOfSectionSepararators, int patternLine, int swappedLine)
        {
            indices = new int[numberOfSectionSepararators + 2];
            lowerBorder = new int[numberOfSectionSepararators + 2];
            upperBorder = new int[numberOfSectionSepararators + 2];
            rndmMatrixSideSize = matrixSize;

            //initializing necessary values so that the border setting work properly
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
        /// <summary>
        /// Initializes indices with lower border values
        /// </summary>
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
        /// Tries to make another distribution. Returns false if another distribution doesn't exist.
        /// </summary>
        public bool NextPosition()
        {
            // increases the last value of indices
            // if it overflows its upper border, the last but one value is increased and the last one is decreased to its lower border. And so on.
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

    class Distributor
    {
        Context context;
        LinesDistributor linesDistributor;
        LinesDistributor columnDistributor;

        // remembers values from column distributor's constructor, because these values are used and changed during the search
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
            if (DistributionPossible(swapedLine, patternLine, context.RndmMatrix.LinesNumber, context.PatternMatrix.LinesNumber))
            {
                linesDistributor = new LinesDistributor(context.RndmMatrix.LinesNumber, context.PatternMatrix.LinesNumber - 1, patternLine, swapedLine);
                linesDistributorLoaded = true;
            }
            else return;

            //column distributor constructor
            if (DistributionPossible(swapedColumn, patternColumn, context.RndmMatrix.ColumnNumber, context.PatternMatrix.ColumnNumber))
            {
                columnDistributor = new LinesDistributor(context.RndmMatrix.ColumnNumber, context.PatternMatrix.ColumnNumber - 1, patternColumn, swapedColumn);
                originalColDistLowerBorder = new int[columnDistributor.indices.Length];
                Array.Copy(columnDistributor.indices, originalColDistLowerBorder, columnDistributor.indices.Length);
                originalColDistUpperBorder = new int[columnDistributor.upperBorder.Length];
                Array.Copy(columnDistributor.upperBorder, originalColDistUpperBorder, columnDistributor.upperBorder.Length);
                columnDistributorLoaded = true;
            }
            else return;
        }

        /// <summary>
        /// For every possible line distribution tries to find forbidden pattern. Returns true if the pattern is found.
        /// </summary>
        public bool TryFindPattern()
        {
            // advanced method for setting its upper and lower borders
            if (SetBordersToLineSeparators())
            {
                // checking all possible positions of line separators 
                do
                {
                    if (MoveWithColumnSeparators())
                    {
                        // for the current line distribution can be column separators moved to positions so that the divided matrix contains forbiden pattern
                        return true;
                    }
                }
                while (linesDistributor.NextPosition());
            }
            // any distribution doesn't contain the forbidden pattern
            return false;
        }

        /// <summary>
        /// Returns true if there exists a valid position of line separators (otherwise forbiden pattern doesn't exist)
        /// </summary>
        bool SetBordersToLineSeparators()
        {
            // auxiliary indices that contains lower and upper borders of each position of column distributor
            // lb(i) ... lowerBorder of the i-th column (counted from 0)
            // ub(i) ... upperBorder of the i-th column - 1

            // lb(0), ub(1), lb(2), ub(3),...
            int[] evenColumnIndices = new int[columnDistributor.indices.Length];
            // 0    , lb(1), ub(2), lb(3), ub(4),...
            int[] oddColumnIndices = new int[columnDistributor.indices.Length];
            // adding necessary auxiliary value to a unused position of colDistr.upperBorder
            columnDistributor.upperBorder[columnDistributor.upperBorder.Length - 1] = columnDistributor.indices[columnDistributor.indices.Length - 1] + 1;
            for (int i = 0; i < columnDistributor.indices.Length - 1; i++)
            {
                if (i % 2 == 0)
                {
                    evenColumnIndices[i] = columnDistributor.lowerBorder[i];
                    evenColumnIndices[i + 1] = columnDistributor.upperBorder[i + 1] - 1;
                }
                else
                {
                    oddColumnIndices[i] = columnDistributor.lowerBorder[i];
                    oddColumnIndices[i + 1] = columnDistributor.upperBorder[i + 1] - 1;
                }
            }
            // setting lower borders
            {
                int[] auxiliaryLineIndices = new int[linesDistributor.lowerBorder.Length];
                Array.Copy(linesDistributor.lowerBorder, auxiliaryLineIndices, linesDistributor.lowerBorder.Length);

                bool contains1;
                // moving from the top to the bottom
                for (int i = 0; i < context.PatternMatrix.LinesNumber; i++)
                {
                    auxiliaryLineIndices[i + 1] = Maximum(linesDistributor.lowerBorder[i + 1], auxiliaryLineIndices[i] + 1);
                    for (int j = 0; j < context.PatternMatrix.ColumnNumber; j++)
                    {
                        // if there must be at least one element in the rectangle
                        if (context.PatternMatrix.Element[i][j])
                        {
                            //moving separator down if neither the biggest possible rectangle doesn't contain any 1 element
                            while (auxiliaryLineIndices[i + 1] < linesDistributor.upperBorder[i + 1])
                            {
                                if (j % 2 == 0)
                                {
                                    contains1 = context.CheckRectangle(auxiliaryLineIndices, evenColumnIndices, i, j);
                                }
                                else
                                {
                                    contains1 = context.CheckRectangle(auxiliaryLineIndices, oddColumnIndices, i, j);
                                }

                                if (contains1)
                                {
                                    break;
                                }
                                else
                                {
                                    auxiliaryLineIndices[i + 1]++;
                                }
                            }
                        }
                    }
                    if (auxiliaryLineIndices[i + 1] == linesDistributor.upperBorder[i + 1])
                    {
                        // the (i+1)-th separator was moved to the last possible possition but still the (i+1)-th column doesn't contain any 1
                        return false;
                    }
                }
                // separators can be moved to valid positions
                linesDistributor.lowerBorder = auxiliaryLineIndices;
            }

            // setting upper borders
            // very similar to setting lower border, the difference is caused by the difference between values saved in lower- and upper-borders (+-1)
            {
                int[] auxiliaryLineIndices = new int[linesDistributor.upperBorder.Length];
                for (int i = 1; i < auxiliaryLineIndices.Length - 1; i++)
                {
                    auxiliaryLineIndices[i] = linesDistributor.upperBorder[i] - 1;
                }
                auxiliaryLineIndices[auxiliaryLineIndices.Length - 1] = linesDistributor.indices[linesDistributor.indices.Length - 1];

                bool contains1;
                //moving separators,from the first to the last, to the right as much as necessary
                for (int i = context.PatternMatrix.LinesNumber - 1; i >= 0; i--)
                {
                    if (i != context.PatternMatrix.LinesNumber - 1)
                    {
                        auxiliaryLineIndices[i] = Minimum(linesDistributor.upperBorder[i] - 1, auxiliaryLineIndices[i + 1] - 1);
                    }
                    for (int j = 0; j < context.PatternMatrix.ColumnNumber; j++)
                    {
                        if (context.PatternMatrix.Element[i][j])
                        {
                            while (auxiliaryLineIndices[i] >= linesDistributor.lowerBorder[i])
                            {
                                if (j % 2 == 0)
                                {
                                    contains1 = context.CheckRectangle(auxiliaryLineIndices, evenColumnIndices, i, j);
                                }
                                else
                                {
                                    contains1 = context.CheckRectangle(auxiliaryLineIndices, oddColumnIndices, i, j);
                                }

                                if (contains1)
                                {
                                    break;
                                }
                                else
                                {
                                    auxiliaryLineIndices[i]--;
                                }
                            }
                        }
                    }
                    if (auxiliaryLineIndices[i + 1] < linesDistributor.lowerBorder[i + 1])
                    {
                        // the separator was moved too far and still the biggest possible rectangle doesn't contain any 1
                        return false;
                    }
                }
                // separators can be moved to valid position
                for (int i = 1; i < auxiliaryLineIndices.Length - 1; i++)
                {
                    linesDistributor.upperBorder[i] = auxiliaryLineIndices[i] + 1;
                }
                linesDistributor.InitializeIndices();
                return true;
            }
        }

        /// <summary>
        /// For the given lines distribution moves with column separators. Returns true if there exists forbidden pattern
        /// </summary>
        /// <returns></returns>
        bool MoveWithColumnSeparators()
        {
            Array.Copy(originalColDistUpperBorder, columnDistributor.upperBorder, originalColDistUpperBorder.Length);
            int[] auxiliaryColumnIndices = new int[originalColDistLowerBorder.Length];
            Array.Copy(originalColDistLowerBorder, auxiliaryColumnIndices, originalColDistLowerBorder.Length);

            //sets last separator most right
            auxiliaryColumnIndices[auxiliaryColumnIndices.Length - 2] = originalColDistUpperBorder[originalColDistUpperBorder.Length - 2] - 1;

            //last separator initialization (moving to the left as much as necessary)
            for (int i = 0; i < context.PatternMatrix.LinesNumber; i++)
            {
                if (context.PatternMatrix.Element[i][context.PatternMatrix.ColumnNumber - 1])
                {
                    while (auxiliaryColumnIndices[auxiliaryColumnIndices.Length - 2] >= originalColDistLowerBorder[originalColDistLowerBorder.Length - 2])
                    {
                        if (context.CheckRectangle(linesDistributor.indices, auxiliaryColumnIndices, i, context.PatternMatrix.ColumnNumber - 1))
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
            for (int j = 0; j < context.PatternMatrix.ColumnNumber - 1; j++)
            {
                auxiliaryColumnIndices[j + 1] = Maximum(originalColDistLowerBorder[j + 1], auxiliaryColumnIndices[j]);
                for (int i = 0; i < context.PatternMatrix.LinesNumber; i++)
                {
                    if (context.PatternMatrix.Element[i][j])
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

        /// <summary>
        /// Checks whether the selected element is not too close to the edge of the generated matrix
        /// </summary>
        bool DistributionPossible(int randomMatrixSwapedLine, int patternLine, int rndmSize, int patternSize)
        {
            if (randomMatrixSwapedLine + patternSize - patternLine <= rndmSize &&
                randomMatrixSwapedLine - patternLine >= 0)
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
        public readonly Matrix01 RndmMatrix;
        public readonly Matrix01 PatternMatrix;
        /// <summary>
        /// position[i][j] contains the sum of all 1 in the left-upper direction from this position
        /// </summary>
        public readonly int[][] CountingMatrix;
        // every new Context is created from the previous. Following variables provide information about the change.
        public bool Element1Created
        {
            get { return RndmMatrix.Element[ChangedLine][ChangedColumn]; }
        }
        public int ChangedLine { get; private set; }
        public int ChangedColumn { get; private set; }

        public Context(Matrix01 RandomMatrix, Matrix01 PatternMatrix, int[][] CountingMatrix)
        {
            RndmMatrix = new Matrix01(RandomMatrix);
            this.PatternMatrix = PatternMatrix;
            this.CountingMatrix = new int[CountingMatrix.Length][];
            for (int i = 0; i < CountingMatrix.Length; i++)
            {
                this.CountingMatrix[i] = new int[CountingMatrix[i].Length];
                for (int j = 0; j < CountingMatrix[i].Length; j++)
                {
                    this.CountingMatrix[i][j] = CountingMatrix[i][j];
                }
            }
        }
        public Context(MatrixSize rndmMatrixSize, Matrix01 PatternMatrix)
        {
            RndmMatrix = new Matrix01(rndmMatrixSize);
            this.PatternMatrix = PatternMatrix;
            CountingMatrix = new int[rndmMatrixSize.linesNumber][];
            for (int i = 0; i < rndmMatrixSize.linesNumber; i++)
            {
                CountingMatrix[i] = new int[rndmMatrixSize.columnNumber];
            }
        }

        /// <summary>
        /// Returns a new context with one change on a random position. Its all fields are correctly initialized.
        /// </summary>
        public Context GetNext(int lineToChange, int columnToChange)
        {
            Context newContext = new Context(RndmMatrix, PatternMatrix, CountingMatrix)
            {
                ChangedLine = lineToChange,
                ChangedColumn = columnToChange
            };
            newContext.RndmMatrix.SwapElement(lineToChange, columnToChange);
            newContext.RepairCountingMatrix(newContext.ChangedLine, newContext.ChangedColumn, newContext.Element1Created);
            return newContext;
        }

        /// <summary>
        /// Depending on element 1 or 0 creation, increases or decreases values in given rectangle
        /// </summary>
        void RepairCountingMatrix(int x, int y, bool element1Created)
        {
            for (int i = x; i < CountingMatrix.Length; i++)
            {
                for (int j = y; j < CountingMatrix[0].Length; j++)
                {
                    if (element1Created)
                    {
                        CountingMatrix[i][j]++;
                    }
                    else CountingMatrix[i][j]--;
                }
            }
        }

        /// <summary>
        /// Checks if there is an 1 element in given rectangle
        /// </summary>
        /// <param name="lineIndices">Indices of the current matrix lines distribution</param>
        /// <param name="columnIndices">Indices of the current matrix column distribution</param>
        /// <param name="lineNum">Index of the required pattern line to check</param>
        /// <param name="colNum">Index of the required pattern column to check</param>
        /// <returns>Returns true if there is an 1 element</returns>
        public bool CheckRectangle(int[] lineIndices, int[] columnIndices, int lineNum, int colNum)
        {
            // adds or subtracts sums of all 1 elements in at most 4 left-upper corner rectangles to get a sum of all 1 in any rectangle in the matrix
            int numberOf1InRectangle = 0;
            if (lineNum == 0)
            {
                if (colNum == 0)
                {
                    //left upper rectangle
                    numberOf1InRectangle = CountingMatrix[lineIndices[1] - 1][columnIndices[1] - 1];
                }
                else
                {
                    //upper rectangles
                    numberOf1InRectangle = CountingMatrix[lineIndices[1] - 1][columnIndices[colNum + 1] - 1]
                                         - CountingMatrix[lineIndices[1] - 1][columnIndices[colNum] - 1];
                }
            }
            else if (colNum == 0)
            {
                //left rectangles
                numberOf1InRectangle = CountingMatrix[lineIndices[lineNum + 1] - 1][columnIndices[1] - 1]
                                     - CountingMatrix[lineIndices[lineNum] - 1][columnIndices[1] - 1];
            }
            else
            {
                //other possibilities
                numberOf1InRectangle = CountingMatrix[lineIndices[lineNum + 1] - 1][columnIndices[colNum + 1] - 1]
                                     - CountingMatrix[lineIndices[lineNum + 1] - 1][columnIndices[colNum] - 1]
                                     - CountingMatrix[lineIndices[lineNum] - 1][columnIndices[colNum + 1] - 1]
                                     + CountingMatrix[lineIndices[lineNum] - 1][columnIndices[colNum] - 1];
            }

            if (numberOf1InRectangle > 0)
            {
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Returns true if the new matrix doesn't contain the forbidden pattern.
        /// </summary>
        public bool TestMatrix()
        {
            if (!Element1Created || RndmMatrix.NumberOf1Elements < PatternMatrix.NumberOf1Elements)
            {
                // if we remove an 1 element from a matrix that doesn't contain the pattern, we cannot create the pattern
                // the matrix containing less 1 elements than the pattern matrix itself cannot contain the pattern
                return true;
            }

            for (int i = 0; i < PatternMatrix.LinesNumber; i++)
            {
                for (int j = 0; j < PatternMatrix.ColumnNumber; j++)
                {
                    if (PatternMatrix.Element[i][j])
                    {
                        //tests all distributions where the new 1 element (and it's rectangular neighborhood) is contracted to element [i][j] of PatternMatrix
                        Distributor distributor = new Distributor(this, i, j, ChangedLine, ChangedColumn);
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
                            return false;
                        }
                    }
                }
            }
            // forbidden pattern wasn't found in any possible distribution of the matrix
            return true;
        }
    }

    /// <summary>
    /// Contains all necessary parameters, method and return value for tasks that search for another satisfying matrix.
    /// </summary>
    class Job
    {
        public Context originalContext;
        public Context newContext;
        public RandomGenerator generator;
        public Point ChangedPoint { get; private set; }

        /// <summary>
        /// True if the corresponding task has found another matrix not containing pattern.
        /// </summary>
        public bool Result { get; private set; }
        /// <summary>
        /// Loop condition, can be set from the main thread to politely abort task in progress.
        /// </summary>
        public bool solutionFound = false;

        public Job(Context context, RandomGenerator generator)
        {
            originalContext = context;
            this.generator = generator;
        }

        /// <summary>
        /// Tries to find another satisfying matrix in cycle until this or another task successfully ends.
        /// </summary>
        public void Run()
        {
            do
            {
                ChangedPoint = generator.Next();
                newContext = originalContext.GetNext(ChangedPoint.line, ChangedPoint.column);
                Result = newContext.TestMatrix();
                if (Result)
                {
                    solutionFound = true;
                }
            } while (!solutionFound);
        }
    }

    class Program
    {
        static MatrixSize LoadMatrixSize(string inputLine)
        {
            string[] tokens = inputLine.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            int linesNumber;
            int columnNumber;
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

        static void ReportError(string message)
        {
            Console.WriteLine(message + " Press any key to exit.");
            Console.ReadLine();
        }

        /// <summary>
        /// It is faster to work with transposed matrix, so in the end it must be transposed again
        /// </summary>
        static bool transpositionNecessary = false;

        /// <summary>
        /// Starts N tasks, waits until at least one task is successful, then returns index of a successful task with the lowest point ID
        /// </summary>
        /// <param name="jobs">Contains control and return variables for each thread</param>
        static int ProcessNTasks(int numberOfThreads, Job[] jobs)
        {
            Task[] tasks = new Task[numberOfThreads];
            for (int i = 0; i < numberOfThreads; i++)
            {
                tasks[i] = new Task(jobs[i].Run);
                tasks[i].Start();
            }
            // this thread waits for the first finished created thread
            int firstEndedIndex = Task.WaitAny(tasks);
            int indexOfNextContext = firstEndedIndex;

            for (int i = 0; i < numberOfThreads; i++)
            {
                // kind ending of all threads
                jobs[i].solutionFound = true;
            }

            // waits for the termination of all threads
            Task.WaitAll(tasks);

            // selecting the first successful thread according to the changed point ID
            for (int i = 0; i < numberOfThreads; i++)
            {
                if (i != firstEndedIndex && jobs[i].Result)
                {
                    if (jobs[i].ChangedPoint.ID < jobs[indexOfNextContext].ChangedPoint.ID)
                    {
                        indexOfNextContext = i;
                    }
                }
            }
            return indexOfNextContext;
        }

        static Context currentContext;
        static RandomGenerator randomGenerator;
        static int numberOfCycles = 1;
        static int totalIterations;
        static int numberOfThreads = 1;
        /// <summary>
        /// Process given number of iterations using multithreading
        /// </summary>
        static void GenerateMultiThreading(int numberOfIterations)
        {
            Job[] jobs;
            int counter = 0;
            int initialID;
            while (counter < numberOfIterations)
            {
                initialID = randomGenerator.IterationsCounter;
                jobs = new Job[numberOfThreads];
                for (int i = 0; i < numberOfThreads; i++)
                {
                    jobs[i] = new Job(currentContext, randomGenerator);
                }

                int indexOfNextContext = ProcessNTasks(numberOfThreads, jobs);

                // increasing counter and changing to the new context
                int difference = jobs[indexOfNextContext].ChangedPoint.ID - initialID;
                counter += difference;
                currentContext = jobs[indexOfNextContext].newContext;

                totalIterations += difference;
                numberOfCycles++;

                // increasing the number of threads in case the average number of empty iterations is greater
                if (numberOfThreads != Environment.ProcessorCount && totalIterations / numberOfCycles > numberOfThreads)
                {
                    numberOfThreads++;
                }
            }
        }

        static void Generate(int maxNumberOfIterations)
        {
            Stopwatch sw = new Stopwatch();
            string entry;
            sw.Start();
            GenerateMultiThreading(maxNumberOfIterations / 5);
            sw.Stop();
            if (sw.Elapsed.CompareTo(new TimeSpan(0, 0, 2)) > 0)
            {
                TimeSpan estimatedDuration = TimeSpan.FromTicks(sw.Elapsed.Ticks * 5);
                Console.WriteLine("Estimated duration: {0}", estimatedDuration);
            }
            sw.Start();

            GenerateMultiThreading(maxNumberOfIterations * 4 / 5);
            sw.Stop();
            Console.WriteLine(sw.Elapsed);
            currentContext.RndmMatrix.Print(transpositionNecessary);

            // working cycle
            while ((entry = Console.ReadLine()) != "end")
            {
                sw.Restart();

                GenerateMultiThreading(maxNumberOfIterations);

                sw.Stop();
                Console.WriteLine(sw.Elapsed);

                currentContext.RndmMatrix.Print(transpositionNecessary);
            }
        }

        static void Main(string[] args)
        {
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
            MatrixSize patternSize;
            MatrixSize rndmMatrixSize;
            if ((rndmMatrixSize = LoadMatrixSize(input)) == null) return;

            if (!loadingFromFile) Console.WriteLine("Enter sizes of pattern matrix.");

            if ((patternSize = LoadMatrixSize(reader.ReadLine())) == null) return;
            else if (patternSize.linesNumber > rndmMatrixSize.linesNumber || patternSize.columnNumber > rndmMatrixSize.columnNumber)
            {
                ReportError("Pattern matrix can't be larger than generating matrix.");
                return;
            }

            Matrix01 patternMatrix = new Matrix01(patternSize);
            if (!loadingFromFile) Console.WriteLine("Enter pattern 01-matrix of given sizes.");
            try
            {
                if (!patternMatrix.LoadPattern(reader))
                {
                    ReportError("The matrix must contain only 0 or 1.");
                    return;
                }
            }
            catch (FormatException)
            {
                ReportError("The row is too short.");
                return;
            }

            // transpose matrix in case the pattern has more rows than columns
            if (patternMatrix.LinesNumber > patternMatrix.ColumnNumber)
            {
                MatrixSize transposedSize = new MatrixSize(rndmMatrixSize.columnNumber, rndmMatrixSize.linesNumber);
                patternMatrix.Transpose();
                transpositionNecessary = true;
                rndmMatrixSize = transposedSize;
            }
            int numberOfIterations;
            if (!loadingFromFile) Console.WriteLine("Enter number of random iterations.");
            if (!int.TryParse(reader.ReadLine(), out numberOfIterations) || numberOfIterations < 1)
            {
                ReportError("Wrong format of integer.");
                return;
            }
            Console.WriteLine("**Enter \"end\" to exit or any key to continue**");

            currentContext = new Context(rndmMatrixSize, patternMatrix);
            randomGenerator = new RandomGenerator(rndmMatrixSize);
            Generate(numberOfIterations);
        }
    }

    class RandomGenerator
    {
        Random rndm = new Random();
        readonly int totalLines;
        readonly int totalColumns;
        // used to number generated points
        public int IterationsCounter { get; private set; }
        readonly object objectToLock = new object();
        public RandomGenerator(MatrixSize size)
        {
            totalLines = size.linesNumber;
            totalColumns = size.columnNumber;
        }
        /// <summary>
        /// returns random coordinates in the matrix
        /// </summary>
        public Point Next()
        {
            Point point;
            lock (objectToLock)
            {
                IterationsCounter++;
                point.line = rndm.Next(totalLines);
                point.column = rndm.Next(totalColumns);
                point.ID = IterationsCounter;
            }
            return point;
        }
    }

    struct Point
    {
        public int line;
        public int column;
        public int ID;
        public Point(int line, int column, int ID)
        {
            this.line = line;
            this.column = column;
            this.ID = ID;
        }
    }
}