using System;
using System.IO;

namespace LogisticGradient
{
    class ModelTrainer
    {
        static void Main (string[] args)
        {
            Console.WriteLine ("\nBegin Logistic Regression (binary) Classification");

            double[][] allData = ReadData (args[0]);

            Console.WriteLine ("Creating train (50%) and test (50%) matrices");
            double[][] trainData;
            double[][] testData;
            MakeTrainTest (allData, 0, out trainData, out testData);
            Console.WriteLine ("Done");

            Console.WriteLine ("Creating LR binary classifier");
            int numFeatures = allData[0].Length - 1;
            LogisticClassifier lc = new LogisticClassifier (numFeatures);

            double alpha = 0.03;
            if(args.Length > 1)
            {
                alpha = Double.Parse(args[1]);
            }
            Console.WriteLine ("Setting learning rate = " + alpha.ToString ("F2"));

            int maxEpochs = 5000;
            if(args.Length > 2)
            {
                maxEpochs = Int32.Parse(args[2]);
            }
            Console.WriteLine ("Setting maxEpochs = " + maxEpochs);
           
            Console.WriteLine ("\nStarting training using (stochastic) gradient descent");
            double[] weights = lc.Train (trainData, maxEpochs, alpha);
            Console.WriteLine ("Training complete");

            Console.WriteLine ("\nBest weights found:");
            ShowVector (weights, 4, true);

            TrainResult trainAcc = lc.Accuracy (trainData, weights);
            Console.WriteLine ("Accuracy / False Positive rate on training data = " +
                trainAcc.Accuracy.ToString ("F4") + " / " + trainAcc.FalsePositiveRate.ToString("F4"));

            TrainResult testAcc = lc.Accuracy (testData, weights);
            Console.WriteLine ("Test data:");

            Console.WriteLine("Accuracy: " + testAcc.Accuracy.ToString("F4"));
            Console.WriteLine("Precision: " + testAcc.Precision.ToString("F4"));
            Console.WriteLine("Recall: " + testAcc.Recall.ToString("F4"));
            Console.WriteLine("F1Score: " + testAcc.F1Score.ToString("F4"));

            Console.WriteLine("TruePositiveRate: " + testAcc.TruePositiveRate.ToString("F4"));
            Console.WriteLine("FalsePositiveRate: " + testAcc.FalsePositiveRate.ToString("F4"));
            Console.WriteLine("TrueNegativeRate: " + testAcc.TrueNegativeRate.ToString("F4"));
            Console.WriteLine("FalseNegativeRate: " + testAcc.FalseNegativeRate.ToString("F4"));
        }

        static double[][] ReadData (string path)
        {
            string[] fileContent = File.ReadAllLines (path);
            double[][] result = new double[fileContent.Length][];

            double[] values = null;
            int dataWidth = 0;

            for (int dataRow = 0; dataRow < fileContent.Length; dataRow++)
            {
                string[] dataElements = fileContent[dataRow].Split (',');

                if(dataRow == 0)
                {
                    values = new double[dataElements.Length];
                    dataWidth = dataElements.Length;
                }
                else
                {
                    values = new double[result[0].Length];
                }

                if(dataElements.Length == dataWidth)
                {
                    for (int dataElement = 0; dataElement < dataElements.Length; dataElement++)
                    {
                        string elementValue = dataElements[dataElement].Replace ("\"", "");

                        Double elementDoubleValue;
                        if (!Double.TryParse (elementValue, out elementDoubleValue))
                        {
                            Console.WriteLine("Could not convert " + elementValue + " on row " + dataRow + " to a Double");
                            elementDoubleValue = 0;
                        }

                        values[dataElement] = elementDoubleValue;
                    }
                }

                result[dataRow] = values;
            }

            return result;
        }

        static void MakeTrainTest (double[][] allData, int seed, out double[][] trainData, out double[][] testData)
        {
            Random rnd = new Random (seed);
            int totRows = allData.Length;

            // Train on 50%
            int numTrainRows = (int) (totRows * 0.8);
            int numTestRows = totRows - numTrainRows;
            trainData = new double[numTrainRows][];
            testData = new double[numTestRows][];

            for (int i = 0; i < numTrainRows; ++i)
                trainData[i] = allData[i];

            for (int i = 0; i < numTestRows; ++i)
                testData[i] = allData[i + numTrainRows];
        }

        static void ShowVector (double[] vector, int decimals, bool newLine)
        {
            for (int i = 0; i < vector.Length; ++i)
                Console.Write (vector[i].ToString ("F" + decimals) + " ");
            Console.WriteLine ("");
            if (newLine == true)
                Console.WriteLine ("");
        }
    }

    public class LogisticClassifier
    {
        private int numFeatures;

        // First index is the bias / constant weight
        private double[] weights;
        private Random rnd;

        public LogisticClassifier (int numFeatures)
        {
            this.numFeatures = numFeatures;
            this.rnd = new Random (0);
            this.weights = new double[numFeatures + 1];
        }

        public double[] Train (double[][] trainData, int maxEpochs, double alpha)
        {
            // alpha is the learning rate
            int epoch = 0;

            // The order in which we will process training data, constantly shuffled
            int[] sequence = new int[trainData.Length];
            for (int i = 0; i < sequence.Length; ++i)
                sequence[i] = i;

            while (epoch < maxEpochs)
            {
                ++epoch;

                if (epoch % 500 == 0 && epoch != maxEpochs)
                {
                    double mse = Error(trainData, weights).ErrorRate;
                    Console.Write ("epoch = " + epoch);
                    Console.WriteLine ("    error = " + mse.ToString ("F4"));
                }

                // Randomize every epoch
                Shuffle (sequence);

                // stochastic/online/incremental approach
                for (int ti = 0; ti < trainData.Length; ++ti)
                {
                    int i = sequence[ti];
                    double computed = ComputeOutput (trainData[i], weights);
                    int targetIndex = trainData[i].Length - 1;
                    double target = trainData[i][targetIndex];

                    weights[0] += alpha * (target - computed) * 1;

                    for (int j = 1; j < weights.Length; ++j)
                    {
                        weights[j] += alpha * (target - computed) * trainData[i][j - 1];
                    }
                }

            }

            return this.weights;
        }

        private void Shuffle (int[] sequence)
        {
            for (int i = 0; i < sequence.Length; ++i)
            {
                int r = rnd.Next (i, sequence.Length);
                int tmp = sequence[r];
                sequence[r] = sequence[i];
                sequence[i] = tmp;
            }
        }

        private ErrorResult Error (double[][] trainData, double[] weights)
        {
            ErrorResult output = new ErrorResult();

            // mean squared error using supplied weights
            // y-value / label (0/1) is last column
            int yIndex = trainData[0].Length - 1; 
            double sumSquaredError = 0.0;
            double falsePositiveCount = 0.0;

            for (int i = 0; i < trainData.Length; ++i) 
            {
                double computed = ComputeOutput (trainData[i], weights);
                double desired = trainData[i][yIndex];
                sumSquaredError += (computed - desired) * (computed - desired);

                if((computed > 0.5) && (desired < 0.5))
                {
                    falsePositiveCount++;
                }
            }

            output.ErrorRate = sumSquaredError / trainData.Length;
            output.FalsePositiveRate = falsePositiveCount / trainData.Length;
            return output;
        }

        private double ComputeOutput (double[] dataItem, double[] weights)
        {
            double z = 0.0;

            // first element is the bias constant
            z += weights[0];

            // Go to lenghth - 1 because the final element is the label
            for (int i = 0; i < weights.Length - 1; ++i) 
                z += (weights[i + 1] * dataItem[i]);

            return 1.0 / (1.0 + Math.Exp (-z));
        }

        private int ComputeDependent (double[] dataItem, double[] weights)
        {
            double y = ComputeOutput (dataItem, weights);

            if (y <= 0.5)
                return 0;
            else
                return 1;
        }

        public TrainResult Accuracy (double[][] dataSet, double[] weights)
        {
            TrainResult output = new TrainResult();

            int truePositives = 0;
            int trueNegatives = 0;
            int falsePositives = 0;
            int falseNegatives = 0;

            int yIndex = dataSet[0].Length - 1;
            for (int i = 0; i < dataSet.Length; ++i)
            {
                if(dataSet[i].Length != weights.Length)
                {
                    continue;
                }

                int computed = ComputeDependent (dataSet[i], weights);
                int target = (int) dataSet[i][yIndex];

                if(target == 0)
                {
                    if(computed < 0.5)
                    {
                        trueNegatives++;
                    }
                    else
                    {
                        falsePositives++;
                    }
                }
                else
                {
                    if(computed < 0.5)
                    {
                        falseNegatives++;
                    }
                    else
                    {
                        truePositives++;
                    }
                }
            }

            output.Accuracy = (truePositives * 1.0 + trueNegatives) / (truePositives + trueNegatives + falsePositives + falseNegatives);
            output.Precision = (truePositives * 1.0) / (truePositives + falsePositives);
            output.Recall = (truePositives * 1.0) / (truePositives + falseNegatives);
            output.F1Score = (2.0 * output.Precision * output.Recall) / (output.Precision + output.Recall);

            output.TruePositiveRate = (truePositives * 1.0) / (truePositives + trueNegatives + falsePositives + falseNegatives);
            output.FalsePositiveRate = (falsePositives * 1.0) / (truePositives + trueNegatives + falsePositives + falseNegatives);
            output.TrueNegativeRate = (trueNegatives * 1.0) / (truePositives + trueNegatives + falsePositives + falseNegatives);
            output.FalseNegativeRate = (falseNegatives * 1.0) / (truePositives + trueNegatives + falsePositives + falseNegatives);

            return output;
        }
    }

    internal class ErrorResult
    {
        public double ErrorRate { get; set; }
        public double FalsePositiveRate { get; set; }
    }

    public class TrainResult
    {
        public double Accuracy { get; set; }
        public double Precision { get; set; }
        public double Recall { get; set; }
        public double F1Score { get; set; }

        public double TruePositiveRate { get; set; }
        public double FalsePositiveRate { get; set; }
        public double TrueNegativeRate { get; set; }
        public double FalseNegativeRate { get; set; }
    }
}