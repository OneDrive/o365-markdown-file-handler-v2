/*
 * Markdown File Handler - Sample Code
 * Copyright (c) Microsoft Corporation
 * All rights reserved. 
 * 
 * MIT License
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of 
 * this software and associated documentation files (the ""Software""), to deal in 
 * the Software without restriction, including without limitation the rights to use, 
 * copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
 * Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
 * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
 * PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION 
 * OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
 * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

namespace FileHandlerActions
{
    using System;

    public class ConverterException : Exception
    {
        public enum Reason
        {
            GenericError,
            ItemNotAFile,
            SourceFormatNotAcceptable,
            UploadError,
            UnableToFetchOriginalItem
        }

        public Reason Code { get; private set; }

        public ConverterException(Reason reason, Exception innerException = null)
            : base(GetMessage(reason), innerException)
        {
            this.Code = reason;   
        }

        public static string GetMessage(Reason reason)
        {
            switch(reason)
            {
                case Reason.ItemNotAFile:
                    return "Source item was not a file.";
                case Reason.SourceFormatNotAcceptable:
                    return "The source item format could not be converted.";
                case Reason.UploadError:
                    return "An error occured uploading the file to OneDrive.";
                case Reason.UnableToFetchOriginalItem:
                    return "Couldn't fetch the original item selected.";
                case Reason.GenericError:
                default:
                    return "Unknown error occured.";

            }

        }

    }

    
}
