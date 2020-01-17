using IDesign.Core.Models;
using IDesign.Extension.Resources;
using IDesign.Recognizers.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDesign.Extension
{
    class SummaryFactory
    {
        private string CreateFilesAnalysedSummary(IEnumerable<string> uniqueFiles, IEnumerable<IEntityNode> uniqueEntityNodes)
        {
            var result = "";
            if (uniqueFiles.Count() <= 1)
            {
                result += $"{SummaryRescources.OneFile} ";
            }
            else
            {
                result += $"{string.Format(SummaryRescources.MultipleFiles, uniqueFiles.Count())} ";
            }

            switch (uniqueEntityNodes.Count())
            {
                case 1:
                    result += $"{SummaryRescources.OneEntityNodeAnalysed} ";
                    break;
                default:
                    result += $"{string.Format(SummaryRescources.MultipleEntiyNodesAnalysed, uniqueEntityNodes.Count())} ";
                    break;
            }

            return result;
        }


        private string CreatePartOfPatternSummary(
            IEnumerable<string> uniqueFiles, IEnumerable<RecognitionResult> allResults,
            IEnumerable<IEntityNode> uniqueEntityNodes, ref int recognizedPatternsCount)
        {
            var resultList = "";
            if (uniqueFiles.Count() == 1)
            {
                var allRecognizedPatters =
                        allResults
                        .Where(x => x.Result.GetScore() >= 80)
                        .OrderByDescending(x => x.Result.GetScore());
                foreach (var (entityNode, partOfPatterns) in from entityNode in uniqueEntityNodes
                                                             let partOfPatterns = allRecognizedPatters
                        .Where(x => x.Result.GetRelatedSubTypes().ContainsKey(entityNode))
                                                             where partOfPatterns.Count() > 0
                                                             select (entityNode, partOfPatterns))
                {
                    resultList += $"{Environment.NewLine}{string.Format(SummaryRescources.ClassSeemsPartOf, entityNode.GetName(), partOfPatterns.First().Pattern.Name)}";
                    recognizedPatternsCount++;
                }
            }
            return resultList;
        }


        private string CreateNoPatternsFoundSummary(IEnumerable<RecognitionResult> results)
        {

            var result = SummaryRescources.NoPatternsRecognized + " ";
            var resultsWithScore = results.Where(x => x.Result.GetScore() > 0)
                                          .OrderByDescending(x => x.Result.GetScore());

            if (resultsWithScore.Count() > 0)
            {
                var highestScored = resultsWithScore.First();
                result += string.Format(SummaryRescources.ClassScoresHighestOn, highestScored.EntityNode.GetName(),
                                        highestScored.Pattern.Name);
            }
            return result;
        }

        public string CreateSummary(IEnumerable<RecognitionResult> results, IEnumerable<RecognitionResult> allResults)
        {
            var result = "";
            if (results.Count() == 0)
            {
                return SummaryRescources.NothingClassesOrInterfacesFound;
            }

            var uniqueFiles = results.Select(x => x.FilePath).Distinct();
            var uniqueEntityNodes = results.Select(x => x.EntityNode).Distinct();

            result += CreateFilesAnalysedSummary(uniqueFiles, uniqueEntityNodes);

            var recognizedPatters = results.Where(x => x.Result.GetScore() >= 80).OrderByDescending(x => x.Result.GetScore());
            var recognizedPatternsCount = recognizedPatters.Count();

            var resultList = CreatePartOfPatternSummary(uniqueFiles, allResults, uniqueEntityNodes, ref recognizedPatternsCount);

            if (resultList.Count() == 0 && recognizedPatternsCount == 0)
            {
                result += CreateNoPatternsFoundSummary(results);
            }
            else switch (recognizedPatternsCount)
                {
                    case 1:
                        result += SummaryRescources.OnePatternRecognized + " ";
                        break;
                    default:
                        result += $"{string.Format(SummaryRescources.MultiplePatternsRecognized, recognizedPatternsCount)} ";
                        break;
                }

            foreach (var recognizedPattern in recognizedPatters)
            {
                result += $"{Environment.NewLine}{string.Format(SummaryRescources.ClassSeemsPartOf, recognizedPattern.EntityNode.GetName(), recognizedPattern.Pattern.Name)}";
            }

            result += resultList;

            return result;
        }
    }
}
