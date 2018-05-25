using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using numl.Model;

namespace Baseball
{
    using numl;
    using numl.Data;
    using numl.Supervised;

    class Program
    {
   

        static void Main(string[] args)
        {
            Baseball[] data = Baseball.GetData();
            var d = Descriptor.Create<Baseball>();
            var g = new DecisionTreeGenerator(d);
            g.SetHint(false);
            var model = Learner.Learn(data, 0.80, 1000, g);

            Baseball b = new Baseball
            {
                Outlook = Outlook.Overcast,
                Temperature = Temperature.Cool,
                Windy = true
            };

            model.Generator.Generate(d, data);
            Baseball ball = model.Model.Predict(b);
            Console.Write("We should play? " + (ball.Play ? "Yes" : "No"));
            Console.ReadKey();

        }
    }



    public class Baseball
    {
        [Feature]
        public Outlook Outlook { get; set; }
        [Feature]
        public Temperature Temperature { get; set; }
        [Feature]
        public bool Windy { get; set; }
        [Label]
        public bool Play { get; set; }

        public static Baseball[] GetData()
        {
            return new Baseball[]  
            {
                new Baseball { Play = true, Outlook=Outlook.Sunny, Temperature = Temperature.Cool, Windy=true},
                new Baseball { Play = false, Outlook=Outlook.Sunny, Temperature = Temperature.Hot, Windy=true},
                new Baseball { Play = false, Outlook=Outlook.Sunny, Temperature = Temperature.Hot, Windy=false},
                new Baseball { Play = true, Outlook=Outlook.Overcast, Temperature = Temperature.Cool, Windy=true},
                new Baseball { Play = true, Outlook=Outlook.Overcast, Temperature = Temperature.Hot, Windy= false},
                new Baseball { Play = true, Outlook=Outlook.Overcast, Temperature = Temperature.Cool, Windy=false},
                new Baseball { Play = false, Outlook=Outlook.Rainy, Temperature = Temperature.Cool, Windy=true},
                new Baseball { Play = true, Outlook=Outlook.Rainy, Temperature = Temperature.Cool, Windy=false}
            };
        }
    }
}
