using System.Diagnostics;


namespace ProgrTech1
{
    public class Operation
    {
        private Form1 form;
        public Operation(Form1 form)
        {
            this.form = form;
        }
        public const int MaxThreads = 8;
        public const int Size = 3000;
        public static float[][] Create()
        {
            Random rnd = new Random();
            float[][] floats = new float[Size][];
            for (int i = 0; i < Size; i++)
            {
                floats[i] = new float[Size];
                for (int j = 0; j < Size; j++)
                {
                    floats[i][j] = (float)rnd.NextDouble();
                }
            }
            return floats;
        }

        public float[][] CreateEmpty()
        {
            float[][] floats = new float[Size][];
            for (int i = 0; i < Size; i++)
            {
                floats[i] = new float[Size];
            }
            return floats;
        }

        public float[][] CreateTenRows()
        {
            float[][] floats = new float[10][];
            for (int i = 0; i < 10; i++)
                floats[i] = new float[Size];
            return floats;
        }

        public float[][] CreateTenRows(float[][] f, int ind)
        {
            int index;
            index = ind * 10;

            float[][] floats = CreateTenRows();
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    floats[i][j] = f[index][j];
                }

                index++;
            }
            return floats;
        }

        public float[][] MultiplyFloats(float[][] matA, float[][] matB)
        {
            float[][] floats = CreateEmpty();
            for(int i = 0; i < Size;i++)
                for (int j = 0; j < Size; j++)
                    floats[i][j] = matA[i][j] * matB[i][j];
            return floats;
        }


        public float[][] Exponentiation(float[][] floats, int k)
        {
            float[][] f = floats;
            float[][] tmp = CreateTenRows(floats, k);
            int indexTens = 0;
            for (int i = k*10; i < (k+1)*10; i++)
            {   for (int j = 0; j < Size; j++)
                {
                    float value = f[i][j];
                    float tmpResult = value;
                    for (int cnt = 0; cnt < i / 10; cnt++)
                    {
                        tmpResult*=value;
                    }
                    tmp[indexTens][j] = tmpResult;
                }
                indexTens++;
            }
            return tmp;
        }

        public float[][] ExponentiationOfTens(float[][] floats, int ind)
        {
            float[][] f = floats;
            for(int i = 0; i < 10; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    float value = f[i][j];
                    for (int cnt = 0; cnt < ind; cnt++)
                        f[i][j] *= value;
                }
            }
            return f;
        }

        public float[][] Update(float[][] floats, float[][] tenFloats, int ind)
        {
            int index = ind;
            int indexTens = 0;
            for (int i = index * 10; i < (index + 1) * 10; i++)
            {
                for(int j = 0; j < Size; j++)
                    floats[i][j] = tenFloats[indexTens][j];
                indexTens++;
            }
            return floats;
        }

        public float Verify(float[][] floats)
        {
            float sum = 0;
            for (int i = 0; i < Size;i++)
                for(int j =0; j < Size;j++)
                    sum += floats[i][j];
            return sum;
        }
        public void Execute(float[][] matA, float[][] matB, float[] verFloats, int[] mSeconds)
        {
            form.button2.Enabled = false;
            StreamWriter streamWriter = new StreamWriter("C:\\Users\\Ilnur\\Desktop\\Output.txt");
            float[] verificationFloats = verFloats;
            int[] milliseconds = mSeconds;
            float[][] floatsMT = CreateEmpty();
            floatsMT = MultiplyFloats(matA, matB);
            Stopwatch swMT = new Stopwatch();
            swMT.Start();
            //floatsMT = Exponentiation(floatsMT);
            for (int i = 0; i < Size / 10; i++)
            {
                var tmp = CreateTenRows(floatsMT, i);
                tmp = ExponentiationOfTens(tmp, i);
                floatsMT = Update(floatsMT, tmp, i);
                //floatsMT = Update(floatsMT, Exponentiation(CreateTenRows(floatsMT,i),i,Size), i);
            }
            swMT.Stop();
            milliseconds[0] = (int)swMT.ElapsedMilliseconds;
            verificationFloats[0] = Verify(floatsMT);
            for (int threads = 1; threads <= MaxThreads; threads++)
            {
                float[][] floats = CreateEmpty();
                floats = MultiplyFloats(matA, matB);
                float[][] resultFloats = CreateEmpty();
                ParallelOptions parallelOptions = new ParallelOptions();
                parallelOptions.MaxDegreeOfParallelism = threads;
                Stopwatch sw = new Stopwatch();
                sw.Start();
                
                object locker = new object();
                Parallel.For(0, Size / 10, parallelOptions, i =>
                {
                    float[][] tmpFloats = Exponentiation(floats, i);
                    
                    lock (locker)
                    {
                        resultFloats = Update(resultFloats, tmpFloats, i);
                    }
                });
                
                sw.Stop();
                milliseconds[threads] = (int)sw.ElapsedMilliseconds;
                verificationFloats[threads] = Verify(resultFloats);
            }

            for (int i = 0; i < verificationFloats.Length; i++)
            {
                streamWriter.WriteLine($"Threads = {i}, Verification Value = {verificationFloats[i]}, time in milliseconds = {milliseconds[i]}");
            }
            streamWriter.Close();
            form.button2.Enabled = true;
            MessageBox.Show("Done!", "Status");
            
        }
    }
}
