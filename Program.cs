using Google.OrTools.LinearSolver;
using Google.OrTools.Glop;

namespace OrtoolsTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // 简单测试
            // SimpleTest();

            // 向量测试
            // VectorTest();

            // 创建求解器
            Solver solver = Solver.CreateSolver("SCIP");

            if (solver == null)
            {
                Console.WriteLine("无法创建求解器。");
                return;
            }

            int m = 3;  // 行数
            int n = 4;  // 列数

            // 定义二维连续型变量矩阵 x[i,j]
            Variable[,] x = new Variable[m, n];

            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    x[i, j] = solver.MakeNumVar(0.0, double.PositiveInfinity, $"x_{i}_{j}");
                }
            }

            // 构建约束条件
            // 约束1: 对每个 i, sum(x[i, j]) <= 5
            for (int i = 0; i < m; i++)
            {
                Constraint constraint = solver.MakeConstraint(double.NegativeInfinity, 5, $"row_constraint_{i}");
                for (int j = 0; j < n; j++)
                {
                    constraint.SetCoefficient(x[i, j], 1);
                }
            }

            // 约束2: 对每个 j, sum(x[i, j]) >= 2
            for (int j = 0; j < n; j++)
            {
                Constraint constraint = solver.MakeConstraint(2, double.PositiveInfinity, $"column_constraint_{j}");
                for (int i = 0; i < m; i++)
                {
                    constraint.SetCoefficient(x[i, j], 1);
                }
            }

            // 构建目标函数: 最小化 sum(x[i, j]^2 + 3 * x[i, j])
            Objective objective = solver.Objective();
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    // 设置二次项: x[i, j]^2
                    objective.SetQuadraticCoefficient(x[i, j], x[i, j], 1);
                    // 设置线性项: 3 * x[i, j]
                    objective.SetCoefficient(x[i, j], 3);
                }
            }

            // 设置目标函数为最小化
            objective.SetMinimization();

            // 求解
            Solver.ResultStatus resultStatus = solver.Solve();

            // 检查求解状态并输出结果
            if (resultStatus == Solver.ResultStatus.OPTIMAL)
            {
                Console.WriteLine("最优解:");
                for (int i = 0; i < m; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        Console.WriteLine($"x[{i},{j}] = {x[i, j].SolutionValue()}");
                    }
                }
                Console.WriteLine($"最小化目标函数值 = {solver.Objective().Value()}");
            }
            else
            {
                Console.WriteLine("无法找到最优解。");
            }
        }

        private static void VectorTest()
        {
            // 创建求解器
            Solver solver = Solver.CreateSolver("CBC_MIXED_INTEGER_PROGRAMMING");

            if (solver == null)
            {
                Console.WriteLine("无法创建求解器。");
                return;
            }

            int n = 5; // 假设变量向量长度为 5
            Variable[] x = new Variable[n];

            // 定义变量 x_1, x_2, ..., x_n 为整数变量
            for (int i = 0; i < n; i++)
            {
                x[i] = solver.MakeIntVar(0.0, double.PositiveInfinity, $"x_{i + 1}");
            }

            // 构建约束条件
            // 约束1: x_1 + 2x_2 + ... + nx_n <= 10
            Constraint constraint1 = solver.MakeConstraint(double.NegativeInfinity, 10, "constraint1");
            for (int i = 0; i < n; i++)
            {
                constraint1.SetCoefficient(x[i], i + 1);  // 系数为 1, 2, ..., n
            }

            // 约束2: 2x_1 - x_2 + ... - x_n >= 5
            Constraint constraint2 = solver.MakeConstraint(5, double.PositiveInfinity, "constraint2");
            for (int i = 0; i < n; i++)
            {
                if (i == 0)
                {
                    constraint2.SetCoefficient(x[i], 2);  // 系数为 2
                }
                else
                {
                    constraint2.SetCoefficient(x[i], -1);  // 系数为 -1
                }
            }

            // 构建目标函数: 最大化 2x_1 + 3x_2 + ... + nx_n
            Objective objective = solver.Objective();
            for (int i = 0; i < n; i++)
            {
                objective.SetCoefficient(x[i], i + 2);  // 系数为 2, 3, ..., n+1
            }
            objective.SetMaximization();

            // 求解
            Solver.ResultStatus resultStatus = solver.Solve();

            // 检查求解状态并输出结果
            if (resultStatus == Solver.ResultStatus.OPTIMAL)
            {
                Console.WriteLine("最优解:");
                for (int i = 0; i < n; i++)
                {
                    Console.WriteLine($"x_{i + 1} = {x[i].SolutionValue()}");
                }
                Console.WriteLine($"最大化目标函数值 = {solver.Objective().Value()}");
            }
            else
            {
                Console.WriteLine("无法找到最优解。");
            }
        }

        private static void SimpleTest()
        {
            // 创建求解器，使用GLOP求解线性问题或者CBC求解整数问题
            Solver solver = Solver.CreateSolver("CBC_MIXED_INTEGER_PROGRAMMING");

            if (solver == null)
            {
                Console.WriteLine("无法创建求解器。");
                return;
            }

            // 定义决策变量
            // 这里设置为整数变量 x 和 y
            Variable x = solver.MakeIntVar(0.0, double.PositiveInfinity, "x");
            Variable y = solver.MakeIntVar(0.0, double.PositiveInfinity, "y");

            // 定义约束条件
            // x + 2y <= 14
            solver.Add(x + 2 * y <= 14);
            // 3x - y >= 0
            solver.Add(3 * x - y >= 0);
            // x - y <= 2
            solver.Add(x - y <= 2);

            // 定义目标函数：最大化 3x + 4y
            Objective objective = solver.Objective();
            objective.SetCoefficient(x, 3);
            objective.SetCoefficient(y, 4);
            objective.SetMaximization();

            // 求解
            Solver.ResultStatus resultStatus = solver.Solve();

            // 检查求解是否成功
            if (resultStatus == Solver.ResultStatus.OPTIMAL)
            {
                Console.WriteLine("最优解:");
                Console.WriteLine($"x = {x.SolutionValue()}");
                Console.WriteLine($"y = {y.SolutionValue()}");
                Console.WriteLine($"最大化目标函数值 = {solver.Objective().Value()}");
            }
            else
            {
                Console.WriteLine("无法找到最优解。");
            }
        }
    }
}
