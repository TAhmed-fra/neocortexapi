using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml; 
using System.IO;

namespace NeocortexApiSamplePerformance
{
    public class ReadData
    {
        public static List<MultiSequenceInput> MultiSequenceExcelInput(string filePath)
        {
            var inputs = new List<MultiSequenceInput>();

            // Set EPPlus License Context
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets[0]; // Assuming data is in the first sheet
                int rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++) // Assuming first row contains headers
                {
                    var input = new MultiSequenceInput
                    {
                        ExperimentId = int.TryParse(worksheet.Cells[row, 1].Text, out int experimentId) ? experimentId : 0,
                        ExperimentName = worksheet.Cells[row, 2].Text,
                        CPU = int.TryParse(worksheet.Cells[row, 3].Text, out int cpu) ? cpu : 0,
                        DotnetVersion = worksheet.Cells[row, 4].Text,
                        W = int.TryParse(worksheet.Cells[row, 5].Text, out int w) ? w : 0,
                        N = int.TryParse(worksheet.Cells[row, 6].Text, out int n) ? n : 0,
                        numColumns = int.TryParse(worksheet.Cells[row, 7].Text, out int numColumns) ? numColumns : 0,
                        Radius = double.TryParse(worksheet.Cells[row, 8].Text, out double radius) ? radius : 0.0,
                        MinVal = double.TryParse(worksheet.Cells[row, 9].Text, out double minVal) ? minVal : 0.0,
                        MaxVal = double.TryParse(worksheet.Cells[row, 10].Text, out double maxVal) ? maxVal : 0.0,
                        Periodic = bool.TryParse(worksheet.Cells[row, 11].Text, out bool periodic) && periodic,
                        ClipInput = bool.TryParse(worksheet.Cells[row, 12].Text, out bool clipInput) && clipInput,
                        Name = worksheet.Cells[row, 13].Text,
                        CellsPerColumn = int.TryParse(worksheet.Cells[row, 14].Text, out int cellsPerColumn) ? cellsPerColumn : 0,
                        GlobalInhibition = bool.TryParse(worksheet.Cells[row, 15].Text, out bool globalInhibition) && globalInhibition,
                        LocalAreaDensity = double.TryParse(worksheet.Cells[row, 16].Text, out double localAreaDensity) ? localAreaDensity : 0.0,
                        NumActiveColumnsPerInhArea = double.TryParse(worksheet.Cells[row, 17].Text, out double numActiveColumns) ? (int)numActiveColumns : 0, // Fixed casting
                        PotentialRadius = int.TryParse(worksheet.Cells[row, 18].Text, out int potentialRadius) ? potentialRadius : 0,
                        MaxBoost = double.TryParse(worksheet.Cells[row, 19].Text, out double maxBoost) ? maxBoost : 0.0,
                        DutyCyclePeriod = int.TryParse(worksheet.Cells[row, 20].Text, out int dutyCyclePeriod) ? dutyCyclePeriod : 0,
                        MinPctOverlapDutyCycles = double.TryParse(worksheet.Cells[row, 21].Text, out double minPct) ? minPct : 0.0,
                        //MaxSynapsesPerSegment = double.TryParse(worksheet.Cells[row, 22].Text, out double maxSynapses) ? maxSynapses : 0.0,
                        ActivationThreshold = int.TryParse(worksheet.Cells[row, 23].Text, out int activationThreshold) ? activationThreshold : 0,
                        ConnectedPermanence = double.TryParse(worksheet.Cells[row, 24].Text, out double connectedPermanence) ? connectedPermanence : 0.0,
                        PermanenceDecrement = double.TryParse(worksheet.Cells[row, 25].Text, out double permanenceDecrement) ? permanenceDecrement : 0.0,
                        PermanenceIncrement = double.TryParse(worksheet.Cells[row, 26].Text, out double permanenceIncrement) ? permanenceIncrement : 0.0,
                        PredictedSegmentDecrement = double.TryParse(worksheet.Cells[row, 27].Text, out double predictedSegmentDecrement) ? predictedSegmentDecrement : 0.0,
                        SequenceLength = int.TryParse(worksheet.Cells[row, 28].Text, out int sequenceLength) ? sequenceLength : 0,