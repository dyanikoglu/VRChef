using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RecipeModule
{
    public class Chop : Action
    {
        private int requiredPieceCount;
        private PieceVolumeSize pieceVolumeSize;
        private float maxPieceVolume;

        public enum PieceVolumeSize
        {
            Small, Middle, Big
        }

        public Chop()
        {
            this.actionType = ActionType.Chop;
            this.requiredPieceCount = 0;
        }

        public Chop(int requiredPieceCount, PieceVolumeSize pieceVolumeSize, InvolvedFood foodToBeChopped)
        {
            this.actionType = ActionType.Chop;
            this.requiredPieceCount = requiredPieceCount;
            this.pieceVolumeSize = pieceVolumeSize;
            this.involvedFoods.Add(foodToBeChopped);
            this.maxPieceVolume = CalculateMaxPieceVolume(foodToBeChopped.GetPrefab());
        }

        /*
         * Calculates max volume of a piece by PieceVolumeSize
         * PieceVolumeSize.Small -> %15 of initial mesh
         * PieceVolumeSize.Middle -> %30 of initial mesh
         * PieceVolumeSize.Big -> %60 of initial mesh
         */
        private float CalculateMaxPieceVolume(GameObject o)
        {
            float volume = o.GetComponent<Renderer>().bounds.size.x * o.GetComponent<Renderer>().bounds.size.y * o.GetComponent<Renderer>().bounds.size.z;
            float maxPieceVolume;

            switch (pieceVolumeSize)
            {
                case PieceVolumeSize.Small:
                    maxPieceVolume = (volume / 100) * 20;
                    break;
                case PieceVolumeSize.Middle:
                    maxPieceVolume = (volume / 100) * 30;
                    break;
                case PieceVolumeSize.Big:
                    maxPieceVolume = (volume / 100) * 40;
                    break;
                default:
                    maxPieceVolume = volume;
                    break;
            }

            return maxPieceVolume;
        }

        #region Mutators
        public float GetMaxPieceVolume()
        {
            return maxPieceVolume;
        }

        public void SetMaxPieceVolume(float maxPieceVolume)
        {
            this.maxPieceVolume = maxPieceVolume;
        }

        public int GetRequiredPieceCount()
        {
            return requiredPieceCount;
        }

        public void SetRequiredPieceCount(int requiredPieceCount)
        {
            this.requiredPieceCount = requiredPieceCount;
        }

        public PieceVolumeSize GetPieceVolumeSize()
        {
            return pieceVolumeSize;
        }

        public void SetPieceVolumeSize(PieceVolumeSize pieceVolumeSize)
        {
            this.pieceVolumeSize = pieceVolumeSize;
        }
        #endregion
    }
}
