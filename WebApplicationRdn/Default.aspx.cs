using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplicationRdn
{
    public partial class RenginePage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Rdn.Init();
        }

        protected void btSubmitStatement_Click(object sender, EventArgs e)
        {
            var statements = this.tbStatement.Text;
            var lines = statements.Replace(Environment.NewLine, "\n").Split('\n');
            foreach (var line in lines)
            {
                if (line.Trim() != "")
                {
                    string result = Rdn.Evaluate(line);
                    this.tbResult.Text = string.Concat(this.tbResult.Text, Environment.NewLine, result);
                }
            }
            this.tbStatement.Text = string.Empty;
        }

        protected void btSubmitScript_Click(object sender, EventArgs e)
        {
            using (var content = new StreamReader(fuRscript.FileContent))
            {
                var txt = content.ReadToEnd();
                var rfn = txt.Replace(@"\", "/");
                string result = Rdn.Evaluate("source(" + rfn +")");
                this.tbResult.Text = string.Concat(this.tbResult.Text, Environment.NewLine, result);
            }

        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            this.tbResult.Text = string.Empty;
        }

        protected void btnSetCStack_Click(object sender, EventArgs e)
        {
            Rdn.SetCStackLimit();
        }
    }
}