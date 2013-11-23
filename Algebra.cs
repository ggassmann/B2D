using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basic2D {
	public static class Algebra {
		public static int Do(string operation) {
			Calc c = new Calc();
			int parenDepth = 0;
			int parenStart = 0;
			for (int i = 0; i < operation.Length; i++) {
				if (operation[i] == '(') {
					parenDepth++;
					if (parenDepth == 1) {
						parenStart = i;
					}
				}
				if (operation[i] == ')') {
					parenDepth--;
					if (parenDepth == 0) {
						var newOp = operation.Substring(parenStart + 1, i - parenStart - 1);
						var ans = Algebra.Do(newOp);
						operation = operation.Substring(0, parenStart) + ans + operation.Substring(i + 1);
						i = 0;
					}
				}
			}
			for (int i = 0; i < operation.Length; i++) {
				if (operation[i] == '/') {
					int leftInt;
					int rightInt;
					GetNums(operation, i, out leftInt, out rightInt);
					operation = PatchOperation(operation, i, leftInt / rightInt);
					i = 0;
				}
				if (operation[i] == '*') {
					int leftInt;
					int rightInt;
					GetNums(operation, i, out leftInt, out rightInt);
					operation = PatchOperation(operation, i, leftInt * rightInt);
					i = 0;
				}
			}
			for (int i = 0; i < operation.Length; i++) {
				if (operation[i] == '+') {
					int leftInt;
					int rightInt;
					GetNums(operation, i, out leftInt, out rightInt);
					operation = PatchOperation(operation, i, leftInt + rightInt);
					i = 0;
				}
				if (operation[i] == '-') {
					int leftInt;
					int rightInt;
					GetNums(operation, i, out leftInt, out rightInt);
					operation = PatchOperation(operation, i, leftInt - rightInt);
					i = 0;
				}
			}
			try {
				return Int32.Parse(operation);
			}
			catch {
				return Program.IntVars[operation];
			}
		}

		private static string PatchOperation(string operation, int i, int num) {
			if (countOperators(operation) >= 2) {
				operation = operation.Substring(0, getLeftBound(operation, i) == 0 ? getLeftBound(operation, i) : getLeftBound(operation, i) + 1) + num + operation.Substring(getRightBound(operation, i));
			}
			else {
				operation = num.ToString();
			}
			return operation;
		}
		private static void GetNums(string operation, int i, out int leftInt, out int rightInt) {
			var leftString = operation.Substring(getLeftBound(operation, i), i - getLeftBound(operation, i));
			var rightString = operation.Substring(i + 1, getRightBound(operation, i) - i - 1);
			try {
				leftInt = Int32.Parse(leftString);
			}
			catch {
				leftInt = Program.IntVars[leftString];
			}
			try {
				rightInt = Int32.Parse(rightString);
			}
			catch {
				rightInt = Program.IntVars[rightString];
			}
		}
		private static int getLeftBound(string s, int root) {
			root--;
			for (int i = root; i >= 0; i--) {
				if (isBound(s[i])) {
					return i;
				}
			}
			return 0;
		}
		private static int getRightBound(string s, int root) {
			root++;
			for (int i = root; i < s.Length; i++) {
				if (isBound(s[i])) {
					return i;
				}
			}
			return s.Length;
		}
		private static int countOperators(string s) {
			int j = 0;
			for (int i = 0; i < s.Length; i++) {
				if (isBound(s[i])) {
					j++;
				}
			}
			return j;
		}
		private static bool isBound(char c) {
			return c == ')' ||
					c == '(' ||
					c == '+' ||
					c == '/' ||
					c == '*' ||
					c == '-';
		}
	}
	public class Calc {
		public List<Calc> SubCalcs = new List<Calc>();
	}
}
