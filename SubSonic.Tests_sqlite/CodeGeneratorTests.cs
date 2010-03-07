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
    /// Summary description for CodeGenerator
    /// </summary>
    [TestFixture]
    public class CodeGeneratorTests
    {
        #region new generator tests

        /// <summary>
        /// Codes the gen2_ run class template.
        /// </summary>
        [Test]
        public void CodeGen2_RunClassTemplate()
        {
            DataProvider provider = DataService.GetInstance("Northwind");
            ICodeLanguage csharp = new VBCodeLanguage();
            TurboTemplate bits = CodeService.BuildClassTemplate("Products", csharp, provider);
            bits.Render();
            Assert.IsTrue(bits.FinalCode.Length > 0);
        }

        /// <summary>
        /// Codes the gen2_ run SP template.
        /// </summary>
        [Test]
        public void CodeGen2_RunSPTemplate()
        {
            DataProvider provider = DataService.GetInstance("Northwind");
            TurboTemplate bits = CodeService.BuildSPTemplate(new CSharpCodeLanguage(), provider);
            bits.Render();
            Assert.IsTrue(bits.FinalCode.Length > 0);
        }

        /// <summary>
        /// Codes the gen2_ run view template.
        /// </summary>
        [Test]
        public void CodeGen2_RunViewTemplate()
        {
            DataProvider provider = DataService.GetInstance("Northwind");
            TurboTemplate bits = CodeService.BuildViewTemplate("Current Product List", new CSharpCodeLanguage(), provider);
            bits.Render();
            Assert.IsTrue(bits.FinalCode.Length > 0);
        }

        /// <summary>
        /// Codes the gen2_ run struct template.
        /// </summary>
        [Test]
        public void CodeGen2_RunStructTemplate()
        {
            DataProvider provider = DataService.GetInstance("Northwind");
            TurboTemplate bits = CodeService.BuildStructsTemplate(new CSharpCodeLanguage(), provider);
            bits.Render();
            Assert.IsTrue(bits.FinalCode.Length > 0);
        }

        /// <summary>
        /// Codes the gen2_ VB run class template.
        /// </summary>
        [Test]
        public void CodeGen2_VBRunClassTemplate()
        {
            DataProvider provider = DataService.GetInstance("Northwind");
            TurboTemplate bits = CodeService.BuildClassTemplate("Products", new VBCodeLanguage(), provider);
            bits.Render();
            Assert.IsTrue(bits.FinalCode.Length > 0);
        }

        /// <summary>
        /// Codes the gen2_ VB run SP template.
        /// </summary>
        [Test]
        public void CodeGen2_VBRunSPTemplate()
        {
            DataProvider provider = DataService.GetInstance("Northwind");
            TurboTemplate bits = CodeService.BuildSPTemplate(new VBCodeLanguage(), provider);
            bits.Render();
            Assert.IsTrue(bits.FinalCode.Length > 0);
        }

        /// <summary>
        /// Codes the gen2_ VB run view template.
        /// </summary>
        [Test]
        public void CodeGen2_VBRunViewTemplate()
        {
            DataProvider provider = DataService.GetInstance("Northwind");
            TurboTemplate bits = CodeService.BuildViewTemplate("Current Product List", new VBCodeLanguage(), provider);
            bits.Render();
            Assert.IsTrue(bits.FinalCode.Length > 0);
        }

        /// <summary>
        /// Codes the gen2_ VB run struct template.
        /// </summary>
        [Test]
        public void CodeGen2_VBRunStructTemplate()
        {
            DataProvider provider = DataService.GetInstance("Northwind");

            TurboTemplate bits = CodeService.BuildStructsTemplate(new VBCodeLanguage(), provider);
            bits.Render();
            Assert.IsTrue(bits.FinalCode.Length > 0);
        }

        //[Test]
        //public void CodeGen2_WBTest_Common_Language() {

        //    //TableSchema.Table tbl = DataService.GetTableSchema("Products", "Northwind");
        //    //string bits = CodeService.RunTemplate(CodeService.TemplateType.Structs,@"D:\@Projects\@OpenSource\SubSonic\SubSonic\CodeGeneration\Templates\StructsTemplate.aspx", "C:\\Bop.cs", "Northwind", "", "");
        //    string bits = CodeService.RunClass("Common_Language", "WB", LanguageType.CSharp);
        //    Assert.IsTrue(bits.Length > 0);
        //}

        #endregion
    }
}