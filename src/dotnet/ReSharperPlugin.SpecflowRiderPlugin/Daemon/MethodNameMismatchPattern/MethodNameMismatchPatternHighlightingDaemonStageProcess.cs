using System;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReSharperPlugin.SpecflowRiderPlugin.Daemon.MethodNameMismatchPattern
{
    public class MethodNameMismatchPatternHighlightingDaemonStageProcess : IDaemonStageProcess
    {
        public IDaemonProcess DaemonProcess { get; }
        private ICSharpFile _file;
        private readonly MethodNameMismatchPatternHighlightingRecursiveElementProcessor _elementProcessor;

        public MethodNameMismatchPatternHighlightingDaemonStageProcess(
            IDaemonProcess process,
            ICSharpFile file
        )
        {
            _file = file;
            DaemonProcess = process;
            _elementProcessor = new MethodNameMismatchPatternHighlightingRecursiveElementProcessor(DaemonProcess);
        }

        public void Execute(Action<DaemonStageResult> committer)
        {
            var consumer = new FilteringHighlightingConsumer(DaemonProcess.SourceFile, _file, DaemonProcess.ContextBoundSettingsStore);
            _file.ProcessDescendants(_elementProcessor, consumer);
            committer(new DaemonStageResult(consumer.Highlightings));
        }
    }

}