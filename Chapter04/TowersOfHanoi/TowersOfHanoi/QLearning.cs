using System;
using System.Linq;
using System.Collections.Generic;

namespace TowerOfHanoi
{
    class QLearning
    {
        private double[, ,] R;
        private double[,] Q;

        private List<string> _States;

        public List<string> States
        {
            get
            {
                return _States;
            }
        }

        private TowerOfHanoi _Puzzle;
        private StateGenerator _Generator;
        private int FinalStateIndex;
        private int _NumberOfDisks;
        private int _StatesMaxCount;

        private List<int> optimalPath = new List<int>();

        public QLearning(TowerOfHanoi puzzle, int numberOfDisks)
        {
            _Puzzle = puzzle;
            _NumberOfDisks = numberOfDisks;

            _Generator = new StateGenerator();

            Init(_NumberOfDisks);
        }

        public void Init(int numberOfDisks)
        {
            _States = _Generator.GenerateStates(numberOfDisks);

            for (int i = 0; i < _States.Count; i++)
            {
                if (_States[i].StartsWith("A0B0"))
                {
                    FinalStateIndex = i;
                    break;
                }
            }

            _StatesMaxCount = Convert.ToInt32(Math.Pow(3, numberOfDisks));

            Learn(_StatesMaxCount);
        }

        private void Learn(int _StatesMaxCount)
        {
            InitQMatrix(_StatesMaxCount);

            InitRMatrix(_StatesMaxCount);

            TrainQMatrix(_StatesMaxCount);

            NormalizeQMatrix(_StatesMaxCount);

            Test(Q, _StatesMaxCount);
        }

        private void InitQMatrix(int _StatesMaxCount)
        {
            Q = new double[_StatesMaxCount, _StatesMaxCount];
        }

        private void InitRMatrix(int _StatesMaxCount)
        {
            R = new double[_StatesMaxCount, 3, 2];

            for (int i = 0; i < _StatesMaxCount; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    R[i, j, 0] = -1;
                    R[i, j, 1] = -1;
                }
            }

            _Generator.StateActionMapping(R, _States, _StatesMaxCount, FinalStateIndex);
        }

        private Dictionary<int, int> pickedActions;

        private void TrainQMatrix(int _StatesMaxCount)
        {
            pickedActions = new Dictionary<int, int>();

            // list of available actions (will be based on R matrix which
            // contains the allowed next actions starting from some state as 0 values in the array)
            List<int> nextActions = new List<int>();

            int counter = 0;
            int rIndex = 0;
            // _StatesMaxCount is the number of all possible states of a puzzle
            // from my experience with this application, 4 times the number
            // of all possible moves has enough episodes to train Q matrix
            while (counter < 3 * _StatesMaxCount)
            {
                var init = Utility.GetRandomNumber(0, _StatesMaxCount);

                do
                {
                    // get available actions
                    nextActions = GetNextActions(_StatesMaxCount, init);

                    // Choose any action out of the available actions randomly
                    if (nextActions != null)
                    {
                        var nextStep = Utility.GetRandomNumber(0, nextActions.Count);
                        nextStep = nextActions[nextStep];

                        // get available actions
                        nextActions = GetNextActions(_StatesMaxCount, nextStep);

                        // set the index of the action to take from this state
                        for (int i = 0; i < 3; i++)
                        {
                            if (R != null && R[init, i, 1] == nextStep)
                                rIndex = i;
                        }

                        // this is the value iteration update rule - discount factor is 0.8
                        Q[init, nextStep] = R[init, rIndex, 0] + 0.8 * Utility.GetMax(Q, nextStep, nextActions);

                        // set the next step as the current step
                        init = nextStep;
                    }
                }
                while (init != FinalStateIndex);

                counter++;
            }
        }

        private List<int> GetNextActions(int _StatesMaxCount, int init)
        {
            List<int> nextActions = new List<int>();

            for (int j = 0; j < 3; j++)
            {
                // if the action i is availabe from the state init
                if (R[init, j, 1] > -1)
                {
                    // add it to the available moves list
                    nextActions.Add(Convert.ToInt32(R[init, j, 1]));
                }
            }

            return nextActions;
        }

        private void NormalizeQMatrix(int _StatesMaxCount)
        {
            double maxQ = (from double d in Q select d).Max();

            for (int i = 0; i < _StatesMaxCount; i++)
            {
                for (int j = 0; j < _StatesMaxCount; j++)
                {
                    Q[i, j] /= maxQ * 100;
                }
            }
        }

        private void Test(double[,] Q, int _StatesMaxCount)
        {
            string strStartingState = "";
            int start = 0;

            do
            {
                optimalPath.Clear();

                Console.WriteLine("Enter the starting state in format A#B#C#. For example, the starting state for 3 disks is A1-2-3B0C0\n(or type \"r\" to reset):");

                string A = "1";

                for (int a = 2; a <= _NumberOfDisks; a++)
                {
                    A += "-" + a.ToString();
                }

                try
                {
                    strStartingState = Console.ReadLine();
                }
                catch
                {
                    strStartingState = string.Format("A{0}B0C0", A);
                }

                if (strStartingState.ToLower().StartsWith("r"))
                    _Puzzle.Init();

                for (int i = 0; i < _StatesMaxCount; i++)
                {
                    if (strStartingState == States[i])
                    {
                        start = i;
                        break;
                    }
                }

                FindOptimalAction(Q, start, _StatesMaxCount);

                string strState = "";

                Console.WriteLine(string.Format("Optimal Solution: {0} Steps", optimalPath.Count - 1));
                for (int i = 0; i < optimalPath.Count; i++)
                {
                    strState = States[optimalPath[i]].Replace("A", "A ").Replace("B", "\t\t\tB ").Replace("C", "\t\t\tC ");
                    Console.WriteLine(strState);
                }
            } while (start >= 0 && start < _StatesMaxCount);
        }

        private void FindOptimalAction(double[,] Q, int start, int _StatesMaxCount)
        {
            optimalPath.Add(start);

            if (start == FinalStateIndex) return;

            List<int> nextActions = GetNextActions(_StatesMaxCount, start);

            int maxQindex = 0;

            if (nextActions.Count > 1)
            {
                maxQindex = nextActions[0];

                if (Q[start, nextActions[1]] > Q[start, nextActions[0]])
                    maxQindex = nextActions[1];

                if (nextActions.Count > 2)
                {
                    if (Q[start, nextActions[2]] > Q[start, nextActions[1]])
                        maxQindex = nextActions[2];
                }
            }

            FindOptimalAction(Q, maxQindex, _StatesMaxCount);
        }
    }
}
