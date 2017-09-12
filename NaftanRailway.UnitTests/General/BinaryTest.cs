using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using Newtonsoft.Json;

namespace NaftanRailway.UnitTests.General {
    /// <summary>
    /// Summary description for BinaryTest
    /// </summary>
    [TestClass]
    public class BinaryTest {
        [TestMethod]
        public void FindBits() {
            int inputA = 5, inputB = 5;
            int countA = 0, countB = 0;

            //it's variant with  &= (bits AND) and (n-1)
            string binary = Convert.ToString(inputA, 2);

            while (inputA != 0) {
                countA++;
                inputA &= (inputA - 1);
            }

            //it's variant with comparer with number 1 (binary) and bitshift right by 1
            while (inputB > 0) {
                if ((inputB & 1) == 1) countB++;

                inputB >>= 1;
            }

            Assert.IsTrue(countA == countB);
        }

        [TestMethod]
        public void Array() {
            var seq = new[] { 1, 2, 3, 7, 5 };

            //linq
            var revSeq = seq.Reverse();

            //hand made
            var retSeq = new int[seq.Length];
            for (int i = seq.Length; i > 0; i--) {
                retSeq[seq.Length - i] = seq[i - 1];
            }

            //shift an array's elements one place to the left
            var shiftSeq = new int[seq.Length];

            for (int i = 1; i < seq.Length; i++) {
                shiftSeq[i - 1] = seq[i];
            }
            //without create new array
            for (int i = 0; i < seq.Length / 2; i++) {
                int temp = seq[i];

                seq[i] = seq[seq.Length - 1 - i];
                seq[seq.Length - 1 - i] = temp;
            }

            //Find all pairs in an array that add up to x
            //i = 0
            //j = n - 1
            //while (i < j) {
            //    if (a[i] + a[j] == target) return (i, j);
            //    else if (a[i] + a[j] < target) i += 1;
            //    else if (a[i] + a[j] > target) j -= 1;
            //}
            //return NOT_FOUND;

            // remove all duplicate values from an array
            // also there're solution with  => copyTo hashset (without duplicate)
            // LINQ distinct
            // foreach and contains
            var distinctSeq = seq.GroupBy(x => x);

        }

        [TestMethod]
        public void LinkedList() {
            LinkedList<string> link = new LinkedList<string>();

            // Добавим несколько элементов
            link.AddFirst("Alex");
            link.AddFirst("Djek");
            link.AddFirst("Bob");
            link.AddFirst("Doug");

            // Отобразим элементы в прямом направлении
            LinkedListNode<string> node;
            Console.WriteLine("Элементы коллекции в прямом направлении: ");
            for (node = link.First; node != null; node = node.Next)
                Console.Write(node.Value + "\t");

            // Отобразить элементы в обратном направлении
            Console.WriteLine("\n\nЭлементы коллекции в обратном направлении: ");
            for (node = link.Last; node != null; node = node.Previous)
                Console.Write(node.Value + "\t");


        }

        /// <summary>
        /// it checks work of json formatter with binary array
        /// </summary>
        [TestMethod]
        public void BinaryJSON() {
            var result = CreateMD5("DfmmC/HdLhyS7ypSKUrSvQ==");
            var binaryFromJsonString = Encoding.ASCII.GetBytes("4ufos4eNC3iX4B4EnFzYmw ==");
            var binary = Encoding.ASCII.GetBytes("0xE2E7E8B3878D0B7897E01E049C5CD89B");

            var strConvertion = String.Empty; // binary.ToString("X2");

            for (int i = 0; i < binary.Length; i++) {
                strConvertion += binary[i].ToString("X2") + " ";  //// <<<--- Here is the problem
            }

            var stringJson = JsonConvert.SerializeObject(binary);

            Assert.IsNotNull(binary);
        }


        public static string CreateMD5(string input = "4ufos4eNC3iX4B4EnFzYmw ==") {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create()) {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++) {
                    sb.Append(hashBytes[i].ToString("X2"));
                }

                return sb.ToString();
            }
        }
    }
}