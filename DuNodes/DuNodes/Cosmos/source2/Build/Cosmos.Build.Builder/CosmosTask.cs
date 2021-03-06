﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cosmos.Build.Installer;
using System.IO;

namespace Cosmos.Build.Builder {
  public class CosmosTask : Task {
    protected string mCosmosPath;
    public bool ResetHive { get; set; }
    protected string mOutputPath;

    public CosmosTask(string aCosmosPath) {
      mCosmosPath = aCosmosPath;
    }

    protected void MsBuild(string aSlnFile, string aBuildCfg) {
      StartConsole(Paths.Windows + @"\Microsoft.NET\Framework\v4.0.30319\msbuild.exe", Quoted(aSlnFile) + @" /maxcpucount /verbosity:normal /nologo /p:Configuration=" + aBuildCfg + " /p:Platform=x86 /p:OutputPath=" + Quoted(mOutputPath));
    }

    protected override void DoRun() {
      mOutputPath = mCosmosPath + @"\Build\VSIP";
      if (!Directory.Exists(mOutputPath)) {
        Directory.CreateDirectory(mOutputPath);
      }

      Section("Compiling X# Compiler");
      MsBuild(mCosmosPath + @"\source2\XSharp.sln", "Debug");

      Section("Compiling X# Sources");
      var xFiles = Directory.GetFiles(mCosmosPath + @"\source2\Compiler\Cosmos.Compiler.DebugStub\", "*.xs");
      foreach (var xFile in xFiles) {
        Echo("Compiling " + Path.GetFileName(xFile));
        string xDest = Path.ChangeExtension(xFile, ".cs");
        if (File.Exists(xDest)) {
          ResetReadOnly(xDest);
        }
        // We dont ref the X# asm directly because then we could not compile it without dynamic loading.
        // This way we can build it and call it directly.
        StartConsole(mOutputPath + @"\xsc.exe", Quoted(xFile) + @" Cosmos.Debug.DebugStub");
      }

      Section("Compiling Cosmos");
      MsBuild(mCosmosPath + @"\source\Cosmos.sln", "Builder");

      CD(mOutputPath);

      Section("Copying Templates");
      // Copy templates
      // .iss does some of this as well.. why some here? And why is VB disabled in .iss?
      SrcPath = mCosmosPath + @"\source2\VSIP\Cosmos.VS.Package\obj\x86\Debug";
      Copy("CosmosProject (C#).zip");
      Copy("CosmosKernel (C#).zip");
      Copy("CosmosProject (F#).zip");
      Copy("Cosmos.zip");
      Copy("CosmosProject (VB).zip");
      Copy("CosmosKernel (VB).zip");
      Copy(mCosmosPath + @"\source2\VSIP\Cosmos.VS.XSharp\Template\XSharpFileItem.zip");

      Section("Creating Setup");
      if (!File.Exists(Paths.ProgFiles32 + @"\Inno Setup 5\ISCC.exe")) {
        throw new Exception("Cannot find Inno setup.");
      }
      StartConsole(Paths.ProgFiles32 + @"\Inno Setup 5\ISCC.exe", @"/Q " + Quoted(mCosmosPath + @"\Setup2\Cosmos.iss") + " /dBuildConfiguration=Devkit");

      Section("Running Setup");
      Start(mCosmosPath + @"\Setup2\Output\CosmosUserKit.exe", @"/SILENT");

      Section("Launching Visual Studio");
      string xVisualStudio = Paths.ProgFiles32 + @"\Microsoft Visual Studio 10.0\Common7\IDE\devenv.exe";
      if (!File.Exists(xVisualStudio)) {
        throw new Exception("Cannot find Visual Studio.");
      }

      if (ResetHive) {
        Echo("Resetting hive");
        Start(xVisualStudio, @"/setup /rootsuffix Exp /ranu");
      }

      Echo("Launching Visual Studio");
      Start(xVisualStudio, mCosmosPath + @"\source\Cosmos.sln", false);
    }
  }
}
