﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PerseusApi.Document;
using PerseusApi.Generic;
using PerseusApi.Matrix;
using PerseusPluginLib;
using PerseusPluginLib.Rearrange;
using BaseLib.Param;
using PerseusLib.Data.Matrix;

namespace PerseusPluginLibTest.Rearrange{
	/// <summary>
	/// Testing the ProcessTextColumns class requires, at a minimum, a regular expression 
	/// and MatrixData for it to act on. The private method TestRegex encapsulates nost
	/// of the mechanics, so that the test methods only have to specify the regex, the 
	/// input data, and the expected output.
	/// </summary>
	[TestClass]
	public class ProcessTextColumnsTest{
		/// <summary>
		/// The regex "^([^;]+)" should output everything before the first semicolon.
		/// </summary>
		[TestMethod] public void TestOnlyToFirstSemicolon(){
			string regexStr = "^([^;]+)";
			string[] stringsInit = new string[]{"just one item", "first item; second item"};
			string[] stringsExpect = new string[]{"just one item", "first item"};
			TestRegex(regexStr, stringsInit, stringsExpect);
		}

		/// <summary>
		/// The regex "B *= *([^,; ]+)" should output the value given to B.
		/// </summary>
		[TestMethod] public void TestAssignmentWithEqualSign(){
			string regexStr = "B *= *([^,; ]+)";
			string[] stringsInit = new string[]{"A = 123, B = 456", "A=123; B=456"};
			string[] stringsExpect = new string[]{"456", "456"};
			TestRegex(regexStr, stringsInit, stringsExpect);
		}

		/// <summary>
		/// The regex "B *= *([^,; ]+)" should output the value given to B.
		/// </summary>
		[TestMethod] public void TestSeparatedBySemicolons(){
			string regexStr = "B *= *([^,; ]+)";
			string[] stringsInit = new string[]{"A = 123, B = 456", "A=123; B=456", "B=123; B=456"};
			string[] stringsExpect = new string[]{"456", ";456", "123;456"};
			TestRegex(regexStr, stringsInit, stringsExpect);
		}

		/// <summary>
		/// An auxiliary method for testing the action of regular expressions. 
		/// Limited to a single column, which should be sufficient for this purpose.
		/// Multiple rows are allowed to test the effect of one regex on several strings.
		/// </summary>
		private void TestRegex(string regexStr, string[] stringsInit, string[] stringsExpect){
			string name = null;
			List<string> expressionColumnNames = null;
			float[,] expressionValues = null;
			List<string> categoryColumnNames = null;
			List<string[][]> categoryColumns = null;
			List<string> numericColumnNames = null;
			List<double[]> numericColumns = null;
			List<string> multiNumericColumnNames = null;
			List<double[][]> multiNumericColumns = null;
			IMatrixData[] supplTables = null;
			IDocumentData[] documents = null;
			ProcessInfo processInfo = null;
			List<string> stringColumnNames = new List<string>{"Column Name"};
			List<string[]> stringColumnsInit = new List<string[]>{stringsInit};
			List<string[]> stringColumnsExpect = new List<string[]>{stringsExpect};
			Parameters param =
				new Parameters(new Parameter[]{
					new MultiChoiceParam("Columns", new int[]{0}){Values = stringColumnNames},
					new StringParam("Regular expression", regexStr), new BoolParam("Keep original columns", false),
					new BoolParam("Strings separated by semicolons are independent", false)
				});
			IMatrixData mdata = new MatrixData();
			mdata.Clear();
			mdata.SetData(name, mdata.ColumnNames, mdata.Values, stringColumnNames, stringColumnsInit, mdata.CategoryColumnNames,
				new List<string[][]>(), mdata.NumericColumnNames, mdata.NumericColumns, mdata.MultiNumericColumnNames,
				mdata.MultiNumericColumns);
			var ptc = new ProcessTextColumns();
			ptc.ProcessData(mdata, param, ref supplTables, ref documents, processInfo);
			Boolean ignoreCase = false;
			for (int rowInd = 0; rowInd < stringColumnsInit[0].Length; rowInd++){
				Assert.AreEqual(mdata.StringColumns[0][rowInd], stringColumnsExpect[0][rowInd], ignoreCase);
			}
		}
	}
}