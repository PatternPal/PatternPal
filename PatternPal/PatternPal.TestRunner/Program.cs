#region

using System.Diagnostics;

#endregion

const string patternPalExe = "I:/Dev/PatternPal/PatternPal/PatternPal.Console/bin/Release/PatternPal.ConsoleApp.exe";

// Collect projects to test
using StreamReader reader = new(
	new FileStream(
		args[ 0 ],
		FileMode.Open ) );
IDictionary< string, string > testProjects = new Dictionary< string, string >( );
while ( reader.ReadLine( ) is { } line )
{
	if ( line[ 0 ] == '#' )
	{
		continue;
	}

	string[ ] lineParts = line.Split( ',' );
	testProjects.Add(
		lineParts[ 0 ],
		lineParts[ 1 ] );
}

IList< ProjectResult > results = new List< ProjectResult >( );

// Run PatternPal for all the test projects
foreach ( ( string projectDir, string implementedPattern ) in testProjects )
{
	Process patternPal = Process.Start(
		new ProcessStartInfo(
			patternPalExe,
			$"\"{projectDir}\" \"{implementedPattern}\"" )
		{
			RedirectStandardOutput = true,
		} )!;

	// Collect the results
	ProjectResult result = new( )
	                       {
		                       ProjectName = projectDir,
		                       ImplementedPattern = implementedPattern,
		                       Results = new List< DetectionResult >( ),
	                       };
	while ( patternPal.StandardOutput.ReadLine( ) is { } line )
	{
		string[ ] parts = line.Split( ',' );
		result.Results.Add(
			new DetectionResult( )
			{
				ClassName = parts[ 0 ],
				DetectedPattern = parts[ 1 ],
				Score = int.Parse( parts[ 2 ] ),
			} );
	}

	patternPal.WaitForExit( );

	results.Add( result );
}

// Calculate correctly detected patterns
int correctlyDetectedPatterns = 0;
int projectsWithoutResults = 0;
foreach ( ProjectResult result in results )
{
	if ( result.Results.Count == 0 )
	{
		projectsWithoutResults++;
		continue;
	}

	if ( result.Correct )
	{
		correctlyDetectedPatterns++;
	}
}

int totalProjectCount = testProjects.Count;
int projectsWithResults = totalProjectCount - projectsWithoutResults;
double recall = projectsWithResults / ( double ) totalProjectCount * 100;
double precision = correctlyDetectedPatterns / ( double ) projectsWithResults * 100;

// Prints stats
Console.WriteLine( $"Implementations with results: {projectsWithResults} of {testProjects.Count}" );
Console.WriteLine( $"Patterns correctly detected: {correctlyDetectedPatterns}" );
Console.WriteLine( $"Patterns incorrectly detected: {results.Count - correctlyDetectedPatterns}" );
Console.WriteLine( );
Console.WriteLine( $"Recall: {recall}%" );
Console.WriteLine( $"Precision: {precision}%" );
Console.WriteLine( );

// Print incorrect results, with detected pattern with highest score
foreach ( ProjectResult result in results )
{
	if ( result.Correct )
	{
		continue;
	}

	if ( result.Results.Count == 0 )
	{
		Console.WriteLine( $"{result.ProjectName}: No pattern detected" );
		continue;
	}

	DetectionResult res = result.Results[ 0 ];
	Console.WriteLine( $"{result.ProjectName}: Expected '{result.ImplementedPattern}', found '{res.DetectedPattern}' with score {res.Score} (implemented in '{res.ClassName}')" );
}

class ProjectResult
{
	internal required string ProjectName { get; init; }
	internal required string ImplementedPattern { get; init; }

	// KNOWN: Results are sorted by score in descending order
	internal required IList< DetectionResult > Results { get; init; }

	internal bool Correct => Results.Count != 0
	                         && Results[ 0 ].DetectedPattern.Replace(
		                         " ",
		                         string.Empty )
	                         == ImplementedPattern.Replace(
		                         " ",
		                         string.Empty );
}

class DetectionResult
{
	internal required string ClassName { get; init; }
	internal required string DetectedPattern { get; init; }
	internal required int Score { get; init; }
}