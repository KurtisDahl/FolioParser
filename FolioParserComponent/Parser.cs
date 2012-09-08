using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;

namespace FolioParserComponent
{
    public sealed class Parser
    {
        public IAsyncOperation<Issue> ParseIssue(string filePath)
        {
            return this.ParseIssueInternal(filePath).AsAsyncOperation<Issue>();
        }

        private async Task<Issue> ParseIssueInternal(string filePath)
        {
            IStorageFile file = await this.GetFileAsync(filePath);
            return await this.issueParser.ParseAsync(file);
        }

        public IAsyncOperation<Article> ParseArticle(string filePath)
        {
            return this.ParseArticleInternal(filePath).AsAsyncOperation<Article>();
        }

        private async Task<Article> ParseArticleInternal(string filePath)
        {
            IStorageFile file = await this.GetFileAsync(filePath);
            return await this.articleParser.ParseAsync(file);
        }

        private async Task<IStorageFile> GetFileAsync(string filePath)
        {
            var projLocation = Windows.ApplicationModel.Package.Current.InstalledLocation;
            IStorageFile file = await projLocation.GetFileAsync(filePath).AsTask();

            return file;
        }

        private IIssueFolioParser issueParser = new IssueFolioParser();
        private IArticleFolioParser articleParser = new ArticleFolioParser();
    }
}
