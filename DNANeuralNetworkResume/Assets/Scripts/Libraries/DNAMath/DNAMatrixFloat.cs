using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;
using System;

namespace DNAMath
{

    /// <summary>
    /// Custom Matrix Class developped for working on the GPU and with DNANeuralNetworks
    /// </summary>
    [System.Serializable]
    public class DNAMatrixFloat
    {
        // 0--------> Width
        // |
        // |
        // |
        // Height

        public struct GPUMatrix
        {
            public float[] Values;
            public int Height;
            public int Width;
        }

        /// <summary>
        /// Shader Script that runs Matrix Multiplication on the GPU
        /// </summary>
        public static ComputeShader matrixMultScript;

        /// <summary>
        /// Shader Script that runs Matrix Addition on the GPU
        /// </summary>
        public static ComputeShader matrixAdditionScript;

        /// <summary>
        /// Shader Script that runs Matrix Substraction on the GPU
        /// </summary>
        public static ComputeShader matrixSubstractionScript;

        /// <summary>
        /// Load the Shader Scripts associated for speeding up the mathematics
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        public static void loadAssets()
        {

        }

        /// <summary>
        /// Describes the number of rows the matrix has
        /// </summary>
        [SerializeField]
        private int _height;

        /// <summary>
        /// Describes the number of rows the matrix has
        /// </summary>
        public int Height { get { return _height; } set { _height = value; } }

        /// <summary>
        /// Describes the number of columns the matrix has
        /// </summary>
        [SerializeField]
        private int _width;

        /// <summary>
        /// Describes the number of columns the matrix has
        /// </summary>
        public int Width { get { return _width; } set { _width = value; } }

        /// <summary>
        /// Describes the number of values in the Matrix
        /// </summary>
        public int Length { get { return Width * Height; } }

        /// <summary>
        /// A list of all values contained in the matrix
        /// </summary>
        [SerializeField]
        private float[] _values;

        /// <summary>
        /// A list of all values contained in the matrix
        /// </summary>
        public float[] Values
        {
            get
            {
                return _values;
            }
            set
            {
                _values = value;
            }
        }

        public DNAMatrixFloat (DNAMatrix matrix)
        {
            this.Width = matrix.Width;
            this.Height = matrix.Height;

            _values = new float[Width * Height];

            for (int i = 0; i < matrix.Length; i ++)
            {
                _values[i] = (float)matrix[i];
            }
        }

        /// <summary>
        /// Constructor function initializing the Matrix
        /// </summary>
        /// <param name="height"></param>
        /// <param name="width"></param>
        public DNAMatrixFloat(int height, int width)
        {
            this.Width = width;
            this.Height = height;

            _values = new float[width * height];
        }

        /// <summary>
        /// Indexer allowing us to get access and sey  to a value using array notation
        /// </summary>
        /// <param name="height"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public float this[int height, int width]
        {
            get
            {
                return _values[GetFlatIndex(height, width)];
            }
            set
            {
                _values[GetFlatIndex(height, width)] = value;
            }
        }

        /// <summary>
        /// Indexer allowing us to access and set the value in array notation
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public float this[int index]
        {
            get
            {
                return _values[index];
            }
            set
            {
                _values[index] = value;
            }
        }

        /// <summary>
        /// Indexer allowing us to get a vector from the matrix
        /// </summary>
        /// <param name="index"></param>
        /// <param name="idk"></param>
        /// <returns></returns>
        public float[] this[int index, bool row]
        {
            get
            {
                float[] vector;
                if (row)
                {
                    //Get a row
                    vector = new float[Width];

                    for (int i = 0; i < Width; i++)
                    {
                        vector[i] = this[index, i];
                    }

                }
                else
                {
                    //Get a column
                    vector = new float[Height];

                    for (int i = 0; i < Height; i++)
                    {
                        vector[i] = this[i, index];
                    }
                }

                return vector;
            }
        }


        /// <summary>
        /// Initializes a matrix that counts from 1 - the number of values based on the dimensions
        /// </summary>
        /// <param name="height"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public static DNAMatrixFloat Increment(int height, int width)
        {
            DNAMatrixFloat matrix = new DNAMatrixFloat(height, width);

            for (int i = 0; i < width * height; i++)
            {
                matrix[i] = i + 1;
            }

            return matrix;
        }

        /// <summary>
        /// Sets the value at the given height and width index
        /// </summary>
        /// <param name="heightIndex"></param>
        /// <param name="widthIndex"></param>
        /// <param name="val"></param>
        public void SetValue(int heightIndex, int widthIndex, float val)
        {
            this._values[GetFlatIndex(heightIndex, widthIndex)] = val;
        }

        /// <summary>
        /// Add to the values at the given height and width index
        /// </summary>
        /// <param name="heightIndex"></param>
        /// <param name="widthIndex"></param>
        /// <param name="val"></param>
        public void AddValue(int heightIndex, int widthIndex, float val)
        {
            this._values[GetFlatIndex(heightIndex, widthIndex)] += val;
        }

        /// <summary>
        /// Gets the value at the given height and width index
        /// </summary>
        /// <param name="heightIndex"></param>
        /// <param name="widthIndex"></param>
        /// <returns></returns>
        public float GetValue(int heightIndex, int widthIndex)
        {
            return _values[GetFlatIndex(heightIndex, widthIndex)];
        }

        /// <summary>
        /// Returns the flat index of a value 
        /// </summary>
        /// <param name="heightIndex"></param>
        /// <param name="widthIndex"></param>
        /// <returns></returns>
        public int GetFlatIndex(int heightIndex, int widthIndex)
        {
            return heightIndex * Width + widthIndex;
        }



        /// <summary>
        /// Returns calculated indeces for the matrix based on a length index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public (int height, int width) GetIndex(int index)
        {
            int height = index / Width;
            int width = index % Width;

            return (height, width);
        }

        /// <summary>
        /// Returns calculated indeces for the matrix based on a length index for static functions
        /// </summary>
        /// <param name="index"></param>
        /// <param name="mat"></param>
        /// <returns></returns>
        public static (int height, int width) GetIndex(int index, DNAMatrixFloat mat)
        {
            int height = index / mat.Width;
            int width = index % mat.Width;

            return (height, width);
        }

        /// <summary>
        /// Returns the Dot Product
        /// </summary>
        /// <param name="vector1"></param>
        /// <param name="vector2"></param>
        /// <returns></returns>
        float DotProduct(float[] vector1, float[] vector2)
        {
            float value = 0;

            if (vector1.Length == vector2.Length)
            {
                for (int i = 0; i < vector1.Length; i++)
                {
                    value += vector1[i] * vector2[i];
                }
            }

            return value;
        }

        /// <summary>
        /// Returns the Dot Product for Static Functions
        /// </summary>
        /// <param name="vector1"></param>
        /// <param name="vector2"></param>
        /// <returns></returns>
        static float DotProductStatic(float[] vector1, float[] vector2)
        {
            float value = 0;

            if (vector1.Length == vector2.Length)
            {
                for (int i = 0; i < vector1.Length; i++)
                {
                    value += vector1[i] * vector2[i];
                }
            }

            return value;
        }

        /// <summary>
        /// Transposes the matrix
        /// </summary>
        public DNAMatrixFloat Transpose()
        {

            DNAMatrixFloat newMat = new DNAMatrixFloat(this.Width, this.Height);

            for (int width = 0; width < this.Width; width++)
            {
                for (int height = 0; height < this.Height; height++)
                {
                    newMat[width, height] = this[height, width];
                }
            }

            return newMat;
        }

        /// <summary>
        /// Operator for adding two matrices together
        /// </summary>
        /// <param name="matrixA"></param>
        /// <param name="matrixB"></param>
        /// <returns></returns>
        public static DNAMatrixFloat operator +(DNAMatrixFloat matrixA, DNAMatrixFloat matrixB)
        {
            DNAMatrixFloat newMat = new DNAMatrixFloat(0, 0);

            // if (matrixAdditionScript != null)
            //    newMat = matrixAdditionGPU(matrixA, matrixB);
            //else
            // {
            if (matrixA.Height == matrixB.Height && matrixA.Width == matrixB.Width)
            {
                newMat = new DNAMatrixFloat(matrixA.Height, matrixA.Width);

                for (int i = 0; i < matrixA.Values.Length; i++)
                {
                    newMat[i] = matrixA[i] + matrixB[i];
                }
            }
            else
            {
                Debug.Log("Error, Dimensions don't match");
            }

            return newMat;
        }

        /// <summary>
        /// Operator handling subtractions between 2 matrices
        /// </summary>
        /// <param name="matrixA"></param>
        /// <param name="matrixB"></param>
        /// <returns></returns>
        public static DNAMatrixFloat operator -(DNAMatrixFloat matrixA, DNAMatrixFloat matrixB)
        {
            DNAMatrixFloat newMat = new DNAMatrixFloat(0, 0);

            if (matrixA.Height == matrixB.Height && matrixA.Width == matrixB.Width)
            {
                newMat = new DNAMatrixFloat(matrixA.Height, matrixA.Width);

                for (int i = 0; i < matrixA.Values.Length; i++)
                {
                    newMat[i] = matrixA[i] - matrixB[i];
                }
            }
            else
            {
                Debug.Log("Error, Dimensions don't match");
            }

            return newMat;
        }

        /// <summary>
        /// Multiplication operation, multiplying matrices together
        /// </summary>
        /// <param name="matrixA"></param>
        /// <param name="matrixB"></param>
        /// <returns></returns>
        public static DNAMatrixFloat operator *(DNAMatrixFloat matrixA, DNAMatrixFloat matrixB)
        {
            DNAMatrixFloat newMat = new DNAMatrixFloat(0, 0);

            if (matrixMultScript != null)
                newMat = multMatrixGPU(matrixA, matrixB);
            else
            {
                //Check if matrixA Width is equal to matrixB Height
                if (matrixA.Width == matrixB.Height)
                {
                    newMat = new DNAMatrixFloat(matrixA.Height, matrixB.Width);

                    System.Threading.Tasks.Parallel.For(0, newMat.Values.Length, (index) =>
                    {
                        (int height, int width) = GetIndex(index, newMat);

                        newMat[index] = DotProductStatic(matrixA[height, true], matrixB[width, false]);

                    });

                }
                else
                    Debug.Log("Error, Dimensions don't match");
            }

            return newMat;
        }

        /// <summary>
        /// Multiplication operation with a factor
        /// </summary>
        /// <param name="matrixA"></param>
        /// <param name="factor"></param>
        /// <returns></returns>
        public static DNAMatrixFloat operator *(DNAMatrixFloat matrixA, float factor)
        {
            DNAMatrixFloat newMat = new DNAMatrixFloat(0, 0);

            newMat = new DNAMatrixFloat(matrixA.Height, matrixA.Width);

            for (int i = 0; i < matrixA.Values.Length; i++)
            {
                newMat[i] = matrixA[i] * factor;
            }

            return newMat;
        }

        /// <summary>
        /// Handles a Matrix Multiplication by handing it off to the GPU, this makes it crazy fast
        /// </summary>
        /// <param name="matrixA"></param>
        /// <param name="matrixB"></param>
        /// <returns></returns>
        public static DNAMatrixFloat multMatrixGPU(DNAMatrixFloat matrixA, DNAMatrixFloat matrixB)
        {
            DNAMatrixFloat newMat = new DNAMatrixFloat(0, 0);
            if (matrixA.Width == matrixB.Height)
            {
                newMat = new DNAMatrixFloat(matrixA.Height, matrixB.Width);

                ComputeShader computeShader = matrixMultScript;

                // Create compute buffers
                ComputeBuffer matrixAVals = new ComputeBuffer(matrixA.Length, sizeof(float));
                ComputeBuffer matrixBVals = new ComputeBuffer(matrixB.Length, sizeof(float));
                ComputeBuffer newMatrixVals = new ComputeBuffer(newMat.Length, sizeof(float));

                ComputeBuffer newMatrixDim = new ComputeBuffer(1, sizeof(uint) * 2);
                ComputeBuffer matrixADim = new ComputeBuffer(1, sizeof(uint) * 2);
                ComputeBuffer matrixBDim = new ComputeBuffer(1, sizeof(uint) * 2);


                matrixAVals.SetData(matrixA.Values);
                matrixBVals.SetData(matrixB.Values);

                matrixADim.SetData(new uint[] { (uint)matrixA.Width, (uint)matrixA.Height });
                matrixBDim.SetData(new uint[] { (uint)matrixB.Width, (uint)matrixB.Height });
                newMatrixDim.SetData(new uint[] { (uint)newMat.Width, (uint)newMat.Height });


                computeShader.SetBuffer(0, "matrixA", matrixAVals);
                computeShader.SetBuffer(0, "matrixB", matrixBVals);
                computeShader.SetBuffer(0, "newMatrix", newMatrixVals);

                computeShader.SetBuffer(0, "matrixDimsA", matrixADim);
                computeShader.SetBuffer(0, "matrixDimsB", matrixBDim);
                computeShader.SetBuffer(0, "newMatrixDims", newMatrixDim);

                //Calculate
                computeShader.Dispatch(0, newMat.Width, newMat.Height, 1);

                //Receaive Result
                newMatrixVals.GetData(newMat.Values);

                //Get rid of memory
                matrixAVals.Dispose();
                matrixBVals.Dispose();
                newMatrixVals.Dispose();

                matrixADim.Dispose();
                matrixBDim.Dispose();
                newMatrixDim.Dispose();
            }
            else
                Debug.Log("Error, Dimensions don't match");

            return newMat;
        }

        /// <summary>
        /// Handles Matrix Additions through the GPU
        /// </summary>
        /// <param name="matrixA"></param>
        /// <param name="matrixB"></param>
        /// <returns></returns>
        public static DNAMatrixFloat matrixAdditionGPU(DNAMatrixFloat matrixA, DNAMatrixFloat matrixB)
        {
            DNAMatrixFloat newMat = new DNAMatrixFloat(0, 0);

            if (_SameDimension(matrixA, matrixB))
            {
                ComputeShader computeShader = matrixAdditionScript;
                newMat = new DNAMatrixFloat(matrixA.Height, matrixB.Width);

                /*
                int sizeOfDimensions = sizeof(int) * 2;
                int sizeOffloat = sizeof(float);

                int sizeOfMatrixAValues = matrixA.Length * sizeOffloat;
                int sizeOfMatrixBValues = matrixB.Length * sizeOffloat;
                int sizeOfMatrixOutputValues = newMat.Length * sizeOffloat;

                int totalSizeA = sizeOfDimensions + sizeOfMatrixAValues;
                int totalSizeB = sizeOfDimensions + sizeOfMatrixBValues;
                int totalSizeOutput = sizeOfDimensions + sizeOfMatrixOutputValues;

                GPUMatrix matrixAGPU = new GPUMatrix { Values = matrixA.Values, Height = matrixA.Height, Width = matrixA.Width};
                GPUMatrix matrixBGPU = new GPUMatrix { Values = matrixB.Values, Height = matrixB.Height, Width = matrixB.Width};
                GPUMatrix matrixOutputGPU = new GPUMatrix { Values = newMat.Values, Height = newMat.Height, Width = newMat.Width };
                */



                ComputeBuffer matrixAValues = new ComputeBuffer(matrixA.Values.Length, sizeof(float));
                matrixAValues.SetData(matrixA.Values);

                ComputeBuffer matrixBValues = new ComputeBuffer(matrixB.Values.Length, sizeof(float));
                matrixBValues.SetData(matrixB.Values);

                ComputeBuffer newMatrixValues = new ComputeBuffer(newMat.Values.Length, sizeof(float));

                computeShader.SetBuffer(0, "matrixAValues", matrixAValues);
                computeShader.SetBuffer(0, "matrixBValues", matrixBValues);
                computeShader.SetBuffer(0, "newMatrixValues", newMatrixValues);

                //Run Calculations
                computeShader.Dispatch(0, (newMat.Values.Length / 1024) + 1, 1, 1);

                //Receive Data
                newMatrixValues.GetData(newMat.Values);

                //Get rid of memory
                matrixAValues.Dispose();
                matrixBValues.Dispose();
                newMatrixValues.Dispose();

            }
            else
                Debug.Log("Error, Dimensions don't match");

            return newMat;
        }

        public static DNAMatrixFloat matrixSubstractionGPU(DNAMatrixFloat matrixA, DNAMatrixFloat matrixB)
        {
            DNAMatrixFloat newMat = new DNAMatrixFloat(0, 0);

            if (_SameDimension(matrixA, matrixB))
            {
                ComputeShader computeShader = matrixSubstractionScript;
                newMat = new DNAMatrixFloat(matrixA.Height, matrixB.Width);

                ComputeBuffer matrixAValues = new ComputeBuffer(matrixA.Values.Length, sizeof(float));
                matrixAValues.SetData(matrixA.Values);

                ComputeBuffer matrixBValues = new ComputeBuffer(matrixB.Values.Length, sizeof(float));
                matrixBValues.SetData(matrixB.Values);

                ComputeBuffer newMatrixValues = new ComputeBuffer(newMat.Values.Length, sizeof(float));
                newMatrixValues.SetData(newMat.Values);

                computeShader.SetBuffer(0, "matrixAValues", matrixAValues);
                computeShader.SetBuffer(0, "matrixBValues", matrixBValues);
                computeShader.SetBuffer(0, "newMatrixValues", newMatrixValues);

                int groupCount = 0;
                if ((newMat.Values.Length / 1024) >= 1)
                    groupCount = (newMat.Values.Length / 1024);
                else
                    groupCount = 1;

                //Run Calculations
                computeShader.Dispatch(0, groupCount, 1, 1);

                //Receive Data
                newMatrixValues.GetData(newMat.Values);

                //Get rid of memory
                matrixAValues.Dispose();
                matrixBValues.Dispose();
                newMatrixValues.Dispose();

            }
            else
                Debug.Log("Error, Dimensions don't match");

            return newMat;
        }

        /// <summary>
        /// Checks if the Matrices are the same Dimension
        /// </summary>
        /// <param name="matrixA"></param>
        /// <param name="matrixB"></param>
        /// <returns></returns>
        private static bool _SameDimension(DNAMatrixFloat matrixA, DNAMatrixFloat matrixB)
        {
            if (matrixA.Height == matrixB.Height && matrixA.Width == matrixB.Width)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Displays the Matrix in the correct fashion, for debugging purposes
        /// </summary>
        public void DisplayMat()
        {
            //Display the matrix
            string line = "\n";
            for (int height = 0; height < this.Height; height++)
            {
                for (int width = 0; width < this.Width; width++)
                {

                    //Debug.Log("Width: " + width + " Height: " + height + " = " + newMat[getIndex(width, height, dim.x)]);

                    line += this[height, width] + "    ";
                }
                line += "\n";

            }

            Debug.Log(line);
        }
    }
}


