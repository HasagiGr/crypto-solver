﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Crypto
{

    public class Equations
    {
        public static int[] primeNumbers;

        public static List<int> EratSito(int number, int[] primeNumbers)
        {
            var list = new List<int>();
            for (int i = 2; i <= number; i++)
            {
                list.Add(i);
            }
            foreach (var prime in primeNumbers)
            {
                list = list.Where(x => x % prime != 0).ToList();
                list.Add(prime);
            }
            list.Sort();
            return list;
        }

        public static int[] MethodDeFermat(int number)
        {
            var v = 0;
            var n = 0;
            for (int i = (int)Math.Sqrt(number) + 1; i < number; i++)
            {
                var prom = Math.Sqrt(i * i - number);
                if (Math.Floor(prom) - prom == 0 && i * i - prom * prom == number)
                {
                    n = i;
                    v = (int)prom;
                    break;
                }
            }
            return new int[] { n - v, n + v };
        }

        public static int[] TestDeFermat(int number, int witness = 2)
        {
            var testRes = Equations.Power(witness, number - 1, number);
            if (testRes== 1)
                return new int[] { 1, witness };
            return new int[] { 0, testRes };
        }

        public static int MethodTrialDiv(int number)
        {
            for (int i = 2; i < (int)Math.Floor(Math.Sqrt(number)); i++)
            {
                if (number % i == 0)
                    return i;
            }
            return -1;
        }

        public static double MistakeRSA(int number)
        {
            var primes = GetPrimeFactors(number);
            if (primes.Length!=2)
            {
                return -1;
            }
            else
            {
                return Math.Pow(primes[0],-1)+Math.Pow(primes[1],-1);
            }
        }

        public static int[] GetPrimeFactors(int number)
        {
            var list = new List<int>();
            var del = 2;
            var max = number;
            while (number != 1 && del < max/2+1)
            {
                if (number % del == 0)
                {
                    list.Add(del);
                    number /= del;
                    del = 2;

                }
                else
                {
                    del++;
                }
                
            }
            if (list.Count != 0)
                return list.ToArray();
            else
                return new int[] { max };
        }

        //Euler function
        #region 
        public static int Gcd(int a, int b)
        {
            if (a == 0)
                return b;
            return Gcd(b % a, a);
        }

        // A simple method to evaluate
        // Euler Totient Function
        public static int Phi(int n)
        {
            int result = 1;
            for (int i = 2; i < n; i++)
                if (Gcd(i, n) == 1)
                    result++;
            return result;
        }
        #endregion

        public static int[] ToBinary(int number)
        {
            var list = new List<int>();
            while (number > 0)
            {
                list.Add(number % 2);
                number /= 2;
            }
            return list.ToArray();
        }

        public static int[] FastExp(int number, int modul, int count)
        {
            var ost = new int[count + 1];
            ost[0] = number - (number / modul) * modul;
            for (int i = 1; i <= count; i++)
            {
                number = ost[i - 1] * ost[i - 1];
                ost[i] = number - (number / modul) * modul;
            }
            return ost;
        }

        public static int GetModded(int res, int mod) // надо переписать с оператором %
        {
            while (res < 0)
                res =res%mod + mod;
            while (res > mod)
                res %= mod;
            return res;
        }


        public static int Power(int text, int power, int mod) // y = text^power % mod
        {
            var prom = 1;
            var needBin = Equations.ToBinary(power);
            var needFastExp = Equations.FastExp(text, mod, needBin.Length-1);
            for (int i = 0; i < needBin.Length; i++)
            {
                if (needBin[i] == 1)
                {
                    prom *= needFastExp[i];
                    prom = Equations.GetModded(prom, mod);
                }
            }
            return prom;
        }

        public static int Multiply(int a, int b, int mod) // a*b % mod
        {
            var sum = 0;
            for (int i = 0; i < b; i++)
            {
                sum += a;
                sum = Equations.GetModded(sum, mod);
            }
            return sum;
        }

        public static double[] Probability(double a, double b) //Размер хеша и количество подборов для коллизии
        {
            double m = 0;
            double t = 0;
            double g = 0;
            var prob = Math.Pow(a, -b);
            if (-(int)b % 2 == 0)
            {
                m = (int)Math.Ceiling(Math.Log(prob, 2));
                g = 1.41;
                t = -(int)b / 2;
            }
            else
            {
                m = (int)Math.Ceiling(Math.Log(prob, 2));
                g = 4.47;
                t = -(int)b / 2;
            }
            m = (int) Math.Ceiling(Math.Log(prob, 2));
            t = -(int)b / 2;
            return new double[] { m, g, t };
        }

        public static int MinimalCycleGroup(int field, int p=1) // Определение минимальной циклической подгруппы, образующей группу, большей чем p 
        {
            var primeNumbers = Equations.GetPrimeFactors(field - 1);
            for (int i = p+1; i < field; i++)
            {
                if (field % i == 0) continue;
                var group = new int[field - 1];
                group[0] = i;
                var isGroup = true;
                for (int k = 1; k < field-1; k++)
                {
                    group[k] = group[k - 1] * i % field;
                    if (group[k] == 1 && k != field - 2)
                        isGroup = false;
                    if (group[field - 2] != 1 && k==field-2)
                        isGroup = false;
                }
                if (isGroup)
                {
                    return i;
                }

            }
            return -1;
        }
        
        public static int[] SolveComparison(int a, int b, int mod) // Решение сравнения
        {
            while (a > mod)
                a -= mod;
            var solvNumber = Gcd(a, mod);
            if (b % solvNumber == 0)
            {
                a /= solvNumber;
                b /= solvNumber;
                mod /= solvNumber;
            }
            var answer = new List<int>();
            for (int i = 1; i < mod; i++)
            {
                if (a * i % mod == b)
                    answer.Add(i);
            }
            if (answer.Count != 0) for (var i = 1; i < solvNumber; i++)
                    answer.Add(answer[0] + mod * i);
            return answer.Count != 0 ? answer.ToArray() : new int[] { -1 };
        }

        public static int SolveSystemOfComp(int[] data) //Решение системы сравнений
        {
            if (data.Length % 3 != 0)
                return -1;
            int m = 1;
            for (int i = 0; i < data.Length / 3; i++)
            {
                m *= data[2 + i * 3];
                if (data[0 + i * 3] == 1) continue;
                else
                {
                    var d = new ReversingEl(data[2 + i * 3], data[0 + i * 3]).CheckRes();
                    data[0 + i * 3] = 1;
                    data[1 + i * 3] = data[1 + i * 3] * d % data[2 + i * 3];
                }
            }
            var phi = new int[data.Length / 3];
            var reversed = new int[data.Length / 3];
            for (int i = 0; i < data.Length / 3; i++)
            {
                phi[i] = m / data[2 + i * 3];
                if (Equations.GetModded(phi[i], data[2 + i * 3]) == 0) return -1;
                reversed[i] = new ReversingEl(data[2 + i * 3], Equations.GetModded(phi[i], data[2 + i * 3])).CheckRes();
            }
            var result = 0;
            for (int i = 0; i < data.Length / 3; i++)
            {
                result += phi[i] * reversed[i] * data[1 + i * 3];
            }
            return Equations.GetModded(result,m);
        }
    }
}
