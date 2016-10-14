using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;

namespace KPMG.Models
{
    public class Upload
    {
        [ValidateFile(ErrorMessage = "Incorrect file format. Only CSV files are allowed.")]
        public HttpPostedFileBase FileData { get; set; }

        public int ImportedLines { get; set; }

        public ErrorList IgnoredLines { get; set; }

        public Upload()
        {
            this.IgnoredLines = new ErrorList();
        }
    }

    // Validation for the extension file type
    public class ValidateFileAttribute : RequiredAttribute
    {
        public override bool IsValid(object value)
        {
            var file = value as HttpPostedFileBase;
            if (file == null)
                return false;

            if (!file.FileName.ToUpper().EndsWith(".CSV"))
                return false;
            
            return true;
        }
    }


    // List to store info regarding lines to be ignored
    public class ErrorList : List<ImportError>
    {
        public ImportError this[string errorMessage]
        {
            get
            {
                return this.Where(x => x.ErrorDescription == errorMessage).FirstOrDefault();
            }
        }

        public void Add(string message, string record)
        {
            var errorObj = this[message];
            if (errorObj == null)
            {
                ImportError ie = new ImportError();
                ie.ErrorDescription = message;                
                ie.Lines.Add(record);
                this.Add(ie);
            }
            else
                errorObj.Lines.Add(record);
        }
     
    }

    // Each error type contains all the lines for the specific error
    public class ImportError
    {
        public string ErrorDescription { get; set; }
        public List<string> Lines { get; set; }

        public ImportError()
        {
            this.Lines = new List<string>();
        }
    }    


}