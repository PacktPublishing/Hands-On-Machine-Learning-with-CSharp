using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.MachineLearning.DecisionTrees;

namespace Gotoworkfor_Decision
{
    using System.Data;
    using Accord.MachineLearning.DecisionTrees.Learning;
    using Accord.Math;
    using Accord.Statistics.Filters;

    class Program
    {
        static void Main(string[] args)
        {

            DataTable data = new DataTable("Should I Go To Work For Company X");

            data.Columns.Add("Scenario");
            data.Columns.Add("Pay");
            data.Columns.Add("Benefits");
            data.Columns.Add("Culture");
            data.Columns.Add("WorkFromHome");
            data.Columns.Add("ShouldITakeJob");

            data.Rows.Add("D1", "Good", "Good", "Mean", "Yes", "Yes");
            data.Rows.Add("D2", "Good", "Good", "Mean", "No", "Yes");
            data.Rows.Add("D3", "Average", "Good", "Good", "Yes", "Yes");
            data.Rows.Add("D4", "Average", "Good", "Good", "No", "Yes");
            data.Rows.Add("D5", "Bad", "Good", "Good", "Yes", "No");
            data.Rows.Add("D6", "Bad", "Good", "Good", "No", "No");
            data.Rows.Add("D7", "Good", "Average", "Mean", "Yes", "Yes");
            data.Rows.Add("D8", "Good", "Average", "Mean", "No", "Yes");
            data.Rows.Add("D9", "Average", "Average", "Good", "Yes", "No");
            data.Rows.Add("D10", "Average", "Average", "Good", "No", "No");
            data.Rows.Add("D11", "Bad", "Average", "Good", "Yes", "No");
            data.Rows.Add("D12", "Bad", "Average", "Good", "No", "No");
            data.Rows.Add("D13", "Good", "Bad", "Mean", "Yes", "Yes");
            data.Rows.Add("D14", "Good", "Bad", "Mean", "No", "Yes");
            data.Rows.Add("D15", "Average", "Bad", "Good", "Yes", "No");
            data.Rows.Add("D16", "Average", "Bad", "Good", "No", "No");
            data.Rows.Add("D17", "Bad", "Bad", "Good", "Yes", "No");
            data.Rows.Add("D18", "Bad", "Bad", "Good", "No", "No");
            data.Rows.Add("D19", "Good", "Good", "Good", "Yes", "Yes");
            data.Rows.Add("D20", "Good", "Good", "Good", "No", "Yes");


            // Create a new codification codebook to
            // convert strings into integer symbols
            Codification codebook = new Codification(data);

            DecisionVariable[] attributes =
            {
                new DecisionVariable("Pay", 3), 
                new DecisionVariable("Benefits", 3), 
                new DecisionVariable("Culture", 3), 
                new DecisionVariable("WorkFromHome", 2)  
            };

            int outputValues = 2; // 2 possible output values: yes or no
            DecisionTree tree = new DecisionTree(attributes, outputValues);
            ID3Learning id3 = new ID3Learning(tree);

#pragma warning disable CS0618 // Type or member is obsolete
            // Translate our training data into integer symbols using our codebook:
            DataTable symbols = codebook.Apply(data);
            int[][] inputs = symbols.ToArray<int>("Pay", "Benefits", $"Culture", "WorkFromHome");
            int[] outputs = symbols.ToIntArray("ShouldITakeJob").GetColumn(0);

            // Learn the training instances!
            id3.Run(inputs, outputs);


            int[] query = codebook.Translate("D19", "Good", "Good", "Good", "Yes");
            int output = tree.Compute(query);
            string answer = codebook.Translate("ShouldITakeJob", output); // answer will be "Yes".

#pragma warning restore CS0618 // Type or member is obsolete

            Console.WriteLine("Answer is: " + answer);
            Console.ReadKey();
        }
    }
}
