﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TracerX;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace TestApp {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private DomainCallbacks _domCallbacks = new DomainCallbacks();
        private Logger Log = Logger.GetLogger("TestApp");

        // Note: When testing in the default domain, manually reinitialize Logger settings
        // that might have been set in another test.


        private void emptyBtn_Click(object sender, EventArgs e) {
            AppDomain otherDomain = AppDomain.CreateDomain("Empty");
            otherDomain.DoCallBack(new CrossAppDomainDelegate(_domCallbacks.Empty));
            AppDomain.Unload(otherDomain);
        }
       
        private void oneLineBtn_Click(object sender, EventArgs e) {
            AppDomain otherDomain = AppDomain.CreateDomain("OneLine");
            otherDomain.DoCallBack(new CrossAppDomainDelegate(_domCallbacks.OneLine));
            AppDomain.Unload(otherDomain);
        }

        private void button1_Click(object sender, EventArgs e) {
            AppDomain otherDomain = AppDomain.CreateDomain("FiveThreadsNonCircular");
            otherDomain.DoCallBack(new CrossAppDomainDelegate(_domCallbacks.FiveThreadsNonCircular));
            AppDomain.Unload(otherDomain);
        }

        private void button2_Click(object sender, EventArgs e) {
            AppDomain otherDomain = AppDomain.CreateDomain("StopAtCircular");
            otherDomain.DoCallBack(new CrossAppDomainDelegate(_domCallbacks.StopAtCircular));
            AppDomain.Unload(otherDomain);
        }

        private void button3_Click(object sender, EventArgs e) {
            AppDomain otherDomain = AppDomain.CreateDomain("CircularWith1Block");
            otherDomain.DoCallBack(new CrossAppDomainDelegate(_domCallbacks.CircularWith1Block));
            AppDomain.Unload(otherDomain);
        }

        private void button4_Click(object sender, EventArgs e) {
            AppDomain otherDomain = AppDomain.CreateDomain("OneThreadWraps");
            otherDomain.DoCallBack(new CrossAppDomainDelegate(_domCallbacks.OneThreadWraps));
            AppDomain.Unload(otherDomain);
        }

        private void button5_Click(object sender, EventArgs e) {
            AppDomain otherDomain = AppDomain.CreateDomain("FiveThreadsWrap");
            otherDomain.DoCallBack(new CrossAppDomainDelegate(_domCallbacks.FiveThreadsWrap));
            AppDomain.Unload(otherDomain);
        }

        private void button6_Click(object sender, EventArgs e) {
            AppDomain otherDomain = AppDomain.CreateDomain("StartCircularByTime");
            otherDomain.DoCallBack(new CrossAppDomainDelegate(_domCallbacks.StartCircularByTime));
            AppDomain.Unload(otherDomain);
        }

        private void button7_Click(object sender, EventArgs e) {
            AppDomain otherDomain = AppDomain.CreateDomain("MissingMethodEntry");
            otherDomain.DoCallBack(new CrossAppDomainDelegate(_domCallbacks.MissingMethodEntry));
            AppDomain.Unload(otherDomain);
        }

        private void button8_Click(object sender, EventArgs e) {
            AppDomain otherDomain = AppDomain.CreateDomain("MissingMethodExit");
            otherDomain.DoCallBack(new CrossAppDomainDelegate(_domCallbacks.MissingMethodExit));
            AppDomain.Unload(otherDomain);
        }

        private void button9_Click(object sender, EventArgs e) {
            AppDomain otherDomain = AppDomain.CreateDomain("MissingEntriesAndExits");
            otherDomain.DoCallBack(new CrossAppDomainDelegate(_domCallbacks.MissingEntriesAndExits));
            AppDomain.Unload(otherDomain);
        }

        private void button10_Click(object sender, EventArgs e) {
            AppDomain otherDomain = AppDomain.CreateDomain("AllUnicodeChars");
            otherDomain.DoCallBack(new CrossAppDomainDelegate(_domCallbacks.AllUnicodeChars));
            AppDomain.Unload(otherDomain);
        }

        private void button11_Click(object sender, EventArgs e) {
            AppDomain otherDomain = AppDomain.CreateDomain("MoreThanUintMaxLines");
            otherDomain.DoCallBack(new CrossAppDomainDelegate(_domCallbacks.MoreThanUintMaxLines));
            AppDomain.Unload(otherDomain);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            WaitCallback wc = new WaitCallback(o =>
             {
                 AppDomain otherDomain = AppDomain.CreateDomain("ControlledLogging");
                 otherDomain.DoCallBack(new CrossAppDomainDelegate(_domCallbacks.ControlledLogging));
                 AppDomain.Unload(otherDomain);
             });

            ThreadPool.QueueUserWorkItem(wc);
        }

        private void btnTimeOneMillionWithWrapping_Click(object sender, EventArgs e) {
            Cursor = Cursors.WaitCursor;

            Logger.BinaryFileLogging.Close();
            Logger.BinaryFileLogging.Name = "TimeOneMillionWithWrapping";
            Logger.BinaryFileLogging.Directory = "%EXEDIR%";
            Logger.BinaryFileLogging.Use_00 = true;
            Logger.BinaryFileLogging.Archives = 3;
            Logger.BinaryFileLogging.CircularStartDelaySeconds = 0;
            Logger.BinaryFileLogging.CircularStartSizeKb = 1;
            Logger.BinaryFileLogging.MaxSizeMb = 1;
            Logger.BinaryFileLogging.AppendIfSmallerThanMb = 0;
            Logger.BinaryFileLogging.Password = null;

            LogOneMillion();

            Cursor = Cursors.Default;
        }

        private void btnTimeOneMillionWithoutWrapping_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;

            Logger.BinaryFileLogging.Close();
            Logger.BinaryFileLogging.Name = "TimeOneMillionWithoutWrapping";
            Logger.BinaryFileLogging.Directory = "%EXEDIR%";
            Logger.BinaryFileLogging.Use_00 = true;
            Logger.BinaryFileLogging.Archives = 3;
            Logger.BinaryFileLogging.CircularStartDelaySeconds = 0;
            Logger.BinaryFileLogging.CircularStartSizeKb = 0;
            Logger.BinaryFileLogging.MaxSizeMb = 100;
            Logger.BinaryFileLogging.AppendIfSmallerThanMb = 0;
            Logger.BinaryFileLogging.Password = null;

            LogOneMillion();

            Cursor = Cursors.Default;
        }

        private void LogOneMillion()
        {

            if (Thread.CurrentThread.Name == null) Thread.CurrentThread.Name = "Main";
            Logger.BinaryFileLogging.Open();
            MessageBox.Show("Opened " + Logger.BinaryFileLogging.FullPath);

            int i;
            const int num = 1000000;
            Stopwatch sw = new Stopwatch();
            sw.Start();


            for (i = 0; i < num; ++i)
            {
                Log.Info("i = ", i);
            }

            sw.Stop();
            Log.Info("Elapased time for ", i.ToString("N0"), " messages = ", sw.Elapsed.TotalSeconds);
            Logger.BinaryFileLogging.Close();

            MessageBox.Show("Seconds = " + sw.Elapsed.TotalSeconds.ToString() + ", Records/sec = " + num / sw.Elapsed.TotalSeconds);
        }

        private void button15_Click(object sender, EventArgs e) {
            Cursor = Cursors.WaitCursor;
            AppDomain otherDomain = AppDomain.CreateDomain("RandomCalls");
            otherDomain.DoCallBack(new CrossAppDomainDelegate(_domCallbacks.RandomCall));
            AppDomain.Unload(otherDomain);
            Cursor = Cursors.Default;
        }

        private void createAppendBtn_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            SetAppendMode();
            AppDomain otherDomain = AppDomain.CreateDomain("CreateAppend");

            if (textFileChk.Checked) {
                otherDomain.DoCallBack(new CrossAppDomainDelegate(_domCallbacks.CreateTextFileForAppending));
            } else {
                otherDomain.DoCallBack(new CrossAppDomainDelegate(_domCallbacks.CreateFileForAppending));
            }

            AppDomain.Unload(otherDomain);
            Cursor = Cursors.Default;
        }

        private void appendToFileBtn_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            SetAppendMode();
            AppDomain otherDomain = AppDomain.CreateDomain("AppendToFile");

            if (textFileChk.Checked) {
                otherDomain.DoCallBack(new CrossAppDomainDelegate(_domCallbacks.AppendToTextFile));
            } else {
                otherDomain.DoCallBack(new CrossAppDomainDelegate(_domCallbacks.AppendToFile));
            } 

            AppDomain.Unload(otherDomain);
            Cursor = Cursors.Default;
        }

        private void SetAppendMode() {
            if (emptyRad.Checked) _domCallbacks.AppendTestMode = DomainCallbacks.AppendMode.Empty;
            if (noCircularRad.Checked) _domCallbacks.AppendTestMode = DomainCallbacks.AppendMode.NoCircular;
            if (noWrapRad.Checked) _domCallbacks.AppendTestMode = DomainCallbacks.AppendMode.CircularNoWrap;
            if (circularWrapsRad.Checked) _domCallbacks.AppendTestMode = DomainCallbacks.AppendMode.CircularWrap;
            if (exceedMaxMbRad.Checked) _domCallbacks.AppendTestMode = DomainCallbacks.AppendMode.ExceedMaxMb;

            _domCallbacks.MaxAppendableFileMb = uint.Parse(maxAppendSizeBox.Text);
            _domCallbacks.MaxSessionSizeMb = uint.Parse(maxSessionSizeBox.Text);
            _domCallbacks.CircularStartKb = uint.Parse(circularStartSizeBox.Text);
            _domCallbacks.Use_00 = use00Chk.Checked;
            _domCallbacks.UseKbForSize = chkUseKb.Checked;
        }

        private void maxAppendSizeBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void textWriterTraceListenerBtn_Click(object sender, EventArgs e)
        {
            AppDomain otherDomain = AppDomain.CreateDomain("TextWriter");
            otherDomain.DoCallBack(new CrossAppDomainDelegate(_domCallbacks.TextWriter));
            AppDomain.Unload(otherDomain);
        }

        private void readConfigBtn_Click(object sender, EventArgs e) {
            AppDomain otherDomain = AppDomain.CreateDomain("ReadConfig");
            otherDomain.DoCallBack(new CrossAppDomainDelegate(_domCallbacks.LoadConfig));
            AppDomain.Unload(otherDomain);
        }

        private void button16_Click(object sender, EventArgs e)
        {
            AppDomain otherDomain = AppDomain.CreateDomain("AllDestinations");
            otherDomain.DoCallBack(new CrossAppDomainDelegate(_domCallbacks.LogToAllDestinations));
            AppDomain.Unload(otherDomain);
        }

        private void button17_Click(object sender, EventArgs e)
        {
            viewerWrapper1.LoadFile(textBox1.Text);
        }

        private void button18_Click(object sender, EventArgs e)
        {
            viewerWrapper1.CloseFile();
        }

        private void changingThreadNameBtn_Click(object sender, EventArgs e) {
            AppDomain otherDomain = AppDomain.CreateDomain("ThreadNames");
            otherDomain.DoCallBack(new CrossAppDomainDelegate(_domCallbacks.ThreadNames));
            AppDomain.Unload(otherDomain);
        }

        private void btnCrossAppDomains_Click(object sender, EventArgs e)
        {
            AppDomain otherDomain = AppDomain.CreateDomain("CrossAppDomains");
            otherDomain.DoCallBack(_domCallbacks.CrossAppDomains);

            // These two loggers should have already been created by the other domain in the callback.
            Logger log1 = Logger.GetLogger("TestApp", otherDomain);
            Logger log2 = Logger.GetLogger("Local Log", otherDomain);

            // This should createa new logger in the other domain, and our calls to it should
            // write to the other domain's log file.
            Logger log3 = Logger.GetLogger("Remote Log", otherDomain);

            log1.Info("From remote domain.");
            log2.Info("Also from remote domain.");
            log3.Info("Again from remote domain.");

            using (
                log3.InfoCall())
            {
                log2.Info("This should be inside a method call.");

                try {
                    throw new Exception("Testing if exception can be passed to other domain for logging.");
                } catch (Exception ex) {
                    log3.Error("Logging an exception: ", ex);
                }
            }

            AppDomain.Unload(otherDomain);
        }

        private void btnReopenNoncircular_Click(object sender, EventArgs e)
        {
            Logger.BinaryFileLogging.Close();
            Logger.BinaryFileLogging.Name = "ReOpen Non-Circular";
            Logger.BinaryFileLogging.Directory = "%EXEDIR%";
            Logger.BinaryFileLogging.Use_00 = true;
            Logger.BinaryFileLogging.Archives = 3;
            Logger.BinaryFileLogging.CircularStartDelaySeconds = 0;
            Logger.BinaryFileLogging.CircularStartSizeKb = 0;
            Logger.BinaryFileLogging.MaxSizeMb = 1;
            Logger.BinaryFileLogging.AppendIfSmallerThanMb = 0;
            Logger.BinaryFileLogging.Password = null;

            Logger.BinaryFileLogging.Opening += new CancelEventHandler(BinaryFileLogging_Opening);
            Logger.BinaryFileLogging.Opened += new EventHandler(BinaryFileLogging_Opened);
            Logger.BinaryFileLogging.Closing += new EventHandler(BinaryFileLogging_Closing);
            Logger.BinaryFileLogging.Closed += new EventHandler(BinaryFileLogging_Closed);

            Logger Log2 = Logger.GetLogger("Log2");

            // Test what happens when method-entries are logged before the file is opened.
            using (Log.InfoCall("Meth-1"))
            {
                using (Log.InfoCall("Meth-2"))
                {
                    Log.Info("Logged inside Meth-2 before opening file");
                    Logger.BinaryFileLogging.Open();
                    Log.Info("Logged inside Meth-2 after opening first file");
                }
            }

            Log.Info("Not inside a method.");
            Log.Info("Closing the file.");
            Logger.BinaryFileLogging.CloseAndReopen();
            Log.Info("Opened second file, entering a method.");

            using (Log.InfoCall("Meth-3"))
            {
                using (Log.InfoCall("Meth-4"))
                {
                    Log.Info("Closing file while in Meth-4.");
                    Logger.BinaryFileLogging.CloseAndReopen();
                    Log.Info("Logged inside Meth-4 after opening third file.");
                }
            }

            Log.Info("Closing last file.");
            Logger.BinaryFileLogging.Close();

            MessageBox.Show("Closed file: " + Logger.BinaryFileLogging.FullPath);

            Logger.BinaryFileLogging.Opening -= new CancelEventHandler(BinaryFileLogging_Opening);
            Logger.BinaryFileLogging.Opened -= new EventHandler(BinaryFileLogging_Opened);
            Logger.BinaryFileLogging.Closing -= new EventHandler(BinaryFileLogging_Closing);
            Logger.BinaryFileLogging.Closed -= new EventHandler(BinaryFileLogging_Closed);
        }

        void BinaryFileLogging_Opening(object sender, CancelEventArgs e)
        {
            Debug.Print("Log file opening.");
        }

        void BinaryFileLogging_Opened(object sender, EventArgs e)
        {
            Debug.Print("Log file opened.");
            Log.Info("BinaryFileLogging_Opened called");
        }

        void BinaryFileLogging_Closing(object sender, EventArgs e)
        {
            Debug.Print("Log file closing.");
            Log.Info("BinaryFileLogging_Closing called");
        }

        void BinaryFileLogging_Closed(object sender, EventArgs e)
        {
            Debug.Print("Log file closed.");
        }

        private void btnReopenCircular_Click(object sender, EventArgs e)
        {
            Logger.BinaryFileLogging.Close();
            Logger.BinaryFileLogging.Name = "ReOpen Circular";
            Logger.BinaryFileLogging.Directory = "%EXEDIR%";
            Logger.BinaryFileLogging.Use_00 = true;
            Logger.BinaryFileLogging.Archives = 3;
            Logger.BinaryFileLogging.CircularStartDelaySeconds = 0;
            Logger.BinaryFileLogging.CircularStartSizeKb = 1;
            Logger.BinaryFileLogging.MaxSizeMb = 1;
            Logger.BinaryFileLogging.AppendIfSmallerThanMb = 0;
            Logger.BinaryFileLogging.Password = null;

            Logger Log2 = Logger.GetLogger("Log2");

            // Test what happens when method-entries are logged before the file is opened.
            using (Log.InfoCall("Meth-1"))
            {
                using (Log.InfoCall("Meth-2"))
                {
                    Log.Info("Logged inside Meth-2 before opening file");
                    Logger.BinaryFileLogging.Open();
                    Log.Info("Logged inside Meth-2 after opening first file");
                }
            }

            Log.Info("Logging till we wrap.");

            while (!Logger.BinaryFileLogging.Wrapped)
            {
                Log.Info("Logging till we wrap.");
            }

            Log.Info("Closing the file.");
            Logger.BinaryFileLogging.CloseAndReopen();
            Log.Info("Opened second file, entering a method.");

            using (Log.InfoCall("Meth-3"))
            {
                while (!Logger.BinaryFileLogging.Wrapped)
                {
                    Log.Info("Logging till we wrap.");
                }

                Log.Info("Closing file while in Meth-3.");
                Logger.BinaryFileLogging.CloseAndReopen();
                Log.Info("Logged inside Meth-3 after opening third file.");
            }

            Log.Info("Closing last file.");
            Logger.BinaryFileLogging.Close();

            MessageBox.Show("Closed file: " + Logger.BinaryFileLogging.FullPath);
        }

        private void btnAutoReopen_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;

            Logger.BinaryFileLogging.Close();
            Logger.BinaryFileLogging.Name = "Auto-ReOpen";
            Logger.BinaryFileLogging.Directory = "%EXEDIR%";
            Logger.BinaryFileLogging.Use_00 = true;
            Logger.BinaryFileLogging.Archives = 4;
            Logger.BinaryFileLogging.FullFilePolicy = FullFilePolicy.CloseAndReopen;
            Logger.BinaryFileLogging.CircularStartDelaySeconds = 1;
            Logger.BinaryFileLogging.CircularStartSizeKb = 1;
            Logger.BinaryFileLogging.MaxSizeMb = 1;
            Logger.BinaryFileLogging.AppendIfSmallerThanMb = 0;
            Logger.BinaryFileLogging.Password = null;

            Logger odd = Logger.GetLogger("odd");
            Logger even = Logger.GetLogger("even");
            const int threadCount = 5;
            Semaphore startsem = new Semaphore(0, threadCount);
            Semaphore stopsem = new Semaphore(0, threadCount);
            int fileCountOffset = Logger.BinaryFileLogging.CurrentFile;

            EventHandler opened = (source, args) => Log.Info("Opened file ", Logger.BinaryFileLogging.CurrentFile - fileCountOffset);

            WaitCallback wc = new WaitCallback(
                (object num) =>
            {
                int n = (int)num;
                Logger.ThreadName = "Th " + n;

                // Wait for main thread to release this thread.
                startsem.WaitOne();

                using (Log.InfoCall("Meth " + n)) {
                    for (int i = 0; i < 50000; ++i)
                    {
                        if (i % 2 == 0) even.Info("i = ", i);
                        else odd.Debug("i = ", i);
                        Thread.Sleep(0);
                    }
                }

                // Inform main thread we're done.
                stopsem.Release();
            }
            );

            odd.BinaryFileTraceLevel = TracerX.TraceLevel.Verbose;

            // Start the threads.
            for (int i = 0; i < threadCount; ++i)
            {
                ThreadPool.QueueUserWorkItem(wc, i);
            }

            Logger.BinaryFileLogging.Opened += opened;
            Logger.BinaryFileLogging.Open();

            // Release the threads.
            Thread.Sleep(500);
            startsem.Release(threadCount);

            // Wait for threads to end.
            for (int i = 0; i < threadCount; ++i)
            {
                stopsem.WaitOne();
            }

            Logger.BinaryFileLogging.Close();
            Logger.BinaryFileLogging.Opened -= opened;

            Cursor = Cursors.Default;
        }

        private void btnEventHandler_Click(object sender, EventArgs e)
        {
            // This tests the EventHandler destination, including the ability
            // to cancel log messages.  It logs to both the EventHandler destination
            // and the BinaryFile destination.  Examine the binary file to verify
            // that some messages were cancelled.
            Cursor = Cursors.WaitCursor;

            Logger.BinaryFileLogging.Close(); // Just to be sure.
            Logger.BinaryFileLogging.Name = "EventHandlerTest";
            Logger.BinaryFileLogging.Directory = "%EXEDIR%";
            Logger.BinaryFileLogging.Use_00 = true;
            Logger.BinaryFileLogging.Archives = 1;
            Logger.BinaryFileLogging.FullFilePolicy = FullFilePolicy.Wrap;
            Logger.BinaryFileLogging.CircularStartDelaySeconds = 0;
            Logger.BinaryFileLogging.CircularStartSizeKb = 0;
            Logger.BinaryFileLogging.MaxSizeMb = 1;
            Logger.BinaryFileLogging.AppendIfSmallerThanMb = 0;
            Logger.BinaryFileLogging.Password = null;

            EventHandler<Logger.MessageCreatedEventArgs> handler =
                (source, args) =>
                {
                    if (args.Message.StartsWith("should be cancelled"))
                    {
                        args.Cancel = true;
                    }
                    else if (args.Method == "GoodMethod")
                    {
                        if (args.MethodExiting)
                        {
                            // Should have no effect.
                            args.Cancel = true;
                        }
                    }
                    else if (args.Method == "BadMethod")
                    {
                        if (args.MethodEntered)
                        {
                            // Should suppress both method-entry and method-exit lines,
                            // but not regular lines.
                            args.Cancel = true;
                        }
                    }
                };

            Logger.MessageCreated += handler;

            Log.BinaryFileTraceLevel = TracerX.TraceLevel.Verbose;
            Log.EventHandlerTraceLevel = TracerX.TraceLevel.Verbose;

            Logger.BinaryFileLogging.Open();

            Log.Info("Messages starting with 'should be cancelled' should not appear.");
            Log.Info("should be cancelled: this should not appear in the binary log.");

            Log.Info("The method-entry and method-exit lines for method 'GoodMethod' should appear.");

            using (Log.InfoCall("GoodMethod")) {
                Log.Info("Message from inside 'GoodMethod'.");
            }

            Log.Info("The method-entry and method-exit lines for method 'BadMethod' should not appear.");

            using (Log.InfoCall("BadMethod")) {
                Log.Info("Message from inside 'BadMethod'.");
            }

            Logger.MessageCreated -= handler;
            Logger.BinaryFileLogging.Close();
            Cursor = Cursors.Default;
            MessageBox.Show("Created log file " + Logger.BinaryFileLogging.FullPath);
        }

    }
}