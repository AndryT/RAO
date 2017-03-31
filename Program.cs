using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingClass
{
    class Program
    {
        static void Main(string[] args)
        {
            TestingRAO();
            Console.ReadLine();
        }

        static void TestingRAO()
        {
            double[] freq = new double[5] { 25.0, 24.0, 23.0, 22.0, 21.0 };
            double[] head = new double[1] { 0.0 };
            Dictionary<double, RAO> raoSerie = new Dictionary<double, RAO>();

            for (int i = 0; i < 31; i+=15)
            {
                RAO temp = new RAO(freq);

                double[] aa = new double[freq.Length];
                aa[0] = 0.962 * i;
                aa[1] = 0.957 * i;
                aa[2] = 0.951 * i;
                aa[3] = 0.944 * i;
                aa[4] = 0.935 * i;
                double[] bb = new double[freq.Length];
                bb[0] = -108.450;
                bb[1] = -110.020;
                bb[2] = -111.800;
                bb[3] = -113.820;
                bb[4] = -116.140;

                temp.Amplitude[0] = new Math2.VectorR(aa);
                temp.Phase[0] = new Math2.VectorR(bb);
                temp.CalculateComplex();

                raoSerie.Add((double)i, temp);
            }

            double[] heading = new double[3] { 0.0, 15.0, 30.0 };
            double[] bla = new double[heading.Length];
            Math2.VectorR surge = new Math2.VectorR(freq.Length);

            for (int ifreq = 0; ifreq < freq.Length; ifreq++)
            {
                for (int iHead = 0; iHead < 31; iHead += 15)
                {
                    bla[iHead/15] = raoSerie[iHead].Amplitude[0][ifreq];                    
                }
                surge[ifreq] = OxyTest.Interpolation.Linear(heading, bla, 7.5);
            }
            RAO temp1 = new RAO(freq);
            temp1.Amplitude[0] = surge;
            raoSerie.Add(7.5, temp1);   

            for (int ifreq = 0; ifreq < freq.Length; ifreq++)
            {
                Console.WriteLine(raoSerie[7.5].Amplitude[0][ifreq]);
            }
        }

    }
}
