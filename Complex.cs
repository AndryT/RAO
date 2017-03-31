using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplexNumbers
{
    public struct Complex
    {
        public double Real;
        public double Imaginary;

        public Complex(double real, double imaginary)
        {
            this.Real = real;
            this.Imaginary = imaginary;
        }

        public override string ToString()
        {
            string s = "";
            if (Imaginary >= 0)
                s = String.Format("{0} + {1} i", Real, Imaginary);
            else
                s = String.Format("{0} - {1} i", Real, Math.Abs(Imaginary));
            return s;
        }

        // Complex Properties **************************************
        public static Complex I
        {
            get { return new Complex(0.0, 1.0); }
        }

        public Complex Conjugate
        {
            get { return new Complex(Real, -Imaginary); }
            set { this = value.Conjugate; }
        }

        public double Modulus
        {
            get { return Math.Sqrt(Real * Real + Imaginary * Imaginary); }
            set 
            {
                if (Real == 0 && Imaginary == 0)
                {
                    Real = value;
                    Imaginary = 0;
                }
                else
                {
                    double x1 = value / Math.Sqrt(Real * Real +
                        Imaginary * Imaginary);
                    Real *= x1;
                    Imaginary *= x1;
                }
            }
        }

        public double Angle
        {
            get
            {
                if (Imaginary == 0 && Real < 0)
                {
                    return Math.PI;
                }
                if (Imaginary == 0 && Real >= 0)
                {
                    return 0;
                }
                return Math.Atan2(Imaginary, Real);
            }
            set 
            {
                double modulus = Modulus;
                Real = Math.Cos(value) * modulus;
                Imaginary = Math.Sin(value) * modulus;
            }
        }

        // Complex Methods *******************************************
        public static Complex Conj(Complex c)
        {
            return c.Conjugate;
        }

        public static double Re(Complex c)
        {
            return c.Real;
        }

        public static double Im(Complex c)
        {
            return c.Imaginary;
        }

        public static double Mod(Complex c)
        {
            return c.Modulus;
        }

        public static double Arg(Complex c)
        {
            return c.Angle;
        }

        // Equality and Hashing *****************************************
        public override bool Equals(object obj)
        {
            return (obj is Complex) && this.Equals((Complex)obj);
        }

        public bool Equals(Complex c)
        {
            return Real == c.Real && Imaginary == c.Imaginary;
        }

        public override int GetHashCode()
        {
            return Real.GetHashCode() ^ Imaginary.GetHashCode();
        }

        public static bool operator ==(Complex c1, Complex c2)
        {
            return c1.Equals(c2);
        }

        public static bool operator !=(Complex c1, Complex c2)
        {
            return !c1.Equals(c2);
        }

        // Complex Operators *****************************************
        // Unary Operators
        public static Complex operator +(Complex c)
        {
            return c;
        }

        public static Complex operator -(Complex c)
        {
            return new Complex(-c.Real, -c.Imaginary);
        }

        // Addition Operator
        public static Complex operator +(Complex c1, Complex c2)
        {
            return new Complex(c1.Real + c2.Real, c1.Imaginary + c2.Imaginary);
        }

        public static Complex operator +(Complex c1, double c2)
        {
            return new Complex(c1.Real + c2, c1.Imaginary);
        }

        public static Complex operator +(double c1, Complex c2)
        {
            return new Complex(c1 + c2.Real, c2.Imaginary);
        }

        // Subtraction Operator
        public static Complex operator -(Complex c1, Complex c2)
        {
            return new Complex(c1.Real - c2.Real, c1.Imaginary - c2.Imaginary);
        }

        public static Complex operator -(Complex c1, double c2)
        {
            return new Complex(c1.Real - c2, c1.Imaginary);
        }

        public static Complex operator -(double c1, Complex c2)
        {
            return new Complex(c1 - c2.Real, -c2.Imaginary);
        }

        // Multiplication Operator
        public static Complex operator *(Complex c1, Complex c2)
        {
            return new Complex(c1.Real * c2.Real - c1.Imaginary * c2.Imaginary,
                c1.Real * c2.Imaginary + c1.Imaginary * c2.Real);
        }

        public static Complex operator *(Complex c1, double c2)
        {
            return new Complex(c1.Real * c2, c1.Imaginary * c2);
        }

        public static Complex operator *(double c1, Complex c2)
        {
            return new Complex(c1 * c2.Real, c1 * c2.Imaginary);
        }

        // Division Operator
        public static Complex operator /(Complex c1, Complex c2)
        {
            if (c2.Real==0 && c2.Imaginary==0)
            {
                return new Complex(1.0e300, 1.0e300);
            }
            double c2m2 = c2.Modulus * c2.Modulus;
            return new Complex((c1.Real * c2.Real + c1.Imaginary * c2.Imaginary) / c2m2,
                (c1.Imaginary * c2.Real - c1.Real * c2.Imaginary) / c2m2);
        }

        public static Complex operator /(Complex c1, double c2)
        {
            if (c2 == 0)
            {
                return new Complex(1.0e300, 1.0e300);
            }
            return new Complex(c1.Real / c2, c1.Imaginary / c2);
        }

        public static Complex operator /(double c1, Complex c2)
        {
            if (c2.Real == 0 && c2.Imaginary == 0)
            {
                return new Complex(1.0e300, 1.0e300);
            }
            double c2m2 = c2.Modulus * c2.Modulus;
            return new Complex(c1 * c2.Real / c2m2, -c1 * c2.Imaginary / c2m2);
        }

        // Complex Functions
        // Square root function
        public static Complex Sqrt(Complex c)
        {
            return new Complex(Math.Sqrt(c.Modulus) * Math.Cos(c.Angle / 2.0),
                               Math.Sqrt(c.Modulus) * Math.Sin(c.Angle / 2.0));
        }

        // Exponential Function
        public static Complex Exp(Complex c)
        {
            return new Complex(Math.Exp(c.Real) * Math.Cos(c.Imaginary),
                               Math.Exp(c.Real) * Math.Sin(c.Imaginary));
        }

        // Pow Function
        public static Complex Pow(Complex c1, Complex c2)
        {
            double x1 = Math.Exp(c2.Real * Math.Log(c1.Modulus) -
                        c2.Imaginary * c1.Angle);
            double x2 = Math.Cos(c2.Real * c1.Angle +
                        c2.Imaginary * Math.Log(c1.Modulus));
            double x3 = Math.Sin(c2.Real * c1.Angle +
                        c2.Imaginary * Math.Log(c1.Modulus));
            return new Complex(x1 * x2, x1 * x3);
        }

        // Logatrithm Function
        public static Complex Log(Complex c)
        {
            return new Complex(Math.Log(c.Modulus), c.Angle);
        }

        // Trigonometric Functions
        // Sine Function
        public static Complex Sin(Complex c)
        {
            return new Complex(Math.Sin(c.Real) * Math.Cosh(c.Imaginary),
                               Math.Cos(c.Real) * Math.Sinh(c.Imaginary));
        }

        // Cosine Function
        public static Complex Cos(Complex c)
        {
            return new Complex(Math.Cos(c.Real) * Math.Cosh(c.Imaginary),
                               -Math.Sin(c.Real) * Math.Sinh(c.Imaginary));
        }

        // Tangent Function
        public static Complex Tan(Complex c)
        {
            return Sin(c) / Cos(c);
        }

        // Inverse Sine Function
        public static Complex Asin(Complex c)
        {
            return -I * Log(I * c + Sqrt(1 - c * c));
        }

        // Inverse Cosine Function
        public static Complex Acos(Complex c)
        {
            return -I * Log(c + I * Sqrt(1 - c * c));
        }

        // Inverse Tangent Function
        public static Complex Atan(Complex c)
        {
            return I / 2 * (Log(1 - I * c) - Log(1 + I * c));
        }

        // Hyperbolic Sine Function
        public static Complex Sinh(Complex c)
        {
            return new Complex(Math.Sinh(c.Real) * Math.Cos(c.Imaginary),
                               Math.Cosh(c.Real) * Math.Sin(c.Imaginary));
        }

        // Hyperbolic Cosine Funciton
        public static Complex Cosh(Complex c)
        {
            return new Complex(Math.Cosh(c.Real) * Math.Cos(c.Imaginary),
                               Math.Sinh(c.Real) * Math.Sin(c.Imaginary));
        }

        // Hyperbolic Tangent Function
        public static Complex Tanh(Complex c)
        {
            return Sinh(c) / Cosh(c);
        }

        // Inverse Hyperbolic Sine Function
        public static Complex Asinh(Complex c)
        {
            return Log(c + Sqrt(1 + c * c));
        }

        // Inverse Hyperbolic Cosine Function
        public static Complex Acosh(Complex c)
        {
            return Log(c + Sqrt(c * c - 1));
        }

        // Inverse Hyperbolic Tangent Function
        public static Complex Atanh(Complex c)
        {
            return 0.5 * (Log(1 + c) - Log(1 - c));
        }
    }
}
