﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Rubberduck.Parsing.Inspections.Abstract;

namespace RubberduckWeb.Controllers
{
    [OutputCache(VaryByParam = "*", Duration = 0, NoStore = true)]
    public class InspectionsController : Controller
    {
        /*
        private readonly DefaultInspector _inspector;
        private readonly RubberduckParserState _state;
        */

        private readonly IReadOnlyList<IInspection> _inspections;

        public InspectionsController(IEnumerable<IInspection> inspections/*, DefaultInspector inspector, RubberduckParserState state*/)
        {
            _inspections = inspections.ToList();
            /*
            _inspector = inspector;
            _state = state;
            */
        }

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            return View(_inspections);
        }

        //public ActionResult Details(string id)
        //{
        //    var inspection = _inspections.FirstOrDefault(x => x.Name == id);

        //    if (inspection == null)
        //    {
        //        return RedirectToAction("List");
        //    }

        //    return View(inspection);
        //}

        //[HttpPost]
        //public Task<PartialViewResult> GetInspectionResults(string code)
        //{
        //    //Arrange
        //    var builder = new MockVbeBuilder();

        //    // ensure line endings are \r\n
        //    code = code.Replace("\r\n", "\n").Replace("\n", "\r\n");
        //    var project = builder.ProjectBuilder("WebInspector", ProjectProtection.Unprotected)

        //                     .AddReference("VBA", MockVbeBuilder.LibraryPathVBA, 4, 1, true)
        //                     .AddReference("Excel", MockVbeBuilder.LibraryPathMsExcel, 1, 7, true)
        //                     .AddReference("Office", MockVbeBuilder.LibraryPathMsOffice, 2, 5, true)
        //                     .AddReference("Scripting", MockVbeBuilder.LibraryPathScripting, 1, 0, true)

        //                     .AddComponent("WebModule", ComponentType.StandardModule, code)
        //                     .Build();
        //    var vbe = builder.AddProject(project).Build();
        //    var mockHost = new Mock<IHostApplication>();
        //    mockHost.SetupGet(m => m.ApplicationName).Returns("Excel");
        //    vbe.Setup(m => m.HostApplication()).Returns(() => mockHost.Object);

        //    var path = Server.MapPath("~/Declarations");
        //    var parser = MockParser.Create(vbe.Object, _state, null, path);
        //    parser.State.AddTestLibrary(path + "/VBA.4.2.xml");
        //    parser.State.AddTestLibrary(path + "/Excel.1.8.xml");
        //    parser.State.AddTestLibrary(path + "/Office.2.7.xml");
        //    parser.State.AddTestLibrary(path + "/Scripting.1.0.xml");

        //    try
        //    {
        //        Task.Run(() => parser.Parse(new CancellationTokenSource())).Wait();
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //    }
        //    if (parser.State.Status >= ParserState.Error)
        //    {
        //        return Task.FromResult(PartialView("InspectionResults", null));
        //    }

        //    var results = _inspector.Inspect(parser.State);

        //    // strip away module & project naming suggestions, they don't make sense in the web UI
        //    //results.RemoveAll(ir => ir.Inspection is UseMeaningfulNameInspection
        //    //            && (ir.QualifiedSelection.QualifiedName.Name == "TestProject1." || ir.QualifiedSelection.QualifiedName.Name == "TestProject1.TestModule1");

        //    return Task.FromResult(PartialView("InspectionResults", results));
        //}

        public static string FormatInspectionName(IInspection inspection)
        {
            return InsertOnCharacter(inspection.Name.Remove(inspection.Name.Length - "Inspection".Length), CharacterType.UpperLetter, " ");
        }
        
        /// <summary>
        /// Inserts a <code>string</code> into another <code>string</code> before each occurrence of the specified <see cref="CharacterType"/>.
        /// </summary>
        /// <param name="source">The <code>string</code> to insert into.</param>
        /// <param name="type">The <see cref="CharacterType"/> to insert before.</param>
        /// <param name="insert">The <code>string</code> to insert.</param>
        /// <returns>The resultant <code>string</code>.</returns>
        /// <remarks>
        /// In the case of the first character of <code>source</code> matching the specified <see cref="CharacterType"/>, the <code>insert</code> will not be inserted.
        /// </remarks>
        public static string InsertOnCharacter(string source, CharacterType type, string insert)
        {
            var esb = new StringBuilder(source.Length);

            var firstRun = false;
            foreach (var c in source)
            {
                var cType = GetCharacterType(c);

                if (firstRun && cType == type)
                {
                    esb.Append(insert);
                }

                esb.Append(c);

                firstRun = true;
            }

            return esb.ToString();
        }

        public static CharacterType GetCharacterType(char c)
        {
            if (c >= 'a' && c <= 'z')
            {
                return CharacterType.LowerLetter;
            }

            if (c >= 'A' && c <= 'Z')
            {
                return CharacterType.UpperLetter;
            }

            if (c >= '0' && c <= '9')
            {
                return CharacterType.Number;
            }

            return CharacterType.Symbol;
        }

        public enum CharacterType
        {
            UpperLetter,
            LowerLetter,
            Number,
            Symbol,
        }
    }
}