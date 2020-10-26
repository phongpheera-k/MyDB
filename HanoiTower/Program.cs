using System;

namespace HanoiTower
{
    class Program
    {
        static void Main(string[] args) {
            Console.WriteLine("Hanoi Tower");
            var repeat = true;
            while (repeat) {
                Console.Write("Input number of disks : ");
                var input = Convert.ToInt32(Console.ReadLine());

                if (input < 1) {
                    Console.WriteLine("Number of disks should more than 0");
                }
                else {
                    HanoiSolution(input, "A", "B", "C");
                    repeat = false;
                }
            }
        }

        static void HanoiSolution(int diskNumber, string sourceTower, string destinationTower, string tempTower) {
            if (diskNumber == 1) {
                Console.WriteLine($"Move disk {diskNumber} from {sourceTower} to {destinationTower}");
            }
            else {
                HanoiSolution(diskNumber - 1, sourceTower, tempTower, destinationTower);
                Console.WriteLine($"Move disk {diskNumber} from {sourceTower} to {destinationTower}");
                HanoiSolution(diskNumber - 1, tempTower, destinationTower, sourceTower);
            }
        }
    }
}