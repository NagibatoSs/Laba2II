using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laba2II
{
    class GeneticAlgorithm
    {
        Func<int, double> function;
        int min;
        int max;
        int x;
        double y;
        int populationSize = 4;
        int crossowerCount = 4;
        int mutationCount = 1;
        int countEvolutionGeneration = 50; //Число поколений, отпущенных на эволюцию (критерий остонова)

        double[] fitness; //массив значений функций
        int[] population; //массив индексов особей популяции
        string[] binPopulation;//массив двоичных значений индексов особей популяции (4 особи)
        List<string> binPopulationCrossower; //список двоичных значений индексов особей + потомков (8 особей), нужен для селекции
        int[] valuesArr; //массив значений [-10;53]


        #region Init
        public GeneticAlgorithm(Func<int, double> func, int min, int max)
        {
            function = func;
            this.max = max;
            this.min = min;
            population = new int[populationSize];
            binPopulation = new string[populationSize];
            fitness = new double[populationSize];
            binPopulationCrossower = new List<string>();
        }

        void FillValuesArr()
        {
            var size = max - min +1;
            valuesArr = new int[size];
            for (int i = 0; i < valuesArr.Length; i++)
                valuesArr[i] = min + i;
        }

        void MakePopulation()
        {
            var rnd = new Random();
            for (int i = 0; i < population.Length; i++)
            {
                var randInd = rnd.Next(0, valuesArr.Length);
                population[i] = randInd;
            }
        }

        void OutputArray<T>(T[] arr)
        {
            foreach (var e in arr)
                Console.Write(e + " ");
            Console.WriteLine();
        }

        void OutputArray <T>(List<T> arr)
        {
            foreach (var e in arr)
                Console.Write(e + " ");
            Console.WriteLine();
        }
        #endregion

        #region FindMin/Max
        public void FindMax()
        {
            Find(false);
            Console.WriteLine("Конец числа поколений отпущенных на эволюцию");
            Console.WriteLine("Максимум функции y=" + y);
            Console.WriteLine("Точка максимума x=" + x);
        }
        public void FindMin()
        {
            Find(true);
            Console.WriteLine("Конец числа поколений отпущенных на эволюцию");
            Console.WriteLine("Минимум функции y=" + y);
            Console.WriteLine("Точка минимума x=" + x);
        }
        void Find(bool isMin)
        {
            FillValuesArr();

            MakePopulation();
            Console.WriteLine("Начальная популяция (индексы):");
            OutputArray(population);

            DecimalToBinaryPopulation();

            Console.WriteLine("Значения функции популяции:");
            FitnessFunc(population, isMin);
            OutputArray(fitness);

            for (int i = 0; i < countEvolutionGeneration; i++)
            {
                Console.WriteLine("Поколение " + (i + 1));

                Crossover();
                Console.WriteLine("Популяция после кроссовера");
                OutputArray(binPopulationCrossower);

                Mutation();
                Console.WriteLine("Популяция после мутации");
                OutputArray(binPopulationCrossower);

                TournamentSelection(isMin);
                Console.WriteLine("Популяция после селекции");
                OutputArray(binPopulation);

                FitnessFunc(binPopulation,isMin);
                Console.WriteLine("Значения функции популяции:");
                OutputArray(fitness);
            }
        }
    #endregion

        #region Transitions

        void DecimalToBinaryPopulation()
        {
            string bin;
            for (int i = 0; i < population.Length; i++)
            {
                bin = Convert.ToString(population[i], 2);
                binPopulation[i] = ExpandBinTo6Bits(bin);
            }  
        }
        int BinaryToDecimal(string individual)
        {
            return Convert.ToInt32(individual, 2);
        }

        string ExpandBinTo6Bits(string bin)
        {
            while(bin.Length<6)
            {
               bin = bin.Insert(0, "0");
            }
            return bin;
        }
        #endregion

        #region GeneticAlgorithm
        void FitnessFunc(int[] populationInd, bool isMin)
        {
            x = valuesArr[populationInd[0]];
            y = function(x);
            for (int i=0;i<fitness.Length;i++)
            {
                var x = valuesArr[populationInd[i]];
                fitness[i] = function(x);
                if (isMin)
                {
                    if (fitness[i] < y)
                    {
                        y = fitness[i];
                        this.x = x;
                    }
                }
                else
                {
                    if (fitness[i] > y)
                    {
                        y = fitness[i];
                        this.x = x;
                    }
                }
            }
        }

        void FitnessFunc(string[] populationInd, bool isMin)
        {
            for (int i = 0; i < fitness.Length; i++)
            {
                var ind = BinaryToDecimal(populationInd[i]);
                var x = valuesArr[ind];
                fitness[i] = function(x);
                if (isMin)
                {
                    if (fitness[i] < y)
                    {
                        y = fitness[i];
                        this.x = x;
                    }
                }
                else
                {
                    if (fitness[i] > y)
                    {
                        y = fitness[i];
                        this.x = x;
                    }
                }
            }
        }


        void Crossover()
        {
            var rnd = new Random();
            for (int i=0;i<binPopulation.Length;i++)
                binPopulationCrossower.Add(binPopulation[i]);
            string[] descendants = new string[crossowerCount];
            for (int i = 0; i < crossowerCount; i++)
            {
                int parent1Id = rnd.Next(0, populationSize);
                int parent2Id;
                do
                {
                    parent2Id = rnd.Next(0, populationSize);
                }
                while (parent2Id == parent1Id);
                var crossPoint = rnd.Next(1, binPopulation[parent1Id].Length);
                descendants[i] = binPopulation[parent1Id].Substring(0, crossPoint + 1) + binPopulation[parent2Id].Substring(crossPoint+1);
                binPopulationCrossower.Add(descendants[i]);
            }
        }

        void TournamentSelection(bool isMin)
        {
            var rnd = new Random();
            int groupSize = 2;
            for (int i = 0;i<populationSize;i++)
            {
                var group = new string[2];
                for (int j = 0; j < groupSize; j++)
                {
                    var rndInd = rnd.Next(binPopulationCrossower.Count);
                    group[j] = binPopulationCrossower[rndInd];
                    binPopulationCrossower.RemoveAt(rndInd);
                }
                if (isMin)
                {
                    binPopulation[i] =
                    function(valuesArr[BinaryToDecimal(group[0])]) < function(valuesArr[BinaryToDecimal(group[1])]) ? group[0] : group[1];
                }
                else
                {
                    binPopulation[i] =
                    function(valuesArr[BinaryToDecimal(group[0])]) > function(valuesArr[BinaryToDecimal(group[1])]) ? group[0] : group[1];
                }
            }
        }

        void Mutation()
        {
            for (int i = 0; i < mutationCount; i++)
            {
                //Инверсия случайного бита 
                var rnd = new Random();
                int rndIdx = rnd.Next(binPopulationCrossower.Count);
                int invBitIdx = rnd.Next(binPopulationCrossower[rndIdx].Length);
                if (binPopulationCrossower[rndIdx][invBitIdx] == '0')
                    binPopulationCrossower[rndIdx] = binPopulationCrossower[rndIdx].Insert(invBitIdx+1, "1");
                else
                    binPopulationCrossower[rndIdx] = binPopulationCrossower[rndIdx].Insert(invBitIdx+1, "0");
                binPopulationCrossower[rndIdx] = binPopulationCrossower[rndIdx].Remove(invBitIdx, 1);
            }
        }
        #endregion

    }
    class Program
    {
        static void Main(string[] args)
        {
            Func<int, double> func = (int x) => 4 - 5 * x - 26 * x * x + 2 * x * x * x;
            var genAlg = new GeneticAlgorithm(func,-10,53);
            Console.WriteLine("Поиск минимума");
            genAlg.FindMin();
            Console.WriteLine();
            Console.WriteLine("Поиск максимума");
            genAlg.FindMax();
            Console.ReadKey();
        }
    }
}
