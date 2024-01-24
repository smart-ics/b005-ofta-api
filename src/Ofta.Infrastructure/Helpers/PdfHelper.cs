using iText.Kernel.Pdf;

namespace Ofta.Infrastructure.Helpers;

public class PdfHelper
{
    public static int GetPageCount(string filePath)
    {
        try
        {
            using var pdfReader = new PdfReader(filePath);
            using var pdfDocument = new PdfDocument(pdfReader);
            return pdfDocument.GetNumberOfPages();
        }
        catch (Exception)
        {
            return 1; // default to 1 page
        }
    }    
}
