namespace HashtagIT.Services.Data
{
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    public interface IRandomizeService
    {
       string Randomize(int hashtagSetId);
    }
}
