using Nuna.Lib.CleanArchHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ofta.Application.DocContext.DocAgg.Contracts;


public record CopyFileRequest(string FilePathName, string FilePathOriginName);

public interface ICopyFileService : INunaService<string, CopyFileRequest>
{

}

