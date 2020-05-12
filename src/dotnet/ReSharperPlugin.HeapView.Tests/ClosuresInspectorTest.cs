﻿using JetBrains.Diagnostics;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReSharperPlugin.HeapView.Analyzers;

namespace ReSharperPlugin.HeapView.Tests
{
  [TestNetFramework46]
  [TestPackages(Packages = new[] {SYSTEM_VALUE_TUPLE_PACKAGE})]
  [TestReferences("System", "System.Core", "Microsoft.CSharp")]
  [CSharpLanguageLevel(CSharpLanguageLevel.Latest)]
  public class ClosuresInspectorTest : BaseTestWithTextControl
  {
    protected override string RelativeTestDataPath => "Closures";

    protected override void DoTest(Lifetime lifetime, IProject testProject)
    {
      var textControl = OpenTextControl(lifetime);
      var sourceFile = textControl.Document.GetPsiSourceFile(Solution).NotNull();
      var psiFile = sourceFile.GetTheOnlyPsiFile<CSharpLanguage>().NotNull();

      ExecuteWithGold(
        testName: TestName2,
        writer =>
        {
          foreach (var declaration in psiFile.ThisAndDescendants<ICSharpDeclaration>())
          {
            var inspector = ClosuresInspector.TryBuild(declaration);
            if (inspector == null) continue;

            writer.WriteLine(DeclaredElementPresenter.Format(
              declaration.Language, DeclaredElementPresenter.KIND_QUALIFIED_NAME_PRESENTER,
              declaration.DeclaredElement.NotNull()));

            inspector.Run();
          }
        });
    }

    [Test] public void TestClosures01() { DoNamedTest2(); }
  }
}