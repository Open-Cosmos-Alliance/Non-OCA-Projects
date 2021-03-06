﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using Vestris.VMWareLib;

namespace Cosmos.Debug.VSDebugEngine.Host {
  public abstract class VMWare : Base {
    protected string mVmxFile;
    protected abstract void ConnectToVMWare(VMWareVirtualHost aHost);

    public VMWare(string aVmxFile) {
      mVmxFile = aVmxFile;
    }

    protected static string GetPathname(string aKey, string aEXE) {
      using (var xRegKey = Registry.LocalMachine.OpenSubKey(@"Software\VMware, Inc.\" + aKey, false)) {
        if (xRegKey != null) {
          string xResult = Path.Combine(((string)xRegKey.GetValue("InstallPath")), aEXE);
          if (File.Exists(xResult)) {
            return xResult;
          }
        }
        return null;
      }
    }

    protected abstract string GetParams();

    public override string Start(string aIsoFile, bool aGDB) {
      string xPath = Path.Combine(PathUtilities.GetBuildDir(), @"VMWare\Workstation\");
      Cleanup();

      // VMWare doesn't like to boot a read only VMX.
      // We also need to make changes based on project / debug settings.
      // Finally we do not want to create VCS checkins based on local user changes.
      // Because of this we use Cosmos.vmx as a template and output a Debug.vmx on every run.
      using (var xSrc = new StreamReader(Path.Combine(xPath, "Cosmos.vmx"))) {
        try {
          // Write out Debug.vmx
          using (var xDest = new StreamWriter(mVmxFile)) {
            string xLine;
            while ((xLine = xSrc.ReadLine()) != null) {
              var xParts = xLine.Split('=');
              if (xParts.Length == 2) {
                string xName = xParts[0].Trim();
                string xValue = xParts[1].Trim();

                // We delete uuid entries so VMWare doenst ask the user "Did you move or copy" the file
                if ((xName == "uuid.location") || (xName == "uuid.bios")) {
                  xValue = null;
                } else if (xName == "ide1:0.fileName") {
                  xValue = "\"" + aIsoFile + "\"";
                } else if (xName == "nvram") {
                  xValue = "\"Debug.nvram\"";
                }

                if (xValue != null) {
                  xDest.WriteLine(xName + " = " + xValue);
                }
              }
            }
            if (aGDB) {
              xDest.WriteLine();
              xDest.WriteLine("debugStub.listen.guest32 = \"TRUE\"");
              xDest.WriteLine("debugStub.hideBreakpoints = \"TRUE\"");
              xDest.WriteLine("monitor.debugOnStartGuest32 = \"TRUE\"");
              xDest.WriteLine("debugStub.listen.guest32.remote = \"TRUE\"");
            }
          }
        } catch (IOException ex) {
          if (ex.Message.Contains(Path.GetFileName(mVmxFile))) {
            throw new Exception("The Vmware image " + mVmxFile + " is still in use! Please exit current Vmware session with Cosmos and try again.", ex);
          }
          throw ex;
        }
      }

      return "false " + GetParams();
    }

    public override void Stop() {
      using (var xHost = new VMWareVirtualHost()) {
        ConnectToVMWare(xHost);
        using (var xMachine = xHost.Open(mVmxFile)) {
          xMachine.PowerOff();
        }
        xHost.Close();
      }
      Cleanup();
    }

    protected void DeleteFiles(string aPath, string aPattern) {
      var xFiles = Directory.GetFiles(aPath, aPattern);
      foreach (var xFile in xFiles) {
        File.Delete(xFile);
      }
    }

    protected void Cleanup() {
      try {
        string xPath = Path.GetDirectoryName(mVmxFile);
        // Delete old Debug.vmx and other files that might be left over from previous run.
        // Especially important with newer versions of VMWare player which defaults to suspend
        // when the close button is used.
        File.Delete(mVmxFile);
        File.Delete(Path.ChangeExtension(mVmxFile, ".nvram"));
        // Delete the auto snapshots that latest vmware players create as default.
        // It creates them with suffixes though, so we need to wild card find them.
        DeleteFiles(xPath, "*.vmxf");
        DeleteFiles(xPath, "*.vmss");
        DeleteFiles(xPath, "*.vmsd");
        DeleteFiles(xPath, "*.vmem");
        // Delete log files so that logged data is only from last boot
        File.Delete(Path.Combine(xPath, "vmware.log"));
        File.Delete(Path.Combine(xPath, "vmware-0.log"));
        File.Delete(Path.Combine(xPath, "vmware-1.log"));
        File.Delete(Path.Combine(xPath, "vmware-2.log"));
      } catch (Exception ex) {
        // Ignore errors, users can stop VS while VMWare is still running and files will be locked.
      }
    }

  }
}
