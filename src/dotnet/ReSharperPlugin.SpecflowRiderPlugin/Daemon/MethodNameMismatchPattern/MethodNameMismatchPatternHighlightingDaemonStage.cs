using System.Collections.Generic;
using System.Linq;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Daemon.Stages;
using JetBrains.ReSharper.Daemon.UsageChecking;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.Util;
using ReSharperPlugin.SpecflowRiderPlugin.Caching.StepsDefinitions;

namespace ReSharperPlugin.SpecflowRiderPlugin.Daemon.MethodNameMismatchPattern
{
    [DaemonStage(StagesBefore = new[] {typeof(GlobalFileStructureCollectorStage)}, StagesAfter = new[] {typeof(CollectUsagesStage)})]
    public class MethodNameMismatchPatternHighlightingDaemonStage : IDaemonStage
    {
        private readonly SpecflowStepsDefinitionsCache _specflowStepsDefinitionsCache;

        public MethodNameMismatchPatternHighlightingDaemonStage(
            ResolveHighlighterRegistrar registrar,
            SpecflowStepsDefinitionsCache specflowStepsDefinitionsCache
        )
        {
            _specflowStepsDefinitionsCache = specflowStepsDefinitionsCache;
        }

        public IEnumerable<IDaemonStageProcess> CreateProcess(
            IDaemonProcess process,
            IContextBoundSettingsStore settings,
            DaemonProcessKind processKind
        )
        {
            if (processKind != DaemonProcessKind.SOLUTION_ANALYSIS && processKind != DaemonProcessKind.VISIBLE_DOCUMENT)
                return Enumerable.Empty<IDaemonStageProcess>();

            if (!_specflowStepsDefinitionsCache.AllStepsPerFiles.ContainsKey(process.SourceFile))
                return Enumerable.Empty<IDaemonStageProcess>();

            return process.SourceFile.GetPsiFiles<CSharpLanguage>()
                .SelectNotNull(file => new MethodNameMismatchPatternHighlightingDaemonStageProcess(process, (ICSharpFile) file));
        }
    }

}