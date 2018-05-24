using System;
using System.Collections.Generic;
using System.Linq;

namespace TowerOfHanoi
{
    class StateGenerator
    {
        private string A, B, C;
        string currMove;
        int bIndex, cIndex;
        List<int> topDisks;

        private int _StatesMaxCount;

        public StateGenerator() { }

        public List<string> GenerateStates(int numberOfDisks)
        {
            List<string> _States = new List<string>();
            topDisks = new List<int>();

            string A = "1";

            for (int a = 2; a <= numberOfDisks; a++)
            {
                A += "-" + a.ToString();
            }

            _StatesMaxCount = Convert.ToInt32(Math.Pow(3, numberOfDisks));

            GetNextStates(_States, string.Format("A{0}B0C0", A), true);

            return _States;
        }

        public void StateActionMapping(double[, ,] R, List<string> _States, int dim, int FinalStateIndex)
        {
            Dictionary<int, string> dctStates = new Dictionary<int, string>();
            List<string> nextStepStates = new List<string>();
            List<int> nextStepIndeces = new List<int>();

            for (int i = 0; i < _States.Count; i++)
                dctStates.Add(i, _States[i]);

            for (int i = 0; i < dctStates.Count; i++)
            {
                GetNextStates(nextStepStates, dctStates[i], false);

                for (int j = 0; j < nextStepStates.Count; j++)
                {
                    nextStepIndeces.Add(dctStates.FirstOrDefault(state => state.Value == nextStepStates[j]).Key);
                }

                for (int j = 0; j < nextStepIndeces.Count; j++)
                {
                    R[i, j, 1] = nextStepIndeces[j];
                    R[i, j, 0] = nextStepIndeces[j] == FinalStateIndex ? 100 : 0;
                }

                nextStepStates.Clear();
                nextStepIndeces.Clear();
            }

            R[FinalStateIndex, 2, 1] = FinalStateIndex;
            R[FinalStateIndex, 2, 0] = 100;
        }

        private void GetNextStates(List<string> States, string state, bool recursive)
        {
            // Once all possible states are added, quit recursion (this will save us some time)
            if (States.Count == _StatesMaxCount)
                return;

            List<string> availableMoves = new List<string>();

            topDisks.Clear();
            availableMoves.Clear();

            bIndex = state.IndexOf('B');
            cIndex = state.IndexOf('C');

            A = state.Substring(1, bIndex - 1);
            B = state.Substring(bIndex + 1, cIndex - bIndex - 1);
            C = state.Substring(cIndex + 1, state.Length - cIndex - 1);

            topDisks.Add(A.IndexOf('-') == -1 ? Convert.ToInt32(A) : Convert.ToInt32(A.Substring(0, 1)));

            topDisks.Add(B.IndexOf('-') == -1 ? Convert.ToInt32(B) : Convert.ToInt32(B.Substring(0, 1)));

            topDisks.Add(C.IndexOf('-') == -1 ? Convert.ToInt32(C) : Convert.ToInt32(C.Substring(0, 1)));

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (topDisks[i] == 0 || topDisks[i] == topDisks[j]) continue;
                    if (topDisks[i] < topDisks[j] || topDisks[j] == 0)
                    {
                        availableMoves.Add($"{Convert.ToChar(i + 65)}{Convert.ToChar(j + 65)}");
                    }
                }
                if (availableMoves.Count == 3) 
                    break;
            }

            foreach (string move in availableMoves)
            {
                bIndex = state.IndexOf('B');
                cIndex = state.IndexOf('C');

                A = state.Substring(1, bIndex - 1);
                B = state.Substring(bIndex + 1, cIndex - bIndex - 1);
                C = state.Substring(cIndex + 1, state.Length - cIndex - 1);

                char sourceRod = move[0];
                char destRod = move[1];

                char notChangedRod = 'B';

                string format = "";

                int diff = destRod - sourceRod;

                switch (Math.Abs(diff))
                {
                    case 2:
                        notChangedRod = 'B';
                        format = sourceRod == 'A' ? "A{0}B{2}C{1}" : "A{1}B{2}C{0}";
                        break;
                    case 1:
                        switch (destRod)
                        {
                            case 'B' when sourceRod == 'A':
                                notChangedRod = 'C';
                                format = "A{0}B{1}C{2}";
                                break;
                            case 'B' when sourceRod == 'C':
                                notChangedRod = 'A';
                                format = "A{2}B{1}C{0}";
                                break;
                            case 'C':
                                notChangedRod = 'A';
                                format = "A{2}B{0}C{1}";
                                break;
                            default:
                                notChangedRod = 'C';
                                format = "A{1}B{0}C{2}";
                                break;
                        }
                        break;
                }

                string notChangedRodContent = RodContent(notChangedRod);
                string srcRodContent = RodContent(sourceRod);
                string destRodContent = RodContent(destRod);

                destRodContent = destRodContent == "0" ? srcRodContent.Substring(0, 1) : $"{srcRodContent.Substring(0, 1)}-{destRodContent}";

                srcRodContent = srcRodContent.IndexOf('-') > -1 ? srcRodContent.Substring(2, srcRodContent.Length - 2) : "0";

                currMove = string.Format(format, srcRodContent, destRodContent, notChangedRodContent);
                if (!States.Contains(currMove))
                {
                    States.Add(currMove);
                    if (recursive)
                        GetNextStates(States, currMove, true);
                }
            }
        }

        private string RodContent(char rod)
        {
            switch (rod)
            {
                case 'A':
                    return A;
                case 'B':
                    return B;
                case 'C':
                    return C;
                default:
                    return "";
            }
        }
    }
}
