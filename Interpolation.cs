using System;
using System.Collections.Generic;
using ComplexNumber;

namespace OxyTest
{
    class Interpolation
    {
        /* METHODS */
        public static double Linear(double[] xarray, double[] yarray, double x)
        {
            double y = double.NaN;
            for (int i = 0; i < xarray.Length; i++)
            {
                if (x >= xarray[i] && x < xarray[i + 1])
                {
                    y = yarray[i] + (x - xarray[i]) * (yarray[i + 1] - yarray[i]) /
                        (xarray[i + 1] - xarray[i]);
                }
            }
            return y;
        }

        public static double[] Linear(double[] xarray, double[] yarray, double[] x)
        {
            double[] y = new double[x.Length];
            for (int i = 0; i < x.Length; i++)
            {
                y[i] = Linear(xarray, yarray, x[i]);
            }
            return y;
        }

        public static Complex Linear(double[] xarray, Complex[] yarray, double x)
        {
            Complex y; // = new Complex(0, 0);
            double[] yReal = new double[yarray.Length];
            double[] yImaginary = new double[yarray.Length];
            for (int i = 0; i < yarray.Length; i++)
            {
                yReal[i] = yarray[i].Real;
                yImaginary[i] = yarray[i].Imaginary;
            }
            y = new Complex(Linear(xarray, yReal, x), Linear(xarray, yImaginary, x));
            return y;
        }

        public static Complex[] Linear(double[] xarray, Complex[] yarray, double[] x)
        {
            Complex[] y = new Complex[x.Length];
            for (int i = 0; i < x.Length; i++)
            {
                y[i] = Linear(xarray, yarray, x[i]);
            }
            return y;
        }
    }
}
