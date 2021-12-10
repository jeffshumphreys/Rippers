
using System.Linq.Expressions;
using System;
using System.Collections.Generic;

public PrecedenceData(SsisObject o)
{
    // Retrieve the two guids
    SsisObject prior = o.GetChildByTypeAndAttr("DTS:Executable", "DTS:IsFrom", "-1");
    BeforeGuid = Guid.Parse(prior.Attributes["IDREF"]);
    SsisObject posterior = o.GetChildByTypeAndAttr("DTS:Executable", "DTS:IsFrom", "0");
    AfterGuid = Guid.Parse(posterior.Attributes["IDREF"]);

    // Retrieve the expression to evaluate
    o.Properties.TryGetValue("Expression", out Expression);
}

public override string ToString()
{
    if (String.IsNullOrEmpty(Expression))
    {
        return String.Format(@"After ""{0}"" execute ""{1}""", SsisObject.GetObjectByGuid(BeforeGuid).GetFunctionName(), SsisObject.GetObjectByGuid(AfterGuid).GetFunctionName());
    }
    else
    {
        return String.Format(@"After ""{0}"", if ({2}), ""{1}""", SsisObject.GetObjectByGuid(BeforeGuid).GetFunctionName(), SsisObject.GetObjectByGuid(AfterGuid).GetFunctionName(), Expression);
    }
}


public Guid BeforeGuid;
public Guid AfterGuid;
public string Expression;
public SsisObject Target
{
    get
    {
        return SsisObject.GetObjectByGuid(AfterGuid);
    }

            // Retrieve the three settings from the for loop
            string init = System.Net.WebUtility.HtmlDecode(this.Properties["InitExpression"]).Replace("@", "");
    string eval = System.Net.WebUtility.HtmlDecode(this.Properties["EvalExpression"]).Replace("@","");
    string assign = System.Net.WebUtility.HtmlDecode(this.Properties["AssignExpression"]).Replace("@","");

    // Next, write all the executable functions to the main file
    var functions = from SsisObject c in o.Children where c.DtsObjectType == "DTS:Executable" select c;
    if (!functions.Any())
    {
        var executables = from SsisObject c in o.Children where c.DtsObjectType == "DTS:Executables" select c;
        List<SsisObject> flist = new List<SsisObject>();
        foreach (var exec in executables)
        {
            flist.AddRange(from e in exec.Children where e.DtsObjectType == "DTS:Executable" select e);

            var variables = from SsisObject c in o.Children where c.DtsObjectType == "DTS:Variable" select c;
            if (child.Name == "DTS:Property" || child.Name == "DTS:PropertyExpression")
            {
                ReadDtsProperty(child_el, o);
