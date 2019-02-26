using System.Collections;
using System.Text;
using Pliant.Utilities;

namespace Pliant.Collections
{
    public class BitMatrix
    {
        public BitMatrix(int count)
        {
            this._matrix = new BitArray[count];
            for (var i = 0; i < count; i++)
            {
                this._matrix[i] = new BitArray(count);
            }
        }

        public int Length => this._matrix.Length;

        public void Clear()
        {
            for (var i = 0; i < this._matrix.Length; i++)
            {
                this._matrix[i].SetAll(false);
            }
        }

        public BitMatrix Clone()
        {
            var bitMatrix = new BitMatrix(this._matrix.Length);
            for (var i = 0; i < this._matrix.Length; i++)
            {
                bitMatrix._matrix[i] = new BitArray(this._matrix[i]);
            }

            return bitMatrix;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var bitMatrix = obj as BitMatrix;
            if (bitMatrix == null)
            {
                return false;
            }

            if (bitMatrix.Length != Length)
            {
                return false;
            }

            for (var i = 0; i < bitMatrix.Length; i++)
            {
                if (!bitMatrix[i].Equals(this[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            for (var i = 0; i < Length; i++)
            {
                hashCode = HashCode.ComputeIncrementalHash(this[i].GetHashCode(), hashCode, i == 0);
            }

            return hashCode;
        }

        public override string ToString()
        {
            const string space = " ";
            const string zero = "0";
            const string one = "1";
            var stringBuilder = new StringBuilder();
            for (var i = 0; i < Length; i++)
            {
                if (i > 0)
                {
                    stringBuilder.AppendLine();
                }

                for (var j = 0; j < Length; j++)
                {
                    if (j > 0)
                    {
                        stringBuilder.Append(space);
                    }

                    stringBuilder.Append(this[i][j] ? one : zero);
                }
            }

            return stringBuilder.ToString();
        }

        public BitMatrix TransitiveClosure()
        {
            var transitiveClosure = Clone();
            for (var k = 0; k < transitiveClosure.Length; k++)
            {
                for (var i = 0; i < transitiveClosure.Length; i++)
                {
                    for (var j = 0; j < transitiveClosure.Length; j++)
                    {
                        transitiveClosure[i][j] = transitiveClosure[i][j]
                                                  || transitiveClosure[i][k] && transitiveClosure[k][j];
                    }
                }
            }

            return transitiveClosure;
        }

        private readonly BitArray[] _matrix;

        #region  not sortable (modify ReSharper template to catch these cases)

        public BitArray this[int index] => this._matrix[index];

        #endregion
    }
}