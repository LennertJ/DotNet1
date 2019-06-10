using Microsoft.Win32;
using System;
using System.IO;

namespace conwayGameOfLife
{
    internal class Patern : Model
    {

        private int arraySize;
        public bool[,] LivingCells { get; set; }//prop {tab} {tab}
        public bool[,] PreviousGen { get; set; }
        public string FileName { get; set; }
        public string Name { get; set; }

        public Patern(int arraySize, string SaveFile = "", string imageFile = "",string name="") : base()
        {
            this.FileName = imageFile;
            this.arraySize = arraySize;
            this.Name = name;
            LivingCells = new bool[arraySize, arraySize];
            PreviousGen = new bool[arraySize, arraySize];
            if (SaveFile != "") { loadCells(SaveFile); }
        }

        private void loadCells(string SaveFile)
        {
            string savedField = "";
            StreamReader inputstream;
            inputstream = File.OpenText(SaveFile);
            savedField = inputstream.ReadToEnd();

            string[] savedRow = savedField.Split('\n');
            for (int i = 0; i < savedRow.Length; i++)
            {
                for (int j = 0; j < savedRow[i].Length; j++)
                {
                    if (savedRow[i][j] == '1')
                    {
                        LivingCells[i, j] = true;
                    }
                    else if (savedRow[i][j] == '0')
                    {
                        LivingCells[i, j] = false;
                    }
                }
            }
        }
        public bool[,] checkCell()
        {
            int livingCount;
            bool[,] nextGen = new bool[arraySize, arraySize];
            for (int rowIndex = 0; rowIndex < LivingCells.GetLength(0); rowIndex++)
            {
                for (int collIndex = 0; collIndex < LivingCells.GetLength(1); collIndex++)
                {
                    livingCount = 0;
                    for (int row = -1; row <= 1; row++)
                    {
                        for (int coll = -1; coll <= 1; coll++)
                        {
                            if (Mod((rowIndex + row), LivingCells.GetLength(0)) == (rowIndex + row) &&
                                Mod((collIndex + coll), LivingCells.GetLength(1)) == (collIndex + coll))
                            {
                                if (LivingCells[rowIndex + row, collIndex + coll])
                                {
                                    livingCount++;
                                }
                            }
                        }
                    }
                    nextGen = GenerateNextGen(rowIndex, collIndex, livingCount, nextGen);
                }
            }
            return nextGen;
        }

        private bool[,] GenerateNextGen(int rowIndex, int collIndex, int livingCount, bool[,] nextGen)
        {
            if (LivingCells[rowIndex, collIndex])
            {
                if (livingCount == 3 || livingCount == 4)
                {
                    nextGen[rowIndex, collIndex] = true;
                }
                else
                {
                    nextGen[rowIndex, collIndex] = false;
                }
            }
            else
            {
                if (livingCount == 3)
                {
                    nextGen[rowIndex, collIndex] = true;
                }
                else
                {
                    nextGen[rowIndex, collIndex] = false;
                }
            }
            return nextGen;
        }
    }
}