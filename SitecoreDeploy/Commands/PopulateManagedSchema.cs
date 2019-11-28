using Sitecore.Diagnostics;
using Sitecore.ContentSearch.Maintenance;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System;
using Sitecore.ContentSearch;

namespace SitecoreDeploy.Commands
{
    public class PopulateManagedSchema : SitecoreDeployCommand
    {
        public PopulateManagedSchema() : base("PopulateManagedSchema") { }

        public override SitecoreDeployCommandArguments Execute(SitecoreDeployCommandArguments args)
        {
            Assert.ArgumentNotNull(args, nameof(args));

            args.Result = "";

            string str = args.Context["Indexes"];

            List<string> list = new List<string>((IEnumerable<string>)str.Split(',', '|')).Where<string>((Func<string, bool>)(l => !string.IsNullOrEmpty(l))).Select<string, string>((Func<string, string>)(l => Sitecore.Web.WebUtil.RemoveAllScripts(l.ToLower()))).ToList<string>();
            StringBuilder stringBuilder = new StringBuilder();
            try
            {
                bool flag = str.ToLower().Equals("all");
                Log.Audit(string.Format("Start Populating Managed Schema from Admin Page, indexes: {0}", (object)string.Join(", ", list.ToArray())), (object)this);
                foreach (ISearchIndex index in ContentSearchManager.Indexes)
                {
                    if (flag || list.Contains(index.Name.ToLower()))
                        SchemaCustodian.PopulateManagedSchema(index, true);
                }
                stringBuilder.Append(" OK<br/>");
            }
            catch (Exception ex)
            {
                stringBuilder.Append("<textarea class='exception'>" + (object)ex + "</textarea><br/>");
            }
            args.Result += stringBuilder.ToString();
            
            return args;
        }
    }
}