using System;
using System.Collections.Generic;
using Math2;

using System.Diagnostics;
using Microsoft.Win32;

class RAO
{
    /* ******************************************************************** 
     * CLASS: RAO
     * 
     * Object that contains the Response Amplitude Operators (RAO)
     * that are provide for a range of:
     *  - Headings [deg]
     *  - Frequqncy [rad/s] or Periods [s]
     *      
     * --------------------------------------------------------------------
     * |Version  | Author            | Date      | Comments
     * |---------|-------------------|-----------|------------------------- 
     * |Beta     | Andrea Turchetto  |16/04/2016 | None 
     * |---------|-------------------|-----------|-------------------------
     * |         |                   |           |   
     * |---------|-------------------|-----------|-------------------------
     * |         |                   |           |   
     * |---------|-------------------|-----------|-------------------------
     * |         |                   |           |   
     * |---------|-------------------|-----------|-------------------------
     *
     * ******************************************************************** 
     * --------------------------------------------------------------------
     * The RAOs can be expressed in COMPLEX format or specifing the 
     * amplitude and phase:
     * 
     *          RAO.complex   = Real + Imaginary*i
     *          
     *          RAO.amplitude = abs(RAO.complex) 
     *                        = sqrt(Real^2+Imaginary^2)
     *                        
     *          RAO.phase     = angle(RAO.complex)
     *                        = atan2(Imaginary,Real)
     * where
     *      i = sqrt(-1)
     * --------------------------------------------------------------------
     */ 
    
    #region METADATA
    /* METADATA */
    /* --------------------------------------------------------------------
     * 6 Degrees of Freedom (DOF):
     * Surge, Sway, Heave, Roll, Pitch, Yaw
     * --------------------------------------------------------------------
     */
    private const int DOF = 6;
    private const double deg2rad = Math.PI / 180;
    //
    /* --------------------------------------------------------------------
     * RAO.amplitude[DOF][frequency,heading]
     * RAO.phase[DOF][frequency,heading]
     * RAO.complex[DOF][frequency,heading]
     *  
     * motion = 6 DoF (Degrees of Freedom)
     * --------------------------------------------------------------------
     * */
    private double[] frequency;
    private double[] period;
    private double[] heading;
    /*
    private MatrixR[] amplitude = new MatrixR[DOF];
    private MatrixR[] phase = new MatrixR[DOF];
    private MatrixC[] complex = new MatrixC[DOF];
    */
    private VectorR[] amplitude = new VectorR[DOF];
    private VectorR[] phase = new VectorR[DOF];
    private VectorC[] complex = new VectorC[DOF];    
    //
    private VectorR referencePoint; // Point at which the RAOs have been calculated
    #endregion
    //
    #region PROPERTIES
    /* PROPERTIES */
    public double[] Frequency
    {
        get { return this.frequency; }
        //set { this.frequency = value; }
    }
    public double[] Period
    {
        get { return this.period; }
    }
    public double[] Heading
    {
        get { return this.heading; }
    }
    public VectorR[] Amplitude
    {
        get { return this.amplitude; }
        set { this.amplitude = value; }
    }
    public VectorR[] Phase
    {
        get { return this.phase; }
        set { this.phase = value; }
    }
    public VectorC[] Complex
    {
        get { return this.complex; }
        set { this.complex = value; }
    }
    #endregion
    //
    #region CONSTUCTOR
    /* CONSTRUCTOR */
    //public RAO() { }
    public RAO(double[] inputFrequency)
    {
        /* Find whether there are more than 3 elements in the frequency array 
             * greater than 5. if so it is very likely that the elements in the 
             * frequency array are Periods.
             * The periods will than be saved in rao.period and converted in frequencies */ 
        double[] matchedItems = Array.FindAll(inputFrequency, x => x > 5);
        if (matchedItems.Length > 3)
        {
            this.period = inputFrequency;
            CalculateFrequency(inputFrequency);
        }
        else
        {
            this.frequency = inputFrequency;
            CalculatePeriod(inputFrequency);
        }             
        //
        for (int iMotion = 0; iMotion < DOF; iMotion++)
        {
            this.amplitude[iMotion] = new VectorR(frequency.Length);
            this.phase[iMotion] = new VectorR(frequency.Length);
            this.complex[iMotion] = new VectorC(frequency.Length);
        }
    }
    /*
    public RAO(double[] Frequency, double[] Heading, List<MatrixR> Amplitude, List<MatrixR> Phase)
    {
        this.frequency = Frequency;
        this.heading = Heading;
        this.amplitude = Amplitude;
        this.phase = Phase;
        CalculateComplex();
    } */
    /*
    public RAO(double[] Frequency, double[] Heading, List<MatrixC> ComplexRAO)
    {
        this.frequency = Frequency;
        this.heading = Heading;
        this.complex = ComplexRAO;
        CalculateAmplitudeAndPhase();
    }*/
    #endregion
    //
    #region METHODS
    /* METHODS */
    #region Get & Set
    public double[] GetFrequency() { return frequency; }
    public double[] GetPeriod() { return period; }
//    public double[] GetHeading() { return heading; }
    public VectorR GetReferencePoint() { return this.referencePoint; }
    public void SetReferencePoint(VectorR inputReferencePoint) { this.referencePoint = inputReferencePoint; }
    #endregion

    public void CalculateAmplitudeAndPhase()
    {
        VectorC Complex1Dof = new VectorC(frequency.Length);
        //VectorC Complex1Dof1Head = new VectorC(frequency.Length);
        VectorR[] Amplitude1Dof = new VectorR[DOF]; 
        VectorR[] Phase1Dof = new VectorR[DOF];  
        for (int iMotion = 0; iMotion < DOF ; iMotion++)
        {
            Amplitude1Dof[iMotion] = new VectorR(frequency.Length);
            Phase1Dof[iMotion] = new VectorR(frequency.Length);
            Complex1Dof = this.complex[iMotion];
            
            for (int iFreq = 0; iFreq < this.frequency.Length; iFreq++)
            {
                Amplitude1Dof[iMotion][iFreq] = Complex1Dof[iFreq].Modulus;
                Phase1Dof[iMotion][iFreq] = Complex1Dof[iFreq].Angle / deg2rad;
            }                            
            this.amplitude[iMotion] = Amplitude1Dof[iMotion];
            this.phase[iMotion] = Phase1Dof[iMotion];
        }        
    }    
    public void CalculateComplex()
    {
        VectorC[] Complex1Dof = new VectorC[DOF]; 
        //VectorR Amplitude1Dof1Head = new VectorR(frequency.Length);
        //VectorR Phase1Dof1Head = new VectorR(frequency.Length);
        VectorR Amplitude1Dof = new VectorR(frequency.Length);
        VectorR Phase1Dof = new VectorR(frequency.Length);
        for (int iMotion = 0; iMotion < DOF; iMotion++)
        {
            Amplitude1Dof = this.amplitude[iMotion];
            Phase1Dof = this.phase[iMotion];
            Complex1Dof[iMotion] = new VectorC(frequency.Length);

            for (int iFreq = 0; iFreq < this.frequency.Length; iFreq++)
            {
                Complex1Dof[iMotion][iFreq] = new ComplexNumber.Complex(
                    Amplitude1Dof[iFreq] * Math.Cos(Phase1Dof[iFreq] * deg2rad),
                    Amplitude1Dof[iFreq] * Math.Sin(Phase1Dof[iFreq] * deg2rad));
            }            
            this.complex[iMotion] = Complex1Dof[iMotion];
        }
        /* For Debugging 
        printComplexRAO();
        CalculateAmplitudeAndPhase();
        PrintRAO();
        */ 
    }
    public void CalculatePeriod(double[] inputFrequency)
    {
        if (inputFrequency.Length > 0)
        {
            this.period = new double[inputFrequency.Length];
            for (int iFreq = 0; iFreq<inputFrequency.Length;iFreq++)
            {
                this.period[iFreq] = 2 * Math.PI / inputFrequency[iFreq];
            }
        }
    }
    public void CalculateFrequency(double[] inputPeriod)
    {
        if (inputPeriod.Length > 0)
        {
            this.frequency = new double[inputPeriod.Length];
            for (int iFreq = 0; iFreq < inputPeriod.Length; iFreq++)
            {
                this.frequency[iFreq] = 2 * Math.PI / inputPeriod[iFreq];
            }
        }
    }

    public RAO TransferRAO(VectorR ReferencePoint, VectorR TransferPoint)
    {
        this.referencePoint = ReferencePoint;
        //
        VectorC diffPoint = new VectorC(new ComplexNumber.Complex[] {
            new ComplexNumber.Complex(TransferPoint[0] - ReferencePoint[0], 0.0),
            new ComplexNumber.Complex(TransferPoint[1] - ReferencePoint[1], 0.0),
            new ComplexNumber.Complex(TransferPoint[2] - ReferencePoint[2], 0.0)});

        VectorC vectorCRotation;

        VectorC newSurge = new VectorC(frequency.Length);
        VectorC newSway =  new VectorC(frequency.Length);
        VectorC newHeave = new VectorC(frequency.Length);
                      
        RAO newRAO = new RAO(frequency);
        /* Rotational RAOs remain invariant with position */
        newRAO.Complex[3] = this.complex[3];    // Roll
        newRAO.Complex[4] = this.complex[4];  // Pitch
        newRAO.Complex[5] = this.complex[5];  // Yaw
        //
        for (int iFreq = 0; iFreq < this.frequency.Length; iFreq++)
        {
            vectorCRotation = new VectorC(new ComplexNumber.Complex[] {
                    this.complex[3][iFreq],
                        this.complex[4][iFreq],
                        this.complex[5][iFreq]});

            newSurge[iFreq] = this.complex[0][iFreq] +
        Math2.VectorC.CrossProduct(vectorCRotation * deg2rad, diffPoint)[0];
            newSway[iFreq] = this.complex[1][iFreq] +
        Math2.VectorC.CrossProduct(vectorCRotation * deg2rad, diffPoint)[1];
            newHeave[iFreq] = this.complex[2][iFreq] +
        Math2.VectorC.CrossProduct(vectorCRotation * deg2rad, diffPoint)[2];
        }
        
        newRAO.Complex[0] = newSurge;
        newRAO.Complex[1] = newSway;
        newRAO.Complex[2] = newHeave;
        newRAO.CalculateAmplitudeAndPhase();
        newRAO.referencePoint = TransferPoint;
        return newRAO;
    }

    /*
    public void PrintRAO()
    {
        SaveFileDialog outputFile = new SaveFileDialog();
        outputFile.Filter = "Text Files (.txt)|*.txt|All Files (*.*)|*.*";
        outputFile.RestoreDirectory = true;

        if (outputFile.ShowDialog() == true)
        {
            System.IO.StreamWriter objWrite =
            new System.IO.StreamWriter(outputFile.FileName);
            objWrite.WriteLine("RAOs");
            objWrite.WriteLine("Number of Headings: " + heading.Length.ToString());
            string stringHeadings = "";
            for (int iHead = 0; iHead<heading.Length;iHead++)
                stringHeadings = stringHeadings + heading[iHead].ToString() + " ";            
            objWrite.WriteLine("Headings: " + stringHeadings);
            objWrite.WriteLine("Number of Periods: " + frequency.Length.ToString());
            string stringPeriods = "";
            for (int iFreq = 0; iFreq < frequency.Length; iFreq++)
                stringPeriods += period[iFreq].ToString() + " ";
            objWrite.WriteLine("Periods: " + stringPeriods);
            if (this.referencePoint.GetSize() != 0)
                objWrite.WriteLine("Reference Point: {0:0.00}m, {1:0.00}m, {2:0.00}m",
                    this.referencePoint[0], this.referencePoint[1], this.referencePoint[2]);
            else
                objWrite.WriteLine("Reference Point: NOT GIVEN");

                for (int iHead = 0; iHead < heading.Length; iHead++)
                {
                    objWrite.WriteLine("\nHeading {0:0.0}", heading[iHead]);
                    objWrite.WriteLine("\t\t\t\t\tSurge\t\t\t\t\tSway\t\t\t\t\tHeave\t\t\t\t\tRoll\t\t\t\t\tPitch\t\t\t\t\tYaw");
                    objWrite.WriteLine("\tPeriod\t\tAmpl\t\tPhase\t\tAmpl\t\tPhase\t\tAmpl\t\tPhase\t\tAmpl\t\tPhase\t\tAmpl\t\tPhase\t\tAmpl\t\tPhase");
                    objWrite.WriteLine("\t[s]\t\t\t[m/m]\t\t[deg]\t\t[m/m]\t\t[deg]\t\t[m/m]\t\t[deg]\t\t[deg/m]\t\t[deg]\t\t[deg/m]\t\t[deg]\t\t[deg/m]\t\t[deg]");
                    for (int iFreq = 0; iFreq < this.frequency.Length; iFreq++)
                    {
                        objWrite.WriteLine("{0,10:0.0000}\t{1,10:0.000}\t{2,10:0.000}" +
                            "\t{3,10:0.000}\t{4,10:0.000}\t{5,10:0.000}\t{6,10:0.000}" +
                            "\t{7,10:0.000}\t{8,10:0.000}\t{9,10:0.000}\t{10,10:0.000}" +
                            "\t{11,10:0.000}\t{12,10:0.000}",
                            period[iFreq],
                            amplitude[0][iFreq, iHead], phase[0][iFreq, iHead],
                            amplitude[1][iFreq, iHead], phase[1][iFreq, iHead],
                            amplitude[2][iFreq, iHead], phase[2][iFreq, iHead],
                            amplitude[3][iFreq, iHead], phase[3][iFreq, iHead],
                            amplitude[4][iFreq, iHead], phase[4][iFreq, iHead],
                            amplitude[5][iFreq, iHead], phase[5][iFreq, iHead]);
                    }
                }                
            objWrite.Close();
        }
    }
    */

    /*
    public void printComplexRAO()
    {
        SaveFileDialog complexFile = new SaveFileDialog();
        complexFile.Filter = "Text File (.txt)|*.txt";
        complexFile.RestoreDirectory = true;
        
        if (complexFile.ShowDialog() != null)
        {
            System.IO.StreamWriter writeComplex = new System.IO.StreamWriter(complexFile.FileName);
            writeComplex.WriteLine("RAO - Complex");

            for (int iHead = 0; iHead < heading.Length; iHead++)
            {
                writeComplex.WriteLine("Heading " + this.heading[iHead].ToString());
                for (int iFreq = 0; iFreq < frequency.Length; iFreq++)
                {
                    writeComplex.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}",
                        this.complex[0][iFreq, iHead].ToString(), this.complex[1][iFreq, iHead].ToString(),
                        this.complex[2][iFreq, iHead].ToString(), this.complex[3][iFreq, iHead].ToString(),
                        this.complex[4][iFreq, iHead].ToString(), this.complex[5][iFreq, iHead].ToString());
                }
            }
        }
    }
    */
    #endregion
}

