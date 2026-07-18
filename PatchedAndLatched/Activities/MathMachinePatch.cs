using HarmonyLib;
using PatchedAndLatched;
using System;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch(typeof(MathMachine))]
    internal static class MathMachinePatch
    {
        [HarmonyPatch("NewProblem")]
        [HarmonyPrefix]
        private static bool NewProblemPrefix(MathMachine __instance)
        {
            if (!PatchedAndLatchedPlugin.EnableMathMachineMultiplication.Value &&
                !PatchedAndLatchedPlugin.EnableMathMachineDivision.Value)
                return true;

            try
            {
                var type = __instance.GetType();
                var fieldAvailableAnswers = type.GetField("_availableAnswers", BindingFlags.Instance | BindingFlags.NonPublic);
                var fieldAnswer = type.GetField("answer", BindingFlags.Instance | BindingFlags.NonPublic);
                var fieldNum1 = type.GetField("num1", BindingFlags.Instance | BindingFlags.NonPublic);
                var fieldNum2 = type.GetField("num2", BindingFlags.Instance | BindingFlags.NonPublic);
                var fieldAddition = type.GetField("addition", BindingFlags.Instance | BindingFlags.NonPublic);
                var fieldVal1Text = type.GetField("val1Text", BindingFlags.Instance | BindingFlags.NonPublic);
                var fieldVal2Text = type.GetField("val2Text", BindingFlags.Instance | BindingFlags.NonPublic);
                var fieldSignText = type.GetField("signText", BindingFlags.Instance | BindingFlags.NonPublic);
                var fieldAnswerText = type.GetField("answerText", BindingFlags.Instance | BindingFlags.NonPublic);

                if (fieldAvailableAnswers == null || fieldAnswer == null || fieldSignText == null)
                    return true;

                var availableAnswers = (List<int>)fieldAvailableAnswers.GetValue(__instance);
                var val1Text = (TMP_Text)fieldVal1Text.GetValue(__instance);
                var val2Text = (TMP_Text)fieldVal2Text.GetValue(__instance);
                var signText = (TMP_Text)fieldSignText.GetValue(__instance);
                var answerText = (TMP_Text)fieldAnswerText.GetValue(__instance);

                availableAnswers.Clear();
                foreach (var num in __instance.currentNumbers)
                {
                    if (num.Available)
                        availableAnswers.Add(num.Value);
                }

                int answer = -1;
                int attempts = 0;
                const int maxAttempts = 50;

                while (answer < 0 && availableAnswers.Count > 0 && attempts < maxAttempts)
                {
                    attempts++;
                    answerText.text = "?";

                    int op = GetRandomOperation();
                    int num1 = 0, num2 = 0;
                    bool isAddition = false;

                    switch (op)
                    {
                        case 0:
                            signText.text = "×";
                            (num1, num2, answer) = GenerateMultiplication();
                            break;
                        case 1:
                            signText.text = "÷";
                            (num1, num2, answer) = GenerateDivision();
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
                    }

                    val1Text.text = num1.ToString();
                    val2Text.text = num2.ToString();
                    fieldNum1.SetValue(__instance, num1);
                    fieldNum2.SetValue(__instance, num2);
                    fieldAnswer.SetValue(__instance, answer);
                    fieldAddition.SetValue(__instance, isAddition);

                    if (!availableAnswers.Contains(answer) || answer < 0 || answer > 9)
                        answer = -1;
                }

                return false; 
            }
            catch
            {
                return true; 
            }
        }

        private static int GetRandomOperation()
        {
            bool mul = PatchedAndLatchedPlugin.EnableMathMachineMultiplication.Value;
            bool div = PatchedAndLatchedPlugin.EnableMathMachineDivision.Value;
            bool replace = PatchedAndLatchedPlugin.ReplaceMathMachineCompletely.Value;

            if (replace)
            {
                var ops = new List<int>();
                if (mul) ops.Add(0);
                if (div) ops.Add(1);
                return ops.Count > 0 ? ops[UnityEngine.Random.Range(0, ops.Count)] : UnityEngine.Random.Range(2, 4);
            }

            var ops2 = new List<int> { 2, 3 };
            if (mul) ops2.Add(0);
            if (div) ops2.Add(1);
            return ops2[UnityEngine.Random.Range(0, ops2.Count)];
        }

        private static (int num1, int num2, int answer) GenerateMultiplication()
        {
            var pool = new (int, int, int)[]
            {
                (1,1,1),(1,2,2),(1,3,3),(1,4,4),(1,5,5),(1,6,6),(1,7,7),(1,8,8),(1,9,9),
                (2,1,2),(2,2,4),(2,3,6),(2,4,8),(3,1,3),(3,2,6),(3,3,9),(4,1,4),(4,2,8),
                (5,1,5),(6,1,6),(7,1,7),(8,1,8),(9,1,9)
            };
            return pool[UnityEngine.Random.Range(0, pool.Length)];
        }

        private static (int num1, int num2, int answer) GenerateDivision()
        {
            var pool = new (int, int, int)[]
            {
                (1,1,1),(2,1,2),(2,2,1),(3,1,3),(3,3,1),(4,1,4),(4,2,2),(4,4,1),(5,1,5),(5,5,1),
                (6,1,6),(6,2,3),(6,3,2),(6,6,1),(7,1,7),(7,7,1),(8,1,8),(8,2,4),(8,4,2),(8,8,1),
                (9,1,9),(9,3,3),(9,9,1)
            };
            return pool[UnityEngine.Random.Range(0, pool.Length)];
        }
    }
}