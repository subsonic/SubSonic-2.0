/*
 * SubSonic - http://subsonicproject.com
 * 
 * The contents of this file are subject to the Mozilla Public
 * License Version 1.1 (the "License"); you may not use this file
 * except in compliance with the License. You may obtain a copy of
 * the License at http://www.mozilla.org/MPL/
 * 
 * Software distributed under the License is distributed on an 
 * "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or
 * implied. See the License for the specific language governing
 * rights and limitations under the License.
*/

using MbUnit.Framework;

namespace SubSonic.Tests
{
    /// <summary>
    /// Summary for the TurboTemplateTests class
    /// </summary>
    [TestFixture]
    public class TurboTemplateTests
    {
        //[Test]
        //public void CanReferenceAssembliesViaDirective()
        //{
        //    DataProvider provider = DataService.GetInstance("Northwind");
        //    ICodeLanguage lang = new CSharpCodeLanguage();

        //    string templateText = "<%@ Assembly Name=\"TestAssembly\" %>\n\npublic class Test { }";
        //    TurboTemplate template = new TurboTemplate(templateText, lang, provider);

        //    //should reference this assembly
        //    string asm = "TestAssembly.dll";

        //    Assert.In(asm, template.References, "template reference list didn't have assembly");
        //}

        /// <summary>
        /// Assemblies the directive is stripped from template.
        /// </summary>
        [Test]
        public void AssemblyDirectiveIsStrippedFromTemplate()
        {
            DataProvider provider = DataService.GetInstance("Northwind");
            ICodeLanguage lang = new CSharpCodeLanguage();

            const string templateText = "<%@ Assembly Name=\"TestAssembly\" %>\n\npublic class Test { }";
            TurboTemplate template = new TurboTemplate(templateText, lang, provider);

            Assert.IsTrue(template.TemplateText.IndexOf("<%@ Assembly Name=\"TestAssembly\" %>") == -1, "the directive wasn't stripped");
        }

        //[Test]
        //public void AssemblyOnlyReferencedOnceWithDirective()
        //{
        //    DataProvider provider = DataService.GetInstance("Northwind");            
        //    ICodeLanguage lang = new CSharpCodeLanguage();

        //    string templateText = "<%@ Assembly Name=\"TestAssembly\" %>\n\npublic class Test { }";
        //    TurboTemplate template = new TurboTemplate(templateText, lang, provider);

        //    int referenceCount = 0;
        //    string asm = "TestAssembly.dll";
        //    foreach(string reference in template.References)
        //    {
        //        if (reference == asm)
        //            referenceCount++;
        //    }

        //    Assert.GreaterThan(referenceCount, 0, "reference wasn't added!");
        //    Assert.AreEqual(1, referenceCount, "reference was added more than once");
        //}
    }
}