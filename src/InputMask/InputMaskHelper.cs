﻿using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using DNZ.MvcComponents;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Microsoft.AspNetCore.Mvc
{
    public static class InputMaskHelper
    {
        private const string pelak_css = "DNZ.MvcComponents.InputMask.pelak.css";

        public static InputMaskOption<TModel, TValue> InputMaskRegexFor<TModel, TValue>(this IHtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string regex, object htmlAttributes = null)
        {
            return new InputMaskOption<TModel, TValue>(html, expression, HtmlHelper.ObjectToDictionary(htmlAttributes)).Regex(regex);
        }

        public static InputMaskOption<TModel, TValue> InputMaskFor<TModel, TValue>(this IHtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string mask, object htmlAttributes = null)
        {
            return new InputMaskOption<TModel, TValue>(html, expression, HtmlHelper.ObjectToDictionary(htmlAttributes)).Mask(mask);
        }

        public static InputMaskOption<TModel, TValue> InputMaskFor<TModel, TValue>(this IHtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, InputMaskType type, object htmlAttributes = null)
        {
            InputMaskOption<TModel, TValue> mask = new InputMaskOption<TModel, TValue>(html, expression, HtmlHelper.ObjectToDictionary(htmlAttributes));
            switch (type)
            {
                //case InputMaskType.Pelak:
                //    mask.Mask("999آ99").Placeholder("---*--");
                //    break;
                case InputMaskType.Integer:
                    mask.Alias("integer").GroupSeparator(",").AutoGroup(true).GroupSize(3);
                    break;
                case InputMaskType.Decimal:
                    mask.Alias("decimal").GroupSeparator(",").AutoGroup(true).GroupSize(3);
                    break;
                case InputMaskType.IP:
                    mask.Alias("ip");
                    break;
                case InputMaskType.Mail:
                    mask.Alias("email");
                    break;
                case InputMaskType.Farsi:
                    mask.Regex("[آ-ی]*");
                    break;
            }
            return mask;
        }

        public static InputMaskOption<TModel, TValue> InputMaskFor<TModel, TValue>(this IHtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes = null)
        {
            return new InputMaskOption<TModel, TValue>(html, expression, HtmlHelper.ObjectToDictionary(htmlAttributes));
        }

        public static InputMaskOption<TModel, string> InputMask<TModel>(this IHtmlHelper<TModel> html, string name, string value = null, object htmlAttributes = null)
        {
            return new InputMaskOption<TModel, string>(html, name, value, HtmlHelper.ObjectToDictionary(htmlAttributes));
        }

        public static InputMaskOption<TModel, string> InputMask<TModel>(this IHtmlHelper<TModel> html, string name, string value = null, Dictionary<string, object> htmlAttributes = null)
        {
            return new InputMaskOption<TModel, string>(html, name, value, htmlAttributes);
        }

        public static IHtmlContent PelakFor<TModel, TValue>(this IHtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributeTextBox1 = null, object htmlAttributeTextBox2 = null)
        {
            ModelExplorer metadata = ExpressionMetadataProvider.FromLambdaExpression(expression, html.ViewData, html.MetadataProvider);
            string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
            string id = html.ViewData.TemplateInfo.GetFullHtmlFieldId(htmlFieldName);
            string name = html.ViewData.TemplateInfo.GetFullHtmlFieldName(htmlFieldName);
            string value = metadata.Model?.ToString() ?? "";
            string id1 = id + "_pelak1";
            string id2 = id + "_pelak2";
            Dictionary<string, object> mergAttr1 = ComponentUtility.MergeAttributes(htmlAttributeTextBox1, new { id = id1, @class = "pelak pelak1" });
            Dictionary<string, object> mergAttr2 = ComponentUtility.MergeAttributes(htmlAttributeTextBox2, new { id = id2, @class = "pelak pelak2" });
            //ایران|13|123|ش|22
            //iran|13|123|a|22
            string value1 = "";
            string value2 = "";
            try
            {
                if (value.HasValue())
                {
                    value1 = value.Substring(5, 2);
                    value2 = value.Substring(8);
                }
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch { }
#pragma warning restore CA1031 // Do not catch general exception types
            InputMaskOption<TModel, string> mask1 = html.InputMask(id1, value1, mergAttr1).Mask("99").Placeholder("--");
            InputMaskOption<TModel, string> mask2 = html.InputMask(id2, value2, mergAttr2).Mask("999آ99").Placeholder("---*--");
            //var mask2 = html.InputMask(id2, value2, mergAttr2).Mask("999 آ99").Placeholder("--- *--");
            string result = @"<div class=""pelak-container"">
                           " + mask1.ToHtmlString() + @"
                           " + mask2.ToHtmlString() + @" 
                           " + html.HiddenFor(expression).ToHtmlString() + @"
                       </div>";
            html.StyleFileSingle(@"<link href=""" + ComponentUtility.GetWebResourceUrl(pelak_css) + @""" rel=""stylesheet"" />");
            html.Script(@"
            <script>
                $(function(){
                    $(""#" + id + @"_pelak1, #" + id + @"_pelak2"").keyup(function () {
                        var hidden = $(""#" + id + @""");
                        hidden.val('').trigger('change');
                        var pelak1 = $(""#" + id + @"_pelak1"").val();
                        var pelak2 = $(""#" + id + @"_pelak2"").val().replace('-', '').replace('*', '');
                        if (pelak1.length == 2 && pelak2.length == 6) {
                            var value = 'ایران|' + pelak1 + '|' + pelak2.substring(0, 3) + '|' + pelak2.substring(3, 4) + '|' + pelak2.substring(4, 6);
                            hidden.val(value).trigger('change');
                        }
                        hidden.valid();
                        if (pelak2.indexOf('ت') > -1)
                            $(this).parent().css('background-color', 'yellow');
                        else
                            $(this).parent().css('background-color', 'white');
                    });
                });
            </script>");
            return new HtmlString(result);
        }
    }
}