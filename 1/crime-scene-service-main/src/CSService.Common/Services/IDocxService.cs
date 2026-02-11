using System.Collections.Generic;
using System.IO;
using CSService.Common.DataAccess.Entities;

namespace CSService.Common.Services;

public interface IDocxService
{
    Stream CreateReports(IEnumerable<Photo> photos, Comment comment);
}
