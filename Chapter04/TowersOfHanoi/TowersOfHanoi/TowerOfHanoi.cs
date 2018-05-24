using System;
using System.Collections.Generic;

namespace TowerOfHanoi
{
    class TowerOfHanoi
    {
        private int numberOfDisks = 3;

        private QLearning _LearningModule;

        public TowerOfHanoi()
        {
            Init();
        }

        public void Init()
        {
            Console.WriteLine("Enter the number of disks: ");
            numberOfDisks = Convert.ToInt32(Console.ReadLine());

            if (_LearningModule == null)
                _LearningModule = new QLearning(this, numberOfDisks);
            else
                _LearningModule.Init(numberOfDisks);
        }
    }
}
