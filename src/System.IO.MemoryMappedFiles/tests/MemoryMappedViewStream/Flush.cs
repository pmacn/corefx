// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using Xunit;

public class MMVS_Flush
{
    private int _iCountTestcases = 0;
    private int _iCountErrors = 0;

    [Fact]
    public static void FlushTestCases()
    {
        bool bResult = false;
        MMVS_Flush test = new MMVS_Flush();

        try
        {
            bResult = test.runTest();
        }
        catch (Exception exc_main)
        {
            bResult = false;
            Console.WriteLine("Fail! Error Err_9999zzz! Uncaught Exception in main(), exc_main==" + exc_main.ToString());
        }

        Assert.True(bResult, "One or more test cases failed.");
    }

    public bool runTest()
    {
        try
        {
            using (MemoryMappedFile mmf = MemoryMappedFile.CreateNew("MMVS_Flush0", Int16.MaxValue))
            {
                using (MemoryMappedViewStream view = mmf.CreateViewStream())
                {
                    _iCountTestcases++;
                    view.Flush();
                }

                // Stream does not support reading
                using (MemoryMappedViewStream view = mmf.CreateViewStream(0, 0, MemoryMappedFileAccess.Read))
                {
                    _iCountTestcases++;
                    view.Flush();
                }

                // Call after view has been disposed
                MemoryMappedViewStream view1 = mmf.CreateViewStream();
                view1.Dispose();
                VerifyFlushException<ObjectDisposedException>("Loc101", view1);
            }

            /// END TEST CASES

            if (_iCountErrors == 0)
            {
                Console.WriteLine("Pass. iCountTestcases==" + _iCountTestcases);
                return true;
            }
            else
            {
                Console.WriteLine("Fail! iCountErrors==" + _iCountErrors);
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("ERR999: Unexpected exception in runTest, {0}", ex);
            return false;
        }
    }

    /// START HELPER FUNCTIONS
    public void VerifyFlushException<EXCTYPE>(String strLoc, MemoryMappedViewStream view) where EXCTYPE : Exception
    {
        _iCountTestcases++;
        try
        {
            view.Flush();
            _iCountErrors++;
            Console.WriteLine("ERROR, {0}: No exception thrown, expected {1}", strLoc, typeof(EXCTYPE));
        }
        catch (EXCTYPE ex)
        {
            Console.WriteLine("{0}: Expected, {1}: {2}", strLoc, ex.GetType(), ex.Message);
        }
        catch (Exception ex)
        {
            _iCountErrors++;
            Console.WriteLine("ERROR, {0}: Unexpected exception, {1}", strLoc, ex);
        }
    }
}