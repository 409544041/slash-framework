// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Vector3F.cs" company="Rainy Games">
//   Copyright (c) Rainy Games. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RainyGames.Math.Algebra.Vectors
{
    using System;
    using System.Globalization;
    using RainyGames.Math.Utils;

    [Serializable]
    public struct Vector3F
    {
        #region Constants and Fields

        /// <summary>
        ///   Unrotated forward vector.
        /// </summary>
        public static Vector3F Forward = new Vector3F(0, 0, 1);

        /// <summary>
        ///   All vector components are 1.
        /// </summary>
        public static Vector3F One = new Vector3F(1, 1, 1);

        /// <summary>
        ///   All vector components are 0.
        /// </summary>
        public static Vector3F Zero = new Vector3F(0, 0, 0);

        /// <summary>
        ///   Unrotated side vector.
        /// </summary>
        public static Vector3F Right = new Vector3F(1, 0, 0);

        /// <summary>
        ///   Unrotated up vector.
        /// </summary>
        public static Vector3F Up = new Vector3F(0, 1, 0);

        /// <summary>
        ///   X component.
        /// </summary>
        public float X;

        /// <summary>
        ///   Y component.
        /// </summary>
        public float Y;

        /// <summary>
        ///   Z component.
        /// </summary>
        public float Z;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Constructor.
        /// </summary>
        /// <param name="vector">Initial vector.</param>
        public Vector3F(Vector3F vector)
            : this()
        {
            this.X = vector.X;
            this.Y = vector.Y;
            this.Z = vector.Z;
        }

        /// <summary>
        ///   Constructor.
        /// </summary>
        /// <param name="x">Initial x value.</param>
        /// <param name="y">Initial y value.</param>
        /// <param name="z">Initial z value.</param>
        public Vector3F(float x, float y, float z)
            : this()
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        /// <summary>
        ///   Constructor.
        /// </summary>
        /// <param name="values">
        ///   Float array which contains the initial vector value. 
        ///   Value at index 0 is taken as the initial x value, 
        ///   value at index 1 is taken as the initial y value,
        ///   value at index 2 is taken as the initial z value.
        /// </param>
        public Vector3F(params float[] values)
            : this()
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }

            if (values.Length < 3)
            {
                throw new ArgumentException("Expected a float array which size is at least 3.", "values");
            }

            this.X = values[0];
            this.Y = values[1];
            this.Z = values[2];
        }

        #endregion

        /// <summary>
        ///   Magnitude of the vector.
        /// </summary>
        public float Magnitude
        {
            get
            {
                return MathF.Sqrt(this.SquareMagnitude);
            }
        }

        /// <summary>
        ///   Square magnitude of the vector.
        /// </summary>
        public float SquareMagnitude
        {
            get
            {
                return (this.X * this.X) + (this.Y * this.Y) + (this.Z * this.Z);
            }
        }

        /// <summary>
        ///   Indicates if at least one vector component is not zero.
        /// </summary>
        public bool IsNonZero
        {
            get
            {
                return this.X != 0 || this.Y != 0 || this.Z != 0;
            }
        }

        /// <summary>
        ///   Indicates if all vector components are zero.
        /// </summary>
        public bool IsZero
        {
            get
            {
                return this.X == 0 && this.Y == 0 && this.Z == 0;
            }
        }

        #region Operators

        /// <summary>
        ///   Sums the components of the passed vectors and returns the resulting vector.
        /// </summary>
        /// <param name="a">First vector.</param>
        /// <param name="b">Second vector.</param>
        /// <returns>Vector which components are the sum of the respective components of the two passed vectors.</returns>
        public static Vector3F operator +(Vector3F a, Vector3F b)
        {
            return new Vector3F(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        /// <summary>
        ///   Divides each component of the passed vector by the passed value.
        /// </summary>
        /// <param name="a">Vector to divide by the float value.</param>
        /// <param name="b">Float value to divide by.</param>
        /// <returns>
        ///   Vector where each component is the result of the particular component of the passed vector divided by
        ///   the passed float value.
        /// </returns>
        public static Vector3F operator /(Vector3F a, float b)
        {
            return new Vector3F(a.X / b, a.Y / b, a.Z / b);
        }

        /// <summary>
        ///   Indicates if the two passed vectors are equal.
        /// </summary>
        /// <param name="a">First vector.</param>
        /// <param name="b">Second vector.</param>
        /// <returns>True if the two passed vectors are equal; otherwise, false.</returns>
        public static bool operator ==(Vector3F a, Vector3F b)
        {
            return a.Equals(b);
        }

        /// <summary>
        ///   Indicates if the two passed vectors are not equal.
        /// </summary>
        /// <param name="a">First vector.</param>
        /// <param name="b">Second vector.</param>
        /// <returns>True if the two passed vectors are not equal; otherwise, false.</returns>
        public static bool operator !=(Vector3F a, Vector3F b)
        {
            return a.Equals(b) == false;
        }

        /// <summary>
        ///   Multiplies each vector component of the two passed vectors.
        /// </summary>
        /// <param name="a">First vector.</param>
        /// <param name="b">Second vector.</param>
        /// <returns>Vector which components are the product of the respective components of the passed vectors.</returns>
        public static Vector3F operator *(Vector3F a, Vector3F b)
        {
            return new Vector3F(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        }

        /// <summary>
        ///   Multiplies each vector component with the passed float value.
        /// </summary>
        /// <param name="a">Vector to multiply.</param>
        /// <param name="b">Float value to multiply by.</param>
        /// <returns>
        ///   Vector which components are the product of the respective component of the passed vector and the float value.
        /// </returns>
        public static Vector3F operator *(Vector3F a, float b)
        {
            return new Vector3F(a.X * b, a.Y * b, a.Z * b);
        }

        /// <summary>
        ///   Multiplies each vector component with the passed float value.
        /// </summary>
        /// <param name="a">Float value to multiply by.</param>
        /// <param name="b">Vector to multiply.</param>
        /// <returns>
        ///   Vector which components are the product of the respective component of the passed vector and the float value.
        /// </returns>
        public static Vector3F operator *(float a, Vector3F b)
        {
            return new Vector3F(a * b.X, a * b.Y, a * b.Z);
        }

        /// <summary>
        ///   Subtracts the components of the second passed vector from the first passed.
        /// </summary>
        /// <param name="a">First vector.</param>
        /// <param name="b">Second vector.</param>
        /// <returns>Vector which components are the difference of the respective components of the two passed vectors.</returns>
        public static Vector3F operator -(Vector3F a, Vector3F b)
        {
            return new Vector3F(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        /// <summary>
        ///   Negates each component of the passed vector.
        /// </summary>
        /// <param name="a">Vector to negate.</param>
        /// <returns>Vector which components have the negated value of the respective components of the passed vector.</returns>
        public static Vector3F operator -(Vector3F a)
        {
            return new Vector3F(-a.X, -a.Y, -a.Z);
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///   Calculates the dot product of the two passed vectors.
        ///   See http://en.wikipedia.org/wiki/Dot_product for more details.
        /// </summary>
        /// <param name="a">First vector.</param>
        /// <param name="b">Second vector.</param>
        /// <returns>Dot product of the two passed vectors.</returns>
        public static float Dot(Vector3F a, Vector3F b)
        {
            return (a.X * b.X) + (a.Y * b.Y) + (a.Z * b.Z);
        }

        /// <summary>
        ///   Returns a vector that is made from the largest components of two vectors.
        /// </summary>
        /// <param name = "lhs">First vector.</param>
        /// <param name = "rhs">Second vector.</param>
        /// <returns>Returns a vector that is made from the largest components of two vectors.</returns>
        public static Vector3F Max(Vector3F lhs, Vector3F rhs)
        {
            return new Vector3F(MathUtils.Max(lhs.X, rhs.X), MathUtils.Max(lhs.Y, rhs.Y), MathUtils.Max(lhs.Z, rhs.Z));
        }

        /// <summary>
        ///   Returns a vector that is made from the smallest components of two vectors.
        /// </summary>
        /// <param name = "lhs">First vector.</param>
        /// <param name = "rhs">Second vector.</param>
        /// <returns>Returns a vector that is made from the smallest components of two vectors.</returns>
        public static Vector3F Min(Vector3F lhs, Vector3F rhs)
        {
            return new Vector3F(MathUtils.Min(lhs.X, rhs.X), MathUtils.Min(lhs.Y, rhs.Y), MathUtils.Min(lhs.Z, rhs.Z));
        }

        public override bool Equals(object obj)
        {
            Vector3F v = (Vector3F)obj;

            return this.X == v.X && this.Y == v.Y && this.Z == v.Z;
        }

        public bool Equals(Vector3F v)
        {
            return this.X == v.X && this.Y == v.Y && this.Z == v.Z;
        }

        /// <summary>
        ///   Calculates the distance between this and the passed vector.
        /// </summary>
        /// <param name="vector">Vector to compute distance to.</param>
        /// <returns>Distance between this and the passed vector.</returns>
        public float GetDistance(Vector3F vector)
        {
            return MathF.Sqrt(this.GetSquareDistance(vector));
        }

        /// <summary>
        ///   Calculates the dot product of this and the passed vector.
        ///   See http://en.wikipedia.org/wiki/Dot_product for more details.
        /// </summary>
        /// <param name="vector">Vector to calculate dot product with.</param>
        /// <returns>Dot product of this and the passed vector.</returns>
        public float CalculateDotProduct(Vector3F vector)
        {
            return Dot(this, vector);
        }

        public override int GetHashCode()
        {
            return this.X.GetHashCode() ^ this.Y.GetHashCode() ^ this.Z.GetHashCode();
        }

        /// <summary>
        ///   Returns the normalized vector.
        /// </summary>
        /// <returns>Normalized vector.</returns>
        public Vector3F GetNormalized()
        {
            float magnitude = this.Magnitude;
            if (magnitude != 0.0f)
            {
                return new Vector3F(this.X / magnitude, this.Y / magnitude, this.Z / magnitude);
            }
            else
            {
                return Zero;
            }
        }

        /// <summary>
        ///   Calculates the square distance between this and the passed vector.
        /// </summary>
        /// <param name="vector">Vector to compute square distance to.</param>
        /// <returns>Square distance between this and the passed vector.</returns>
        public float GetSquareDistance(Vector3F vector)
        {
            return MathF.Pow2(vector.X - this.X) + MathF.Pow2(vector.Y - this.Y) +
                   MathF.Pow2(vector.Z - this.Z);
        }

        /// <summary>
        ///   Normalizes this vector.
        /// </summary>
        public void Normalize()
        {
            float magnitude = this.Magnitude;
            if (magnitude != 0.0f)
            {
                this.X /= magnitude;
                this.Y /= magnitude;
                this.Z /= magnitude;
            }
        }

        public override string ToString()
        {
            return string.Format(
                "({0},{1},{2})",
                this.X.ToString(CultureInfo.InvariantCulture.NumberFormat),
                this.Y.ToString(CultureInfo.InvariantCulture.NumberFormat),
                this.Z.ToString(CultureInfo.InvariantCulture.NumberFormat));
        }

        #endregion
    }
}