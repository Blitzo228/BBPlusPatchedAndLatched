using HarmonyLib;
using System.Collections.Generic;
using TMPro;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch(typeof(MathMachine))]
    internal static class MathMachinePatch
    {
        private static readonly AccessTools.FieldRef<MathMachine, List<int>> FieldAvailableAnswers =
            AccessTools.FieldRefAccess<MathMachine, List<int>>("_availableAnswers");
        private static readonly AccessTools.FieldRef<MathMachine, int> FieldAnswer =
            AccessTools.FieldRefAccess<MathMachine, int>("answer");
        private static readonly AccessTools.FieldRef<MathMachine, int> FieldNum1 =
            AccessTools.FieldRefAccess<MathMachine, int>("num1");
        private static readonly AccessTools.FieldRef<MathMachine, int> FieldNum2 =
            AccessTools.FieldRefAccess<MathMachine, int>("num2");
        private static readonly AccessTools.FieldRef<MathMachine, bool> FieldAddition =
            AccessTools.FieldRefAccess<MathMachine, bool>("addition");
        private static readonly AccessTools.FieldRef<MathMachine, TMP_Text> FieldVal1Text =
            AccessTools.FieldRefAccess<MathMachine, TMP_Text>("val1Text");
        private static readonly AccessTools.FieldRef<MathMachine, TMP_Text> FieldVal2Text =
            AccessTools.FieldRefAccess<MathMachine, TMP_Text>("val2Text");
        private static readonly AccessTools.FieldRef<MathMachine, TMP_Text> FieldSignText =
            AccessTools.FieldRefAccess<MathMachine, TMP_Text>("signText");
        private static readonly AccessTools.FieldRef<MathMachine, TMP_Text> FieldAnswerText =
            AccessTools.FieldRefAccess<MathMachine, TMP_Text>("answerText");

        private static readonly (int num1, int num2, int answer)[] MultiplicationPool = new[]
        {
            (1,1,1),(1,2,2),(1,3,3),(1,4,4),(1,5,5),(1,6,6),(1,7,7),(1,8,8),(1,9,9),
            (2,1,2),(2,2,4),(2,3,6),(2,4,8),(3,1,3),(3,2,6),(3,3,9),(4,1,4),(4,2,8),
            (5,1,5),(6,1,6),(7,1,7),(8,1,8),(9,1,9)
        };

        private static readonly (int num1, int num2, int answer)[] DivisionPool = new[]
        {
            (1,1,1),(2,1,2),(2,2,1),(3,1,3),(3,3,1),(4,1,4),(4,2,2),(4,4,1),(5,1,5),(5,5,1),
            (6,1,6),(6,2,3),(6,3,2),(6,6,1),(7,1,7),(7,7,1),(8,1,8),(8,2,4),(8,4,2),(8,8,1),
            (9,1,9),(9,3,3),(9,9,1)
        };

        private static readonly (int num1, int num2, int answer)[] ExponentPool = new[]
        {
            (0,1,0),(0,2,0),(0,3,0),
            (1,0,1),(1,1,1),(1,2,1),(1,3,1),
            (2,0,1),(2,1,2),(2,2,4),(2,3,8),
            (3,0,1),(3,1,3),(3,2,9),
            (4,0,1),(4,1,4),
            (5,0,1),(5,1,5),
            (6,0,1),(6,1,6),
            (7,0,1),(7,1,7),
            (8,0,1),(8,1,8),
            (9,0,1),(9,1,9)
        };

        private static readonly int[] ValidOpsBuffer = new int[5];

        [HarmonyPatch("NewProblem")]
        [HarmonyPrefix]
        private static bool NewProblemPrefix(MathMachine __instance)
        {
            bool mul = PatchedAndLatchedPlugin.EnableMathMachineMultiplication!.Value;
            bool div = PatchedAndLatchedPlugin.EnableMathMachineDivision!.Value;
            bool exp = PatchedAndLatchedPlugin.EnableMathMachineExponent!.Value;

            if (!mul && !div && !exp)
                return true;

            try
            {
                var availableAnswers = FieldAvailableAnswers(__instance);
                var val1Text = FieldVal1Text(__instance);
                var val2Text = FieldVal2Text(__instance);
                var signText = FieldSignText(__instance);
                var answerText = FieldAnswerText(__instance);

                if (availableAnswers == null || signText == null)
                    return true;

                availableAnswers.Clear();
                int currentCount = __instance.currentNumbers.Count;
                for (int i = 0; i < currentCount; i++)
                {
                    var num = __instance.currentNumbers[i];
                    if (num != null && num.Available)
                        availableAnswers.Add(num.Value);
                }

                int answer = -1;
                int attempts = 0;
                const int maxAttempts = 50;

                while (answer < 0 && availableAnswers.Count > 0 && attempts < maxAttempts)
                {
                    attempts++;
                    answerText.text = "?";

                    int op = GetRandomOperation(mul, div, exp);
                    int num1 = 0, num2 = 0;
                    bool isAddition = false;

                    switch (op)
                    {
                        case 0:
                            signText.text = "×";
                            (num1, num2, answer) = MultiplicationPool[UnityEngine.Random.Range(0, MultiplicationPool.Length)];
                            break;
                        case 1:
                            signText.text = "÷";
                            (num1, num2, answer) = DivisionPool[UnityEngine.Random.Range(0, DivisionPool.Length)];
                            break;
                        case 2:
                            signText.text = "+";
                            isAddition = true;
                            num1 = UnityEngine.Random.Range(0, 10);
                            num2 = UnityEngine.Random.Range(0, 10 - num1);
                            answer = num1 + num2;
                            break;
                        case 3:
                            signText.text = "-";
                            num1 = UnityEngine.Random.Range(0, 10);
                            num2 = UnityEngine.Random.Range(0, num1 + 1);
                            answer = num1 - num2;
                            break;
                        case 4:
                            signText.text = "^";
                            (num1, num2, answer) = ExponentPool[UnityEngine.Random.Range(0, ExponentPool.Length)];
                            break;
                    }

                    val1Text.text = num1.ToString();
                    val2Text.text = num2.ToString();

                    FieldNum1(__instance) = num1;
                    FieldNum2(__instance) = num2;
                    FieldAnswer(__instance) = answer;
                    FieldAddition(__instance) = isAddition;

                    if (answer < 0 || answer > 9 || !availableAnswers.Contains(answer))
                        answer = -1;
                }

                return false;
            }
            catch
            {
                return true;
            }
        }

        private static int GetRandomOperation(bool mul, bool div, bool exp)
        {
            bool replace = PatchedAndLatchedPlugin.ReplaceMathMachineCompletely!.Value;
            int count = 0;

            if (replace)
            {
                if (mul) ValidOpsBuffer[count++] = 0;
                if (div) ValidOpsBuffer[count++] = 1;
                if (exp) ValidOpsBuffer[count++] = 4;
                return count > 0 ? ValidOpsBuffer[UnityEngine.Random.Range(0, count)] : UnityEngine.Random.Range(2, 4);
            }

            ValidOpsBuffer[count++] = 2;
            ValidOpsBuffer[count++] = 3;
            if (mul) ValidOpsBuffer[count++] = 0;
            if (div) ValidOpsBuffer[count++] = 1;
            if (exp) ValidOpsBuffer[count++] = 4;

            return ValidOpsBuffer[UnityEngine.Random.Range(0, count)];
        }
    }
}
