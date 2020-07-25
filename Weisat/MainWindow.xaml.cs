// Copyright (c) Hannes Barbez. All rights reserved.
// Licensed under the GNU General Public License v3.0

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Management.Automation;
using System.Windows;
using System.Windows.Media;

namespace Weisat
{
    public partial class MainWindow : Window
    {
        readonly List<PSMemberInfo> interestingMembers = new List<PSMemberInfo>();
        private readonly BackgroundWorker bw = new BackgroundWorker();
        private const string 
            HOMEPAGE = "http://www.barbez.eu/",
            PLUGINCOMPUTER = "The assessment will not run on battery power. Please connect the computer's charger, then hit OK.",
            PLUGINCOMPUTERTITLE = "Connect your charger";

        public MainWindow()
        {
            InitializeComponent();
            bw.DoWork += Bw_DoWork;
            bw.RunWorkerCompleted += Bw_RunWorkerCompleted;
        }

        void Bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SetBaseScore();

            try
            {
                if (this.interestingMembers.Count == 5)
                {
                    foreach (PSMemberInfo member in this.interestingMembers)
                    {
                        if (member.Name == "CPUScore") lblProcessorScore.Content = GetMemberInfoAsString(member);
                        if (member.Name == "D3DScore") lblGamingGraphicsScore.Content = GetMemberInfoAsString(member);
                        if (member.Name == "DiskScore") lblPrimaryHardDiskScore.Content = GetMemberInfoAsString(member);
                        if (member.Name == "GraphicsScore") lblGraphicsScore.Content = GetMemberInfoAsString(member);
                        if (member.Name == "MemoryScore") lblMemoryScore.Content = GetMemberInfoAsString(member);
                    }
                }
            }
            catch { }

            PowerCheck();
            HighlightBaseScore();
            progressBar.Visibility = Visibility.Hidden;
            btnRunAssessment.Visibility = Visibility.Visible;
        }

        private float GetMemberInfoAsFloat(PSMemberInfo member)
        {
            float.TryParse(GetMemberInfoAsString(member), out float result);
            return result;
        }

        private string GetMemberInfoAsString(PSMemberInfo member)
        {
            var memberValue = (float)member.Value;

            //Fix for bug 326:
            memberValue = (float)Math.Round((Decimal)memberValue, 1, MidpointRounding.AwayFromZero);

            var memberValueString = memberValue.ToString();

            //Fix for bug 327:
            if (memberValueString.Length > 1 && !(memberValueString.Contains(",") || memberValueString.Contains(".")))
                memberValueString = memberValueString[0] + "." + memberValueString[1];

            return memberValueString;
        }

        private void HighlightBaseScore()
        {
            if ((string)lblProcessorScore.Content == ((string)lblBaseScore.Content))
                lblProcessorScore.Background = new SolidColorBrush(Color.FromRgb(228, 231, 242));
            if ((string)lblGamingGraphicsScore.Content == (string)lblBaseScore.Content) lblGamingGraphicsScore.Background = new SolidColorBrush(Color.FromRgb(228, 231, 242));
            if ((string)lblPrimaryHardDiskScore.Content == (string)lblBaseScore.Content) lblPrimaryHardDiskScore.Background = new SolidColorBrush(Color.FromRgb(228, 231, 242));
            if ((string)lblGraphicsScore.Content == (string)lblBaseScore.Content) lblGraphicsScore.Background = new SolidColorBrush(Color.FromRgb(228, 231, 242));
            if ((string)lblMemoryScore.Content == (string)lblBaseScore.Content) lblMemoryScore.Background = new SolidColorBrush(Color.FromRgb(228, 231, 242));
        }

        private void SetBaseScore()
        {
            try
            {
                float lowest = 9000;

                foreach (PSMemberInfo member in this.interestingMembers)
                {
                    float f = GetMemberInfoAsFloat(member);
                    if (f < lowest) lowest = f;
                }

                lblBaseScore.Content = lowest.ToString();
            }
            catch { }
        }

        void Bw_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                CmdPart();
                PsPart();
            }
            catch { }
        }

        private void PowerCheck()
        {
            if (((string)lblBaseScore.Content)[0] == '0')
            {
                MessageBoxResult mbr = MessageBox.Show(PLUGINCOMPUTER, PLUGINCOMPUTERTITLE, MessageBoxButton.OKCancel, MessageBoxImage.Information);
                if (mbr == MessageBoxResult.OK)
                    RunExternalProcess(GetThisAppsName(), ""); //"Restart" myself
                this.Close();
            }
        }

        private string GetThisAppsName()
        {
            string file = this.GetType().Assembly.Location;
            return Path.GetFileNameWithoutExtension(file) + ".exe";
        }

        private void PsPart()
        {
            using (PowerShell PowerShellInstance = PowerShell.Create())
            {
                PowerShellInstance.AddScript("Get-WmiObject -Class Win32_WinSAT");
                Collection<PSObject> PSOutput = PowerShellInstance.Invoke();
                PSMemberInfoCollection<PSMemberInfo> members = PSOutput[0].Members;
                foreach (PSMemberInfo member in members)
                {
                    if (member != null)
                        if (member.Value != null)
                            if (member.Name == "CPUScore" | member.Name == "D3DScore" | member.Name == "DiskScore" | member.Name == "GraphicsScore" | member.Name == "MemoryScore")
                                this.interestingMembers.Add(member);
                }
            }
        }

        private void CmdPart()
        {
            ProcessStartInfo cmdStartInfo = new ProcessStartInfo
            {
                FileName = @"winsat",
                Arguments = "formal prepop",
                Verb = "runas",
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            Process cmdProcess = new Process
            {
                StartInfo = cmdStartInfo
            };
            cmdProcess.Start();
            cmdProcess.WaitForExit();
        }

        private void LblAbout_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            RunExternalProcess("explorer.exe", HOMEPAGE);
        }

        private static void RunExternalProcess(string appPath, string args)
        {
            ProcessStartInfo info = new ProcessStartInfo(appPath, @args)
            {
                UseShellExecute = false
            };

            Process p = new Process
            {
                StartInfo = info
            };
            p.Start();
        }

        private void BtnRunAssessment_Click(object sender, RoutedEventArgs e)
        {
                bw.RunWorkerAsync();
                progressBar.IsIndeterminate = true;
                btnRunAssessment.Visibility = Visibility.Hidden;
                progressBar.Visibility = Visibility.Visible;
        }
    }
}
