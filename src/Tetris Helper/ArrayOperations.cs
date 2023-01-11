using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.AnarchyCollab2022 {
    public static class ArrayOperations {
        //utility, from https://stackoverflow.com/questions/29483660/how-to-transpose-matrix
        public static bool[,] Transpose(bool[,] matrix) {
            int w = matrix.GetLength(0);
            int h = matrix.GetLength(1);

            bool[,] result = new bool[h, w];

            for (int i = 0; i < w; i++) {
                for (int j = 0; j < h; j++) {
                    result[j, i] = matrix[i, j];
                }
            }

            return result;
        }

        public static bool[,] ReverseHorizontal(bool[,] matrix) {
            int w = matrix.GetLength(0);
            int h = matrix.GetLength(1);

            bool[,] result = new bool[h, w];

            for (int i = 0; i < w; i++) {
                for (int j = 0; j < h; j++) {
                    result[j, i] = matrix[j, w - i - 1];
                }
            }

            return result;
        }

        public static bool[,] ReverseVertical(bool[,] matrix) {
            int w = matrix.GetLength(0);
            int h = matrix.GetLength(1);

            bool[,] result = new bool[h, w];

            for (int i = 0; i < w; i++) {
                for (int j = 0; j < h; j++) {
                    result[j, i] = matrix[h - j - 1, i];
                }
            }

            return result;
        }

        public static bool[,] RotateCW(bool[,] matrix) {
            return ReverseHorizontal(Transpose(matrix));
        }

        public static bool[,] RotateCCW(bool[,] matrix) {
            return Transpose(ReverseHorizontal(matrix));
        }

        public static bool[,] Rotate180(bool[,] matrix) {
            return ReverseVertical(ReverseHorizontal(matrix));
        }

        public static bool[,] IntToBool(int[,] matrix) {
            int w = matrix.GetLength(0);
            int h = matrix.GetLength(1);

            bool[,] result = new bool[h, w];

            for (int i = 0; i < w; i++) {
                for (int j = 0; j < h; j++) {
                    //reverse
                    //absolutely no clue why this needs to be done
                    result[i, j] = matrix[j, i] == 1;
                }
            }

            return result;
        }
    }
}
