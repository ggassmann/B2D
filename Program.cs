using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Basic2D {
	class Program {
		static public Dictionary<string, int> IntVars = new Dictionary<string, int>();

		static public int x = 0;
		static public int y = 0;
		static public int direction = 0; //NSEW

		static public bool forcePrintNextChar = false; //n*\ outputs \n for newline

		static public bool inVarOperation = false;
		static public bool gettingVarName = false;
		static public bool gettingOtherValue = false;
		static public int operationType = 0; //0: Int operation 
											 //1: String operation
		static public int operand = 0; //0: (+)addition 
									   //1: (@)print var
									   //2: (=)set var
									   //3: (-)subtraction
		static public string value = "";
		static public string varName = "";

		static public char[,] program;
		static void Main(string[] args) {
			LoadProgram();
			IntVars.Add("c", 0);
			while (true) {
				var current = program[x, y];
				bool print = true;
				if (!forcePrintNextChar) {
					if (!inVarOperation && current == '[') {
						inVarOperation = true;
					}
					if (inVarOperation) {
						if (gettingOtherValue && current == ']') {
							if (operationType == 0) {
								if (operand == 0) {
									IntVars[varName] += Int32.Parse(value);
								}
								if (operand == 1) {
									Console.Write(IntVars[varName]);
								}
								if (operand == 2) {
									IntVars[varName] = Int32.Parse(value);
								}
								if (operand == 3) {
									IntVars[varName] -= Int32.Parse(value);
								}
							}
							operationType = 0;
							operand = 0;
							varName = "";
							inVarOperation = false;
							gettingOtherValue = false;
							gettingVarName = false;
							value = "";
						}
						else {
							if (gettingOtherValue) {
								value += current;
							}
							if (gettingVarName) {
								if (current == '+') {
									operand = 0;
									gettingVarName = false;
									gettingOtherValue = true;
								}
								else if (current == '@') {
									operand = 1;
									gettingVarName = false;
									gettingOtherValue = true;
								}
								else if (current == '=') {
									operand = 2;
									gettingVarName = false;
									gettingOtherValue = true;
								}
								else if (current == '-') {
									operand = 3;
									gettingVarName = false;
									gettingOtherValue = true;
								}
								else {
									varName += current;
								}
							}
							if (current == '$') {
								operationType = 0;
								gettingVarName = true;
							}
						}
						print = false;
					}
					else if (ChangeDirection(current) || 
							 NoOperation(current) ||
							 ForcePrint(current) ||
							 NewLine(current)){
						print = false;
						if (ForcePrint(current)) {
							forcePrintNextChar = true;
						}
					}
					if (print) {
						Console.Write(current);
					}
				}
				else {
					Console.Write(current);
					forcePrintNextChar = false;
				}
				MovePointer();
				Thread.Sleep(4);
			}
		}

		private static bool NewLine(char current) {
			if (current == '\"') {
				Console.WriteLine();
			}
			return current == '\"';
		}
		private static bool ForcePrint(char current) {
			return current == '*';
		}

		private static bool NoOperation(char current) {
			return current == ',';
		}

		private static bool ChangeDirection(char current) {
			if (current == '>') {
				direction = 2;
				return true;
			}
			if (current == '<') {
				direction = 3;
				return true;
			}
			if (current == '^') {
				direction = 0;
				return true;
			}
			if (current == 'v') {
				direction = 1;
				return true;
			}
			return false;
		}

		private static void MovePointer() {
			if (direction == 0) {
				y--;
			}
			if (direction == 1) {
				y++;
			}
			if (direction == 2) {
				x++;
			}
			if (direction == 3) {
				x--;
			}
		}
		static void mirrorBackslash() {
			if (direction == 2) { //East
				direction = 1; //South
			}
			else if (direction == 1) { //South
				direction = 2; //East
			}
			if (direction == 3) { //West
				direction = 0; //North
			}
			else if (direction == 0) { //North
				direction = 3; //West
			}
		}
		private static void LoadProgram() {
			var file = File.ReadAllLines("testcode.b2d");
			int maxX = 0;
			int maxY = file.Length;
			for (int i = 0; i < maxY; i++) {
				maxX = Math.Max(maxX, file[i].Length);
			}
			program = new char[maxX, maxY];
			for (int i = 0; i < maxX; i++) {
				for (int j = 0; j < maxY; j++) {
					if (file[j].Length > i) {
						program[i, j] = file[j][i];
					}
				}
			}
		}

	}
}
