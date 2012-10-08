﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using CsQuery;

namespace CsQuery.Tests.Csharp.HtmlParser
{
    [TestClass]
    public class Rendering: CsQueryTest 
    {

        protected string node = "<div class='a b c c' attr1='{\"somejson\": \"someval\"}'>";

        [TestMethod, Test]
        public void HtmlCleanup()
        {
            var dom = CQ.CreateFragment(node);
            var expected =  "<div class=\"a b c\" attr1='{\"somejson\": \"someval\"}'></div>";
            Assert.AreEqual(expected, dom.Render(), "Basic cleanup - no duplicated class - missing end tag");


            // TODO
            // test attribute rendering options
            // Doctype options

        }
        [TestMethod,Test]
        public void AttributeQuoting()
        {


        }
        [TestMethod,Test]
        public void AttributeHandling()
        {
            string test1html = "<input type=\"text\" id=\"\" checked custom=\"sometext\">";
            var dom = CQ.CreateFragment(test1html);
            Assert.AreEqual("<input type=\"text\" id checked custom=\"sometext\">", dom.Render(), "Missing & boolean attributes are parsed & render correctly");

            // remove "quote all attributes"

            Assert.AreEqual("<input type=text id checked custom=sometext>", dom.Render(DomRenderingOptions.None), "Missing & boolean attributes are parsed & render correctly");

            dom = CQ.CreateFragment("<div id='test' quotethis=\"must've\" class=\"one two\" data='\"hello\"' noquote=\"regulartext\">");

            var expected = "<div class=\"one two\" id=test quotethis=\"must've\" data='\"hello\"' noquote=regulartext></div>";
            Assert.AreEqual(expected, dom.Render(DomRenderingOptions.None), "Handle various quoting situations");

            // go back to test 1
            dom = CQ.CreateFragment(test1html);

            var jq = dom["input"];
            var el = jq[0];

            Assert.AreEqual("", el["id"], "Empty attribute is empty");
            Assert.AreEqual("",el["checked"], "Boolean attribute is the same as empty");
            Assert.AreEqual(null, el["missing"], "Missing attribute is null");

            Assert.AreEqual("", jq.Attr("id"), "Empty attribute is empty");
            Assert.AreEqual("checked", jq.Attr("checked"), "Boolean attribute is true");
            Assert.AreEqual(true, jq.Prop("checked"), "Boolean attribute is true");
            Assert.AreEqual(null, jq.Attr("selected"), "Boolean attribute is true");
            // TODO - actually jquery would return "undefined" b/c selected doesn't apply to input. Need to do this mapping somewhere.
            Assert.AreEqual(false, jq.Prop("selected"), "Boolean attribute is true");
            Assert.AreEqual(null, jq.Attr("missing"), "Missing attribute is null");


        }

        [TestMethod, Test]
        public void Utf8Handling()
        {
            string spanish = "Romney dice que Chávez expande la tiranía";
            var dom = CQ.CreateFragment("<div>" + spanish + "</div>");
        
            // the "render" method should turn UTF8 characters to HTML. Accessing the node value directly should not.

            Assert.AreEqual(spanish,dom["div"][0].ChildNodes[0].NodeValue);
            Assert.AreEqual(spanish, dom.Text());
            Assert.AreEqual("<div>Romney dice que Ch&#225;vez expande la tiran&#237;a</div>", dom.Render());

        }
    }
}
