﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Reflection;
using Microsoft.Samples.Debugging.CorSymbolStore;
using System.Diagnostics.SymbolStore;
using System.IO;
using System.Diagnostics;
using FirebirdSql.Data.FirebirdClient;

namespace Cosmos.Debug.Common {

  public class SourceInfos : SortedList<uint, SourceInfo> {
    public SourceInfo GetMapping(uint aValue) {
      for (int i = Count - 1; i >= 0; i--) {
        if (Keys[i] <= aValue) {
          return Values[i];
        }
      }
      return null;
    }
  }

  public class SourceInfo {
    public string MethodName {
      get;
      set;
    }

    public string SourceFile {
      get;
      set;
    }
    public int Line {
      get;
      set;
    }
    public int Column {
      get;
      set;
    }
    public int LineEnd {
      get;
      set;
    }
    public int ColumnEnd {
      get;
      set;
    }

    public static List<KeyValuePair<uint, string>> ParseMapFile(String buildPath) {
      var xSourceStrings = File.ReadAllLines(Path.Combine(buildPath, "main.map"));
      var xSource = new List<KeyValuePair<uint, string>>();
      uint xIndex = 0;
      for (xIndex = 0; xIndex < xSourceStrings.Length; xIndex++) {
        if (xSourceStrings[xIndex].StartsWith("Real ")) {
          // further check it:
          //Virtual   Name"))
          if (!xSourceStrings[xIndex].Substring(4).TrimStart().StartsWith("Virtual ")
              || !xSourceStrings[xIndex].EndsWith(" Name")) {
            continue;
          }
          xIndex++;
          break;
        }
      }
      for (; xIndex < xSourceStrings.Length; xIndex++) {
        string xLine = xSourceStrings[xIndex];
        var xLineParts = xLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        if (xLineParts.Length == 3) {
          uint xAddress = UInt32.Parse(xLineParts[0], System.Globalization.NumberStyles.HexNumber);
          xSource.Add(new KeyValuePair<uint, string>(xAddress, xLineParts[2]));
        }
      }
      return xSource;
    }

    public static int GetIndexClosestSmallerMatch(IList<int> aList, int aValue) {
      int xIdx = -1;
      for (int i = 0; i < aList.Count; i++) {
        if (aList[i] <= aValue) {
          xIdx = i;
        } else {
          break;
        }
      }
      return xIdx;
    }

    public static SourceInfos GetSourceInfo(List<KeyValuePair<uint, string>> aAddressLabelMappings, IDictionary<string, uint> aLabelAddressMappings, DebugInfo debugInfo) {
      var xSymbolsList = new List<DebugInfo.MLDebugSymbol>();
      debugInfo.ReadSymbolsList(xSymbolsList);

      // Sort
      xSymbolsList.Sort(delegate(DebugInfo.MLDebugSymbol a, DebugInfo.MLDebugSymbol b) {
        if (a == null) {
          throw new ArgumentNullException("a");
        } else if (b == null) {
          throw new ArgumentNullException("b");
        }
        int xCompareResult = StringComparer.InvariantCultureIgnoreCase.Compare(a.AssemblyFile, b.AssemblyFile);
        if (xCompareResult == 0) {
          xCompareResult = a.TypeToken.CompareTo(b.TypeToken);
          if (xCompareResult == 0) {
            xCompareResult = a.MethodToken.CompareTo(b.MethodToken);
            if (xCompareResult == 0) {
              return a.ILOffset.CompareTo(b.ILOffset);
            }

          }
        }
        return xCompareResult;
      });

      var xResult = new SourceInfos();
      string xOldAssembly = null;
      ISymbolReader xSymbolReader = null;
      int[] xCodeOffsets = null;
      ISymbolDocument[] xCodeDocuments = null;
      int[] xCodeLines = null;
      int[] xCodeColumns = null;
      int[] xCodeEndLines = null;
      int[] xCodeEndColumns = null;
      int? xOldMethodToken = null;
      ISymbolMethod xMethodSymbol = null;
      foreach (var xSymbol in xSymbolsList) {
        if (!xSymbol.AssemblyFile.Equals(xOldAssembly, StringComparison.InvariantCultureIgnoreCase)) {
          try {
            xMethodSymbol = null;
            xSymbolReader = SymbolAccess.GetReaderForFile(xSymbol.AssemblyFile);
          } catch {
            xSymbolReader = null;
            xMethodSymbol = null;
          }
          xOldAssembly = xSymbol.AssemblyFile;
        }
        if (xOldMethodToken != xSymbol.MethodToken) {
          if (xSymbolReader != null) {
            try {
              xMethodSymbol = xSymbolReader.GetMethod(new SymbolToken(xSymbol.MethodToken));
              if (xMethodSymbol != null) {
                xCodeOffsets = new int[xMethodSymbol.SequencePointCount];
                xCodeDocuments = new ISymbolDocument[xMethodSymbol.SequencePointCount];
                xCodeLines = new int[xMethodSymbol.SequencePointCount];
                xCodeColumns = new int[xMethodSymbol.SequencePointCount];
                xCodeEndLines = new int[xMethodSymbol.SequencePointCount];
                xCodeEndColumns = new int[xMethodSymbol.SequencePointCount];
                xMethodSymbol.GetSequencePoints(xCodeOffsets, xCodeDocuments, xCodeLines, xCodeColumns, xCodeEndLines, xCodeEndColumns);
              }
            } catch {
              xMethodSymbol = null;
            }
          }
          xOldMethodToken = xSymbol.MethodToken;
        }

        if (xMethodSymbol != null) {
          if (aLabelAddressMappings.ContainsKey(xSymbol.LabelName)) {
            uint xAddress = aLabelAddressMappings[xSymbol.LabelName];
            // Each address could have mult labels, but this wont matter for SourceInfo, its not tied to label.
            // So we just ignore duplicate addresses.
            if (!xResult.ContainsKey(xAddress)) {
              int xIdx = GetIndexClosestSmallerMatch(xCodeOffsets, xSymbol.ILOffset);
              var xSourceInfo = new SourceInfo() {
                SourceFile = xCodeDocuments[xIdx].URL,
                Line = xCodeLines[xIdx],
                LineEnd = xCodeEndLines[xIdx],
                Column = xCodeColumns[xIdx],
                ColumnEnd = xCodeEndColumns[xIdx],
                MethodName = xSymbol.MethodName
              };
              xResult.Add(xAddress, xSourceInfo);
            }
          }
        }
      }
      return xResult;
    }
  }
}