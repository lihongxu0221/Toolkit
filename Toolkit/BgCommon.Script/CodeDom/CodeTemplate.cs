namespace BgCommon.Script.CodeDom;

public class CodeTemplate
{
    public static string s_RawScript = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using BgBase;
using BgMotion;
using LG.Core;
using HandyControl;
using HandyControl.Data;

public class Code : FlowBase
{
    public override void Start()
    {
        base.Start();
    }
    public override bool RunBefore()
    {
        return true;
    }
    public override bool RunAfter()
    {
        return true;
    }
    public override bool Run()
    {
        MessageBox.Show(""Hello World!!!"");
        return true;
    }
}";
}