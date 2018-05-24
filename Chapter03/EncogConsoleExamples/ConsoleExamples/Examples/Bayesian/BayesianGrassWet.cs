using System;
using Encog.ML.Bayesian;
using Encog.ML.Bayesian.Query.Enumeration;
using ConsoleExamples.Examples;

namespace Encog.Examples.Bayesian
{
    public class BayesianGrassWet : IExample
    {
        public static ExampleInfo Info
        {
            get
            {
                var info = new ExampleInfo(
                    typeof(BayesianGrassWet),
                    "bayesian-grass",
                    "The grass is wet due to rain or the sprinkler with Bayesian networks.",
                    "Perform a query of a bayesian network to answer the problem.");
                return info;
            }
        }

        public void Execute(IExampleInterface app)
        {
            BayesianNetwork network = new BayesianNetwork();
            BayesianEvent grass = network.CreateEvent("grass");
            BayesianEvent sprinkler = network.CreateEvent("sprinkler");
            BayesianEvent rain = network.CreateEvent("rain");
            BayesianEvent DidRain = network.CreateEvent("did_rain");
            BayesianEvent SprinklerOn = network.CreateEvent("sprinkler_on");

            network.CreateDependency(rain, DidRain);
            network.CreateDependency(sprinkler, SprinklerOn);
            network.FinalizeStructure();

            // build the truth tales
            rain.Table.AddLine(0.35, true);
            // 80% of the time they were right
            DidRain.Table.AddLine(0.80, false, true);
            // 20% of the time they were wrong
            DidRain.Table.AddLine(0.20, true, true);

            sprinkler.Table.AddLine(.65, true);
            // 60% of the time they were right
            SprinklerOn.Table.AddLine(.40, true, true);
            // 40% of the time they were wrong
            SprinklerOn.Table.AddLine(.60, false, true);

            // validate the network
            network.Validate();
            // display basic stats
            Console.WriteLine(network.ToString());
            Console.WriteLine("Parameter count: " + network.CalculateParameterCount());
            EnumerationQuery query = new EnumerationQuery(network);
            //SamplingQuery query = new SamplingQuery(network);

            query.DefineEventType(SprinklerOn, EventType.Evidence);
            query.DefineEventType(sprinkler, EventType.Outcome);
            query.DefineEventType(DidRain, EventType.Evidence);
            query.DefineEventType(rain, EventType.Outcome);

            query.Execute();
            Console.WriteLine(query.ToString());
        }
    }
}
