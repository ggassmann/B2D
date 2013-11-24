using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Basic2D {
	class Position {
		public int X;
		public int Y;
		public Position(int x, int y) {
			X = x;
			Y = y;
		}
		public override string ToString() {
			return "[" + X + ", " + Y + "]";
		}
	}
	public class Program {
		static public Dictionary<string, object> Vars = new Dictionary<string, object>();

		static public int x = 0;
		static public int y = 0;
		static List<Position> path = new List<Position>();
		static public int direction = 0; //NSEW

		static public bool forcePrintNextChar = false; //n*\ outputs \n for newline

		static public bool inVarOperation = false;
		static public bool gettingVarName = false;
		static public bool gettingOtherValue = false;
		static public int operationType = 0; //0: ($) Int operation 
											 //1: (") String operation
		static public int operand = 0;
									   //1: (@)print var
									   //2: (=)set var
									   //3: (!)save var
									   //4: (==)equals
									   //5: (~)clear var
									   //6: (?)read var
		static public string value = "";
		static public string varName = "";

		static public bool equalEquals = false;

		static public char[,] program;
		static public void Main(string[] args) {
			LoadProgram(args[0]);
			bool quit = false;
			while (!quit) {
				var currentChar = program[x, y];
				Char nextChar = default(char);
				try {
					nextChar = program[x + (direction == 2 ? 1 : 0) + (direction == 3 ? -1 : 0),
									   y + (direction == 0 ? -1 : 0) + (direction == 1 ? 1 : 0)];
				}
				catch { };
				bool print = true;
				if (!forcePrintNextChar) {
					if (!inVarOperation && currentChar == '[') {
						inVarOperation = true;
					}
					if (inVarOperation) {
						if (gettingOtherValue && currentChar == ']') {
							if (operationType == 0) {
								if (operand == 1) {
									Console.Write(Vars[varName]);
								}
								if (operand == 2) {
									if (!Vars.ContainsKey(varName)) {
										Vars.Add(varName, 0);
									}
									try {
										Vars[varName] = Int32.Parse(Algebra.Do(value));
									}
									catch {
										Vars[varName] = Int32.Parse((Vars[Algebra.Do(value)].ToString()));
									}
								}
								if (operand == 3) {
									File.WriteAllText(value, Vars[varName].ToString());
								}
								if (operand == 4) {
									equalEquals = false;
									if ((int)Vars[varName] == Int32.Parse(value)) {
										MovePointer();
									}
								}
								if (operand == 5) {
									Vars.Remove(varName);
								}
								if (operand == 6) {
									Vars[varName] = Int32.Parse(Console.ReadLine());
								}
							}
							else if (operationType == 1) {
								if (operand == 1) {
									Console.Write(Vars[varName]);
								}
								else if (operand == 2) {
									if (!Vars.ContainsKey(varName)) {
										Vars.Add(varName, "");
									}
									Vars[varName] = Algebra.Do(value);
								}
								else if (operand == 6) {
									Vars[varName] = Console.ReadLine();
								}
								else {
									throw new Exception("Unknown operand");
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
								value += currentChar;
							}
							if (gettingVarName) {
								if (currentChar == '@') {
									operand = 1;
									gettingVarName = false;
									gettingOtherValue = true;
								}
								else if (currentChar == '!') {
									operand = 3;
									gettingVarName = false;
									gettingOtherValue = true;
								}
								else if (currentChar == '~') {
									operand = 5;
									gettingVarName = false;
									gettingOtherValue = true;
								}
								else if (currentChar == '?') {
									operand = 6;
									gettingVarName = false;
									gettingOtherValue = true;
								}
								else if (currentChar == '=') {
									operand = equalEquals == true ? 4 : 2;
									if (nextChar == '=') {
										operand = 4;
										equalEquals = true;
									}
									else {
										gettingVarName = false;
										gettingOtherValue = true;
									}
								}
								else {
									varName += currentChar;
								}
							}
							if (currentChar == '$') {
								operationType = 0;
								gettingVarName = true;
							}
							if (currentChar == '\"') {
								operationType = 1;
								gettingVarName = true;
							}
						}
						print = false;
					}
					else if (ChangeDirection(currentChar) || 
							 NoOperation(currentChar) ||
							 ForcePrint(currentChar) ||
							 NewLine(currentChar) ||
							 Mirror(currentChar) ||
							 ChangeDirectionRandom(currentChar)
							 ){
						print = false;
						if (ForcePrint(currentChar)) {
							forcePrintNextChar = true;
						}
					}
					if (print) {
						if(currentChar == ';') {
							quit = true;
						}
						else {
							Console.Write(currentChar);
						}
					}
				}
				else {
					Console.Write(currentChar);
					forcePrintNextChar = false;
				}
				MovePointer();
				//Thread.Sleep(4);
			}
			Console.WriteLine("\nDone...");
			Console.ReadKey();
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
		static Random _r = new Random();
		private static bool ChangeDirectionRandom(char current) {
			if (current == '#') {
				direction = _r.Next(0, 4);
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
			path.Add(new Position(x, y));
		}
		static bool Mirror(Char current) {
			if (current == '\\') {
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
				return true;
			}
			if (current == '/') {
				if (direction == 2) { //East
					direction = 0; //North
				}
				else if (direction == 0) { //North
					direction = 2; //East
				}
				if (direction == 3) { //West
					direction = 1; //South
				}
				else if (direction == 1) { //South
					direction = 3; //West
				}
				return true;
			}
			return false;
		}
		private static void LoadProgram(string filePath) {
			var file = File.ReadAllLines(filePath);
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
