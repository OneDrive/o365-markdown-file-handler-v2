using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileHandlerActions
{
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
