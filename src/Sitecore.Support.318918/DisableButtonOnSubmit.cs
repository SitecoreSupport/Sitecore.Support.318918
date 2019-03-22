using Sitecore.Diagnostics;
using Sitecore.ExperienceForms.Mvc;
using Sitecore.ExperienceForms.Mvc.Pipelines.RenderForm;
using Sitecore.Mvc.Pipelines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc.Ajax;
using Sitecore.Mvc.Extensions;

namespace Sitecore.Support.ExperienceForms.Mvc.Pipelines.RenderForm
{
  public class InitializeAjaxOptions : MvcPipelineProcessor<RenderFormEventArgs>
  {
    private readonly IFormRenderingContext _formRenderingContext;
    public InitializeAjaxOptions(IFormRenderingContext formRenderingContext)
    {
      Assert.ArgumentNotNull(formRenderingContext, "formRenderingContext");
      _formRenderingContext = formRenderingContext;
    }

    public override void Process(RenderFormEventArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      if (args.ViewModel.IsAjax)
      {
        if (args.HtmlHelper.ViewContext.UnobtrusiveJavaScriptEnabled)
        {
          AjaxOptions ajaxOptions = new AjaxOptions();
          ajaxOptions.OnBegin = "beforesend";//fix 318918
          ajaxOptions.HttpMethod = "Post";
          ajaxOptions.InsertionMode = InsertionMode.ReplaceWith;
          ajaxOptions.UpdateTargetId = args.FormHtmlId;
          ajaxOptions.OnSuccess = FormattableString.Invariant(FormattableStringFactory.Create("$.validator.unobtrusive.parse('#{0}');$.fxbFormTracker.parse('#{1}');", args.FormHtmlId, args.FormHtmlId));
          AjaxOptions ajaxOptions2 = ajaxOptions;
          IDictionary<string, object> dictionary = ajaxOptions2.ToUnobtrusiveHtmlAttributes();
          foreach (string key in dictionary.Keys)
          {
            args.Attributes[key] = dictionary[key];
          }
        }
        if (!args.IsPost)
        {
          args.QueryString.Add("fxb.FormItemId", args.ViewModel.ItemId.ToGuid());
          args.QueryString.Add("fxb.HtmlPrefix", _formRenderingContext.Prefix.Trim('.'));
        }
        args.RouteName = "FormBuilder";
      }
    }

  }
  
}