using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Core
{
    //Considers all the actions
    public class AIBrain : MonoBehaviour
    {
        private AIAction _bestAction;
        private NPCUnit _npcUnit;

        //public AIAction bestAction { get => _bestAction; }
        
        // Start is called before the first frame update
        void Start()
        {
            _npcUnit = GetComponent<NPCUnit>();
        }

        public void DecideBestAction(AIAction[] actions, Action<AIAction> onDecided)
        {
            float score = 0f;
            int nextBestActionIndex = 0;

            for (int i = 0; i < actions.Length; i++)
            {
                if (ScoreAction(actions[i]) > score)
                {
                    nextBestActionIndex = i;
                    score = actions[i].score;
                }
            }

            _bestAction = actions[nextBestActionIndex];
            onDecided(_bestAction);
        }
        
        //Get the score of an action
        public float ScoreAction(AIAction action)
        {
            float lumpedScore = 1f;
            for (int i = 0; i < action.considerations.Length; i++)
            {
                float considerationScore = action.considerations[i].ScoreConsideration(_npcUnit);
                lumpedScore *= considerationScore;

                if (lumpedScore == 0)
                {
                    action.score = 0;
                    return action.score;
                }
            }

            //Average Scheme the lumped score
            float averageScore = lumpedScore;
            float modFactor = 1 - (1 / action.considerations.Length);
            float makeupValue = (1 - averageScore) * modFactor;
            action.score = averageScore + (makeupValue * averageScore);
            
            return action.score;
        }
    }
}