using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp14
{
    class Program
    {

        public static double a = 0, b = 0.4, c = 0, d = Math.PI / 25;
        public static int n1 = 4, n2 = 1;
        public static double h1, h2;
        public static double A1 = 1;
        public static double R = 0.1;
        public static double A2 = 0.1;
        public static double k1 = 0;
        public static double k2 = 10;
        public static double P = 0;
        public static double v = 0.3, h = 0.005, E = 80.1 * 1000000000;
        public static int nodecount;

        public static double[,] matrix;

        public static int[,] elements;

        public static int[] nodes;

        public static double[,] check;

        public static double[,] Global;

        public static double[,] GlobalB;

        public static double[,] G = new double[9, 2];

        public static double[,] B = new double[11, 11];

        public static double[,] EM = new double[11, 11];

        public static double[,] M = new double[48, 48];

        public static double[,,] MM = new double[20, 48, 48];//20!!!!!!!!!!!!

        public static double[] H = new double[3];
        public static double[] Barray;

        public static void GetH()
        {
            H[0] = 0.555555;
            H[1] = 0.888888;
            H[2] = 0.555555;
        }


        public static void GetG()
        {
            G[0, 0] = -0.77459;
            G[0, 1] = -0.77459;
            G[1, 0] = 0;
            G[1, 1] = -0.77459;
            G[2, 0] = 0.77459;
            G[2, 1] = -0.77459;
            G[3, 0] = -0.77459;
            G[3, 1] = 0;
            G[4, 0] = 0;
            G[4, 1] = 0;
            G[5, 0] = 0.77459;
            G[5, 1] = 0;
            G[6, 0] = -0.77459;
            G[6, 1] = 0.77459;
            G[7, 0] = 0;
            G[7, 1] = 0.77459;
            G[8, 0] = 0.77459;
            G[8, 1] = 0.77459;
        }

        public static void GetB(double v, double h, double E)
        {
            for (int i = 0; i < 11; i++)
            {
                for (int j = 0; j < 11; j++)
                {
                    if (i < 3 && i != j && j < 3)
                    {
                        B[i, j] = (v * E * h) / ((1 + v) * (1 - 2 * v));
                    }
                    else if (i < 3 && j == i)
                    {
                        B[i, j] = (E * (1 - v) * h) / ((1 + v) * (1 - 2 * v));
                    }
                    else if ((i > 2 && i < 6) && i == j)
                    {
                        B[i, j] = (E * h) / ((1 + v));
                    }
                    else if ((i == 6 && j == 7) || (j == 6 && i == 7))
                    {
                        B[i, j] = (E * Math.Pow(h, 3) * v) / (12 * (1 + v) * (1 - 2 * v));
                    }
                    else if ((i == 6 || i == 7) && j == i)
                    {
                        B[i, j] = (E * Math.Pow(h, 3) * (1 - v)) / (12 * (1 + v) * (1 - 2 * v));
                    }
                    else if (i > 7 && j == i)
                    {
                        B[i, j] = (E * Math.Pow(h, 3)) / (12 * (1 + v));
                    }
                }
            }

            //for (int i = 0; i < 11; i++)
            //{
            //    for (int j = 0; j < 11; j++)
            //    {
            //        Console.Write(B[i, j] + " ");
            //    }
            //    Console.WriteLine();
            //}

        }

        public static void GetE()
        {
            for (int i = 0; i < 11; i++)
            {
                for (int j = 0; j < 11; j++)
                {
                    if (i == j)
                    {
                        if (i == 0 || i == 1 || i == 2 || i == 6 || i == 7)
                            EM[i, i] = 1;
                        else
                            EM[i, i] = 2;
                    }
                    else
                    {
                        EM[i, j] = 0;
                    }
                }
            }
        }

        public static double[,] Matrix(double a, double b, double c, double d, int n1, int n2)
        {
            h1 = (b - a) / n1;
            h2 = (d - c) / n2;
            double[,] matrix = new double[(n2 * (3 * n1 + 2) + 2 * n1 + 1), 2];
            nodecount = n2 * (3 * n1 + 2) + 2 * n1 + 1;
            nodes = new int[n2 * (3 * n1 + 2) + 2 * n1 + 1];
            //Console.WriteLine(a + " " + b + " " + c + " " + d);
            if (n1 < n2)
            {
                int k = 0;
                int kek = 0;
                for (double y = c; y <= d + 0.01; y += h2 / 2)
                {
                    if (y > d - 0.01 && y < d + 0.1)
                        y = d;
                    if (kek % 2 == 0)
                    {
                        for (double x = a; x <= b + 0.01; x += h1 / 2)
                        {
                            if (x > b - 0.01 && x < b + 0.1)
                                x = b;
                            matrix[k, 0] = x;
                            matrix[k, 1] = y;
                            if (y == c && x == a)
                            {
                                nodes[k] = 3;
                            }
                            else if (y == c && x == b)
                            {
                                nodes[k] = 8;
                            }
                            else if (y == d && x == b)
                            {
                                nodes[k] = 11;
                            }
                            else if (y == d && x == a)
                            {
                                nodes[k] = 6;
                            }
                            else if (y == c && x != a)
                            {
                                nodes[k] = 1;
                            }
                            else if (y != c && x == a)
                            {
                                nodes[k] = 2;
                            }
                            else if (y != c && x == b)
                            {
                                nodes[k] = 7;
                            }
                            else if (y == d && x != a)
                            {
                                nodes[k] = 4;
                            }
                            else if (true)
                            {
                                nodes[k] = 0;
                            }

                            k++;
                        }
                    }
                    else
                    {
                        for (double x = a; x <= b + 0.01; x += h1)
                        {
                            if (x > b - 0.01 && x < b + 0.1)
                                x = b;
                            matrix[k, 0] = x;
                            matrix[k, 1] = y;
                            if (y == c && x == a)
                            {
                                nodes[k] = 3;
                            }
                            else if (y == c && x == b)
                            {
                                nodes[k] = 8;
                            }
                            else if (y == d && x == b)
                            {
                                nodes[k] = 11;
                            }
                            else if (y == d && x == a)
                            {
                                nodes[k] = 6;
                            }
                            else if (y == c && x != a)
                            {
                                nodes[k] = 1;
                            }
                            else if (y != c && x == a)
                            {
                                nodes[k] = 2;
                            }
                            else if (y != c && x == b)
                            {
                                nodes[k] = 7;
                            }
                            else if (y == d && x != a)
                            {
                                nodes[k] = 4;
                            }
                            else
                            {
                                nodes[k] = 0;
                            }
                            k++;
                        }
                    }
                    kek++;
                }

            }
            else
            {
                int k = 0;
                int kek = 0;
                for (double x = a; x <= b + 0.01; x += h1 / 2)
                {
                    if (x > b - 0.01 && x < b + 0.1)
                        x = b;
                    if (kek % 2 == 0)
                    {
                        for (double y = c; y <= d + 0.01; y += h2 / 2)
                        {
                            if (y > d - 0.01 && y < d + 0.1)
                                y = d;
                            matrix[k, 0] = x;
                            matrix[k, 1] = y;
                            if (y == d && x == a)
                            {
                                nodes[k] = 3;
                            }
                            else if (y == c && x == a)
                            {
                                nodes[k] = 8;
                            }
                            else if (y == c && x == b)
                            {
                                nodes[k] = 11;
                            }
                            else if (y == d && x == b)
                            {
                                nodes[k] = 6;
                            }
                            else if (y != c && x == a)
                            {
                                nodes[k] = 1;
                            }
                            else if (y == d && x != a)
                            {
                                nodes[k] = 2;
                            }
                            else if (y == c && x != a)
                            {
                                nodes[k] = 7;
                            }
                            else if (y != c && x == b)
                            {
                                nodes[k] = 4;
                            }
                            else
                            {
                                nodes[k] = 0;
                            }
                            k++;
                        }
                    }
                    else
                    {
                        for (double y = c; y <= d + 0.01; y += h2)
                        {
                            if (y > d - 0.01 && y < d + 0.1)
                                y = d;
                            matrix[k, 0] = x;
                            matrix[k, 1] = y;
                            if (y == d && x == a)
                            {
                                nodes[k] = 3;
                            }
                            else if (y == c && x == a)
                            {
                                nodes[k] = 8;
                            }
                            else if (y == c && x == b)
                            {
                                nodes[k] = 11;
                            }
                            else if (y == d && x == b)
                            {
                                nodes[k] = 6;
                            }
                            else if (y != c && x == a)
                            {
                                nodes[k] = 1;
                            }
                            else if (y == d && x != a)
                            {
                                nodes[k] = 2;
                            }
                            else if (y == c && x != a)
                            {
                                nodes[k] = 7;
                            }
                            else if (y != c && x == b)
                            {
                                nodes[k] = 4;
                            }
                            else
                            {
                                nodes[k] = 0;
                            }
                            k++;
                        }
                    }
                    kek++;
                }

            }
            return matrix;
        }

        public static int[,] MatrixElements(int n1, int n2)
        {
            elements = new int[n1 * n2, 8];
            int minN = Math.Min(n1, n2);
            int pos = 0;
            int it = 0;
            if (n1 < n2)
            {
                for (int i = 0; i < n1 * n2; i++)
                {
                    pos = 0;
                    it = i / minN;
                    if (it > 0)
                    {
                        pos = (minN * 2 + 1 + minN + 1) * it;
                    }
                    pos += (i % (minN)) * 2;
                    //        cout << i << " ";
                    elements[i, 0] = pos;//1
                    elements[i, 1] = pos + 1;//2
                    elements[i, 2] = pos + 2;//3

                    pos = (minN * 2 + 1 + minN + 1) * it;
                    pos += minN * 2 + 1;
                    pos += i % minN;

                    elements[i, 7] = pos;//4
                    elements[i, 3] = pos + 1;//5

                    pos = (minN * 2 + 1 + minN + 1) * (it + 1);
                    pos += ((i) % (minN)) * 2;
                    elements[i, 6] = pos;
                    elements[i, 5] = pos + 1;
                    elements[i, 4] = pos + 2;

                }
            }
            else
            {
                for (int i = 0; i < n1 * n2; i++)
                {
                    pos = 0;
                    it = i / minN;
                    if (it > 0)
                    {
                        pos = (minN * 2 + 1 + minN + 1) * it;
                    }
                    pos += (i % (minN)) * 2;
                    //        cout << i << " ";
                    elements[i, 0] = pos;//1
                    elements[i, 7] = pos + 1;//8
                    elements[i, 6] = pos + 2;//7

                    pos = (minN * 2 + 1 + minN + 1) * it;
                    pos += minN * 2 + 1;
                    pos += i % minN;

                    elements[i, 1] = pos;//2
                    elements[i, 5] = pos + 1;//5

                    pos = (minN * 2 + 1 + minN + 1) * (it + 1);
                    pos += ((i) % (minN)) * 2;
                    elements[i, 2] = pos;
                    elements[i, 3] = pos + 1;
                    elements[i, 4] = pos + 2;

                }
            }

            return elements;
        }

        public static double FidE(int i, int d, double x, double y)
        {
            switch (i)
            {
                case 1:
                    if (d == 1)
                    {
                        return -0.25 * ((y - 1) * (y + 2 * x));
                    }
                    else
                    {
                        return -0.25 * (((x - 1) * (2 * y + x)));
                    }
                case 2:
                    if (d == 1)
                    {
                        return (x) * (y - 1);
                    }
                    else
                    {
                        return (0.5) * (Math.Pow(x, 2) - 1);
                    }
                case 3:
                    if (d == 1)
                    {
                        return 0.25 * ((y - 1) * (y - 2 * x));
                    }
                    else
                    {
                        return 0.25 * ((x + 1) * (2 * y - x));
                    }
                case 4:
                    if (d == 1)
                    {
                        return -0.5 * (Math.Pow(y, 2) - 1);
                    }
                    else
                    {
                        return (-1 * x) * y - y;
                    }
                case 5:
                    if (d == 1)
                    {
                        return 0.25 * ((y + 1) * (y + 2 * x));
                    }
                    else
                    {
                        return 0.25 * ((x + 1) * (2 * y + x));
                    }
                case 6:
                    if (d == 1)
                    {
                        return (-1 * x) * y - x;
                    }
                    else
                    {
                        return (-0.5) * (Math.Pow(x, 2) - 1);
                    }
                case 7:
                    if (d == 1)
                    {
                        return -0.25 * ((y + 1) * (y - 2 * x));
                    }
                    else
                    {
                        return -0.25 * ((x - 1) * (2 * y - x));
                    }
                case 8:
                    if (d == 1)
                    {
                        return 0.5 * (Math.Pow(y, 2) - 1);
                    }
                    else
                    {
                        return (x * y) - y;
                    }
                default:
                    Console.WriteLine("SWITCH OUT OF CASE EXCEPTION");
                    return 0;
            }

        }

        public static double Fi(int pos, double x1, double x2)
        {
            switch (pos)
            {
                case 1:
                    return -0.25 * (1.0 - x1) * (1.0 - x2) * (1.0 + x1 + x2);
                case 2:
                    return 0.5 * (1.0 - x1 * x1) * (1.0 - x2);
                case 3:
                    return -0.25 * (1.0 + x1) * (1.0 - x2) * (1.0 - x1 + x2);
                case 4:
                    return 0.5 * (1.0 - x2 * x2) * (1.0 + x1);
                case 5:
                    return -0.25 * (1.0 + x1) * (1.0 + x2) * (1.0 - x1 - x2);
                case 6:
                    return 0.5 * (1.0 - x1 * x1) * (1.0 + x2);
                case 7:
                    return -0.25 * (1.0 - x1) * (1.0 + x2) * (1 + x1 - x2);
                case 8:
                    return 0.5 * (1.0 - x2 * x2) * (1 - x1);
                default:
                    Console.WriteLine("SWITCH OUT OF CASE EXCEPTION");
                    return 0;
            }
        }

        public static double DetJe(int Gp, int element)
        {
            double result = 0;
            for (int i = 1; i <= 8; i++)
            {
                for (int j = 1; j <= 8; j++)
                {
                    if (i != j)
                    {
                        result += ((FidE(i, 1, G[Gp, 0], G[Gp, 1]) * FidE(j, 2, G[Gp, 0], G[Gp, 1])) *
                            ((matrix[elements[element, i - 1], 0] * matrix[elements[element, j - 1], 1]) -
                            (matrix[elements[element, j - 1], 0] * matrix[elements[element, i - 1], 1])));
                    }
                }
            }

            return result;
        }

        public static double[,] Je_1(double DetJe, int element, int Gp)
        {
            double[,] result = new double[2, 2];

            for (int i = 0; i < 8; i++)
            {
                result[0, 0] += FidE(i + 1, 2, G[Gp, 0], G[Gp, 1]) * matrix[elements[element, i], 1];
                result[1, 1] += FidE(i + 1, 1, G[Gp, 0], G[Gp, 1]) * matrix[elements[element, i], 0];
                result[0, 1] += -1 * (FidE(i + 1, 1, G[Gp, 0], G[Gp, 1]) * matrix[elements[element, i], 1]);
                result[1, 0] += -1 * (FidE(i + 1, 2, G[Gp, 0], G[Gp, 1]) * matrix[elements[element, i], 0]);
            }
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    result[i, j] *= 1.0 / DetJe;
                }
            }
            return result;
        }

        public static double[] Overpass(double[,] Je_1, int FI, int Gp)
        {
            double[] temp = new double[2];
            temp[0] = FidE(FI, 1, G[Gp, 0], G[Gp, 1]);
            temp[1] = FidE(FI, 2, G[Gp, 0], G[Gp, 1]);

            double[] result = new double[2];
            result[0] = Je_1[0, 0] * temp[0] + Je_1[0, 1] * temp[1];
            result[1] = Je_1[1, 0] * temp[0] + Je_1[1, 1] * temp[1];

            return result;
        }

        public static double[,] CLT(double[] FIdea, double Fi)
        {
            double[,] result = new double[6, 11];
            result[0, 0] = FIdea[0] / A1;
            result[1, 1] = FIdea[1] / A2;
            result[2, 1] = k2* Fi;
            result[5, 2] = 1.0*Fi;
            result[0, 3] = (A1 / (2 * A2)) * (FIdea[1] / A1);
            result[1, 3] = (A2 / (2 * A1)) * (FIdea[0] / A2);
            result[2, 4] = FIdea[0] / (2 * A1);
            result[3, 4] = 0.5*Fi;
            result[1, 5] = (k2 / -2.0)*Fi;
            result[2, 5] = FIdea[1] / (2 * A2);
            result[4, 5] = 0.5*Fi;
            result[3, 6] = FIdea[0] / A1;
            result[4, 7] = FIdea[1] / A2;
            result[5, 7] = k2*Fi;
            result[1, 8] = 0.5 * ((k2 * FIdea[0]) / A1);
            result[3, 8] = (A1 / (2 * A2)) * (FIdea[1] / A1);
            result[4, 8] = (A2 / (2 * A1)) * (FIdea[0] / A2);
            result[5, 9] = 0.5 * (FIdea[0] / A1);
            result[5, 10] = 0.5 * (FIdea[1] / A2);

            return result;
        }

        public static double[,] CL(double[] FIdea, double Fi)
        {
            double[,] result = new double[11, 6];
            result[0, 0] = FIdea[0] / A1;
            result[1, 1] = FIdea[1] / A2;
            result[1, 2] = k2*Fi;
            result[2, 5] = 1.0* Fi;
            result[3, 0] = (A1 / (2 * A2)) * (FIdea[1] / A1);
            result[3, 1] = (A2 / (2 * A1)) * (FIdea[0] / A2);
            result[4, 2] = FIdea[0] / (2 * A1);
            result[4, 3] = 0.5*Fi;
            result[5, 1] = (k2 / -2.0)*Fi;
            result[5, 2] = FIdea[1] / (2 * A2);
            result[5, 4] = 0.5*Fi;
            result[6, 3] = FIdea[0] / A1;
            result[7, 4] = FIdea[1] / A2;
            result[7, 5] = k2*Fi;
            result[8, 1] = 0.5 * ((k2 * FIdea[0]) / A1);
            result[8, 3] = (A1 / (2 * A2)) * (FIdea[1] / A1);
            result[8, 4] = (A2 / (2 * A1)) * (FIdea[0] / A2);
            result[9, 5] = 0.5 * (FIdea[0] / A1);
            result[10, 5] = 0.5 * (FIdea[1] / A2);

            return result;
        }

        public static double[,] CLTBCL(double[,] CLT, double[,] CL, int Gpos, int elem)
        {
            double[,] result = new double[6, 6];
            double[,] temp = new double[6, 11];
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 11; j++)
                {
                    temp[i, j] = 0;
                    for (int l = 0; l < 11; l++)
                    {
                        temp[i, j] += CLT[i, l] * B[l, j];
                    }
                }
            }


            double[,] temp1 = new double[6, 11];
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 11; j++)
                {
                    temp1[i, j] = 0;
                    for (int l = 0; l < 11; l++)
                    {
                        temp1[i, j] += temp[i, l] * EM[l, j];
                    }
                }
            }


            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    result[i, j] = 0;
                    for (int l = 0; l < 11; l++)
                    {
                        result[i, j] += temp1[i, l] * CL[l, j];
                    }
                }
            }

            double EX = 0;
            switch (Gpos)
            {
                case 0:
                    EX = H[0] * H[0] * DetJe(Gpos, elem) * A1 * A2;
                    break;
                case 1:
                    EX = H[1] * H[0] * DetJe(Gpos, elem) * A1 * A2;
                    break;
                case 2:
                    EX = H[2] * H[0] * DetJe(Gpos, elem) * A1 * A2;
                    break;
                case 3:
                    EX = H[0] * H[1] * DetJe(Gpos, elem) * A1 * A2;
                    break;
                case 4:
                    EX = H[1] * H[1] * DetJe(Gpos, elem) * A1 * A2;
                    break;
                case 5:
                    EX = H[2] * H[1] * DetJe(Gpos, elem) * A1 * A2;
                    break;
                case 6:
                    EX = H[0] * H[2] * DetJe(Gpos, elem) * A1 * A2;
                    break;
                case 7:
                    EX = H[1] * H[2] * DetJe(Gpos, elem) * A1 * A2;
                    break;
                case 8:
                    EX = H[2] * H[2] * DetJe(Gpos, elem) * A1 * A2;
                    break;
            }
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    result[i, j] *= EX;
                }
            }

            return result;
        }

        public static void M48x48(double[,] CLTBCL, int Fi1, int Fi2)
        {
            int i1 = 0;
            for (int i = (Fi1 - 1) * 6; i < Fi1 * 6; i++)
            {
                int j1 = 0;
                for (int j = (Fi2 - 1) * 6; j < Fi2 * 6; j++)
                {
                    M[i, j] += CLTBCL[i1, j1];
                    j1++;
                }
                i1++;
            }
        }

        public static void doAllInTask2(int N)
        {
            GlobalB = new double[N,48];
            for (int i = 0; i < N; i++)
            {
                double[] temp = new double[48];
                for (int j = 0; j < 9; j++)
                {
                    double[,] localB = new double[6, 48];
                    double[,] over = Je_1(DetJe(j, i), i, j);
                    for (int Fi1 = 1; Fi1 <= 8; Fi1++)
                    {
                        for (int Fi2 = 1; Fi2 <= 8; Fi2++)
                        {
                            if (Fi2 == 2 && Fi1 == 1 && j == 0 && i == 0)
                            {
                                check = CL(Overpass(over, Fi2, j), Fi(Fi2, G[j, 0], G[j, 1]));
                            }
                            M48x48(CLTBCL(CLT(Overpass(over, Fi1, j), Fi(Fi1, G[j, 0], G[j, 1])),
                                CL(Overpass(over, Fi2, j), Fi(Fi2, G[j, 0], G[j, 1])), j, i),
                                Fi1, Fi2);
                            if (Fi1 == Fi2)
                            {
                                for (int k11 = 0; k11 < 6; k11++)
                                {
                                    for (int k12 = (Fi2-1) * 6; k12 < (Fi1) * 6; k12++)
                                    {
                                        if (k11 == k12%6)
                                        {
                                            localB[k11, k12] = Fi(Fi1, G[j, 0], G[j, 1]);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    localB = Transpose(localB,6,48);


                    double EX = 0;
                    switch (j)
                    {
                        case 0:
                            EX = H[0] * H[0] * DetJe(j, i) * A1 * A2;
                            break;
                        case 1:
                            EX = H[1] * H[0] * DetJe(j,i) * A1 * A2;
                            break;                   
                        case 2:                      
                            EX = H[2] * H[0] * DetJe(j,i) * A1 * A2;
                            break;                   
                        case 3:                      
                            EX = H[0] * H[1] * DetJe(j,i) * A1 * A2;
                            break;                   
                        case 4:                      
                            EX = H[1] * H[1] * DetJe(j,i) * A1 * A2;
                            break;                   
                        case 5:                      
                            EX = H[2] * H[1] * DetJe(j,i) * A1 * A2;
                            break;                   
                        case 6:                      
                            EX = H[0] * H[2] * DetJe(j,i) * A1 * A2;
                            break;                   
                        case 7:                      
                            EX = H[1] * H[2] * DetJe(j,i) * A1 * A2;
                            break;                   
                        case 8:                      
                            EX = H[2] * H[2] * DetJe(j,i) * A1 * A2;
                            break;
                    }

                    double[] array = new double[6] {0,0,-1,0,0,0};

                    for (int mult = 0; mult < 48; mult++)
                    {
                        for (int mult2 = 0; mult2 < 6; mult2++)
                        {
                            temp[mult] += localB[mult, mult2] * array[mult2];
                        }
                    }

                    for (int mult = 0; mult < 48; mult++)
                    {
                        temp[mult] *= EX;
                        GlobalB[i,mult] += temp[mult];

                    }
                    //R48_1
                    //for (int jk = 0; jk < 48; jk++)
                    //{
                    //    Console.Write(GlobalB[0, jk] + " ");
                    //}
                    //Console.WriteLine();

                    //if (i == 0 && j == 0)
                    //{
                    //    Console.WriteLine("48x48 Matrix on the first finite element: ");
                    //    Console.WriteLine();
                    //    for (int l = 0; l < 48; l++)
                    //    {
                    //        for (int l1 = 0; l1 < 48; l1++)
                    //        {
                    //            Console.Write(M[l, l1] + " ");

                    //        }
                    //        Console.WriteLine();
                    //    }
                    //}
                }

                if (i == 3)
                {
                    int dh = 0;
                }
                //Console.WriteLine("Element : " + i );
                for (int t = 0;t<8;t++)
                {
                    //Console.WriteLine(elements[i,t]+ " ("+ matrix[elements[i,t],0] + " , " + matrix[elements[i,t],1] + " )" + " ->" + nodes[elements[i,t]] +  " ");
                    if (nodes[elements[i, t]] == 0)
                        continue;
                    else
                    {
                        if(nodes[elements[i, t]] == 1)
                        {
                            M[t * 6, t * 6] = 1 * Math.Pow(10, 50);
                            M[t*6 + 3, t*6 + 3] = 1 * Math.Pow(10, 50);
                            GlobalB[i, t * 6] = 0;
                            GlobalB[i, t * 6 + 3] = 0;
                        }
                        else if(nodes[elements[i, t]] == 2)
                        {
                            M[t * 6 + 1, t * 6 + 1] = 1 * Math.Pow(10, 50);
                            M[t * 6 + 4, t * 6 + 4] = 1 * Math.Pow(10, 50);
                            GlobalB[i, t * 6 + 1] = 0;
                            GlobalB[i, t * 6 + 4] = 0;
                        }
                        else if(nodes[elements[i, t]] == 3)
                        {
                            M[t * 6, t * 6] = 1 * Math.Pow(10, 50);
                            M[t * 6 + 3, t * 6 + 3] = 1 * Math.Pow(10, 50);
                            M[t * 6 + 1, t * 6 + 1] = 1 * Math.Pow(10, 50);
                            M[t * 6 + 4, t * 6 + 4] = 1 * Math.Pow(10, 50);
                            GlobalB[i, t * 6 + 1] = 0;
                            GlobalB[i, t * 6 + 4] = 0;
                            GlobalB[i, t * 6] = 0;
                            GlobalB[i, t * 6 + 3] = 0;
                        }
                        else if(nodes[elements[i, t]] == 4)
                        {
                            M[t * 6 + 2, t * 6 + 2] = 1 * Math.Pow(10, 50);
                            M[t * 6 + 5, t * 6 + 5] = 1 * Math.Pow(10, 50);
                            GlobalB[i, t * 6 + 2] = 0;
                            GlobalB[i, t * 6 + 5] = 0;

                        }
                        else if(nodes[elements[i, t]] == 6)
                        {
                            M[t * 6 + 1, t * 6 + 1] = 1 * Math.Pow(10, 50);
                            M[t * 6 + 4, t * 6 + 4] = 1 * Math.Pow(10, 50);
                            M[t * 6 + 2, t * 6 + 2] = 1 * Math.Pow(10, 50);
                            M[t * 6 + 5, t * 6 + 5] = 1 * Math.Pow(10, 50);
                            GlobalB[i, t * 6 + 2] = 0;
                            GlobalB[i, t * 6 + 5] = 0;
                            GlobalB[i, t * 6 + 1] = 0;
                            GlobalB[i, t * 6 + 4] = 0;
                        }
                        else if(nodes[elements[i, t]] == 7)
                        {
                            M[t * 6 + 1, t * 6 + 1] = 1 * Math.Pow(10, 50);
                            M[t * 6 + 4, t * 6 + 4] = 1 * Math.Pow(10, 50);
                            GlobalB[i, t * 6 + 1] = 0;
                            GlobalB[i, t * 6 + 4] = 0;
                        }
                        else if(nodes[elements[i, t]] == 11)
                        {
                            M[t * 6 + 1, t * 6 + 1] = 1 * Math.Pow(10, 50);
                            M[t * 6 + 4, t * 6 + 4] = 1 * Math.Pow(10, 50);
                            M[t * 6 + 2, t * 6 + 2] = 1 * Math.Pow(10, 50);
                            M[t * 6 + 5, t * 6 + 5] = 1 * Math.Pow(10, 50);
                            GlobalB[i, t * 6 + 2] = 0;
                            GlobalB[i, t * 6 + 5] = 0;
                            GlobalB[i, t * 6 + 1] = 0;
                            GlobalB[i, t * 6 + 4] = 0;
                        }
                        else if (nodes[elements[i, t]] == 8)
                        {
                            M[t * 6, t * 6] = 1 * Math.Pow(10, 50);
                            M[t * 6 + 3, t * 6 + 3] = 1 * Math.Pow(10, 50);
                            M[t * 6 + 1, t * 6 + 1] = 1 * Math.Pow(10, 50);
                            M[t * 6 + 4, t * 6 + 4] = 1 * Math.Pow(10, 50);
                            GlobalB[i, t * 6 + 1] = 0;
                            GlobalB[i, t * 6 + 4] = 0;
                            GlobalB[i, t * 6] = 0;
                            GlobalB[i, t * 6 + 3] = 0;
                        }
                    }
                }
                //Console.WriteLine(); Console.WriteLine();

                for (int k = 0; k < 48; k++)
                {
                    for (int k3 = 0; k3 < 48; k3++)
                    {
                        MM[i, k, k3] = M[k, k3];
                    }

                }
                for (int k = 0; k < 48; k++)
                {
                    for (int k3 = 0; k3 < 48; k3++)
                    {
                        
                        M[k, k3] = 0;
                    }
                }
            }
            //R48
            //for (int j = 0; j < 48; j++)
            //{
            //    Console.Write(GlobalB[3, j] + " ");
            //}
            //Console.WriteLine();

            //Console.WriteLine("Array 48 x 1");
            //for (int i = 0; i < N; i++)
            //{
            //    for (int j = 0; j < 48; j++)
            //    {
            //        Console.Write(GlobalB[i, j] + " ");
            //    }
            //    Console.WriteLine();
            //}
        }

        public static void setGlobal(double[,] globalb)
        {
            Global = new double[nodecount*6, nodecount* 6];
            Barray = new double[nodecount * 6];
            int j1 = 0, j2 = 0;
            for(int i =0;i<n1*n2;i++)
            {

                for(int t1= 0;t1 < 8;t1++)
                {
                    for (int count = 0; count < 6; count++)
                    {
                        Barray[elements[i, t1] * 6 + count] += globalb[i, t1 * 6 + count];
                    }
                    for (int t2 = 0;t2 < 8;t2++)
                    {
                        for(int iter1 = elements[i, t1]*6;iter1 < (elements[i,t1] + 1)*6;iter1++)
                        {
                            
                            for (int iter2 = elements[i, t2] * 6; iter2 < (elements[i, t2] + 1) * 6; iter2++)
                            {
                                Global[iter1, iter2] += MM[i, t1 * 6 + j1, t2 * 6 + j2];
                                j2++;
                            }
                            j1++;
                            j2 = 0;
                        }
                        j2 = 0;
                        j1 = 0;
                    }
                }
            }
        }

        public static double[,] Transpose(double[,] a, int n, int m)
        {
            double[,] result = new double[m, n];
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    result[i, j] = a[j, i];
                }
            }
            return result;
        }


        static void Main(string[] args)
        {
            GetG();
            GetH();
            GetE();
            GetB(v, h, E);
            matrix = Matrix(a, b, c, d, n1, n2);
            MatrixElements(n1, n2);
            doAllInTask2(n1 * n2);
            setGlobal(GlobalB);
            //Console.WriteLine();
            //Console.WriteLine("DetJe : " + DetJe(0, 0));
            //Console.WriteLine();
            //Console.WriteLine("CL matrix on the first finite element and Gauss node: " );
            //for (int i = 0; i < 11; i++)
            //{
            //    for (int j = 0; j < 6; j++)
            //    {

            //        Console.Write("{0} ", check[i, j]);
            //    }
            //    Console.WriteLine();
            //}
            //Console.WriteLine();
            //Console.WriteLine("48x48 on first finite element:");
            //Console.WriteLine();
            //for (int j = 0; j < 48; j++)
            //{
            //    for (int l = 0; l < 48; l++)
            //    {
            //        Console.Write(MM[0, j, l] + "\t");
            //    }
            //    Console.WriteLine();
            //}
            Console.WriteLine();
            Console.WriteLine("Nodecount x 6 matrix:");
            Console.WriteLine();
            for (int i = 0; i < nodecount * 6; i++)
            {
                for (int j = 0; j < nodecount * 6; j++)
                {
                    Console.Write(Global[i, j] + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine("B_Array print:");
            for (int i = 0; i < nodecount * 6; i++)
            {
                if(i%6 == 0)
                    Console.WriteLine();
                Console.Write(Barray[i] + " ");
            }
            Console.WriteLine();
        }
    }
}
